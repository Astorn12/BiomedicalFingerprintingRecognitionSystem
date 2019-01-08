using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class DatabaseElement
    {
        public String FingerprntName { get; set; }
       public MinutiaWektor MinutiaesWektor { get; set; }

        public DatabaseElement(string fingetrprontName,MinutiaWektor minuteasList )
        {
            this.FingerprntName = fingetrprontName;
            this.MinutiaesWektor = minuteasList;
        }
    }
}
