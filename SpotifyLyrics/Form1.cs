using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace SpotifyLyrics
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
            if (File.ReadAllText("Settings.json") != "")
            {
                Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json"));
                AccessToken.Text = settings.GeniusToken;
                SaveBox.Checked = settings.SaveLyricsToFile;
            }
        }

        private void SaveBtn_MouseDown(object sender, MouseEventArgs e)
        {
            Settings settings = new Settings();
            settings.GeniusToken = AccessToken.Text;
            settings.SaveLyricsToFile = SaveBox.Checked;

            try
            {
                string json = JsonConvert.SerializeObject(settings);
                File.WriteAllText("Settings.json", json);
                this.Close();
            }
            catch
            {

                throw;
            }
            
        }
    }
}
