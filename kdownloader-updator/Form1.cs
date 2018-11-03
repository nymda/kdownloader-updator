using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kdownloader_updator
{
    public partial class updater : Form
    {

        public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string location = "";
        public string oldID = "";

        public updater()
        {
            InitializeComponent();
        }

        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var req = base.GetWebRequest(address);
                req.Timeout = 5000;
                return req;
            }
        }

        public void log(string t)
        {
            textBox1.AppendText(t + "\n");
        }

        public bool checkforupdate()
        {
            using (MyWebClient client = new MyWebClient())
            {
                byte[] dat = client.DownloadData("http://knedit.pw/software/updateID");
                string uids = System.Text.Encoding.UTF8.GetString(dat);
                int uid;
                Int32.TryParse(uids, out uid);
                if (uid > Int32.Parse(oldID))
                {
                    return true;
                }
            }

            return false;
        }

        private void updater_Load(object sender, EventArgs e)
        {
            string[] data = (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + "data.txt").Split(','));
            oldID = data[1];

            if (!checkforupdate())
            {
                log("update not needed.");
                return;
            }

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + "data.txt"))
            {
                log("data file not present");
                button1.Enabled = true;
                return;
            }

            string location = data[0];

            using (MyWebClient client = new MyWebClient())
            {
            log("downloading new version");
            client.DownloadFile(@"http://knedit.pw/software/KDownloader.exe", appdata + "/kdownloader.exe");
            log("removing old version\n");
            File.Delete(location);
            log("replacing files");
            File.Move(appdata + "/kdownloader.exe", location);
            log("starting");
            Process.Start(location);
            log("complete.");
            button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
