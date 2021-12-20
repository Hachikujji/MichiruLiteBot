using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MichiruLite.Modules.Audio.Services
{
    public interface IAudioService
    {
        public Task<string> GetStreamUrlAsync(string url);

        public Task<string> GetTitleAsync(string url);

        public Task<List<SearchItem>> GetSearchItemsAsync(string searchText);
    }
}