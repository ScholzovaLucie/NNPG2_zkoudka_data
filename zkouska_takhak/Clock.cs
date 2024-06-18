using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    public partial class Clock : Form
    {
        private Timer timer;
        private PictureBox digitalClock;
        private PictureBox analogClock;
        private Bitmap[] numbers;
        private Bitmap sun;

        public Clock()
        {
            this.InitializeComponent();
            LoadResources();

            SetupTimer();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);

            this.Text = "Hodiny";
            this.Size = new Size(800, 600);

            int digitalClockHeight = this.ClientSize.Height / 5;
            int analogClockHeight = this.ClientSize.Height - digitalClockHeight - 20;

            digitalClock = new PictureBox
            {
                Location = new Point(10, this.ClientSize.Height - digitalClockHeight - 10),
                Size = new Size(this.ClientSize.Width - 20, digitalClockHeight)
            };
            digitalClock.Paint += new PaintEventHandler(DrawDigitalClock);
            this.Controls.Add(digitalClock);

            analogClock = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(this.ClientSize.Width - 20, analogClockHeight),
            };
            analogClock.Paint += new PaintEventHandler(DrawAnalogClock);
            this.Controls.Add(analogClock);

            this.Resize += new EventHandler(MainForm_Resize);
        }

        private void LoadResources()
        {
            numbers = new Bitmap[12];
            numbers[0] = Properties.Resources._12;
            numbers[1] = Properties.Resources._1;
            numbers[2] = Properties.Resources._2;
            numbers[3] = Properties.Resources._3;
            numbers[4] = Properties.Resources._4;
            numbers[5] = Properties.Resources._5;
            numbers[6] = Properties.Resources._6;
            numbers[7] = Properties.Resources._7;
            numbers[8] = Properties.Resources._8;
            numbers[9] = Properties.Resources._9;
            numbers[10] = Properties.Resources._10;
            numbers[11] = Properties.Resources._11;

            sun = Properties.Resources.SLUNCE;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int digitalClockHeight = this.ClientSize.Height / 5;
            int analogClockHeight = this.ClientSize.Height - digitalClockHeight - 20;

            digitalClock.Size = new Size(this.ClientSize.Width - 20, digitalClockHeight);
            digitalClock.Location = new Point(10, this.ClientSize.Height - digitalClockHeight - 10);
            digitalClock.Invalidate();

            analogClock.Size = new Size(this.ClientSize.Width - 20, analogClockHeight);
            analogClock.Invalidate();
        }

        private void SetupTimer()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(UpdateClocks);
            timer.Start();
        }

        private void UpdateClocks(object sender, EventArgs e)
        {
            digitalClock.Invalidate();
            analogClock.Invalidate();
        }

        private void DrawDigitalClock(object sender, PaintEventArgs e)
        {
            DateTime now = DateTime.Now;
            string timeString = now.ToString("HH:mm:ss");

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            float fontSize = digitalClock.Height - 10;
            Font font = new Font("Courier New", fontSize, FontStyle.Bold);

            SizeF textSize = g.MeasureString(timeString, font);
            float scaleFactor = Math.Min(digitalClock.Width / textSize.Width, digitalClock.Height / textSize.Height);
            fontSize *= scaleFactor;
            font = new Font("Courier New", fontSize, FontStyle.Bold);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(timeString, font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), StringFormat.GenericDefault);

                // Přepočítej textSize pro nový font
                textSize = g.MeasureString(timeString, font);
                float x = (digitalClock.Width - textSize.Width) / 2;
                float y = (digitalClock.Height - textSize.Height) / 2;

                g.TranslateTransform(x, y);

                Pen pen = new Pen(Color.Black, 2);
                g.DrawPath(pen, path);
                g.FillPath(Brushes.Transparent, path);

                g.ResetTransform();
            }
        }


        private void DrawAnalogClock(object sender, PaintEventArgs e)
        {
            DateTime now = DateTime.Now;
            Graphics g = e.Graphics;

            int centerX = analogClock.Width / 2;
            int centerY = analogClock.Height / 2;
            int radius = Math.Min(centerX, centerY) - 10;

            g.TranslateTransform(centerX, centerY);

            // Draw clock face
            int clockSize = radius * 2;
            g.FillEllipse(Brushes.Transparent, -radius, -radius, clockSize, clockSize);
            g.DrawEllipse(new Pen(Color.Red, 5), -radius, -radius, clockSize, clockSize);

            // Draw hour markers with images
            for (int i = 0; i < 12; i++)
            {
                double angle = i * 30 * Math.PI / 180;
                int x = (int)(Math.Cos(angle) * (radius - 40));
                int y = (int)(Math.Sin(angle) * (radius - 40));
                int numberSize = (int)(radius * 0.25);
                Bitmap numberImage = new Bitmap(numbers[i], new Size(numberSize, numberSize));

                numberImage.MakeTransparent(Color.Transparent);

                e.Graphics.DrawImage(numberImage, new Rectangle(x - numberSize / 2, y - numberSize / 2, numberSize, numberSize));
            }

            // Draw hour and minute ticks
            for (int i = 0; i < 60; i++)
            {
                double angle = i * 6 * Math.PI / 180;
                int x1 = (int)(Math.Cos(angle) * (radius - 10));
                int y1 = (int)(Math.Sin(angle) * (radius - 10));
                int x2 = (int)(Math.Cos(angle) * (radius - (i % 5 == 0 ? 20 : 15)));
                int y2 = (int)(Math.Sin(angle) * (radius - (i % 5 == 0 ? 20 : 15)));
                g.DrawLine(new Pen(Color.Black, i % 5 == 0 ? Math.Max(1, radius / 30) : Math.Max(1, radius / 100)), x1, y1, x2, y2);

            }

            // Draw sun face
            int sunSize = radius;
            Bitmap sunImage = new Bitmap(sun, new Size(sunSize, sunSize));

            // Make the default transparent color transparent for myBitmap.
            sunImage.MakeTransparent(Color.Transparent);

            e.Graphics.DrawImage(sunImage, new Rectangle(-sunSize / 2, -sunSize / 2, sunSize, sunSize));


            // Draw hour hand
            DrawHand(g, now.Hour % 12 * 30 + now.Minute / 2, radius * 0.5f, Math.Max(3, radius / 20), true);

            // Draw minute hand
            DrawHand(g, now.Minute * 6, radius * 0.8f, Math.Max(3, radius / 12), true);

            // Draw second hand
            DrawHand(g, now.Second * 6, radius * 0.9f, Math.Max(1, radius / 100), false, Color.Red);

            g.ResetTransform();
        }

        private void DrawHand(Graphics g, double angle, float length, int thickness, bool withArrow, Color? color = null)
        {
            angle -= 90;
            double radian = angle * Math.PI / 180;
            int x = (int)(Math.Cos(radian) * length);
            int y = (int)(Math.Sin(radian) * length);

            Pen pen = new Pen(color ?? Color.Black, thickness);

            if (withArrow)
            {
                pen.EndCap = LineCap.ArrowAnchor;
            }
            g.DrawLine(pen, 0, 0, x, y);
        }

    }
}
