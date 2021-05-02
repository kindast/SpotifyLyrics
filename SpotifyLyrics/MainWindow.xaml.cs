using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Newtonsoft.Json;
using GeniusLibrary;

namespace SpotifyLyrics
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainWin.MaxHeight = SystemParameters.PrimaryScreenHeight - 50;
            MainWin.MaxWidth = SystemParameters.PrimaryScreenWidth - 50;

            if (!File.Exists("Settings.json"))
            {
                Settings settings = new Settings();
                settings.GeniusToken = "";
                settings.SaveLyricsToFile = false;
                File.WriteAllText("Settings.json", JsonConvert.SerializeObject(settings));
            }
            if (!File.Exists("Songs.json"))
                File.Create("Songs.json");
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MoveWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
       
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json"));
            if (settings.GeniusToken == "")
                settings.GeniusToken = "aiKi_0g7GTxx8ik1dzl9F679S5T1lib5tuz6t_OttjvYRFeLDiPANPM5QXWcx_Uo";

            string song = Lyrics.CurrentlyPlaying();
            LyricsText.Visibility = Visibility.Hidden;
            SongName.Visibility = Visibility.Hidden;

            if (song == "Spotify Premium" || song == "Spotify")
                return;

            MainWin.Title = song;

            SongName.Visibility = Visibility.Visible;
            SongName.Content = song;
            string lyrics = "";

            if (settings.SaveLyricsToFile)
            {
                string json = File.ReadAllText("Songs.json");

                ListOfSongs list = JsonConvert.DeserializeObject<ListOfSongs>(json);
                if (list == null)
                {
                    SongInfo songInfo = Lyrics.SearchSong(song, settings.GeniusToken);
                    lyrics = Lyrics.GetLyrics(songInfo);

                    ListOfSongs listOfSongs = new ListOfSongs();
                    Song song1 = new Song();
                    song1.SongName = song;
                    song1.LyricsText = lyrics;

                    listOfSongs.Songs = new List<Song>();
                    listOfSongs.Songs.Add(song1);
                    File.WriteAllText("Songs.json", JsonConvert.SerializeObject(listOfSongs));
                }
                else
                {
                    bool IsFinded = false;
                    for (int i = 0; i < list.Songs.Count; i++)
                    {
                        if (list.Songs[i].SongName == song)
                        {
                            lyrics = list.Songs[i].LyricsText;
                            IsFinded = true;
                            break;
                        }
                    }

                    if (!IsFinded)
                    {
                        SongInfo songInfo = Lyrics.SearchSong(song, settings.GeniusToken);
                        lyrics = Lyrics.GetLyrics(songInfo);

                        Song song1 = new Song();
                        song1.SongName = song;
                        song1.LyricsText = lyrics;
                        list.Songs.Add(song1);
                        File.WriteAllText("Songs.json", JsonConvert.SerializeObject(list));
                    }
                }
            }
            else
            {
                SongInfo songInfo = Lyrics.SearchSong(song, settings.GeniusToken);
                lyrics = Lyrics.GetLyrics(songInfo);
            }
            LyricsText.Text = lyrics;
            if (LyricsText.Text.Length > 10)
            {
                LyricsText.Visibility = Visibility.Visible;
            }
            else
                return;
        }
    }
}
