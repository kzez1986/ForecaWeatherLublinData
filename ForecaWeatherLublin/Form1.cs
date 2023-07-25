using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace ForecaWeatherLublin
{
    public partial class foPogodaLublin : Form
    {
        PictureBox[] pb = new PictureBox[10];
        Label[,] data = new Label[10, 5];
        WebClient client = new WebClient();

        public foPogodaLublin()
        {
            InitializeComponent();
            CreateDynamicComponents();
            DownloadData();
            GetData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            Left = 0;
            Top = 0;
        }

        public void GetWeather()
        {

        }

        public void DownloadData()
        {
            
        }

        public void CreateDynamicComponents()
        {
            for(int i=0;i<10;i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    data[i, j] = new Label();
                    data[i, j].Name = "Dana" + i.ToString() + j.ToString();
                    //if (j == 0) 
                    data[i, j].Top = 200 + j * 20;
                    //else data[i, j].Top = 260 + j * 20;
                    data[i, j].Visible = true;
                    data[i, j].AutoSize = true;
                    data[i, j].Left = i * 120;
                    data[i, j].TextAlign = ContentAlignment.TopCenter;
                    data[i, j].BackColor = Color.Transparent;
                    data[i, j].ForeColor = Color.Yellow;
                    data[i, j].Font = new Font("Courier", 9, FontStyle.Bold);
                    this.Controls.Add(data[i, j]);
                }
                
                /*
                pb[i] = new PictureBox();
                pb[i].Top = 220;
                pb[i].Width = 50;
                pb[i].Height = 50;
                pb[i].Left = data[i,0].Left;
                pb[i].BackColor = Color.Transparent;
                */
            }
        }

        public string ConvertWeekDay(string day)
        {
            switch(day)
            {
                case "Mon":
                    return "PN";
                case "Tue":
                    return "WT";
                case "Wed":
                    return "ŚR";
                case "Thu":
                    return "CZ";
                case "Fri":
                    return "PT";
                case "Sat":
                    return "SB";
                case "Sun":
                    return "ND";
                default:
                    return "Unknown";
            }
                
        }

        public void stripTags(StreamReader cur, string outFileName)
        {
            StreamWriter out_file = new StreamWriter(outFileName);
            bool tag = false;
            char znak;

            while (!cur.EndOfStream)
            {
                znak = (char)cur.Read();
                if (znak == '<')
                    tag = true;
                if (znak == '>')
                {
                    tag = false;
                }

                if (znak != '>' && tag == false) out_file.Write(znak);
            }

            out_file.Close();
        }

        public void GetData()
        {
            try
            {
                client.DownloadFile("http://www.foreca.com/Poland/Lublin", "current.txt");
                client.DownloadFile("http://www.foreca.com/Poland/Lublin?tenday", "tenday.txt");
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem z podłączeniem do serwera. Sprawdź połączenie z Internetem lub poczekaj jakiś czas aż serwer będxzie znowu dostępny. Dopóki problem nie zostanie rozwiązany, dane nie zostały zaktualizowane.");
                return;
            }
            StreamReader cur = new StreamReader("current.txt");
            string s;
            int indeks = -1;

            stripTags(cur, "stripped_current.txt");
            cur.Close();
            cur.Dispose();

            cur = new StreamReader("stripped_current.txt");

            //warunki bieżące
            do
            {
                s = cur.ReadLine();
                indeks = s.IndexOf("HistoryLublin");
            }
            while (indeks == -1);

            cur.Close();
            cur.Dispose();

            //temperatura
            s = s.Substring(indeks + 13);
            indeks = s.IndexOf("&");
            int temp = int.Parse(s.Substring(0, indeks));
            
            if (temp > 30) laTemperatura.ForeColor = Color.DarkRed;
            else if (temp > 25 && temp <= 30) laTemperatura.ForeColor = Color.Red;
            else if (temp > 20 && temp <= 25) laTemperatura.ForeColor = Color.OrangeRed;
            else if (temp > 15 && temp <= 20) laTemperatura.ForeColor = Color.Orange;
            else if (temp > 10 && temp <= 15) laTemperatura.ForeColor = Color.Yellow;
            else if (temp > 5 && temp <= 10) laTemperatura.ForeColor = Color.YellowGreen;
            else if (temp > 0 && temp <= 5) laTemperatura.ForeColor = Color.Green;
            else if (temp > -5 && temp <= 0) laTemperatura.ForeColor = Color.LightSkyBlue;
            else if (temp > -10 && temp <= -5) laTemperatura.ForeColor = Color.SkyBlue;
            else if (temp > -15 && temp <= -10) laTemperatura.ForeColor = Color.DeepSkyBlue;
            else laTemperatura.ForeColor = Color.Blue;
            laTemperatura.Text = s.Substring(0, indeks) + "°C";
            
            //temperatura odczuwalna
            indeks = s.IndexOf("Feels like");
            s = s.Substring(indeks + 11);
            indeks = s.IndexOf("&");
            laOdczuwalna.Text = s.Substring(0, indeks) + "°C";

            //prędkość wiatru
            indeks = s.IndexOf("mph");
            s = s.Substring(indeks + 4);
            indeks = s.IndexOf(" ");
            laPredkoscWiatru.Text = s.Substring(0, indeks) + "km/h";

            //wiatr w porywach
            indeks = s.IndexOf("mph");
            s = s.Substring(indeks + 4);
            indeks = s.IndexOf(" ");
            laWiatrWPorywach.Text = s.Substring(0, indeks) + "km/h";

            //opad
            double opad;
            indeks = s.IndexOf("Bft");
            s = s.Substring(indeks + 3);
            int counter = 0;
            while(!(s[counter] >= '0' && s[counter] <= '9'))
            {
                counter++;
            }
            indeks = counter;
            s = s.Substring(indeks);
            indeks = s.IndexOf(" ");
            LaOpad.Text = s.Substring(0, indeks) + "mm";

            //wilgotność
            indeks = s.IndexOf("Rain");
            s = s.Substring(indeks + 4);
            indeks = s.IndexOf("%");
            laWilgotność.Text = s.Substring(0, indeks) + "%";

            //ciśnienie
            indeks = s.IndexOf("hum.");
            s = s.Substring(indeks + 4);
            indeks = s.IndexOf(" ");
            laCiśnienie.Text = s.Substring(0, indeks) + "hpa";

            //wschód słońca
            indeks = s.IndexOf("AM");
            s = s.Substring(indeks + 3);
            indeks = s.IndexOf("S");
            laWschódSłońca.Text = s.Substring(0, indeks);

            //zachód słońca
            indeks = s.IndexOf("PM");
            s = s.Substring(indeks + 3);
            indeks = s.IndexOf("S");
            laZachódSłońca.Text = s.Substring(0, indeks);

            //Prognoza na nastęne dni
            StreamReader ten = new StreamReader("tenday.txt");

            stripTags(ten, "stripped_ten_days.txt");
            ten.Close();
            ten.Dispose();

            ten = new StreamReader("stripped_ten_days.txt");

            indeks = -1;
            do
            {
                s = ten.ReadLine();
                indeks = s.IndexOf("WeatherObservations Observation History");
            }
            while (indeks == -1);

            ten.Close();

            indeks = s.IndexOf("Lublin 10 day forecast");
            
            indeks += 22; //Ustawiamy się na 1 dniu prognozy
            s = s.Substring(indeks);

            for (int i = 0; i < 10; i++)
            {
                //dzień tygodnia i data
                string dzienTygodnia = ConvertWeekDay(s.Substring(0, 3));
                indeks = s.IndexOf(" ") + 1;
                s = s.Substring(indeks);
                string prognozaNaDzien = s.Substring(0, 5);
                prognozaNaDzien = prognozaNaDzien.Replace('/', '.');
                data[i, 0].Text = dzienTygodnia + " " + prognozaNaDzien;

                //maksymalna temperatura
                indeks = s.IndexOf(" ") + 1;
                s = s.Substring(indeks);
                indeks = s.IndexOf("&");
                data[i, 1].Text = "Max: " + s.Substring(0, indeks) + "°C";

                //minimalna temperatura
                indeks = s.IndexOf(" ") + 1;
                s = s.Substring(indeks);
                indeks = s.IndexOf("&");
                data[i, 2].Text = "Min: " + s.Substring(0, indeks) + "°C";

                //prędkość wiatru
                indeks = s.IndexOf("m/s") + 4;
                s = s.Substring(indeks);
                indeks = s.IndexOf(" ");
                data[i, 3].Text = "Wiatr: " + s.Substring(0, indeks) + "km/h";

                //opad
                indeks = s.IndexOf("in") + 3;
                s = s.Substring(indeks);
                indeks = s.IndexOf("mm");
                string opad_str = s.Substring(0, indeks);
                opad_str = opad_str.Replace(" ", "");
                opad_str = opad_str.Replace("&lt;", "< ");
                data[i, 4].Text = "Opad: " + opad_str + " mm";

                //ustawienie na kolejny dzień
                indeks += 2;
                s = s.Substring(indeks);
            }

        }

        private void foPogodaLublin_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void foPogodaLublin_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetData();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
