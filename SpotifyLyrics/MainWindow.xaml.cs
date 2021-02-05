using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GeniusAndSpotify;
using Newtonsoft.Json;

namespace SpotifyLyrics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        private void Refresh(object sender, MouseButtonEventArgs e)
        {
            Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json"));
            if (settings.GeniusToken == "")
                settings.GeniusToken = "aiKi_0g7GTxx8ik1dzl9F679S5T1lib5tuz6t_OttjvYRFeLDiPANPM5QXWcx_Uo";

            string song = Lyrics.CurrentlyPlaying();
            Intro.Visibility = Visibility.Hidden;
            Error.Visibility = Visibility.Hidden;
            LyricsText.Visibility = Visibility.Hidden;
            Avatar.Visibility = Visibility.Hidden;
            SongName.Visibility = Visibility.Hidden;

            if (song == "Spotify Premium" || song == "Spotify")
            {
                Error.Visibility = Visibility.Visible;
                return;
            }

            SongName.Visibility = Visibility.Visible;
            Avatar.Visibility = Visibility.Visible;

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
                    Lyrics.DownloadSongAvatar(songInfo);
                    string FileName = new Random().Next(100000).ToString();
                    File.Copy("SongAvatar.png", $"{System.IO.Path.GetTempPath()}{FileName}.png");
                    BitmapImage bitmap = new BitmapImage(new Uri($"{System.IO.Path.GetTempPath()}{FileName}.png", UriKind.RelativeOrAbsolute));
                    Avatar.Source = bitmap;

                    ListOfSongs listOfSongs = new ListOfSongs();
                    Song song1 = new Song();
                    song1.SongName = song;
                    song1.LyricsText = lyrics;
                    song1.AvatarUrl = songInfo.response.hits[0].result.song_art_image_url;

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
                        Lyrics.DownloadSongAvatar(songInfo);
                        string FileName = new Random().Next(100000).ToString();
                        File.Copy("SongAvatar.png", $"{System.IO.Path.GetTempPath()}{FileName}.png");
                        BitmapImage bitmap = new BitmapImage(new Uri($"{System.IO.Path.GetTempPath()}{FileName}.png", UriKind.RelativeOrAbsolute));
                        //Avatar.Source = bitmap;

                        Song song1 = new Song();
                        song1.SongName = song;
                        song1.LyricsText = lyrics;
                        song1.AvatarUrl = songInfo.response.hits[0].result.song_art_image_url;
                        list.Songs.Add(song1);
                        File.WriteAllText("Songs.json", JsonConvert.SerializeObject(list));
                    }
                }
            }
            else
            {
                SongInfo songInfo = Lyrics.SearchSong(song, settings.GeniusToken);
                lyrics = Lyrics.GetLyrics(songInfo);
                Lyrics.DownloadSongAvatar(songInfo);
                string FileName = new Random().Next(100000).ToString();
                File.Copy("SongAvatar.png", $"{System.IO.Path.GetTempPath()}{FileName}.png");
                BitmapImage bitmap = new BitmapImage(new Uri($"{System.IO.Path.GetTempPath()}{FileName}.png", UriKind.RelativeOrAbsolute));
                Avatar.Source = bitmap;
            }
            LyricsText.Text = lyrics;
            if (LyricsText.Text.Length > 10) 
            { 
                LyricsText.Visibility = Visibility.Visible; 
            }
            else
            {
                Error.Visibility = Visibility.Visible;
                return;
            }
        }

        private void OpenSettings(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainWin.Width <= 700)
            {
                LyricsText.FontSize = 16;
                SongName.FontSize = 12;
                InfoBlock.Height = new GridLength(50);
            }
            if (MainWin.Width > 700 && MainWin.Width <= 800)
            {
                LyricsText.FontSize = 18;
                SongName.FontSize = 14;
                InfoBlock.Height = new GridLength(60);
            }
            if (MainWin.Width > 800 && MainWin.Width <= 900)
            {
                LyricsText.FontSize = 20;
                SongName.FontSize = 16;
                InfoBlock.Height = new GridLength(70);
            }
            if (MainWin.Width > 900 && MainWin.Width <= 1000)
            {
                LyricsText.FontSize = 22;
                SongName.FontSize = 18;
                InfoBlock.Height = new GridLength(80);
            }
            if (MainWin.Width > 1000 && MainWin.Width <= 1100)
            {
                LyricsText.FontSize = 24;
                SongName.FontSize = 20;
                InfoBlock.Height = new GridLength(90);
            }
            if (MainWin.Width > 1100 && MainWin.Width <= 1200)
            {
                LyricsText.FontSize = 26;
                SongName.FontSize = 22;
                InfoBlock.Height = new GridLength(100);
            }
        }
    }
}
