using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusAndSpotify
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
