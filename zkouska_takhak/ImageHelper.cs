using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    internal class ImageHelper : Form
    {
        public static Image LoadImage(string path)
        {
            return Image.FromFile(path);
        }

        public static void DisplayImage(Graphics g, Image image, Rectangle destRect, bool proportional = true)
        {
            if (proportional)
            {
                float aspectRatio = (float)image.Width / image.Height;
                if (destRect.Width / (float)destRect.Height > aspectRatio)
                {
                    destRect.Width = (int)(destRect.Height * aspectRatio);
                }
                else
                {
                    destRect.Height = (int)(destRect.Width / aspectRatio);
                }
            }
            g.DrawImage(image, destRect);

        }
    }
}
