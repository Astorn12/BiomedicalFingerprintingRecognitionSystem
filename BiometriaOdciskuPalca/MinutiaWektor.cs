using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class MinutiaWektor
    {
        public List<Minutia> m { get; set; }
       
        public MinutiaWektor(List<Minutia> m)
        {
            this.m = m;
        }

        public int GetMinutiaCount()
        {
            return m.Count();
        }


        public int GetForkCount()
        {
            int n = 0;
            foreach(var item in m )
            {
                if (item.kind.Equals(KindOfMinutia.ROZWIDLENIE))
                n++;
            }
            return n;
        }

        public int GetEndCount()
        {
            int n = 0;
            foreach (var item in m)
            {
                if (item.kind.Equals(KindOfMinutia.ZAKONCZENIE))
                n++;
            }
            return n;
        }
    }
}
