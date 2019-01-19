using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class Minutia : ICloneable
    {
        public Point p { get; set; }
        public double direction { get; set; }
         public KindOfMinutia kind { get; set; }

        public Minutia(Point point,double angle,KindOfMinutia kind)
        {
            this.p = point;
            this.direction = angle;
            this.kind = kind;
            
        }

        

         public object Clone()
        {
            return new Minutia(new Point(p.X, p.Y), direction, kind);
        }

        
    }
}
