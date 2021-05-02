using System;
using Newtonsoft.Json;
using Leaf.xNet;
using CsQuery;
using System.Diagnostics;
using System.IO;

namespace GeniusLibrary
{
    public class Lyrics
    {
        public static string CurrentlyPlaying()
        {
            string full_song = "";
            string song = "";

            Process[] processes = Process.GetProcessesByName("Spotify");
            foreach (var item in processes)
            {
                if (item.MainWindowTitle != "")
                {
                    full_song = $"{item.MainWindowTitle}";
                    break;
                }
            }

            int CharCounter = 0;
            foreach (var item in full_song)
            {
                if (item == '-' || item == '(')
                {
                    CharCounter++;
                }

                if (CharCounter <= 1)
                {
                    song += item;
                }
            }

            if (song == "")
            {
                Exception e = new Exception("Restart the song");
                WriteLog(e);
            }

            return song;
        }
        public static SongInfo SearchSong(string Song, string GeniusToken)
        {
            using (var request = new HttpRequest())
            {
                var Params = new RequestParams();

                Params["q"] = Song;
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", $"Bearer {GeniusToken}");

                var response = request.Get("https://api.genius.com/search", Params).ToString();
                SongInfo songURL = JsonConvert.DeserializeObject<SongInfo>(response);

                return songURL;
            }
        }
        //public static void DownloadLyrics(SongInfo SongInfo)
        //{
        //    string Url = SongInfo.response.hits[0].result.url;
        //    using (var request = new HttpRequest())
        //    {
        //        try
        //        {
        //            string html = request.Get(Url).ToString();
        //            File.WriteAllText($"{Path.GetTempPath()}/temp.html", html);
        //        }
        //        catch (HttpException e)
        //        {
        //            WriteLog(e);
        //        }
        //    }
        //}

        public static string GetLyrics(SongInfo SongInfo)
        {
            //DownloadLyrics(SongInfo);
            //CQ cq = CQ.CreateFromFile($"{Path.GetTempPath()}temp.html");
            CQ cq = CQ.CreateFromUrl($"{SongInfo.response.hits[0].result.url}");
            var lyrics = cq.Find("div.lyrics p").Text();
            if (lyrics == "")
            {
                while (lyrics == "")
                {
                    //DownloadLyrics(SongInfo);
                    //cq = CQ.CreateFromFile($"{Path.GetTempPath()}temp.html");
                    cq = CQ.CreateFromUrl($"{SongInfo.response.hits[0].result.url}");
                    lyrics = cq.Find("div.lyrics p").Text();
                }
            }
            return lyrics;
        }

        private static void WriteLog(Exception e)
        {
            File.WriteAllText($"/Logs/{DateTime.Now}.txt", $"{e.Message}\n{e.Source}");
        }
    }
}
