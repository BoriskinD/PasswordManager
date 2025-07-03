using SkiaSharp;

namespace Client
{
    public class Image
    {
        public string ResizeImage(string imagePath, int newWidth, int newHeight)
        { 
            FileStream inputStream = File.OpenRead(imagePath);
            using SKBitmap originalBitmap = SKBitmap.Decode(inputStream);
            inputStream.Close();

            SKBitmap resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default);

            using SKImage image = SKImage.FromBitmap(resizedBitmap);
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);

            using FileStream outputStream = File.OpenWrite(imagePath);
            data.SaveTo(outputStream);

            return imagePath;
        }
    }
}
