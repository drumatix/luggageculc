using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace luggageculc
{
    public partial class Form1 : Form
    {
        DateTime dat1, dat4, now1B;
        TimeSpan dat2, dat3, wts, s2t, t2s, datA, datB, now1A, ragA;
        DateTime[] sts = new DateTime[481];
        int[] rett;
        string[] s2;
        string now1;
        int nv1, nv2, fbpc;
        decimal rag;

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface Imbba
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }
        public Form1()
        {
            InitializeComponent();
            this.TransparencyKey = Color.Magenta;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            FileInfo fim = new FileInfo(@"sPA_data.txt");
            if (File.Exists(@"sPA_data.txt") && fim.Length > 0)
            {
                writefile();
            }
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            int nuv2 = (int)numericUpDown2.Value;
            if (nuv2 < 59 || nuv2 < -59) { nv2 = nuv2; }
            if (nuv2 >= 59) { numericUpDown2.Value = 59; }
            if (nuv2 <= -59) { numericUpDown2.Value = -59; }
            dat3 = new TimeSpan(0, nv1, nv2);
            writefile();
            re_start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            decimal ay = numericUpDown3.Value * 10000;
            int au = (int)ay;
            s2[5] = au.ToString();
            writefile();
            re_start();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("working");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int nuv1 = (int)numericUpDown1.Value;
            if (nuv1 < 59 || nuv1 < -59) { nv1 = nuv1; }
            if (nuv1 >= 59) { numericUpDown1.Value = 59; }
            if (nuv1 <= -59) { numericUpDown1.Value = -59; }
            dat3 = new TimeSpan(0, nv1, nv2);
            writefile();
            re_start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            {
                if (textBox2.Text == "日時を枠に入れて下さい" || textBox2.Text == ".")
                {
                    return;
                }
                string[] charsToRemove = new string[] { "/", ":", " " };
                foreach (var c in charsToRemove)
                {
                    if (now1 == null) { return; }
                    now1 = now1.Replace(c, string.Empty);

                }
                s2[2] = now1;
                s2[3] = rett[1].ToString("D2") + rett[2].ToString("D2") + rett[3].ToString("D2");
                writefile();
                re_start();
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            {
                Bitmap bitmap = new Bitmap(140, 42);
                Graphics g = Graphics.FromImage(bitmap);
                Point p = panel1.PointToScreen(new Point(0, 0));
                g.CopyFromScreen(new Point(p.X, p.Y), new Point(0, 0), bitmap.Size);
                Bitmap bmpOrijinal = new Bitmap(bitmap);
                int scale = 2;
                Bitmap bmpResize = new Bitmap(bmpOrijinal, bmpOrijinal.Width * scale, bmpOrijinal.Height * scale);
                SoftwareBitmap y = Mswb(bmpResize);
                OcrResult t = await RecognizeText(y);
                string tt = t.Text;
                rett = Regex.Matches(tt, "[0-9]+")
                     .Cast<Match>()
                     .Select(m => int.Parse(m.Value))
                     .ToArray();
                if (rett.Length == 4)
                {
                    textBox2.Text = rett[0].ToString() + "日" + rett[1].ToString() + "時間" + rett[2].ToString() + "分" + rett[3].ToString() + "秒";
                }
                else
                {
                    textBox2.Text = "日時を枠に入れて下さい";
                    return;
                }
                now1A = new TimeSpan((-5 + rett[0]), 0, 0, 0);
                now1B = DateTime.Now + now1A;
                now1 = now1B.ToString("yyyy/MM/dd HH:mm:ss");
                textBox1.Text = now1;
                g.Dispose();
            }
        }
        private void Form1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Console.WriteLine("working");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            this.Text = now.ToString("MM/dd (ddd)      H : m : s");

            for (int ii = 1; ii <= 480; ii++)
            {
                if (ii % 2 == 0)
                {
                    if (now > sts[ii] - wts - s2t && now < sts[ii] - wts)
                    {
                        label1.Text = ("ツインクラウンへ航行中");
                        string fix = (sts[ii] - wts - now).ToString();
                        string fix2 = fix.Substring(3, 5);
                        label2.Text = fix2;
                        string[] charsToRemove = new string[] { ":" };
                        foreach (var c in charsToRemove)
                        {
                            fix2 = fix2.Replace(c, string.Empty);
                        }
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = 1021;
                        progressBar1.Value = 1021 - int.Parse(fix2);
                        break;
                    }
                    if (now > sts[ii] - wts && now < sts[ii])
                    {
                        label1.Text = ("ツインクラウンに停泊中");
                        string fix = (sts[ii] - now).ToString();
                        string fix2 = fix.Substring(3, 5);
                        label2.Text = fix2;
                        string[] charsToRemove = new string[] { ":" };
                        foreach (var c in charsToRemove)
                        {
                            fix2 = fix2.Replace(c, string.Empty);
                        }
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = 2000;
                        progressBar1.Value = int.Parse(fix2);
                        break;
                    }
                }
                else
                {
                    if (sts[ii] - wts - t2s < now && sts[ii] - wts > now)
                    {
                        label1.Text = ("サンライズ半島へ航行中");
                        string fix = (sts[ii] - wts - now).ToString();
                        string fix2 = fix.Substring(3, 5);
                        label2.Text = fix2;
                        string[] charsToRemove = new string[] { ":" };
                        foreach (var c in charsToRemove)
                        {
                            fix2 = fix2.Replace(c, string.Empty);
                        }
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = 1047;
                        progressBar1.Value = 1047 - int.Parse(fix2);
                        break;
                    }
                    if (now > sts[ii] - wts && now < sts[ii])
                    {
                        label1.Text = ("サンライズ半島に停泊中");
                        string fix = (sts[ii] - now).ToString();
                        string fix2 = fix.Substring(3, 5);
                        label2.Text = fix2;
                        string[] charsToRemove = new string[] { ":" };
                        foreach (var c in charsToRemove)
                        {
                            fix2 = fix2.Replace(c, string.Empty);
                        }
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = 2000;
                        progressBar1.Value = int.Parse(fix2);
                        break;
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            FileInfo fim = new FileInfo(@"sPA_data.txt");
            if (File.Exists(@"sPA_data.txt") && fim.Length > 0)
            {
                re_start();
            }
            else
            {
                label1.Text = ("sPA_data.txtの確認をして");
                label2.Text = ("下さい");
                this.HelpButton = false;
            }
        }
        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            fbpc++;
            if (fbpc % 2 == 0)
            {
                this.Height = 72;
            }
            else
            {
                this.Height = 260;
            }
            if (fbpc >= 2)
            {
                fbpc = 0;
            }
        }
        private void re_start()
        {
            using (StreamReader sr = new StreamReader(@"sPA_data.txt"))
            {
                string s1 = sr.ReadToEnd();
                s1 = s1.Replace(Environment.NewLine, "\r");
                s1 = s1.Trim('\r');
                s2 = s1.Split('\r');
                foreach (string item in s2)
                {
                    if (item == "end") { break; }
                }
                double aq = double.Parse(s2[5]);
                double aw = 10000;
                double ae = aq / aw;
                decimal ar = (decimal)(ae);
                numericUpDown3.Value = ar;

            }
            this.SetDesktopLocation(int.Parse(s2[0]), int.Parse(s2[1]));
            if (s2[2].Length < 14 || s2[2].Length > 14)
            {
                label1.Text = ("sPA_data.txtの3行目");
                label2.Text = ("確認");
                return;
            }
            string tmpe = (s2[2].Substring(0, 4) + "/" + s2[2].Substring(4, 2) + "/" + s2[2].Substring(6, 2) + " " + s2[2].Substring(8, 2) + ":" + s2[2].Substring(10, 2) + ":" + s2[2].Substring(12, 2));
            dat1 = DateTime.Parse(tmpe);
            if (dat1 < DateTime.Now - new TimeSpan(10, 0, 0, 0) || DateTime.Now + new TimeSpan(10, 0, 0, 0) < dat1)
            {
                label1.Text = ("sPA_data.txtの日付");
                label2.Text = ("確認");
                return;
            }
            textBox1.Text = dat1.ToString();
            datA = new TimeSpan(24, 0, 0);
            dat2 = new TimeSpan(int.Parse(s2[3].Substring(0, 2)), int.Parse(s2[3].Substring(2, 2)), int.Parse(s2[3].Substring(4, 2)));
            textBox2.Text = dat2.ToString();
            int nud1 = int.Parse(s2[4].Substring(0, 3));
            int nud2 = int.Parse(s2[4].Substring(3, 3));
            if (nud1 > 100) { nud1 = 100 - nud1; }
            if (nud2 > 100) { nud2 = 100 - nud2; }

            dat3 = new TimeSpan(0, nud1, nud2);
            numericUpDown2.Value = nud2;
            numericUpDown1.Value = nud1;


            dat4 = dat1 - datA;
            datB = new TimeSpan(0, 10, 16);
            if (s2[4] == "000000")
            {
                dat4 = dat4 + dat2 + datB;
            }
            else
            {
                dat4 = dat4 + dat2 + datB + dat3;
            }
            wts = TimeSpan.Parse("0:20:0.0");
            s2t = TimeSpan.Parse("0:10:21.140");
            t2s = TimeSpan.Parse("0:10:47.180");
            string at = ("0:0:0." + s2[5]);
            ragA = TimeSpan.Parse(at);
            var dat5 = dat4 + wts;
            sts[0] = dat5;
            DateTime test;
            for (int i = 1; i <= 479; i += 2)
            {
                sts[i] = sts[i - 1] + wts + t2s + ragA;
                test = sts[i];
                sts[i + 1] = test + wts + s2t + ragA;
            }
        }
        private void writefile()
        {
            using (var writer = new System.IO.StreamWriter(@"sPA_data.txt"))
            {
                writer.WriteLine(this.Location.X);
                writer.WriteLine(this.Location.Y);
                writer.WriteLine(s2[2]);
                writer.WriteLine(s2[3]);
                if (nv1 < 0) { nv1 = 100 + -nv1; }
                if (nv2 < 0) { nv2 = 100 + -nv2; }
                s2[4] = nv1.ToString("D3") + nv2.ToString("D3");
                writer.WriteLine(s2[4]);
                writer.WriteLine(s2[5]);
                writer.Write("end");
            }
        }
        private async Task<OcrResult> RecognizeText(SoftwareBitmap snap)
        {
            OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            OcrResult ocrResult = await ocrEngine.RecognizeAsync(snap);
            return ocrResult;
        }
        private SoftwareBitmap Mswb(System.Drawing.Bitmap bmp)
        {
            unsafe
            {
                var softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, bmp.Width, bmp.Height, BitmapAlphaMode.Premultiplied);
                System.Drawing.Imaging.BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                byte* pSrc = (byte*)bd.Scan0;
                using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write))
                {
                    using (var reference = buffer.CreateReference())
                    {
                        uint capacity;
                        ((Imbba)reference).GetBuffer(out byte* pDest, out capacity);
                        BitmapPlaneDescription bl = buffer.GetPlaneDescription(0);
                        for (int y = 0; y < bl.Height; y++)
                        {
                            int blOffset = bl.StartIndex + y * bl.Stride;
                            int yb = y * bd.Stride;
                            for (int x = 0; x < bl.Width; x++)
                            {
                                pDest[blOffset + 4 * x] = pSrc[yb + 4 * x];
                                pDest[blOffset + 4 * x + 1] = pSrc[yb + 4 * x + 1];
                                pDest[blOffset + 4 * x + 2] = pSrc[yb + 4 * x + 2];
                                pDest[blOffset + 4 * x + 3] = (byte)255;
                            }
                        }
                    }
                }
                bmp.UnlockBits(bd);
                return softwareBitmap;
            }
        }
    }
}