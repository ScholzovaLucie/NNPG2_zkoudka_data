using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNPG2_2024_Uloha_06_Bajer_Lukas
{


    internal class MosquitoImagesInicializator
    {
        public List<Image> aliveImages;
        public List<Image> deadFallingImages;
        public List<Image> deadOnTheGroundImages;

        public MosquitoImagesInicializator()
        {
            aliveImages = InitializeAliveImages();
            deadFallingImages = InitializeDeadFallingImages();
            deadOnTheGroundImages = InitializeDeadOnTheGroundImages();
        }

        private List<Image> InitializeAliveImages()
        {
            List<Image> images = new List<Image>
            {
            Image.FromFile("Obrazky/KOMAR_01.GIF"),
            Image.FromFile("Obrazky/KOMAR_02.GIF"),
            Image.FromFile("Obrazky/KOMAR_03.GIF"),
            Image.FromFile("Obrazky/KOMAR_04.GIF"),
            Image.FromFile("Obrazky/KOMAR_05.GIF"),
            Image.FromFile("Obrazky/KOMAR_06.GIF"),
            Image.FromFile("Obrazky/KOMAR_07.GIF"),
            Image.FromFile("Obrazky/KOMAR_08.GIF"),
            Image.FromFile("Obrazky/KOMAR_09.GIF"),
            Image.FromFile("Obrazky/KOMAR_10.GIF"),
            Image.FromFile("Obrazky/KOMAR_11.GIF"),
            Image.FromFile("Obrazky/KOMAR_12.GIF"),
            };
            return ConvertToTransparent(images);
        }

        private List<Image> InitializeDeadFallingImages()
        {
            List<Image> images = new List<Image>
            {
                Image.FromFile("Obrazky/KOMAR_13.GIF"),
                Image.FromFile("Obrazky/KOMAR_14.GIF"),
                Image.FromFile("Obrazky/KOMAR_15.GIF"),
                Image.FromFile("Obrazky/KOMAR_16.GIF"),
                Image.FromFile("Obrazky/KOMAR_17.GIF")
            };
            return ConvertToTransparent(images);
        }

        private List<Image> InitializeDeadOnTheGroundImages()
        {
            List<Image> images = new List<Image>
            {
                 Image.FromFile("Obrazky/KOMAR_17.GIF")
            };
            return ConvertToTransparent(images);
        }

        private List<Image> ConvertToTransparent(List<Image> originalImages)
        {
            var transparentImages = new List<Image>();
            foreach (var image in originalImages)
            {
                Bitmap bitmap = new Bitmap(image);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color originalColor = bitmap.GetPixel(x, y);
                        if (originalColor.R > 245 && originalColor.G > 245 && originalColor.B > 245) // Adjust threshold as needed
                            bitmap.SetPixel(x, y, Color.Transparent);
                    }
                }
                transparentImages.Add(bitmap);
            }
            return transparentImages;
        }
    }
}
