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

   static class ImageSupporter
    {


        static public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        /* static public BitmapImage Bitmap2BitmapImage2(Bitmap bitmap)
         {
             IntPtr hBitmap = bitmap.GetHbitmap();
             BitmapImage retval;

             try
             {
                 retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
                              hBitmap,
                              IntPtr.Zero,
                              Int32Rect.Empty,
                             
             }
             finally
             {
                 DeleteObject(hBitmap);
             }

             return retval;
         }*/

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
                // bitmapImage.DecodePixelHeight = bitmap.Height;
                //bitmapImage.DecodePixelWidth = bitmap.Width;
                bitmapImage.EndInit();
                bitmapImage.Freeze();


                return bitmapImage;
            }
        }

        public static Bitmap To8bit(this Bitmap image)
        {
            using (var bitmap = image)
            using (var stream = new MemoryStream())
            {
                var parameters = new EncoderParameters(1);
                parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8L);

                var info = GetEncoderInfo("image/tiff");
                bitmap.Save(stream, info, parameters);

                return new Bitmap(Image.FromStream(stream));
            }
        }
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

        public static Image convertBitmapToImage(Bitmap bitmap)
        {
            return (Image)bitmap;

        }
        public static BitmapImage convertImageToBitmapimage(Image image)
        {
            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static BitmapImage bitmapToBitmapimage2(Bitmap bitmap)
        {
            return convertImageToBitmapimage(convertBitmapToImage(bitmap));
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
        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              System.Windows.Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

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

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }



        public static void Save(this BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        public static double restrictAngle(double an)
        {
            if (an > DegreeToRadian(180)) an -= DegreeToRadian(180);
            return an;
        }


        public static void matchLine(Bitmap canvas, Point s, Point e)
        {
            matchLine(canvas, s, e, Color.Blue, 1);
        }
        public static void matchLine(Bitmap canvas, Point s, Point e, int thickness)
        {
            matchLine(canvas, s, e, Color.Blue, thickness);
        }
        public static void matchLine(Bitmap canvas, Point s, Point e, Color color, int thickness)//canvas->bitmapa na której zaznaczamy prostą; s->punkt startowy; e-punkt końcowy; nazewnictwo s i e jes tbez znaczenia
        {
            Point p = s;
            Point k = e;
            double angle;
            if (Math.Abs(p.X - k.X) == 0)
            {
                angle = 0;
            }
            else
                angle = Math.Atan(Math.Abs(p.Y - k.Y) / Math.Abs(p.X - k.X));
            angle += DegreeToRadian(90);
            Dictionary<Point, int> section = new Dictionary<Point, int>();
            List<Point> sectionOne = new List<Point>();
            Bitmap bitmap1;//= new Bitmap(1, 1);
            if (ImageSupporter.RadianToDegree(angle) > 80 && ImageSupporter.RadianToDegree(angle) < 100)
            {
                int x = 0;
            }
            if (Math.Abs(p.Y - k.Y) > 0 && Math.Abs(p.X - k.X) > 0)
            {
                bitmap1 = new Bitmap(Math.Abs(p.X - k.X), Math.Abs(p.Y - k.Y));
            }
            else if (Math.Abs(p.Y - k.Y) == 0 && Math.Abs(p.X - k.X) == 0)
            {
                bitmap1 = new Bitmap(1, 1);
            }
            else if (Math.Abs(p.Y - k.Y) == 0)
            {
                bitmap1 = new Bitmap(Math.Abs(p.X - k.X), 1);
            }
            else
            {
                bitmap1 = new Bitmap(1, Math.Abs(p.Y - k.Y));
            }
            int xprzesuniecie = p.X < k.X ? p.X : k.X;
            int yprzesuniecie = p.Y < k.Y ? p.Y : k.Y;




            double a = Math.Tan(angle);
            if (ImageSupporter.RadianToDegree(angle) > 45 && ImageSupporter.RadianToDegree(angle) <= 90)
            {
                a = Math.Tan(ImageSupporter.DegreeToRadian(90) - angle);

                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Height; i++)
                    {


                        if ((int)((j - (bitmap1.Width) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Height) / 2) + 0.5)))
                        {

                            sectionOne.Add(new Point(j, i));
                        }

                    }
                }
            }
            else if (ImageSupporter.RadianToDegree(angle) < 135 && ImageSupporter.RadianToDegree(angle) > 90)
            {
                a = Math.Tan(ImageSupporter.DegreeToRadian(270) - angle);
                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Height; i++)
                    {


                        if ((int)((j - (bitmap1.Width) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Height) / 2) + 0.5)))
                        {
                            sectionOne.Add(new Point(j, i));
                        }

                    }
                }
            }

            else
            {
                for (int j = 0; j < bitmap1.Height; j++)
                {
                    for (int i = 0; i < bitmap1.Width; i++)
                    {
                        if ((int)((j - (bitmap1.Height) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Width) / 2) + 0.5)))
                        {
                            sectionOne.Add(new Point(i, j));
                        }

                    }
                }
            }


            foreach (Point n in sectionOne)
            {
                if (n.Y + yprzesuniecie < canvas.Height && n.Y + yprzesuniecie > 0 && n.X + xprzesuniecie < canvas.Width && n.X + xprzesuniecie > 0)
                {
                    canvas.SetPixel(n.X + xprzesuniecie, n.Y + yprzesuniecie, color);
                }
            }

            bitmap1.Dispose();

        }

        public static void matchLine(Bitmap canvas, Point c, double angle, Color color, int sectionLen)
        {

            angle += ImageSupporter.DegreeToRadian(90);
            angle = ImageSupporter.restrictAngle(angle);

            Point p = new Point((int)(c.X - Math.Cos(angle) * sectionLen + 0.5), (int)(c.Y - Math.Sin(angle) * sectionLen + 0.5));
            Point k = new Point((int)(c.X + Math.Cos(angle) * sectionLen + 0.5), (int)(c.Y + Math.Sin(angle) * sectionLen + 0.5));
            Dictionary<Point, int> section = new Dictionary<Point, int>();
            List<Point> sectionOne = new List<Point>();
            Bitmap bitmap1;//= new Bitmap(1, 1);
            if (ImageSupporter.RadianToDegree(angle) > 80 && ImageSupporter.RadianToDegree(angle) < 100)
            {
                int x = 0;
            }
            if (Math.Abs(p.Y - k.Y) > 0 && Math.Abs(p.X - k.X) > 0)
            {
                bitmap1 = new Bitmap(Math.Abs(p.X - k.X), Math.Abs(p.Y - k.Y));
            }
            else if (Math.Abs(p.Y - k.Y) == 0 && Math.Abs(p.X - k.X) == 0)
            {
                bitmap1 = new Bitmap(1, 1);
            }
            else if (Math.Abs(p.Y - k.Y) == 0)
            {
                bitmap1 = new Bitmap(Math.Abs(p.X - k.X), 1);
            }
            else
            {
                bitmap1 = new Bitmap(1, Math.Abs(p.Y - k.Y));
            }
            int xprzesuniecie = p.X < k.X ? p.X : k.X;
            int yprzesuniecie = p.Y < k.Y ? p.Y : k.Y;




            double a = Math.Tan(angle);
            if (ImageSupporter.RadianToDegree(angle) > 45 && ImageSupporter.RadianToDegree(angle) <= 90)
            {
                a = Math.Tan(ImageSupporter.DegreeToRadian(90) - angle);

                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Height; i++)
                    {


                        if ((int)((j - (bitmap1.Width) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Height) / 2) + 0.5)))
                        {

                            sectionOne.Add(new Point(j, i));
                        }

                    }
                }
            }
            else if (ImageSupporter.RadianToDegree(angle) < 135 && ImageSupporter.RadianToDegree(angle) > 90)
            {
                a = Math.Tan(ImageSupporter.DegreeToRadian(270) - angle);
                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Height; i++)
                    {


                        if ((int)((j - (bitmap1.Width) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Height) / 2) + 0.5)))
                        {
                            sectionOne.Add(new Point(j, i));
                        }

                    }
                }
            }

            else
            {
                for (int j = 0; j < bitmap1.Height; j++)
                {
                    for (int i = 0; i < bitmap1.Width; i++)
                    {
                        if ((int)((j - (bitmap1.Height) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Width) / 2) + 0.5)))
                        {
                            sectionOne.Add(new Point(i, j));
                        }

                    }
                }
            }


            foreach (Point n in sectionOne)
            {
                if (n.Y + yprzesuniecie < canvas.Height)
                {
                    canvas.SetPixel(n.X + xprzesuniecie, n.Y + yprzesuniecie, color);
                }
            }

        }

        public static void matchLine1(Bitmap canvas, Point s, Point e, Color color, int thickness)
        {
            for (int i = s.X < e.X ? s.X : e.X; i < (s.X > e.X ? s.X : e.X); i++)
            {
                for (int j = s.Y < e.Y ? s.Y : e.Y; j < (s.Y > e.Y ? s.Y : e.Y); j++)
                {
                    if (Math.Abs((s.X - i) / (s.Y - j) - (s.X - e.X) / (s.Y - e.Y)) < 0.1)
                    {
                        canvas.SetPixel(i, j, Color.Blue);
                    }
                }
            }
            canvas.SetPixel(s.X, s.Y, Color.Green);
            canvas.SetPixel(e.X, e.Y, Color.Green);

        }
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
                            //color = Color.FromArgb(0, 0, 128);
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

                // EITHER horizontal OR vertical step (but not both!)
                if (e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                }
                else if (e2 < dx)
                { // <--- this "else" mxzcczxakes the difference
                    err += dx;
                    y0 += sy;
                }
            }
        }
        public static double angletoIIIandIV(double angle)
        {
            if (ImageSupporter.DegreeToRadian(180) < angle && ImageSupporter.DegreeToRadian(180) > angle)
            {
                angle += ImageSupporter.DegreeToRadian(180);
            }
            return angle;
        }

        public static Color getRandomColor()
        {
            Random random = new Random();

            return Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
        }

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
           // line.Reverse();
           
            return line;


        }


        public static Bitmap ColorToGrayscale(Bitmap bmp)
        {
            int w = bmp.Width,
                h = bmp.Height,
                r, ic, oc, bmpStride, outputStride, bytesPerPixel;
            PixelFormat pfIn = bmp.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            //Create the new bitmap
            output = new Bitmap(w, h, PixelFormat.Format8bppIndexed);

            //Build a grayscale color Palette
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            //No need to convert formats if already in 8 bit
            if (pfIn == PixelFormat.Format8bppIndexed)
            {
                output = (Bitmap)bmp.Clone();

                //Make sure the palette is a grayscale palette and not some other
                //8-bit indexed palette
                output.Palette = palette;

                return output;
            }

            //Get the number of bytes per pixel
            switch (pfIn)
            {
                case PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException("Image format not supported");
            }

            //Lock the images
            bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
                                   pfIn);
            outputData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
                                         PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            //Traverse each pixel of the image
            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                if (bytesPerPixel == 3)
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = .299*R + .587*G + .114*B
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 3, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                (0.299f * bmpPtr[r * bmpStride + ic] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 2]);
                }
                else //bytesPerPixel == 4
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = alpha * (.299*R + .587*G + .114*B)
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 4, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                ((bmpPtr[r * bmpStride + ic] / 255.0f) *
                                (0.299f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 2] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 3]));
                }
            }

            //Unlock the images
            bmp.UnlockBits(bmpData);
            output.UnlockBits(outputData);

            return output;
        }

        public static int[,] GetArea(Point srodek, int wielkoscObszaru, Bitmap bitmapaOrginalna)
        {
            //Bitmap bitmap = new Bitmap(wielkoscObszaru, wielkoscObszaru);
            int[,] area = new int[wielkoscObszaru, wielkoscObszaru];
            int a = 0;
            int b = 0;
            for (int i = srodek.X - wielkoscObszaru / 2; i <= srodek.X + wielkoscObszaru / 2; i++)
            {
                for (int j = srodek.Y - wielkoscObszaru / 2; j <= srodek.Y + wielkoscObszaru / 2; j++)
                {
                    // bitmap.SetPixel(a, b, bitmapaOrginalna.GetPixel(i, j));
                    area[a, b] = bitmapaOrginalna.GetPixel(i, j).R;
                    a++;
                }
                b++;
                a = 0;
            }

            return area;
        }
        public static Bitmap GetAreaBitmap(Point srodek, int wielkoscObszaru, Bitmap bitmapaOrginalna)
        {
            //Bitmap bitmap = new Bitmap(wielkoscObszaru, wielkoscObszaru);
            // int[,] area = new int[wielkoscObszaru, wielkoscObszaru];
            Bitmap area = new Bitmap(wielkoscObszaru, wielkoscObszaru);
            int a = 0;
            int b = 0;
            for (int i = srodek.X - wielkoscObszaru / 2; i <= srodek.X + wielkoscObszaru / 2; i++)
            {
                for (int j = srodek.Y - wielkoscObszaru / 2; j <= srodek.Y + wielkoscObszaru / 2; j++)
                {
                    // bitmap.SetPixel(a, b, bitmapaOrginalna.GetPixel(i, j));
                    if(i<bitmapaOrginalna.Width&&i>0&&j<bitmapaOrginalna.Height&&j>0&a<area.Width&&a>0&&b<area.Height&&b>0)
                    area.SetPixel(a,b,bitmapaOrginalna.GetPixel(i, j));
                    a++;
                }
                b++;
                a = 0;
            }

            return area;
        }


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

        public static Bitmap GrayScaleToColor(Bitmap bitmap)
        {
            GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
            return grayscaleToRGB.Apply(ImageSupporter.ColorToGrayscale(bitmap));
        }


        public static void MatchMinutia2(Bitmap bitmap, Color zakonczenie, Color rozwidlenie, Minutia minutia)
        {
            int xo = minutia.p.X, yo = minutia.p.Y;// center of circle
            int r, rr;

            r = 3;
            //rr =(int)( Math.Pow(r, 2));
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
    }
   
    

}
