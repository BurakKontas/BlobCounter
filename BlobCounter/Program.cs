using OpenCvSharp;
using System;
using System.IO;

namespace BlobCounter
{
    internal class Program
    {
        private static readonly string FILE_PATH = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "");

        static void Main(string[] args)
        {
            for (int i = 1; i < 11; i++)
            {
                Test(i);
            }
            //Test(12);
        }

        private static void Test(int number)
        {
            string originalPath = GetAssets($"original{number}.png");
            string bgPath = GetAssets("background.png");

            int threshold = 400;

            Mat original = Cv2.ImRead(originalPath, ImreadModes.Color);
            Mat background = Cv2.ImRead(bgPath, ImreadModes.Color);

            if (original.Width != background.Width || original.Height != background.Height)
            {
                Console.WriteLine("Hata: Resimlerin boyutları eşleşmiyor.");
                return;
            }

            Mat diff = new Mat();
            Cv2.Absdiff(original, background, diff);

            Cv2.ImShow("Diff", diff);

            Mat mask = new Mat();
            Cv2.CvtColor(diff, mask, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(mask, mask, 36.5, 400, ThresholdTypes.BinaryInv);

            Cv2.ImShow("Mask", mask);

            Point[][] contours;
            Cv2.FindContours(mask, out contours, out _, RetrievalModes.List, ContourApproximationModes.ApproxTC89KCOS);
   
            int objectCounter = 0;
            foreach (Point[] contour in contours)
            {
                Rect rect = Cv2.BoundingRect(contour);

                int area = rect.Width * rect.Height;
                if (area < threshold) continue;

                Cv2.Rectangle(original, rect, Scalar.Red, 2);

                Point labelPosition = new Point(rect.X, rect.Y - 7);
                Cv2.PutText(original, $"Nesne {++objectCounter}", labelPosition, HersheyFonts.HersheyComplex, 0.5, Scalar.Red);
            }

            Cv2.ImShow("Segmented Objects", original);
            Cv2.WaitKey(0);
        }

        private static string GetAssets(string fileName)
        {
            return Path.Combine(FILE_PATH, "Assets", fileName);
        }
    }
}
