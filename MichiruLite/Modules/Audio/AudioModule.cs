using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using MichiruLite.Modules.Audio.Exceptions;

namespace MichiruLite.Modules.Audio
{
    public class AudioModule : InteractiveBase
    {
        private readonly AudioService _audioService;
        private const string _errMsg = "This command must be ran from within a server";

        public AudioModule(AudioService audioService)
        {
            _audioService = audioService;
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("play")]
        [Alias("p")]
        public async Task ExecutePlayCommand(params string[] query)
        {
            if (query.Length == 0)
            {
                await ReplyAsync("$p [URL] [pitch]\n$p [Search query]");
                return;
            }
            float pitch;
            float.TryParse(query.Last(), out pitch);
            var url = string.Join(" ", query);
            try
            {
                await PlayAsync(url, pitch);
            }
            catch (UnableToParseException e)
            {
                var searchResults = await _audioService.GetSearchItemsAsync(url);
                var sb = new StringBuilder();
                sb.Append("__Search results:\n");
                var i = 1;
                foreach (var item in searchResults)
                {
                    sb.Append($"[{i++}] {item.Title} {item.Author} [{item.Duration.ToString()}]\n");
                }
                sb.Append("Choose from list.__");
                var message = await ReplyAsync(sb.ToString());
                var response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(20));
                int index = 0;
                await message.DeleteAsync();
                if (response != null && (index = Int32.Parse(response.Content))! > 0 && index < 6)
                {
                    await ReplyAsync($"Selected video: {searchResults[index - 1].Title}.");
                    await PlayAsync(searchResults[index - 1].Url);
                }
                else
                    await ReplyAsync("You did not reply before the timeout or replay was incorrect");
            }
        }

        private async Task PlayAsync(string url, float pitch = 1, int offset = 0)
        {
            if (!_audioService.IsBotAlreadyPlaying(Context.Guild))
                await _audioService.StartAsync((Context.User as IGuildUser)?.VoiceChannel, Context.Guild, Context.Channel, url, pitch, offset);
            else
                await _audioService.AddToQueueAsync(Context.Guild, url, pitch, offset);
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("leave")]
        [Alias("l")]
        public async Task LeaveAsync()
        {
            var text = string.Empty;
            if (await _audioService.TryLeaveAudioAsync(Context.Guild, (Context.User as IGuildUser)?.VoiceChannel))
                text = $"__Bot disconnected from audio channel__";
            else
                text = $"__Bot has not joined to audio channel yet__";
            await ReplyAndDeleteAsync(text, false, null, TimeSpan.FromSeconds(10));
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("queue")]
        [Alias("q")]
        public async Task PrintQueue()
        {
            if (_audioService.IsBotAlreadyPlaying(Context.Guild))
            {
                var queue = _audioService.GetQueue(Context.Guild);
                if (!string.IsNullOrEmpty(queue))
                {
                    await ReplyAsync(queue);
                }
                else
                    await ReplyAsync("__Queue is empty__");
            }
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("skip")]
        [Alias("s")]
        public async Task SkipQueueItem()
        {
            if (_audioService.IsBotAlreadyPlaying(Context.Guild))
            {
                await _audioService.SkipQueueItemAsync(Context.Guild, Context.Channel);
                await ReplyAsync("__Skipped.__");
            }
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("help")]
        [Alias("h")]
        public async Task Help()
        {
            await ReplyAsync("___$p [link] | [query] - play/add song\n$s - skip song \n$f [second] - move to [second] second\n$q - show queue___");
        }

        [RequireContext(ContextType.Guild, ErrorMessage = _errMsg)]
        [Command("forward")]
        [Alias("f")]
        public async Task ForwardSong(int offset)
        {
            if (_audioService.IsBotAlreadyPlaying(Context.Guild))
            {
                await _audioService.ForwardTo(Context.Guild, Context.Channel, offset);
                await ReplyAsync($"__Moved into {offset} second.__");
            }
        }
    }
}