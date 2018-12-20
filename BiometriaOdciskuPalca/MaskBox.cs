using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class MaskBox
    {
        static double[, ] Sobelx=new double[3, 3] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
        static double[, ] Sobely=new double[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

        static double[,] Robertsx=new double[3, 3] { { 0, 0, 1 }, { 0, -1, 0 }, { 0, 0,0  } };
        static double[,] Robertsy=new double[3, 3] { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 0 } };

        static double[,] Pretittx = new double[3, 3] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
        static double[,] Prewitty = new double[3, 3] { { -1, 0, 1 }, { -1,0 , 1 }, { -1, 0, 1 } };

        public static double[,] GetMaskX(EMask name)
        {
            switch(name)
            {
                case EMask.Robert:
                    return Robertsx;
                    break;
                case EMask.Sobel:
                    return Sobelx;
                    break;
                case EMask.Prewitt:
                    return Pretittx;
                    break;
                default:
                    return null;
                    break;
             



            }
        }

        public static double[,] GetMaskY(EMask name)
        {
            switch (name)
            {
                case EMask.Robert:
                    return Robertsy;
                    break;
                case EMask.Sobel:
                    return Sobely;
                    break;
                case EMask.Prewitt:
                    return Prewitty;
                    break;
                default:
                    return null;
                    break;



            }
        }

    }
}
