using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class BitmapExampleControl : Control
    {
        public BitmapExampleControl()
        {
            RepaintMode = ControlRepaintMode.IncrementalGrowth;
            PreferredSize = new Size(800, 600);
        }

        private NBitmap bitmap;
        private int[,] distances;
        private Color[] colors;

        private Stopwatch sw;
        private int frames;

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            if (sw == null)
            {
                sw = Stopwatch.StartNew();
            }

            UpdateBitmap();

            using (var image = Application.ImageCodec.Create(bitmap.Width, bitmap.Height))
            {
                // todo: create image from bitmap instead 
                image.CopyFromBitmap(bitmap, Point.Empty, Point.Empty, new Size(bitmap.Width, bitmap.Height));
                // todo: swap image and location parameters?
                canvas.DrawImage(image, 0, 0);
                long time = sw.ElapsedMilliseconds;
                double fps = frames++ * 1000 / (double) (time + 1);
                canvas.DrawString(Color.Black, new FontConfig("Arial", 16), 0, 0, $"Time: {time / 1000d:0.0}s, Frames: {frames}, FPS: {fps:0.0}");
            }

            // todo: use timer?
            InvalidatePainting();
        }

        private void UpdateBitmap()
        {
            if (colors == null)
            {
                colors = new Color[720];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = ColorFromHsv(i / 2d, 1, 1);
                }
            }

            if (bitmap == null || bitmap.Width != Area.Width || bitmap.Height != Area.Height)
            {
                bitmap = new NBitmap(Area.Width, Area.Height);
                distances = new int[Area.Height, Area.Width];

                PointF center = new PointF(Area.Width / 2f, Area.Height / 2f);
                double radius = Math.Min(Area.Height, Area.Width) * 3d / 8;

                PointF[] points = new PointF[9];
                for (int i = 0; i < points.Length; i++)
                {
                    double angle = i * 2 * Math.PI / points.Length - Math.PI / 2;
                    points[i] = new PointF(center.X + (float) (Math.Cos(angle) * radius), center.Y + (float) (Math.Sin(angle) * radius));
                }

                Parallel.For(0, bitmap.Height, y =>
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        double distSum = 0;
                        double weightSum = 0;

                        for (int i = 0; i < points.Length; i++)
                        {
                            double dist = GetDistance(new PointF(x, y), points[i]);
                            double weight = Math.Pow(1 / (1 + dist), 2);

                            distSum += dist * weight;
                            weightSum += weight;
                        }

                        double wDist = distSum / weightSum;
                        distances[y, x] = (int) (wDist * 4);
                    }
                });
            }

            int time = (int) DateTime.Now.TimeOfDay.TotalMilliseconds;

            Parallel.For(0, bitmap.Height, y =>
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int dist = distances[y, x];
                    int colorIndex = (dist - time / 4) % colors.Length;
                    if (colorIndex < 0)
                    {
                        colorIndex += colors.Length;
                    }

                    bitmap.SetColor(x, y, colors[colorIndex]);
                }
            });
        }

        private double GetDistance(PointF p1, PointF p2)
        {
            float xd = p1.X - p2.X;
            float yd = p1.Y - p2.Y;
            return Math.Sqrt(xd * xd + yd * yd);
        }

        private Color ColorFromHsv(double h, double s, double v)
        {
            double c = v * s;
            double hh = h / 60 % 6;
            if (hh < 0)
            {
                hh += 6;
            }

            double x = c * (1 - Math.Abs(hh % 2 - 1));

            int x255 = Convert.ToInt32(x * 255);
            int c255 = Convert.ToInt32(c * 255);

            switch ((int) hh)
            {
                case 0: return Color.FromArgb(c255, x255, 0);
                case 1: return Color.FromArgb(x255, c255, 0);
                case 2: return Color.FromArgb(0, c255, x255);
                case 3: return Color.FromArgb(0, x255, c255);
                case 4: return Color.FromArgb(x255, 0, c255);
                case 5: return Color.FromArgb(c255, 0, x255);
                default: return Color.Black;
            }
        }
    }
}