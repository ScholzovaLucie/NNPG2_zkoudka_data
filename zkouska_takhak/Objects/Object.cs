using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    public abstract class Object
    {
        protected Rectangle rectangle;
        protected Panel panel;

        public Object(Panel panel, int x, int y, int size)
        {
            rectangle = new Rectangle(x, y, size, size);
            this.panel = panel;
        }

        public virtual void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.Red, rectangle);
        }

        public abstract void Update();
    }
}
