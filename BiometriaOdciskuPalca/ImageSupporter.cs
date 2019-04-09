using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using  System.Drawing;
using System.Drawing.Imaging;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
//using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BiometriaOdciskuPalca
{
    //klasa zawierająza metody statyczne wspierające pracę z obrazem linii papilarnych
   static class ImageSupporter
    {
        #region Methods

        #region Konwertowanie pomiędzy klasami Bitamap i BitmapImage
        static public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        public static BitmapImage Bitmap2BitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
               
                bitmapImage.EndInit();
                bitmapImage.Freeze();


                return bitmapImage;
            }
        }
        #endregion
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }

        public static Bitmap convertToModuloBitmap(Bitmap bitmap, int modulo)
        {
            Bitmap newMap = new Bitmap(bitmap.Width - (bitmap.Width % modulo)+modulo, bitmap.Height - (bitmap.Height % modulo)+modulo);

            for (int i = 0; i < newMap.Width; i++)
            {
                for (int j = 0; j < newMap.Height; j++)
                {
                    Color color;
                    if (i<bitmap.Width&&j<bitmap.Height)
                    {
                        color = bitmap.GetPixel(i, j);
                    }
                    else
                    {
                        color = Color.White;
                    }
                    newMap.SetPixel(i, j, color);
                }
            }
            return newMap;

        }

        [DllImport("gdi32", EntryPoint = "DeleteObject")]
        public static extern bool DeleteHBitmap(IntPtr hObject);
        public static BitmapSource ToBitmapSource(this Bitmap target)
        {
            var hbitmap = target.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteHBitmap(hbitmap);
            }
        }

        public static Bitmap Scale(int count, Bitmap source)
        {
            if (count <= 0) { return source; }
            var bitmap = new Bitmap(source.Size.Width * count, source.Size.Height * count);
            var sourcedata = source.LockBits(new Rectangle(new System.Drawing.Point(0, 0), source.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitmapdata = bitmap.LockBits(new Rectangle(new System.Drawing.Point(0, 0), bitmap.Size), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            unsafe
            {
                var srcByte = (byte*)sourcedata.Scan0.ToPointer();
                var dstByte = (byte*)bitmapdata.Scan0.ToPointer();
                for (var y = 0; y < bitmapdata.Height; y++)
                {
                    for (var x = 0; x < bitmapdata.Width; x++)
                    {
                        long index = (x / count) * 4 + (y / count) * sourcedata.Stride;
                        dstByte[0] = srcByte[index];
                        dstByte[1] = srcByte[index + 1];
                        dstByte[2] = srcByte[index + 2];
                        dstByte[3] = srcByte[index + 3];
                        dstByte += 4;
                    }
                }
            }
            source.UnlockBits(sourcedata);
            bitmap.UnlockBits(bitmapdata);

            return bitmap;
        }
        
        //wypisuje poziomy R w RGB wszystkich pikseli
        public static void WriteBitmap(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Console.Write(bitmap.GetPixel(x, y).R + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        public static void matchArea(System.Windows.Controls.Image image, int xStart, int yStart, int width, int height)
        {
            Bitmap bitmap = ImageSupporter.BitmapImage2Bitmap((BitmapImage)image.Source);


            for (int a = 0; a < width; a++)
            {
                for (int b = 0; b < height; b++)
                {
                    byte bit = bitmap.GetPixel(a + xStart, b + yStart).R;
                    if (bit + 122 < 255)
                        bitmap.SetPixel(a + xStart, b + yStart, Color.FromArgb(bit + 122, bit, bit));
                    else
                        bitmap.SetPixel(a + xStart, b + yStart, Color.FromArgb(bit - 122, bit, bit));
                }
            }
            image.Source = ImageSupporter.Bitmap2BitmapImage(bitmap);

        }
        #region Konwersja między stopniami i radianami
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        #endregion
        //zapisuje obraz zgodnie z zadaną ścieżką
        public static void Save(this BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
        //zmienia zadany kąt na kąt w zakresie od 0 do 180 stopni
        public static double restrictAngle(double an)
        {
            if (an > DegreeToRadian(180)) an -= DegreeToRadian(180);
            return an;
        }
        //zaznacza na obrazie piksele reprezentujące odcinek między punktami s i e
        public static void matchLine2(Bitmap canvas, Point s, Point e, Color color, int thickness)
        {
            foreach (var item in EnumerateLineNoDiagonalSteps(s.X, s.Y, e.X, e.Y))
            {
                try
                {
                    canvas.SetPixel(item.Item1, item.Item2, color);
                    for (int i = -(thickness / 2); i < (int)((float)thickness / 2 + 0.5); i++)
                    {
                        for (int j = -(thickness / 2); j < (int)((float)thickness / 2 + 0.5); j++)
                        {
                            
                            canvas.SetPixel(item.Item1 + i, item.Item2 + j, color);

                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }


        }

        public static IEnumerable<Tuple<int, int>> EnumerateLineNoDiagonalSteps(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                yield return Tuple.Create(x0, y0);

                if (x0 == x1 && y0 == y1) break;

                e2 = 2 * err;

                
                if (e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                }
                else if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
        //zmienia zadany kąt na kąt od 180 do 360
        public static double angletoIIIandIV(double angle)
        {
            if (ImageSupporter.DegreeToRadian(180) < angle && ImageSupporter.DegreeToRadian(180) > angle)
            {
                angle += ImageSupporter.DegreeToRadian(180);
            }
            return angle;
        }
        //zwraca listę punktów reprezentującą odcinek pd punktu s do punktu e
        public static List<Point> GetLine(Point s, Point e)
        {
            List<Point> line = new List<Point>();
            foreach (var item in EnumerateLineNoDiagonalSteps(s.X, s.Y, e.X, e.Y))
            {
                try
                {
                    line.Add(new Point(item.Item1, item.Item2));
                    

                }
                catch (Exception ex)
                {

                }
            }
            
           
            return line;


        }
        //konwertowanie do 8 bitowego obrazu w odcieniu szarości
        public static Bitmap ColorToGrayscale(Bitmap bmp)
        {
            int w = bmp.Width,
                h = bmp.Height,
                r, ic, oc, bmpStride, outputStride, bytesPerPixel;
            PixelFormat pfIn = bmp.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            
            output = new Bitmap(w, h, PixelFormat.Format8bppIndexed);

            
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            
            if (pfIn == PixelFormat.Format8bppIndexed)
            {
                output = (Bitmap)bmp.Clone();

               
                output.Palette = palette;

                return output;
            }

          
            switch (pfIn)
            {
                case PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException("Image format not supported");
            }

           
            bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
                                   pfIn);
            outputData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
                                         PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                if (bytesPerPixel == 3)
                {
                   
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 3, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                (0.299f * bmpPtr[r * bmpStride + ic] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 2]);
                }
                else 
                {
                   
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 4, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                ((bmpPtr[r * bmpStride + ic] / 255.0f) *
                                (0.299f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 2] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 3]));
                }
            }

            
            bmp.UnlockBits(bmpData);
            output.UnlockBits(outputData);

            return output;
        }
        //odwracanie odcieni na obrazie
        public static Bitmap ReverseBitmap(Bitmap b)
       
        {
            for(int i=0;i<b.Width;i++)
            {
                for(int j=0;j<b.Height;j++)
                {
                    byte rev = (byte)(255 - b.GetPixel(i, j).R);
                    b.SetPixel(i, j, Color.FromArgb(rev, rev, rev));
                }
            }
            return b;
        }
        //konwertowanie do 8 bitowego obrazu w odcieniu szarości
        public static Bitmap GrayScaleToColor(Bitmap bitmap)
        {
            GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
            return grayscaleToRGB.Apply(ImageSupporter.ColorToGrayscale(bitmap));
        }
        //metoda zaznaczająca minucje na bitmapie
        public static void MatchMinutia2(Bitmap bitmap, Color zakonczenie, Color rozwidlenie, Minutia minutia)
        {
            int xo = minutia.p.X, yo = minutia.p.Y;// center of circle
            int r, rr;

            r = 3;
           
            Color color;
            if (minutia.kind.Equals(KindOfMinutia.ZAKONCZENIE)) color = zakonczenie;
            else color = rozwidlenie;
            for (int i = xo - (int)r; i <= xo + r; i++)
                for (int j = yo - (int)r; j <= yo + r; j++)
                    if (Math.Abs(Math.Sqrt(Math.Pow(i - xo, 2) + Math.Pow(j - yo, 2)) - (float)r) < 0.5f)
                    {
                        try
                        {
                            bitmap.SetPixel(i, j, color);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
        }
#endregion
    }
   
    

}
