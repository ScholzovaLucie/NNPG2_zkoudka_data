using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak.Objects
{
    public class MovableObject : Object
    {
        private Point target;
        private int targetSize;
        private  Random random;
        private int speed = 1;
        private string MovementType = "wander";
        protected string MovementDirection = "left";

        public MovableObject(Panel panel, int x, int y, int size) : base(panel, x, y, size)
        {
            SetNewTarget();
        }

        public void ChangeMovementType(string movementType)
        {
            MovementType = movementType;
        }

        public void Wander()
        {
            int X = rectangle.X;
            int Y = rectangle.Y;

            if (X == target.X && Y == target.Y && rectangle.Width == targetSize)
            {
                SetNewTarget();
            }
            else
            {
                if (X < target.X)
                {
                    rectangle.X += speed;
                    MovementDirection = "right";
                }
                else if (X > target.X)
                {
                    rectangle.X -= speed;
                    MovementDirection = "left";
                }

                if (Y < target.Y) rectangle.Y += speed;
                else if (Y > target.Y) rectangle.Y -= speed;

                if (rectangle.Width < targetSize)
                {
                    rectangle.Width++;
                    rectangle.Height++;
                }
                else if (rectangle.Width > targetSize)
                {
                    rectangle.Width--;
                    rectangle.Height--;
                };
            }
        }

        public void Fall()
        {
            rectangle.Y += speed;
        }


        public override void Update()
        {
            switch (MovementType)
            {
                case "wander":
                    {
                        Wander();
                        break;
                    }
                case "fall":
                    {
                        Fall();
                        break;
                    };
                case "stop":
                default: break;
            }
        }

        private void SetNewTarget()
        {
            random = new Random((int)DateTime.Now.Ticks);
            target = new Point(random.Next(0, panel.Width), random.Next(0, panel.Height));
            targetSize = random.Next(25, 200);
        }
    }
}
