using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using NWindows;

namespace Test.NWindows
{
    public class TestImageCodec
    {
        private NApplication app;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            app = new NApplication();
            app.Init();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            app.Dispose();
        }

        [Test]
        public void TestLoadImageFromFile([ValueSource(nameof(samples))] ImageSample sample)
        {
            string testFolder = Path.Combine("Temp", "Test");
            Directory.CreateDirectory(testFolder);

            string fileName = Path.Combine(testFolder, sample.ResourceName);
            using (var stream = sample.CreateResourceStream())
            {
                CopyToFile(stream, fileName);
            }

            using (var image = app.ImageCodec.LoadImageFromFile(fileName))
            {
                Assert.That(image.Width, Is.EqualTo(sample.Width), () => $"{sample.ResourceName}, {nameof(image.Width)}");
                Assert.That(image.Height, Is.EqualTo(sample.Height), () => $"{sample.ResourceName}, {nameof(image.Height)}");

                NBitmap bitmap = new NBitmap(image.Width, image.Height);
                image.CopyToBitmap(Point.Empty, bitmap, Point.Empty, new Size(image.Width, image.Height));

                Assert.That(bitmap.GetColor(0, 0), Is.EqualTo(sample.TopLeftColor), nameof(sample.TopLeftColor));
                Assert.That(bitmap.GetColor(bitmap.Width - 1, 0), Is.EqualTo(sample.TopRightColor), nameof(sample.TopRightColor));
                Assert.That(bitmap.GetColor(0, bitmap.Height - 1), Is.EqualTo(sample.BottomLeftColor), nameof(sample.BottomLeftColor));
                Assert.That(bitmap.GetColor(bitmap.Width - 1, bitmap.Height - 1), Is.EqualTo(sample.BottomRightColor), nameof(sample.BottomRightColor));
            }
        }

        [Test]
        public void TestLoadBitmapFromStream([ValueSource(nameof(samples))] ImageSample sample)
        {
            NBitmap bitmap;
            using (var stream = sample.CreateResourceStream())
            {
                bitmap = app.ImageCodec.LoadBitmapFromStream(stream);
            }

            Assert.That(bitmap.Width, Is.EqualTo(sample.Width), () => $"{sample.ResourceName}, {nameof(bitmap.Width)}");
            Assert.That(bitmap.Height, Is.EqualTo(sample.Height), () => $"{sample.ResourceName}, {nameof(bitmap.Height)}");

            Assert.That(bitmap.GetColor(0, 0), Is.EqualTo(sample.TopLeftColor), nameof(sample.TopLeftColor));
            Assert.That(bitmap.GetColor(bitmap.Width - 1, 0), Is.EqualTo(sample.TopRightColor), nameof(sample.TopRightColor));
            Assert.That(bitmap.GetColor(0, bitmap.Height - 1), Is.EqualTo(sample.BottomLeftColor), nameof(sample.BottomLeftColor));
            Assert.That(bitmap.GetColor(bitmap.Width - 1, bitmap.Height - 1), Is.EqualTo(sample.BottomRightColor), nameof(sample.BottomRightColor));
        }

        private static Color[] SampleForegroundColors = new Color[]
        {
            Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0x00, 0x00, 0x00),
            Color.FromArgb(0x00, 0x00, 0x00, 0x00),
            Color.FromArgb(0xFF, 0x80, 0x40, 0x20),
            Color.FromArgb(0x80, 0x40, 0x80, 0xC0)
        };

        private static Color[] SampleBackgroundColors = new Color[]
        {
            Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0x00, 0x00, 0x00),
            Color.FromArgb(0x00, 0x00, 0x00, 0x00)
        };

        [Test]
        public void TestImageBitmapConversion
        (
            [ValueSource(nameof(SampleBackgroundColors))] Color color1,
            [ValueSource(nameof(SampleForegroundColors))] Color color2,
            [ValueSource(nameof(SampleBackgroundColors))] Color color3
        )
        {
            NBitmap bitmap1 = new NBitmap(100, 90);
            FillBitmap(bitmap1, color1);

            NBitmap bitmap2 = new NBitmap(60, 40);
            FillBitmap(bitmap2, color2);

            NBitmap bitmap3 = new NBitmap(100, 90);
            FillBitmap(bitmap3, color3);

            NBitmap result1 = new NBitmap(100, 90);
            NBitmap result2 = new NBitmap(100, 90);
            NBitmap result3 = new NBitmap(100, 90);

            using (NImage image = app.ImageCodec.Create(100, 90))
            {
                image.CopyFromBitmap(bitmap1, new Point(0, 0), new Point(0, 0), new Size(100, 90));
                image.CopyFromBitmap(bitmap2, new Point(0, 0), new Point(20, 30), new Size(60, 40));

                image.CopyToBitmap(new Point(0, 0), result1, new Point(0, 0), new Size(100, 90));

                FillBitmap(result2, color3);
                image.CopyToBitmap(new Point(20, 30), result2, new Point(10, 20), new Size(60, 40));

                image.CopyFromBitmap(bitmap3, new Point(0, 0), new Point(0, 0), new Size(100, 90));
                image.CopyFromBitmap(result1, new Point(20, 30), new Point(30, 40), new Size(60, 40));

                image.CopyToBitmap(new Point(0, 0), result3, new Point(0, 0), new Size(100, 90));
            }

            Rectangle result1Rect = new Rectangle(20, 30, 60, 40);
            Rectangle result2Rect = new Rectangle(10, 20, 60, 40);
            Rectangle result3Rect = new Rectangle(30, 40, 60, 40);

            for (int y = 0; y < 90; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    Point point = new Point(x, y);
                    Color expectedColor1 = result1Rect.Contains(point) ? color2 : color1;
                    Color expectedColor2 = result2Rect.Contains(point) ? color2 : color3;
                    Color expectedColor3 = result3Rect.Contains(point) ? color2 : color3;

                    AssertColorEquals(expectedColor1, result1.GetColor(x, y), () => $"{nameof(result1)}, X={x}, Y={y}");
                    AssertColorEquals(expectedColor2, result2.GetColor(x, y), () => $"{nameof(result2)}, X={x}, Y={y}");
                    AssertColorEquals(expectedColor3, result3.GetColor(x, y), () => $"{nameof(result3)}, X={x}, Y={y}");
                }
            }
        }

        private void AssertColorEquals(Color expectedColor, Color actualColor, Func<string> message)
        {
            int expectedArgb = expectedColor.ToArgb();
            int actualArgb = actualColor.ToArgb();
            if (expectedArgb != actualArgb)
            {
                throw new AssertionException($"{message()}\n\tExpected Color: 0x{expectedArgb:X8}\n\tActual Color 0x{actualArgb:X8}");
            }
        }

        private void FillBitmap(NBitmap bitmap, Color color)
        {
            Parallel.For(0, bitmap.Height, y =>
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmap.SetColor(x, y, color);
                }
            });
        }

        private void CopyToFile(Stream stream, string filename)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                stream.CopyTo(fileStream);
            }
        }

        private static List<ImageSample> samples = new List<ImageSample>
        {
            new ImageSample("colors-1bpp.bmp", 64, 64, ImageSample.BWWB),
            new ImageSample("colors-4bpp.bmp", 64, 64, ImageSample.RGBW),
            new ImageSample("colors-8bpp.bmp", 64, 64, ImageSample.RGBC),
            new ImageSample("colors-8bpp.gif", 64, 64, ImageSample.RGBT),
            new ImageSample("colors-8bpp.png", 64, 64, ImageSample.RGBT),
            new ImageSample("colors-8bpp.tiff", 64, 64, ImageSample.RGBC),
            new ImageSample("colors-24bpp.bmp", 64, 64, ImageSample.RGBC),
            new ImageSample("colors-24bpp.jpg", 64, 64, ImageSample.RGBC_JPG),
            new ImageSample("colors-24bpp.png", 64, 64, ImageSample.RGBC),
            new ImageSample("colors-24bpp.tiff", 64, 64, ImageSample.RGBC),
            new ImageSample("colors-32bpp.bmp", 64, 64, ImageSample.RGBA),
            new ImageSample("colors-32bpp.png", 64, 64, ImageSample.RGBA),
            new ImageSample("colors-32bpp.tiff", 64, 64, ImageSample.RGBA),
            new ImageSample("colors-19x17-32bpp.png", 19, 17, ImageSample.RGBA)
        };


        public class ImageSample
        {
            public static Color[] BWWB = new Color[] {Color.Black, Color.White, Color.White, Color.Black};
            public static Color[] RGBA = new Color[] {Color.Red, Color.Lime, Color.Blue, Color.FromArgb(100, 128, 192, 255)};
            public static Color[] RGBC = new Color[] {Color.Red, Color.Lime, Color.Blue, Color.FromArgb(255, 205, 230, 255)};
            public static Color[] RGBT = new Color[] {Color.Red, Color.Lime, Color.Blue, Color.FromArgb(0, 0, 0, 0)};
            public static Color[] RGBW = new Color[] {Color.Red, Color.Lime, Color.Blue, Color.White};

            public static Color[] RGBC_JPG = new Color[]
            {
                Color.FromArgb(255, 254, 1, 3),
                Color.FromArgb(255, 1, 255, 3),
                Color.FromArgb(255, 1, 2, 253),
                Color.FromArgb(255, 206, 230, 255)
            };

            public string ResourceName { get; }
            public int Width { get; }
            public int Height { get; }
            public Color TopLeftColor { get; }
            public Color TopRightColor { get; }
            public Color BottomLeftColor { get; }
            public Color BottomRightColor { get; }

            public ImageSample(string resourceName, int width, int height, Color[] colors)
            {
                ResourceName = resourceName;
                Width = width;
                Height = height;
                TopLeftColor = Color.FromArgb(colors[0].ToArgb());
                TopRightColor = Color.FromArgb(colors[1].ToArgb());
                BottomLeftColor = Color.FromArgb(colors[2].ToArgb());
                BottomRightColor = Color.FromArgb(colors[3].ToArgb());
            }

            public override string ToString()
            {
                return ResourceName;
            }

            public Stream CreateResourceStream()
            {
                Stream stream = typeof(TestImageCodec).Assembly.GetManifestResourceStream(typeof(TestImageCodec), $"Resources.{ResourceName}");

                if (stream == null)
                {
                    throw new IOException($"Resource '{ResourceName}' was not found.");
                }

                return stream;
            }
        }
    }
}