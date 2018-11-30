using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class ColorPoint
    {
        public int color { get; set; }
        public Point point { get; set; }
        public ColorPoint()
        {
            point = new Point(0, 0);
            color = 0;
        }
        public ColorPoint(int color,Point point)
        {
            this.color = color;
            this.point = point;
        }
    }
}
