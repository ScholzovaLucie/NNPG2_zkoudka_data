using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    internal class vyhlazovani : Form
    {
        private Bitmap originalImage;
        private Bitmap processedImage;
        private ComboBox filterSelector;
        private Button applyFilterButton;

        public vyhlazovani()
        {
            this.Text = "Image Processing";
            this.Size = new Size(800, 600);

            filterSelector = new ComboBox();
            filterSelector.Items.AddRange(new string[] { "Average", "Gaussian", "Median", "Sobel", "Prewitt", "Laplace", "Sharpen" });
            filterSelector.SelectedIndex = 0;
            filterSelector.Dock = DockStyle.Top;
            this.Controls.Add(filterSelector);

            applyFilterButton = new Button();
            applyFilterButton.Text = "Apply Filter";
            applyFilterButton.Dock = DockStyle.Top;
            applyFilterButton.Click += ApplyFilterButton_Click;
            this.Controls.Add(applyFilterButton);

            this.Paint += new PaintEventHandler(this.MainForm_Paint);

            LoadImage("example.png"); // Load a default image
        }

        private void LoadImage(string path)
        {
            originalImage = new Bitmap(Properties.Resources.IKONA);
            processedImage = (Bitmap)originalImage.Clone();
        }

        private void ApplyFilterButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            string selectedFilter = filterSelector.SelectedItem.ToString();
            switch (selectedFilter)
            {
                case "Average":
                    processedImage = ApplyFilter(originalImage, GetAverageKernel());
                    break;
                case "Gaussian":
                    processedImage = ApplyFilter(originalImage, GetGaussianKernel());
                    break;
                case "Median":
                    processedImage = ApplyMedianFilter(originalImage);
                    break;
                case "Sobel":
                    processedImage = ApplyFilter(originalImage, GetSobelKernel());
                    break;
                case "Prewitt":
                    processedImage = ApplyFilter(originalImage, GetPrewittKernel());
                    break;
                case "Laplace":
                    processedImage = ApplyFilter(originalImage, GetLaplaceKernel());
                    break;
                case "Sharpen":
                    processedImage = ApplySharpenFilter(originalImage);
                    break;
            }
            this.Invalidate();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (processedImage != null)
            {
                g.DrawImage(processedImage, new Point(0, 50));
            }
        }

        private Bitmap ApplyFilter(Bitmap image, double[,] kernel)
        {
            int width = image.Width;
            int height = image.Height;
            BitmapData srcData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Bitmap resultImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytes = Math.Abs(srcData.Stride) * height;
            byte[] srcBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(srcData.Scan0, srcBuffer, 0, bytes);
            image.UnlockBits(srcData);

            int filterOffset = (kernel.GetLength(0) - 1) / 2;
            double blue, green, red;

            for (int offsetY = filterOffset; offsetY < height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < width - filterOffset; offsetX++)
                {
                    blue = green = red = 0.0;

                    int byteOffset = offsetY * srcData.Stride + offsetX * 3;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            int calcOffset = byteOffset + (filterX * 3) + (filterY * srcData.Stride);

                            blue += (double)(srcBuffer[calcOffset]) * kernel[filterY + filterOffset, filterX + filterOffset];
                            green += (double)(srcBuffer[calcOffset + 1]) * kernel[filterY + filterOffset, filterX + filterOffset];
                            red += (double)(srcBuffer[calcOffset + 2]) * kernel[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    resultBuffer[byteOffset] = ClipByte(blue);
                    resultBuffer[byteOffset + 1] = ClipByte(green);
                    resultBuffer[byteOffset + 2] = ClipByte(red);
                }
            }

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            resultImage.UnlockBits(resultData);

            return resultImage;
        }

        private byte ClipByte(double color)
        {
            return (byte)(color < 0 ? 0 : (color > 255 ? 255 : color));
        }

        private double[,] GetAverageKernel()
        {
            return new double[,]
            {
            { 1/9.0, 1/9.0, 1/9.0 },
            { 1/9.0, 1/9.0, 1/9.0 },
            { 1/9.0, 1/9.0, 1/9.0 }
            };
        }

        private double[,] GetGaussianKernel()
        {
            return new double[,]
            {
            { 1/16.0, 2/16.0, 1/16.0 },
            { 2/16.0, 4/16.0, 2/16.0 },
            { 1/16.0, 2/16.0, 1/16.0 }
            };
        }

        private Bitmap ApplyMedianFilter(Bitmap image)
        {
            // Implementace medianového filtru
            // Kód podobný ApplyFilter, ale používá medián místo konvolučního produktu
            return image;
        }

        private double[,] GetSobelKernel()
        {
            return new double[,]
            {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
            };
        }

        private double[,] GetPrewittKernel()
        {
            return new double[,]
            {
            { -1, 0, 1 },
            { -1, 0, 1 },
            { -1, 0, 1 }
            };
        }

        private double[,] GetLaplaceKernel()
        {
            return new double[,]
            {
            { 0, -1, 0 },
            { -1, 4, -1 },
            { 0, -1, 0 }
            };
        }

        private Bitmap ApplySharpenFilter(Bitmap image)
        {
            // Superpozice původního obrazu a Laplaceova operátoru
            Bitmap laplaceImage = ApplyFilter(image, GetLaplaceKernel());
            return CombineImages(image, laplaceImage);
        }

        private Bitmap CombineImages(Bitmap image1, Bitmap image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            Bitmap resultImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color1 = image1.GetPixel(x, y);
                    Color color2 = image2.GetPixel(x, y);

                    int red = ClipByte(color1.R + color2.R);
                    int green = ClipByte(color1.G + color2.G);
                    int blue = ClipByte(color1.B + color2.B);

                    resultImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return resultImage;
        }
    }
}
