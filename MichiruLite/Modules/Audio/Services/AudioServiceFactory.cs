using MichiruLite.Modules.Audio.Exceptions;
using System.Text.RegularExpressions;

namespace MichiruLite.Modules.Audio.Services
{
    public static class AudioServiceFactory
    {
        public static IAudioService CreateInstance(string url)
        {
            var youtubeRegex = new Regex(@"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");
            bool isYoutubeVideoUrl = youtubeRegex.IsMatch(url);
            if (isYoutubeVideoUrl)
            {
                return new YoutubeService();
            }

            throw new UnableToParseException();
        }
    }
}