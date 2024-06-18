using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak.Objects
{
    public class Mosquito : MovableIntercativeAnimatedImageObject
    {
        public List<Image> aliveImages;
        public List<Image> deadFallingImages;
        public List<Image> deadOnTheGround;
        public string state = "alive";

        public Mosquito(List<Image> AliveImages, List<Image> DeadFallingImages, List<Image> DeadOnTheGroundImages, Panel panel, int x, int y, int size) : base(AliveImages, panel, x, y, size)
        {
            state = "alive";
            aliveImages = AliveImages;
            deadFallingImages = DeadFallingImages;
            deadOnTheGround = DeadOnTheGroundImages;
        }

        public void ChangeState(string newState)
        {
            state = newState;

            switch (state)
            {
                case "alive":
                    {
                        SwapImages(aliveImages);
                        break;
                    }
                case "deadFaling":
                    {
                        SwapImages(deadFallingImages);
                        ChangeMovementType("fall");
                        break;
                    }
                case "deadOnTheGround":
                    {
                        SwapImages(deadOnTheGround);
                        break;
                    }
            }
        }

        public override void Update()
        {
            if (state == "deadFaling" && rectangle.Y > panel.Height - (10 * (rectangle.Width/10)))
            {
                ChangeState("deadOnTheGround");
                ChangeMovementType("stop");
            }
            base.Update();
        }
    }

}
