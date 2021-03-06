﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{      //clasa opisująza o jaki kąt i jakie przesunięcie należy zmodyfikować obraz odcisku palca
    class ModyficationElement
    {
        public int x { get; set; }//przesunięcie względem osi X
        public int y { get; set; }//przesunięcie względem osi Y
        public int angle { get; set; }//obrót
                                      //       public int vote { get; set; }//ilość głosów oddanych na modyfikację

        public ModyficationElement(int x, int y, int angle)
        {
            this.x = x;
            this.y = y;
            this.angle = angle;
          

        }
        
            public override string ToString()
            {
           
            return this.angle + " (" + this.x + "," + this.y + ")";

            }
    }
}
