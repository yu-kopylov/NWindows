using System.IO;
using NUnit.Framework;
using NWindows;

namespace Test.NWindows
{
    public class TestImageCodec
    {
        [Test]
        public void TestLoadFromFile()
        {
            string testFolder = Path.Combine("Temp", "Test");
            Directory.CreateDirectory(testFolder);

            var app = new NApplication();
            app.Init();

            TestLoadFromFile(app, testFolder, "colors-1bpp.bmp");
            TestLoadFromFile(app, testFolder, "colors-4bpp.bmp");
            TestLoadFromFile(app, testFolder, "colors-8bpp.bmp");
            TestLoadFromFile(app, testFolder, "colors-8bpp.gif");
            TestLoadFromFile(app, testFolder, "colors-8bpp.png");
            TestLoadFromFile(app, testFolder, "colors-8bpp.tiff");
            TestLoadFromFile(app, testFolder, "colors-24bpp.bmp");
            TestLoadFromFile(app, testFolder, "colors-24bpp.jpg");
            TestLoadFromFile(app, testFolder, "colors-24bpp.png");
            TestLoadFromFile(app, testFolder, "colors-24bpp.tiff");
            TestLoadFromFile(app, testFolder, "colors-32bpp.bmp");
            TestLoadFromFile(app, testFolder, "colors-32bpp.png");
            TestLoadFromFile(app, testFolder, "colors-32bpp.tiff");
        }

        private void TestLoadFromFile(NApplication app, string testFolder, string resourceName)
        {
            CopyResourceToFile(testFolder, resourceName);
            var image = app.ImageCodec.LoadImageFromFile(Path.Combine(testFolder, resourceName));
            Assert.That(image.Width, Is.EqualTo(64), () => $"{resourceName}, {nameof(image.Width)}");
            Assert.That(image.Height, Is.EqualTo(64), () => $"{resourceName}, {nameof(image.Height)}");
        }

        private void CopyResourceToFile(string outputFolder, string resourceName)
        {
            using (var resourceStream = typeof(TestImageCodec).Assembly.GetManifestResourceStream(typeof(TestImageCodec), $"Resources.{resourceName}"))
            {
                if (resourceStream == null)
                {
                    throw new IOException($"Resource '{resourceName}' was not found.");
                }

                using (var fileStream = new FileStream(Path.Combine(outputFolder, resourceName), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
        }
    }
}