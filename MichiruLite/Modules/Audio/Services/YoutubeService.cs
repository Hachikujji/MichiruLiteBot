using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MichiruLite.Modules.Audio.Services
{
    public class YoutubeService : IAudioService
    {
        private static YoutubeClient _youtubeExplode = new YoutubeClient();

        public async Task<string> GetStreamUrlAsync(string url)
        {
            var manifest = await _youtubeExplode.Videos.Streams.GetManifestAsync(url);
            var stream = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            return stream.Url;
        }

        public async Task<string> GetTitleAsync(string url)
        {
            var metadata = await _youtubeExplode.Videos.GetAsync(url);
            return metadata.Title;
        }

        public async Task<List<SearchItem>> GetSearchItemsAsync(string searchText)
        {
            var searchResult = _youtubeExplode.Search.GetVideosAsync(searchText);
            var searchResults = await searchResult.Take(5).ToListAsync();
            var searchItems = new List<SearchItem>();
            foreach (var item in searchResults)
            {
                var newItem = new SearchItem()
                {
                    Author = item.Author.Title,
                    Duration = item.Duration,
                    Title = item.Title,
                    Url = item.Url,
                };
                searchItems.Add(newItem);
            }
            return searchItems;
        }
    }
}