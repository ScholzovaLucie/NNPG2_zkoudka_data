using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new MouseEventHandler(this.MainForm_MouseUp);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawBezierCurve(g);
            DrawBezierCurve(g, new Point(10,200), new Point(250, 150), new Point(400, 400), new Point(400, 10));
            MakeTransparent_Example2(e);
            Rotace(g);
            DrawSlunce(g);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Implementace MouseDown
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            // Implementace MouseMove
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            // Implementace MouseUp
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void DrawBezierCurve(Graphics g)
        {
            // Implementace Bézierových křivek
            GraphicsPath path = new GraphicsPath();
            path.AddBezier(new Point(10, 10), new Point(20, 20), new Point(30, 10), new Point(40, 20));
            g.DrawPath(Pens.Black, path);
        }


        public static void RotateGraphics(Graphics g, float angle, PointF center)
        {
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            g.TranslateTransform(-center.X, -center.Y);
        }

        public static void DrawBezierCurve(Graphics g, Point start, Point control1, Point control2, Point end)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddBezier(start, control1, control2, end);
            g.DrawPath(Pens.Black, path);
        }

        private void MakeTransparent_Example2(PaintEventArgs e)
        {

            // Create a Bitmap object from an image file.
            Bitmap myBitmap = new Bitmap(Properties.Resources.SLUNCE);

            // Draw myBitmap to the screen.
            e.Graphics.DrawImage(
                myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);

            // Get the color of a background pixel.
            Color backColor = myBitmap.GetPixel(1, 1);

            // Make backColor transparent for myBitmap.
            myBitmap.MakeTransparent(backColor);

            // Draw the transparent bitmap to the screen.
            e.Graphics.DrawImage(
                myBitmap, myBitmap.Width, 0, myBitmap.Width, myBitmap.Height);
        }


        private void Rotace(Graphics g)
        {
            int size = 300;
            int mezera = 5;
            Bitmap bit = new Bitmap(size, size);
            using (Graphics g2 = Graphics.FromImage(bit))
            {
                Pen pen;
                int x = mezera;

                for (int y = 0; y < (size / mezera); y++)
                {
                    if (y % 2 == 0)
                    {
                        pen = new Pen(Color.Aqua);
                    }
                    else
                    {
                        pen = new Pen(Color.BlueViolet);
                    }

                    Point start = new Point(x, 0);
                    Point end = new Point(x, size);
                    g2.DrawLine(pen, start, end);
                    x = x + mezera;
                }
            }

            TextureBrush textB = new TextureBrush(bit);

            textB.RotateTransform(20);
            g.FillRectangle(textB, 600, 200, 100, 100);
            g.DrawRectangle(new Pen(Color.Red), 600, 200, 100, 100);
        }


        private void DrawSlunce(Graphics g)
        {

            int polomer = 100;
            int prumer = polomer * 2;
            Point center = new Point(400, 300);
            g.DrawEllipse(new Pen(Color.Red), center.X - polomer, center.Y - polomer, prumer, prumer);

            g.DrawEllipse(new Pen(Color.Red), center.X - 2, center.Y - 2, 5, 5);

            float angle = 360f / 12;

            for (int i = 0; i < 12; i++)
            {
                g.Save();
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(i * angle);
                Rectangle rec = new Rectangle(0, 10, 10, 2);
                g.FillRectangle(new SolidBrush(Color.Black), -5, -polomer, 5, 20);
                g.ResetTransform();
            }

            angle = 360f / 60;

            for (int i = 0; i < 60; i++)
            {
                g.Save();
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(i * angle);
                Rectangle rec = new Rectangle(0, 10, 10, 2);
                g.FillRectangle(new SolidBrush(Color.Black), -5, -polomer, 2, 10);
                g.ResetTransform();
            }

        }



    }
}
