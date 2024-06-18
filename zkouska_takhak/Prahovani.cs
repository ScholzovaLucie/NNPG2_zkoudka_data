using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    internal class Prahovani : Form
    {
        private Bitmap originalImage;
        private Bitmap processedImage;
        private ComboBox thresholdSelector;
        private Button applyThresholdButton;
        private Button skeletonButton;
        private TextBox thresholdValueTextBox;

        public Prahovani()
        {
            this.Text = "Image Processing";
            this.Size = new Size(800, 600);

            thresholdSelector = new ComboBox();
            thresholdSelector.Items.AddRange(new string[] { "Manual", "Automatic", "Otsu" });
            thresholdSelector.SelectedIndex = 0;
            thresholdSelector.Dock = DockStyle.Top;
            this.Controls.Add(thresholdSelector);

            thresholdValueTextBox = new TextBox();
            thresholdValueTextBox.Dock = DockStyle.Top;
            thresholdValueTextBox.Text = "128"; // Default value for manual threshold
            this.Controls.Add(thresholdValueTextBox);

            applyThresholdButton = new Button();
            applyThresholdButton.Text = "Apply Threshold";
            applyThresholdButton.Dock = DockStyle.Top;
            applyThresholdButton.Click += ApplyThresholdButton_Click;
            this.Controls.Add(applyThresholdButton);

            skeletonButton = new Button();
            skeletonButton.Text = "Find Skeleton";
            skeletonButton.Dock = DockStyle.Top;
            skeletonButton.Click += SkeletonButton_Click;
            this.Controls.Add(skeletonButton);

            this.Paint += new PaintEventHandler(this.MainForm_Paint);

            LoadImage("example.png"); // Load a default image
        }

        private void LoadImage(string path)
        {
            originalImage = new Bitmap(path);
            processedImage = (Bitmap)originalImage.Clone();
        }

        private void ApplyThresholdButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            string selectedMethod = thresholdSelector.SelectedItem.ToString();
            switch (selectedMethod)
            {
                case "Manual":
                    int threshold = int.Parse(thresholdValueTextBox.Text);
                    processedImage = ApplyManualThreshold(originalImage, threshold);
                    break;
                case "Automatic":
                    processedImage = ApplyAutomaticThreshold(originalImage);
                    break;
                case "Otsu":
                    processedImage = ApplyOtsuThreshold(originalImage);
                    break;
            }
            this.Invalidate();
        }

        private void SkeletonButton_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            processedImage = FindSkeleton(processedImage);
            this.Invalidate();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (processedImage != null)
            {
                g.DrawImage(processedImage, new Point(0, 100));
            }
        }

        private Bitmap ApplyManualThreshold(Bitmap image, int threshold)
        {
            Bitmap resultImage = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int gray = (color.R + color.G + color.B) / 3;
                    if (gray < threshold)
                    {
                        resultImage.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        resultImage.SetPixel(x, y, Color.White);
                    }
                }
            }
            return resultImage;
        }

        private Bitmap ApplyAutomaticThreshold(Bitmap image)
        {
            int threshold = CalculateAutomaticThreshold(image);
            thresholdValueTextBox.Text = threshold.ToString();
            return ApplyManualThreshold(image, threshold);
        }

        private int CalculateAutomaticThreshold(Bitmap image)
        {
            int[] histogram = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int gray = (color.R + color.G + color.B) / 3;
                    histogram[gray]++;
                }
            }

            int totalPixels = image.Width * image.Height;
            int threshold = 128;
            int previousThreshold = -1;

            while (threshold != previousThreshold)
            {
                previousThreshold = threshold;
                int sum1 = 0, sum2 = 0, count1 = 0, count2 = 0;

                for (int i = 0; i < threshold; i++)
                {
                    sum1 += i * histogram[i];
                    count1 += histogram[i];
                }

                for (int i = threshold; i < 256; i++)
                {
                    sum2 += i * histogram[i];
                    count2 += histogram[i];
                }

                int mean1 = (count1 == 0) ? 0 : (sum1 / count1);
                int mean2 = (count2 == 0) ? 0 : (sum2 / count2);

                threshold = (mean1 + mean2) / 2;
            }

            return threshold;
        }

        private Bitmap ApplyOtsuThreshold(Bitmap image)
        {
            int threshold = CalculateOtsuThreshold(image);
            thresholdValueTextBox.Text = threshold.ToString();
            return ApplyManualThreshold(image, threshold);
        }

        private int CalculateOtsuThreshold(Bitmap image)
        {
            int[] histogram = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int gray = (color.R + color.G + color.B) / 3;
                    histogram[gray]++;
                }
            }

            int total = image.Width * image.Height;
            float sum = 0;
            for (int i = 0; i < 256; i++) sum += i * histogram[i];

            float sumB = 0;
            int wB = 0;
            int wF = 0;

            float maxVariance = 0;
            int threshold = 0;

            for (int i = 0; i < 256; i++)
            {
                wB += histogram[i];
                if (wB == 0) continue;

                wF = total - wB;
                if (wF == 0) break;

                sumB += i * histogram[i];

                float mB = sumB / wB;
                float mF = (sum - sumB) / wF;

                float betweenVariance = (float)wB * (float)wF * (mB - mF) * (mB - mF);

                if (betweenVariance > maxVariance)
                {
                    maxVariance = betweenVariance;
                    threshold = i;
                }
            }

            return threshold;
        }

        private Bitmap FindSkeleton(Bitmap image)
        {
            Bitmap resultImage = (Bitmap)image.Clone();
            bool[,] binaryImage = new bool[image.Width, image.Height];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    binaryImage[x, y] = image.GetPixel(x, y).R == 0;
                }
            }

            bool pixelsRemoved;
            do
            {
                pixelsRemoved = false;
                for (int y = 1; y < image.Height - 1; y++)
                {
                    for (int x = 1; x < image.Width - 1; x++)
                    {
                        if (binaryImage[x, y] && ShouldRemovePixel(binaryImage, x, y))
                        {
                            binaryImage[x, y] = false;
                            pixelsRemoved = true;
                        }
                    }
                }
            } while (pixelsRemoved);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    resultImage.SetPixel(x, y, binaryImage[x, y] ? Color.Black : Color.White);
                }
            }

            return resultImage;
        }

        private bool ShouldRemovePixel(bool[,] image, int x, int y)
        {
            int[] neighbors = new int[8];
            neighbors[0] = image[x, y - 1] ? 1 : 0;
            neighbors[1] = image[x + 1, y - 1] ? 1 : 0;
            neighbors[2] = image[x + 1, y] ? 1 : 0;
            neighbors[3] = image[x + 1, y + 1] ? 1 : 0;
            neighbors[4] = image[x, y + 1] ? 1 : 0;
            neighbors[5] = image[x - 1, y + 1] ? 1 : 0;
            neighbors[6] = image[x - 1, y] ? 1 : 0;
            neighbors[7] = image[x - 1, y - 1] ? 1 : 0;

            int count = 0;
            for (int i = 0; i < 8; i++) count += neighbors[i];

            if (count < 2 || count > 6) return false;

            int transitions = 0;
            for (int i = 0; i < 7; i++) if (neighbors[i] == 0 && neighbors[i + 1] == 1) transitions++;
            if (neighbors[7] == 0 && neighbors[0] == 1) transitions++;

            if (transitions != 1) return false;

            if (neighbors[0] * neighbors[2] * neighbors[4] != 0) return false;
            if (neighbors[2] * neighbors[4] * neighbors[6] != 0) return false;

            return true;
        }

    }
}
