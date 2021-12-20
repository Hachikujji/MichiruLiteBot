using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using MichiruLite.Modules.Audio.Services;
using NAudio.Wave;

namespace MichiruLite.Modules.Audio
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, AudioContainer> _connectedChannels = new ConcurrentDictionary<ulong, AudioContainer>();
        private readonly IAudioService _searchService;

        public AudioService(YoutubeService searchService)
        {
            _searchService = searchService;
        }

        private async Task<bool> JoinAudio(IVoiceChannel voiceChannel, IMessageChannel channel)
        {
            if (voiceChannel == null)
            {
                await channel.SendMessageAsync("__User must be in a voice channel__");
                return false;
            }
            var guildId = voiceChannel.Guild.Id;
            if (_connectedChannels.TryGetValue(guildId, out _)) return false;

            var Container = new AudioContainer
            {
                AudioClient = await voiceChannel.ConnectAsync(),
                CancellationTokenSource = new CancellationTokenSource(),
                QueueService = new QueueService(),
            };
            Container.AudioOutStream = Container.AudioClient.CreatePCMStream(AudioApplication.Music, bitrate: 128000);
            _connectedChannels.TryAdd(guildId, Container);

            return true;
        }

        public async Task<bool> TryLeaveAudioAsync(IGuild guild, IVoiceChannel voiceChannel)
        {
            if (!_connectedChannels.TryRemove(guild.Id, out AudioContainer container))
                return false;
            await container.AudioClient.StopAsync();
            return true;
        }

        private async Task SendAudioAsync(IGuild guild, string url, int rate, int offset = 0)
        {
            _connectedChannels.TryGetValue(guild.Id, out AudioContainer container);

            var audioOutStream = container.AudioOutStream;
            var token = container.CancellationTokenSource.Token;
            var waveFormat = new WaveFormat(rate, 16, 2);
            using var reader = new MediaFoundationReader(url);
            reader.Skip(offset);
            using var resamplerDmo = new ResamplerDmoStream(reader, waveFormat);
            try
            {
                container.ResamplerDmoStream = resamplerDmo;
                await resamplerDmo.CopyToAsync(audioOutStream, token)
                   .ContinueWith(t => { return; });
            }
            finally
            {
                await audioOutStream.FlushAsync();
                container.CancellationTokenSource = new CancellationTokenSource();
                container.QueueService.RemoveFirst();
            }
        }

        public async Task AddToQueueAsync(IGuild guild, string url, float pitch = 1, int offset = 0)
        {
            if (_connectedChannels.TryGetValue(guild.Id, out AudioContainer container))
            {
                IAudioService service = AudioServiceFactory.CreateInstance(url);

                var streamUrl = await service.GetStreamUrlAsync(url);
                var title = await service.GetTitleAsync(url);

                container.QueueService.Add(title, streamUrl, pitch, offset);
                if (container.QueueService.Count == 1)
                {
                    var item = container.QueueService.GetFirstOrDefault();
                    await SendAudioAsync(guild, item.Url, item.Rate, item.Offset);
                }
            }
        }

        private async Task SendAudioRecursion(IGuild guild, IMessageChannel channel, IVoiceChannel voiceChannel)
        {
            _connectedChannels.TryGetValue(guild.Id, out AudioContainer container);
            while (!container.QueueService.isEmpty)
            {
                var queueItem = container.QueueService.GetFirstOrDefault();
                await SendAudioAsync(guild, queueItem.Url, (int)(queueItem.Rate * queueItem.PitchMul), queueItem.Offset);
            }
            await TryLeaveAudioAsync(guild, voiceChannel);
        }

        public bool IsBotAlreadyPlaying(IGuild guild)
        {
            return _connectedChannels.TryGetValue(guild.Id, out AudioContainer container);
        }

        public async Task StartAsync(IVoiceChannel voiceChannel, IGuild guild, IMessageChannel channel, string url, float pitch = 1, int offset = 0)
        {
            if (await JoinAudio(voiceChannel, channel))
            {
                await AddToQueueAsync(guild, url, pitch, offset);
                await SendAudioRecursion(guild, channel, voiceChannel);
            }
        }

        public string GetQueue(IGuild guild)
        {
            if (IsBotAlreadyPlaying(guild))
            {
                _connectedChannels.TryGetValue(guild.Id, out AudioContainer container);
                return container.QueueService.Print();
            }
            return string.Empty;
        }

        public void RemoveFirstQueueItem(IGuild guild)
        {
            if (IsBotAlreadyPlaying(guild))
            {
                _connectedChannels.TryGetValue(guild.Id, out AudioContainer container);
                container.QueueService.RemoveFirst();
            }
        }

        public async Task SkipQueueItemAsync(IGuild guild, IMessageChannel messageChannel)
        {
            if (_connectedChannels.TryGetValue(guild.Id, out AudioContainer container))
            {
                container.CancellationTokenSource.Cancel();
            }
            else
            {
                await messageChannel.SendMessageAsync($"__Nothing is playing now__");
            }
        }

        public async Task ForwardTo(IGuild guild, IMessageChannel messageChannel, int offset)
        {
            if (IsBotAlreadyPlaying(guild))
            {
                if (_connectedChannels.TryGetValue(guild.Id, out AudioContainer container))
                {
                    var queueItem = container.QueueService.GetFirstOrDefault();
                    queueItem.Offset = offset;
                    container.QueueService.Put(1, queueItem);
                    await SkipQueueItemAsync(guild, messageChannel);
                    await messageChannel.SendMessageAsync($"__Music forwarded__");
                }
                else
                {
                    await messageChannel.SendMessageAsync($"__Nothing to forward__");
                }
            }
        }

        public async Task<List<SearchItem>> GetSearchItemsAsync(string searchText)
        {
            return await _searchService.GetSearchItemsAsync(searchText);
        }
    }
}