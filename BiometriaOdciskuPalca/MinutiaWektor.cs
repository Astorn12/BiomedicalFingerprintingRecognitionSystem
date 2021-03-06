﻿using System;
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
        public MinutiaWektor()
        {
            this.m = new List<Minutia>();
        }

        public int GetMinutiaCount()
        {
            return m.Count();
        }

        public MinutiaWektor Clone()
        {
            List<Minutia> cloneList = new List<Minutia>();
            foreach(var item in m)
            {
                cloneList.Add((Minutia)item.Clone());
            }
            return new MinutiaWektor(cloneList);
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

        public void Add(Minutia m)
        {
            this.m.Add(m);
        }
    }
}
