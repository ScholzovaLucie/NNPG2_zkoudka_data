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
     
        private Timer timer;

        public Form1()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new MouseEventHandler(this.MainForm_MouseUp);

            timer = new Timer();
            timer.Interval = 1000; // 1 sekunda
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawBezierCurve(g);
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
            Bitmap myBitmap = new Bitmap("Grapes.gif");

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

        


       

    }
}
