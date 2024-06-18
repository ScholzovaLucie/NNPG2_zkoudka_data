using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak.Objects
{
    public class MovableIntercativeAnimatedImageObject : MovableInteractiveObject
    {
        protected List<Image> Images = new List<Image>();
        private Stopwatch stopwatch = new Stopwatch();
        private int currentImageIndex = 0;
        private int animationInterval = 100;

        public MovableIntercativeAnimatedImageObject(List<Image> images, Panel panel, int x, int y, int size) : base(panel, x, y, size)
        {
            Images = images;
            stopwatch.Start();
        }

        public void SwapImages(List<Image> images)
        {
            currentImageIndex = 0;
            Images = images;
        }

        public override void Draw(Graphics g)
        {
            if (stopwatch.ElapsedMilliseconds > animationInterval)
            {
                currentImageIndex = (currentImageIndex + 1) % Images.Count;
                stopwatch.Restart();
            }

            if (Images.Count > 0)
            {
                if (MovementDirection == "right")
                {
                    g.DrawImage(Images[currentImageIndex], rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                }
                else
                {
                    g.DrawImage(Images[currentImageIndex], rectangle.X + rectangle.Width, rectangle.Y, -rectangle.Width, rectangle.Height);
                }
            }
        }
    }
}
