using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using InoueLab;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundMaker
{
    public partial class Form1 : Form
    {
        TrackBar[] trackBars = new TrackBar[7];

        double[] f0 = { 261.63, 293.66, 329.63, 349.23, 391.1, 440.0, 493.88 }; //C4～B4基本周波数
        double[] freqs = new double[7]; //パワー調整可能周波数
        double[] freq_power = new double[7]; //トラックバーの値格納
        double[] real_freqs; //書き込み用ｆ
        double[] wavdata;

        int chord_flag = 0;
        int fs;
        int fft_length;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "44100";
            textBox2.Text = "1024";
            textBox4.Text = "2";
            fft_length = int.Parse(textBox2.Text);
            fs = int.Parse(textBox1.Text);


            trackBars[0] = trackBar1;
            trackBars[1] = trackBar2;
            trackBars[2] = trackBar3;
            trackBars[3] = trackBar4;
            trackBars[4] = trackBar5;
            trackBars[5] = trackBar6;
            trackBars[6] = trackBar7;
            for (int i = 0; i < trackBars.Length; i++)
                trackBars[i].Enabled = false;
            axWindowsMediaPlayer1.settings.autoStart = false;
        }
        //---------------------------------------------------------------------------
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label9.Text = "";
            for (int i = 0; i < comboBox1.Items.Count; i++)
                if (comboBox1.SelectedIndex == i)
                {
                    chord_flag = i;
                    break;
                }
            for (int i = 0; i < trackBars.Length; i++)
                trackBars[i].Enabled = true;
            setFreqs();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            string output_fullpass;
            //folder dialog
            string output_foldername = "";
            FolderBrowserDialog fdb = new FolderBrowserDialog();
            fdb.Description = "出力フォルダを選択してください";
            fdb.RootFolder = Environment.SpecialFolder.Desktop;
            fdb.ShowNewFolderButton = true;
            if (fdb.ShowDialog(this) == DialogResult.OK)
                output_foldername = fdb.SelectedPath;

            if (textBox3.Text == "")
                MessageBox.Show("出力ファイル名を入力してね", "注意", MessageBoxButtons.YesNo);
            else
            {
                axWindowsMediaPlayer1.URL = "";
                output_fullpass = output_foldername + "\\" + textBox3.Text + ".wav";
                WaveFile.Save(output_fullpass, makeStereo(wavdata), fs);
                axWindowsMediaPlayer1.URL = output_fullpass;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            checkParameter();
            //ifft
            Complex[] spect = new Complex[fft_length + 1];
            for (int n = 0; n < spect.Length-1; n++)
                spect[n] = new Complex(real_freqs[n], 0);
            spect[spect.Length-1] = new Complex(0, 0); //直流成分

            MySignalProcessing.SP.Real_IFFT(spect, out wavdata);
            label9.Text = "IFFT has completed.";
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label9.Text = "";
            if (textBox1.Text != "")
                fs = int.Parse(textBox1.Text);
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label9.Text = "";
            if (textBox2.Text != "")
                fft_length = int.Parse(textBox2.Text);
        }
        //---------------------------------------------------------------------------
        private void setFreqs()
        {
            double f = f0[chord_flag];
            for (int n = 0; n < freqs.Length; n++)
                freqs[n] = f * (n + 1);
        }
        //---------------------------------------------------------------------------
        private void setFreqPower()
        {
            label9.Text = "";
            for (int n = 0; n < trackBars.Length; n++)
                freq_power[n] = trackBars[n].Value * 10; //調整必要
        }
        //---------------------------------------------------------------------------
        private void checkParameter()
        {
            if (fft_length == 0 || fs == 0)
                MessageBox.Show("値を入力してください");
            if (Math.Log(fft_length, 2) != Math.Floor(Math.Log(fft_length, 2)))
                MessageBox.Show("fft_lengthは2の冪乗にしてください");
        }
        //---------------------------------------------------------------------------
        private void drawChart()
        {
            double delta_f = fs / 2.0 / fft_length;
            double[] x = new double[fft_length]; //freq
            double[] y = new double[fft_length]; //power

            for (int n = 0; n < x.Length; n++)
                x[n] = n * delta_f;
            int m = 0;
            for (int n = 0; n < x.Length; n++)
            {

                if (x[n] <= freqs[m] && freqs[m] < x[n + 1])
                {
                    y[n] = freq_power[m];
                    m++;
                    if (m == freqs.Length)
                        break;
                }
            }
            chart1.Series[0].Points.Clear();
            foreach (double element in y)
            {
                chart1.Series[0].Points.Add(new DataPoint(0, element));
            }
            chart1.ChartAreas[0].AxisX.Maximum = 500;

            real_freqs = (double[])y.Clone();
        }
        //---------------------------------------------------------------------------
        private double[][] makeStereo(double[] data)
        {
            double[][] output = new double[2][];
            int data_Length = (int)(fs * double.Parse(textBox4.Text));
            output[0] = new double[data_Length];
            output[1] = new double[data_Length];

            for (int i = 0; i < data_Length; i++)
            {
                output[0][i] = data[i % data.Length];
                output[1][i] = data[i % data.Length];
            }
            return output;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }
        private void trackBar2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }
        private void trackBar3_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }
        private void trackBar4_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }
        private void trackBar5_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }
        private void trackBar6_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }
        private void trackBar7_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            setFreqPower();
            drawChart();
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
        }


    }
}
