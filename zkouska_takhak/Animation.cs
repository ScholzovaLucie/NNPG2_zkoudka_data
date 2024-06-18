using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    public class MainForm : Form
    {
        private Timer timer;
        private List<AnimObj> animObjs;
        private Random rand;

        public MainForm()
        {
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new MouseEventHandler(this.MainForm_MouseDown);

            animObjs = new List<AnimObj>();
            rand = new Random();

            timer = new Timer();
            timer.Interval = 50; // 50 ms pro plynulou animaci
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (var obj in animObjs)
            {
                obj.Draw(g);
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (var obj in animObjs)
                {
                    if (obj.HitTest(e.Location))
                    {
                        obj.Kill();
                        return;
                    }
                }

                // Přidání nového objektu
                int size = rand.Next(20, 50);
                int speed = rand.Next(1, 5);
                PointF direction = new PointF((float)(rand.NextDouble() * 2 - 1), (float)(rand.NextDouble() * 2 - 1));
                direction.Normalize();
                animObjs.Add(new AnimObj(e.Location, size, speed, direction, this.ClientSize));
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var obj in animObjs)
            {
                obj.Update();
            }
            this.Invalidate();
        }

    }

    public class AnimObj
    {
        private PointF position;
        private int size;
        private int speed;
        private PointF direction;
        private Size clientSize;
        private bool isAlive;
        private Image image;
        private int frameIndex;
        private int frameCount;
        private byte[] frameDurations;
        private int frameTimer;

        public AnimObj(PointF startPosition, int size, int speed, PointF direction, Size clientSize)
        {
            this.position = startPosition;
            this.size = size;
            this.speed = speed;
            this.direction = direction;
            this.clientSize = clientSize;
            this.isAlive = true;

            this.image = Image.FromFile("komar.gif");
            this.frameIndex = 0;
            this.frameCount = image.GetFrameCount(FrameDimension.Time);
            this.frameDurations = image.GetPropertyItem(0x5100).Value;
            this.frameTimer = 0;
        }

        public void Draw(Graphics g)
        {
            if (!isAlive)
            {
                // Zobraz mrtvého komára
                g.FillEllipse(Brushes.Black, position.X - size / 2, position.Y - size / 2, size, size);
                return;
            }

            image.SelectActiveFrame(FrameDimension.Time, frameIndex);
            g.DrawImage(image, position.X - size / 2, position.Y - size / 2, size, size);
        }

        public void Update()
        {
            if (!isAlive) return;

            // Aktualizace pozice
            position.X += direction.X * speed;
            position.Y += direction.Y * speed;

            // Odrážení od okrajů
            if (position.X < 0 || position.X > clientSize.Width)
            {
                direction.X = -direction.X;
            }
            if (position.Y < 0 || position.Y > clientSize.Height)
            {
                direction.Y = -direction.Y;
            }

            // Aktualizace animace
            frameTimer += 50;
            int frameDuration = BitConverter.ToInt32(frameDurations, 4 * frameIndex) * 10;
            if (frameTimer >= frameDuration)
            {
                frameTimer = 0;
                frameIndex = (frameIndex + 1) % frameCount;
            }
        }

        public void Kill()
        {
            isAlive = false;
        }

        public bool HitTest(PointF point)
        {
            return (point.X > position.X - size / 2 && point.X < position.X + size / 2 &&
                    point.Y > position.Y - size / 2 && point.Y < position.Y + size / 2);

        }
    }
    public static class PointFExtensions
    {
        public static void Normalize(this PointF point)
        {
            float length = (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
            if (length > 0)
            {
                point.X /= length;
                point.Y /= length;
            }
        }
    }
}
