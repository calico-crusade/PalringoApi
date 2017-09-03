using PalringoApi.Networking;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// Helpful methods for image manipulation
    /// </summary>
    public static class ImageService
    {
        /// <summary>
        /// The size images are scaled to by default in the bot (set to 0 to not scale)
        /// </summary>
        public static int ImageScaleMax = 1000;

        private static string _avatarLink = "https://www.palringo.com/showavatar.php?id={0}&size=640";

        /// <summary>
        /// Cache of stored images
        /// </summary>
        public static Dictionary<string, byte[]> Cache { get; set; } = new Dictionary<string, byte[]>();

        /// <summary>
        /// Gets an image from a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string url)
        {
            if (Cache.ContainsKey(url))
                return BitmapFromBytes(Cache[url]);

            var data = new WebClient().DownloadData(url);
            Cache.Add(url, data);
            return BitmapFromBytes(data);
        }

        /// <summary>
        /// Gets an avatar for a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Bitmap GetAvatar(int id)
        {
            return GetImage(string.Format(_avatarLink, id));
        }

        /// <summary>
        /// Gets a bitmap from a bytearray
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Bitmap BitmapFromBytes(byte[] data)
        {
            using(var ms = new MemoryStream(data))
                return new Bitmap(ms);
        }

        /// <summary>
        /// Converts a bitmap into a byte array
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static byte[] BytesFromBitmap(Bitmap map)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(map, typeof(byte[]));
        }

        /// <summary>
        /// Creates an UpdateAvatar packet stream
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public static Packet[] UpdateAvatar(byte[] avatar)
        {
            return PacketChunkanizer.Chunk(new Packet
            {
                Command = "ICON",
                Headers = new Dictionary<string, string>(),
                Payload = ""
            }, Static.PalringoEncoding.GetString(avatar));
        }

        /// <summary>
        /// Updates and avatar from a bitmap
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public static Packet[] UpdateAvatar(Bitmap avatar)
        {
            return UpdateAvatar(BytesFromBitmap(avatar));
        }
        
        /// <summary>
        /// Scales an image with low resolution cost
        /// </summary>
        /// <param name="img"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Bitmap Scale(Bitmap img, int maxWidth, int maxHeight)
        {
            double scale = 1;

            if (img.Width > maxWidth || img.Height > maxHeight)
            {
                double scaleW, scaleH;

                scaleW = maxWidth / (double)img.Width;
                scaleH = maxHeight / (double)img.Height;

                scale = scaleW < scaleH ? scaleW : scaleH;
            }

            return ResizeImage(img, (int)(img.Width * scale), (int)(img.Height * scale));
        }

        /// <summary>
        /// Resizes an image with low resolution cost
        /// </summary>
        /// <param name="srcImage"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Bitmap srcImage, int newWidth, int newHeight)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
            }
            return newImage;
        }

        /// <summary>
        /// Compiles all the images? (not actually sure, wrote it a long time ago and its alot of code)
        /// </summary>
        /// <param name="images"></param>
        /// <param name="rotate"></param>
        /// <param name="halfImage"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Bitmap CompileImages(List<Bitmap> images, bool rotate, bool halfImage = false, float angle = 180)
        {
            int newImgH = 0;
            int newImgW = 0;
            foreach (Bitmap image in images)
            {
                newImgH = image.Height * 2;
                newImgW = newImgW + (image.Width / 2);
            }

            var img = new Bitmap(newImgW, newImgH);
            Graphics gfx = Graphics.FromImage(img);
            int imageCount = 0;
            newImgH = 0;
            newImgW = 0;
            foreach (Bitmap image in images)
            {
                imageCount++;
                if (imageCount > images.Count / 2)
                {
                    gfx.DrawImage(image, newImgW, newImgH);
                    newImgW = newImgW + image.Width;
                }
                else
                {
                    gfx.DrawImage(image, newImgW, newImgH);
                    newImgW = newImgW + image.Width;
                    if (imageCount == images.Count / 2)
                    {
                        newImgH = image.Height;
                        newImgW = 0;
                    }
                }
            }
            gfx.Dispose();
            if (rotate)
            {
                img = (Bitmap)RotateImage(img, angle);
            }
            if (halfImage)
            {
                img = ResizeImage(img, img.Width / 3, img.Height / 3);
            }
            return img;
        }

        /// <summary>
        /// Rotates an image by a certain angle
        /// </summary>
        /// <param name="img"></param>
        /// <param name="rotationAngle"></param>
        /// <returns></returns>
        public static Image RotateImage(Image img, float rotationAngle)
        {
            var bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            return bmp;
        }

        /// <summary>
        /// Converts an image to Jpeg
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ConvertToJpg(Bitmap image)
        {
            var ms = new MemoryStream();
            if (ImageScaleMax != 0)
                image = Scale(image, ImageScaleMax, ImageScaleMax);
            image.Save(ms, ImageFormat.Jpeg);
            return (Bitmap)Image.FromStream(ms);
        }

        /// <summary>
        /// Converts a byte array of an image to a jpeg
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] ConvertToJpg(byte[] bytes)
        {
            return BytesFromBitmap(ConvertToJpg(BitmapFromBytes(bytes)));
        }

        /// <summary>
        /// Gets text size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fnt"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Size TextSize(string text, Font fnt, int width = -1)
        {
            using (var btmp = new Bitmap(1, 1))
            {
                using (var gfx = Graphics.FromImage(btmp))
                {
                    if (width <= 0)
                        return gfx.MeasureString(text, fnt).ToSize();
                    return gfx.MeasureString(text, fnt, width).ToSize();
                }
            }
        }
    }
}
