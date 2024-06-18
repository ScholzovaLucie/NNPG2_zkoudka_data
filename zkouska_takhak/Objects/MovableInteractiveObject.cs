using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak.Objects
{
    public class MovableInteractiveObject : MovableObject
    {
        public MovableInteractiveObject(Panel panel, int x, int y, int size) : base(panel, x, y, size) { }

        public bool Contains(Point point)
        {
            return rectangle.Contains(point);
        }
    }
}
