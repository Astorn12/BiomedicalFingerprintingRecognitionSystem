using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    //klasa reprezentująca elementy bazy danych, jest to klasa łącząca wektor cech obrazu z nazwą obrazu
    class DatabaseElement
    {
       public String FingerprntName { get; set; }//nazwa obrazu linii papilarnych, w programie imię i nazwisko posiadacza linii papilarnej
       public MinutiaWektor MinutiaesWektor { get; set; }//wektor cech obrazu linii papilarnej

        public DatabaseElement(string fingetrprontName,MinutiaWektor minuteasList )
        {
            this.FingerprntName = fingetrprontName;
            this.MinutiaesWektor = minuteasList;
        }


        public DatabaseElement Clone()
        {
            return new DatabaseElement(this.FingerprntName, this.MinutiaesWektor.Clone());

        }
    }
}
