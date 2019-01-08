using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class MinutiaWektorComperer
    {
        public int Limit { get; set; }

        public MinutiaWektorComperer (int limit)
        {
            this.Limit = limit;
        }



        public Tuple<bool,float> Compere(MinutiaWektor froDatabase, MinutiaWektor scaned)
        {
            return new Tuple<bool, float>(false, 0);
        }

    }
}
