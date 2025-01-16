using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerMonitor
{
    public partial class Main : Form
    {
        private bool isMonitoring = false;
        private int time = 30;
        private string csvFile = "url_reports.csv";
        private readonly TextBox[] urlTextBoxes = new TextBox[10];
        private readonly ProgressBar[] progressBar = new ProgressBar[10];

        public Main()
        {
            InitializeComponent();
            urlTextBoxes[0] = URL_TextBox1;
            urlTextBoxes[1] = URL_TextBox2;
            urlTextBoxes[2] = URL_TextBox3;
            urlTextBoxes[3] = URL_TextBox4;
            urlTextBoxes[4] = URL_TextBox5;
            urlTextBoxes[5] = URL_TextBox6;




            progressBar[0] = progressBar1;
            progressBar[1] = progressBar2;
            progressBar[2] = progressBar3;
            progressBar[3] = progressBar4;
            progressBar[4] = progressBar5;
            progressBar[5] = progressBar6;
        }

        private async void Start_Button_Click(object sender, EventArgs e)
        {
            if (int.TryParse(Time_TextBox.Text.Trim(), out time))
            {
                if (time < 5)
                {
                    time = 5;
                }
            }
            else
            {
                // Handle invalid input (you can set a default value or display an error message)
                time = 5; // Default value
            }
            if (isMonitoring)
            {
                isMonitoring = false;
                Start_Button.Text = "Start Monitoring";
                return;
            }

            isMonitoring = true;
            Start_Button.Text = "Stop Monitoring";

            bool writeHeaders = !File.Exists(csvFile);
            string directory = Path.GetDirectoryName(csvFile);
             

            while (isMonitoring)
            {
                for (int i = 0; i < 6; i++)
                {
                    string url = urlTextBoxes[i].Text.Trim();
                    if (string.IsNullOrEmpty(url))
                    {
                        continue; // Skip empty URLs
                    }
                    try
                    {
                        var pingTime = await PingHostAsync(new Uri(url).Host);
                        var (loadingTime, statusCode, responseSize, errorMessage) = await CheckUrlAsync(url);
                       

                        var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        var data = $"{currentTime},{pingTime},{loadingTime},{responseSize},{statusCode},{errorMessage}";

                        using (var sw = new StreamWriter(csvFile, true))
                        {
                            if (writeHeaders)
                            {
                                sw.WriteLine("Date,Ping Time (ms),Loading Time (s),Response Size (bytes),Status Code,Error Message");
                                writeHeaders = false;
                            }
                            sw.WriteLine(data);
                        }
                        if (statusCode == 200)
                        {
                            progressBar[i].BackColor = Color.Green;
                            progressBar[i].Maximum = 100;
                            progressBar[i].Minimum = 0;
                            progressBar[i].Value = (int)NormalizeLoadingTime(pingTime);
                        }
                        else
                        {
                            if (Alert_CheckBox.Checked)
                            {
                                PlaySound();
                            }
                            progressBar[i].BackColor = Color.Red;
                        }
                        textBoxLog.AppendText($"{currentTime} - Ping: {pingTime} ms, Load: {loadingTime} s, Size: {responseSize} bytes, Status: {statusCode}, Error: {errorMessage}{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText($"Error: {ex.Message}{Environment.NewLine}");
                    }
                }

                await Task.Delay(time* 1000); // Wait for 30 seconds
            }
        }

        private async Task<long> PingHostAsync(string hostNameOrAddress)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(hostNameOrAddress);
                    return reply.RoundtripTime;
                }
            }
            catch (PingException ex)
            {
                // Handle ping exception
                textBoxLog.AppendText($"Ping Error: {ex.Message}{Environment.NewLine}");
                return -1; // Return an invalid ping time
            }
        }

        private async Task<(double loadingTime, int? statusCode, long? responseSize, string errorMessage)> CheckUrlAsync(string url)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    var contentLength = response.Content.Headers.ContentLength;
                    var statusCode = (int)response.StatusCode;
                    stopwatch.Stop();

                    return (stopwatch.Elapsed.TotalSeconds, statusCode, contentLength, string.Empty);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return (stopwatch.Elapsed.TotalSeconds, null, null, ex.Message);
            }
        }
        private double NormalizeLoadingTime(double loadingTime)
        {
            // Update min and max values
            double minLoadingTime = Math.Min(0, loadingTime);
            double maxLoadingTime = Math.Max(100, loadingTime);

            // Avoid division by zero if minLoadingTime == maxLoadingTime
            if (minLoadingTime == maxLoadingTime)
            {
                return 100; // In case of no variation, set it to 100
            }

            // Normalize the loading time to be between 0 and 100
            double normalizedLoadingTime = ((loadingTime - minLoadingTime) / (maxLoadingTime - minLoadingTime)) * 100;

            return Math.Min(100, Math.Max(0, normalizedLoadingTime)); // Ensure it's between 0 and 100
        }
        public void PlaySound()
        {
            SoundPlayer player = new SoundPlayer("path_to_sound_file.wav");
            player.Play();  
        }
    }
}
