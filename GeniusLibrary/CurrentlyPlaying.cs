using System.Collections.Generic;

namespace GeniusLibrary

{
    public class CurrentlyPlaying
    {
        public Item item { get; set; }
    }
    public class Item
    {
        public IList<Artist> artists { get; set; }
        public string name { get; set; }

    }
    public class Artist
    {
        public string name { get; set; }
    }

}
