using System.Collections.Generic;

namespace SpotifyLyrics
{
    public class ListOfSongs
    {
        public List<Song> Songs { get; set; }
    }

    public class Song
    {
        public string SongName { get; set; }
        public string LyricsText { get; set; }
        public string AvatarUrl { get; set; }
    }
}
