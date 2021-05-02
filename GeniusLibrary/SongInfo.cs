using System.Collections.Generic;

namespace GeniusLibrary
{
    public class Result
    {
        public string url { get; set; }
    }

    public class Hit
    {
        public Result result { get; set; }
    }

    public class Response
    {
        public List<Hit> hits { get; set; }
    }

    public class SongInfo
    {
        public Response response { get; set; }
    }
}
