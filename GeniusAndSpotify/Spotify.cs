using System;
using xNet;
using Newtonsoft.Json;
using CsQuery;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace GeniusAndSpotify
{
    public class Lyrics
    {

        private static string GetErrorHtml(string ErrorText)
        {
            return $@"<!doctype html>
                     <html lang = {"ru"}>
                     <head>
                       <meta charset = {"utf-8"}/>
                       <title></title>
                       <link rel = {"stylesheet"} href = {"style.css"}/>
                     </head>
                     <body>
                       <div class = {"lyrics"}>
                         <p>{ErrorText}</p>
                       </div>
                     </body>
                     </html>";
        }
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
                song = "Temperance - Alive Again";
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
        public static string GetLyrics(SongInfo SongInfo)
        {
            string Url = SongInfo.response.hits[0].result.url;
            using (var request = new HttpRequest())
            {
                try
                {
                    string html = request.Get(Url).ToString();
                    File.WriteAllText($"{Path.GetTempPath()}/temp.html", html);
                }
                catch (xNet.HttpException Exception)
                {
                    File.WriteAllText($"{Path.GetTempPath()}/temp.html", GetErrorHtml(Exception.Message));

                }
            }
            CQ cq = CQ.CreateFromFile($"{Path.GetTempPath()}temp.html");
            var lyrics = cq.Find("div.lyrics p").Text();
            if (lyrics == null)
                return "lyrics = null";
            if (cq.Find("div.lyrics p").Text() == "")
            {
                string text = cq.Find("div.Lyrics__Container-sc-1ynbvzw-2.jgQsqn").Text();
                string result = "";
                for (int i = 0; i < text.Length; i++)
                {
                    if (Char.IsUpper(text, i))
                        result += "\n" + text[i];
                    else
                        result += text[i];
                }
                lyrics = result;
            }
            return lyrics;
        }

        public static void DownloadSongAvatar(SongInfo songInfo)
        {
            string Url = songInfo.response.hits[0].result.song_art_image_url;

            try
            {
                using (WebClient web = new WebClient())
                {
                    web.DownloadFile(Url, "SongAvatar.png");
                }
            }
            catch (Exception e)
            {
                //Random rd = new Random();
                //File.WriteAllText($"/Logs/{DateTime.Now}_{rd.Next(5000)}.txt", $"{e.Message}\n{e.Source}");
            }
        }
    }
}

