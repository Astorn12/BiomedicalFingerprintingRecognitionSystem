using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class GridedImage
    {
        #region Atributes
        ImageCell[,] cellTab;
        public Bitmap bitmap { get; set; }
        int cellSize;
        #endregion
        #region Constructors
        public GridedImage(Bitmap bitmap,int cellSize)
        {
            this.bitmap = bitmap;
            this.cellSize = cellSize;
            int number = bitmap.Width / cellSize;
            int numberH = bitmap.Height / cellSize;
            cellTab = new ImageCell[number, numberH];
            gridIt();
        }
        public GridedImage(int cellQuantity,Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.cellSize = bitmap.Width / cellQuantity;
            int number = bitmap.Width / cellSize;
            int numberH = bitmap.Height / cellSize;
            cellTab = new ImageCell[number, numberH];
            gridIt();
        }
        #endregion
        #region Method
        public void gridIt()
        {
            for(int i=0;i<bitmap.Width;i+=cellSize)
            {
                for(int j=0;j<bitmap.Height;j+=cellSize)
                {
                    Bitmap gridmap = new Bitmap(cellSize, cellSize);
                    for(int a=i;a<i+cellSize;a++)
                    {
                        for(int b=j;b<j+cellSize;b++)
                        {
                            gridmap.SetPixel(a - i, b - j, bitmap.GetPixel(a, b));
                        }
                    }
                    ImageCell imageCell = new ImageCell(gridmap);
                    cellTab[i / cellSize, j / cellSize] =imageCell;

                }
            }
        }


        public void regridIt()
        {
            for (int i = 0; i < bitmap.Width; i += cellSize)
            {
                for (int j = 0; j < bitmap.Height; j += cellSize)
                {
                    Bitmap gridmap = new Bitmap(cellSize, cellSize);
                    for (int a = i; a < i + cellSize; a++)
                    {
                        for (int b = j; b < j + cellSize; b++)
                        {
                            gridmap.SetPixel(a - i, b - j, bitmap.GetPixel(a, b));
                        }
                    }
                    //ImageCell imageCell = new ImageCell(gridmap);

                    cellTab[i / cellSize, j / cellSize].bitmap = gridmap;

                }
            }
        }
        public void mergeIt()
        {
           int number = bitmap.Width / cellSize;
            int numberH = bitmap.Height / cellSize;
            for(int i=0;i<number;i++)
            {
                for(int j=0;j<numberH;j++)
                {
                    for(int a=i*cellSize;a<i*cellSize+cellSize;a++)
                    {
                        for(int b=j*cellSize;b<j*cellSize+cellSize;b++)
                        {
                            bitmap.SetPixel(a, b, cellTab[i, j].bitmap.GetPixel(a - i*cellSize, b - j*cellSize));
                        }
                    }
                }
            }
           // this.bitmap = cellTab[4, 4].bitmap;
            
            

        }
        #endregion

        #region Tests
        public void boundGrids()
        {
           
            for(int i=0;i<cellTab.GetLength(0);i++)
            {
                for(int j=0;j<cellTab.GetLength(1); j++)
                {
                    cellTab[i, j].setBorder(Color.Black);
                }
            }
            //mergeIt();
        }

        public void convertToDirectionMap()
        {
            for (int i = 0; i < cellTab.GetLength(0); i++)
            {
                for (int j = 0; j < cellTab.GetLength(1); j++)
                {
                    cellTab[i, j].toDirectionMap();
                }
            }
        }

        public ImageCell getRandomCell(System.Windows.Controls.Image image)
        {
            int number = bitmap.Width / cellSize;
            int numberH = bitmap.Height / cellSize;
            Random random = new Random();
            int i = random.Next(number);
            int j = random.Next(numberH);
            for(int a=0;a<cellTab[i,j].bitmap.Width;a++)
            {
                for(int b=0;b<cellTab[i,j].bitmap.Height;b++)
                {
                    byte bit =bitmap.GetPixel(a+i*16, b+j*16).R;
                    if(bit+122<255)
                   bitmap.SetPixel(a+i*16, b+j*16, Color.FromArgb(bit + 122, bit, bit));
                    else
                        bitmap.SetPixel(a+i*16, b+j*16, Color.FromArgb(bit - 122, bit, bit));
                }
            }

            ImageSupporter.matchArea(image, i * 16 , j * 16 , 16, 16);
            /*for(int a=0;a<this.bitmap.Width;a++)
            {
                for (int b = 0; b < this.bitmap.Height; b++)
                {
                    bitmap.SetPixel(a, b, Color.Black);
                }
            }*/
            return cellTab[i, j];
        }

        public ImageCell getCell(System.Windows.Controls.Image image,int x, int y)
        {

            int number = bitmap.Width / cellSize;
            int numberH = bitmap.Height / cellSize;
           // if (x < number && y < numberH)
            
                Random random = new Random();
                int i = x;
                int j = y;
                for (int a = 0; a < cellTab[i, j].bitmap.Width; a++)
                {
                    for (int b = 0; b < cellTab[i, j].bitmap.Height; b++)
                    {
                        byte bit = bitmap.GetPixel(a + i * 16, b + j * 16).R;
                        if (bit + 122 < 255)
                            bitmap.SetPixel(a + i * 16, b + j * 16, Color.FromArgb(bit + 122, bit, bit));
                        else
                            bitmap.SetPixel(a + i * 16, b + j * 16, Color.FromArgb(bit - 122, bit, bit));
                    }
                }

                ImageSupporter.matchArea(image, i * 16, j * 16, 16, 16);
            
           
                /*for(int a=0;a<this.bitmap.Width;a++)
                {
                    for (int b = 0; b < this.bitmap.Height; b++)
                    {
                        bitmap.SetPixel(a, b, Color.Black);
                    }
                }*/
                return cellTab[i, j];
            
        }

        public double getDirectionForPixel(int x, int y)
        {
            return getCellContainedPixel(x, y).getAngle();
        }

        public ImageCell getCellContainedPixel(int x,int y)
        {
            return this.cellTab[x/cellSize,y/cellSize]; //tutaj trzeba sprawdzić czy wybierają dobre okno
        }

        public Bitmap  GaborFilter()
        {
            foreach (var item in cellTab)
            {
                item.GaborFilter();
            }
            mergeIt();
            return bitmap;
        }


        public Bitmap GetGaborFIlteredImage(List<Bitmap> bank)
        {
            
            for(int i=0;i<cellTab.GetLength(0);i++)
            {
                for(int j=0;j<cellTab.GetLength(1);j++)
                {
                    cellTab[i, j].bitmap = ChangeImageCellImage(GetFilterFromBank(bank, cellTab[i, j].getAngle()), i, j);
                }
                     
            }
            mergeIt();
            return bitmap;
        }

        public Bitmap GetFilterFromBank(List<Bitmap> bank,double angle)
        {
            double jump =(3.14)/ bank.Count();
            int a = 0;
            for(double i=0;i<3.14;i+=jump)
            {
                
                if(angle>=i&&angle<i+jump)
                {
                    if (Math.Abs(angle - i) > Math.Abs(angle - i - jump))
                    {
                        if (a + 1 == bank.Count())
                            return bank[a];
                        else
                        return bank[a + 1];
                    }
                    else
                        return bank[a];


                    
                }
                a++;
            }

            return null;
        }

        public Bitmap ChangeImageCellImage(Bitmap bank,int i,int j)
        {
            Bitmap bi = new Bitmap(cellSize, cellSize);
            int x = 0;int y = 0;
            for(int a=i*cellSize;a<i*cellSize+cellSize;a++)
            {
                for (int b = j * cellSize; b < j * cellSize + cellSize; b++)
                {
                    
                    if(a>0&&b>0&&a<bank.Width&&b<bank.Height)
                    bi.SetPixel(x, y, bank.GetPixel(a, b));
                    y++;
                }
                y = 0;
                x ++;
                
            }

            return bi;
        }

        public void ShowAngles()
        {
            foreach(var item in cellTab)
            {
                Console.Write(item.getAngle()+" ");
            }
        }
        #endregion
    }
}
