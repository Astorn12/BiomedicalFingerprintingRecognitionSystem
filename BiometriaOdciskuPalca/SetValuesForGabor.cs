using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{   //klasa przechowująca zbiór parametrów dla filtru Gabora
    class SetValuesForGabor
    {
        public float gamma { get; set; }
        public float lambda { get; set; }
        public float psi { get; set; }
        public float sigma { get; set; }
        public float theta { get; set; }

        public SetValuesForGabor(float a, float b, float c, float d, float e)
        {
            gamma = a;
            lambda = b;
            psi = c;
            sigma = d;
            theta = e;
        }
        public void SetGaborFilter(GaborFilter gb)
        {
            gb.Lambda = lambda;
            gb.Gamma = gamma;
            gb.Psi = psi;
            gb.Sigma = sigma;
            gb.Theta = theta;
        }
    }
}
