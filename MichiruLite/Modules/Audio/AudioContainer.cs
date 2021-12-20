using Discord.Audio;
using NAudio.Wave;
using System.Threading;

namespace MichiruLite.Modules.Audio
{
    public class AudioContainer
    {
        public IAudioClient AudioClient { get; set; }

        public AudioOutStream AudioOutStream { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public ResamplerDmoStream ResamplerDmoStream { get; set; }

        public QueueService QueueService { get; set; }
    }
}