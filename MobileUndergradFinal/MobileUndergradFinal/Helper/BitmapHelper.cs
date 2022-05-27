using System.IO;
using Android.Graphics;
using Android.Media;
using Java.Lang;
using Math = System.Math;
using Orientation = Android.Media.Orientation;
using Stream = System.IO.Stream;

namespace MobileUndergradFinal.Helper
{
    public static class BitmapHelper
    {
        public static void WriteToStream(this Bitmap bitmap, Stream stream)
        {
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
        }

        public static Bitmap GetOfScale(int intendedWidth, int intendedHeight, Stream imageStream)
        {
            var bmOptions = new BitmapFactory.Options { InJustDecodeBounds = true };

            BitmapFactory.DecodeStream(imageStream, null, bmOptions);

            var photoW = bmOptions.OutWidth;
            var photoH = bmOptions.OutHeight;

            var scaleFactor = Math.Max(1, Math.Min(photoW / intendedWidth, photoH / intendedHeight));

            bmOptions.InJustDecodeBounds = false;
            bmOptions.InSampleSize = scaleFactor;

            imageStream.Seek(0, SeekOrigin.Begin);

            return BitmapFactory.DecodeStream(imageStream, null, bmOptions);
        }

        public static Bitmap GetRotated(int intendedWidth, int intendedHeight, Stream imageStream)
        {
            var bm = GetOfScale(intendedWidth, intendedHeight, imageStream);
            imageStream.Seek(0, SeekOrigin.Begin);
            var exif = new ExifInterface(imageStream);
            var orientation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation,
            (int)Orientation.Undefined);
            return bm.RotateBitmap(orientation);
        }

        public static Bitmap RotateBitmap(this Bitmap bitmap, Orientation orientation)
        {

            var matrix = new Matrix();
            switch (orientation)
            {
                case Orientation.Normal:
                    return bitmap;
                case Orientation.FlipHorizontal:
                    matrix.SetScale(-1, 1);
                    break;
                case Orientation.Rotate180:
                    matrix.SetRotate(180);
                    break;
                case Orientation.FlipVertical:
                    matrix.SetRotate(180);
                    matrix.PostScale(-1, 1);
                    break;
                case Orientation.Transpose:
                    matrix.SetRotate(90);
                    matrix.PostScale(-1, 1);
                    break;
                case Orientation.Rotate90:
                    matrix.SetRotate(90);
                    break;
                case Orientation.Transverse:
                    matrix.SetRotate(-90);
                    matrix.PostScale(-1, 1);
                    break;
                case Orientation.Rotate270:
                    matrix.SetRotate(-90);
                    break;
                default:
                    return bitmap;
            }
            try
            {
                var bmRotated = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
                bitmap.Recycle();
                return bmRotated;
            }
            catch (OutOfMemoryError e)
            {
                e.PrintStackTrace();
                return null;
            }
        }
    }
}