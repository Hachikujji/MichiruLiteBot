using System;

namespace MichiruLite.Modules.Audio.Services
{
    public class SearchItem
    {
        public string Title { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Author { set; get; }
        public string Url { get; set; }
    }
}