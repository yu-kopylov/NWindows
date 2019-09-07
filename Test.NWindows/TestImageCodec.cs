using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            // todo: dispose?
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

            var image = app.ImageCodec.LoadImageFromFile(fileName);
            Assert.That(image.Width, Is.EqualTo(sample.Width), () => $"{sample.ResourceName}, {nameof(image.Width)}");
            Assert.That(image.Height, Is.EqualTo(sample.Height), () => $"{sample.ResourceName}, {nameof(image.Height)}");
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
            Assert.That(bitmap.GetColor(0, bitmap.Width - 1), Is.EqualTo(sample.TopRightColor), nameof(sample.TopRightColor));
            Assert.That(bitmap.GetColor(bitmap.Height - 1, 0), Is.EqualTo(sample.BottomLeftColor), nameof(sample.BottomLeftColor));
            Assert.That(bitmap.GetColor(bitmap.Height - 1, bitmap.Width - 1), Is.EqualTo(sample.BottomRightColor), nameof(sample.BottomRightColor));
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
            new ImageSample("colors-1bpp.bmp", ImageSample.BWWB),
            new ImageSample("colors-4bpp.bmp", ImageSample.RGBW),
            new ImageSample("colors-8bpp.bmp", ImageSample.RGBC),
            new ImageSample("colors-8bpp.gif", ImageSample.RGBT),
            new ImageSample("colors-8bpp.png", ImageSample.RGBT),
            new ImageSample("colors-8bpp.tiff", ImageSample.RGBC),
            new ImageSample("colors-24bpp.bmp", ImageSample.RGBC),
            new ImageSample("colors-24bpp.jpg", ImageSample.RGBC_JPG),
            new ImageSample("colors-24bpp.png", ImageSample.RGBC),
            new ImageSample("colors-24bpp.tiff", ImageSample.RGBC),
            new ImageSample("colors-32bpp.bmp", ImageSample.RGBA),
            new ImageSample("colors-32bpp.png", ImageSample.RGBA),
            new ImageSample("colors-32bpp.tiff", ImageSample.RGBA)
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
            public int Width { get; } = 64;
            public int Height { get; } = 64;
            public Color TopLeftColor { get; }
            public Color TopRightColor { get; }
            public Color BottomLeftColor { get; }
            public Color BottomRightColor { get; }

            public ImageSample(string resourceName, Color[] colors)
            {
                ResourceName = resourceName;
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