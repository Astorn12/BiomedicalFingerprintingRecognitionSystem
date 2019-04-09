using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{     //klasa służąca do przechowywaniu obrazów banku gabora
    class GaborBank
    {
        List<SetValuesForGabor> bank;
        public GaborBank()
        {
            bank = new List<SetValuesForGabor>();
            LoadDefaultBank();
        }

        private void LoadDefaultBank()
        {
            bank.Add(new SetValuesForGabor(4, 8, 0.5f, 9.2f, 0));
            bank.Add(new SetValuesForGabor(4, 8, 0.5f, 4.8f, 0.26f));
            bank.Add(new SetValuesForGabor(4, 6.5f, 0.5f, 4.3f, 0.52f));
            bank.Add(new SetValuesForGabor(4, 7, 0.5f, 4.3f, 0.78f));
            bank.Add(new SetValuesForGabor(4, 6.5f, 0.5f, 4.3f, 1.04f));
            bank.Add(new SetValuesForGabor(4, 8, 0.5f, 4.8f, 1.3f));
            bank.Add(new SetValuesForGabor(4, 7, 0.5f, 4.6f, 1.57f));
            bank.Add(new SetValuesForGabor(4, 7.3f, 0.5f, 4.4f, 1.83f));
            bank.Add(new SetValuesForGabor(4, 6.2f, 0.5f, 3.7f, 2.09f));
            bank.Add(new SetValuesForGabor(4, 7, 0.5f, 4.3f, 2.35f));
            bank.Add(new SetValuesForGabor(4, 6.5f, 0.5f, 4.3f, 2.61f));
            bank.Add(new SetValuesForGabor(4, 8, 0.5f, 4.8f, 2.88f));
        }

        public void UpdateBank(List<SetValuesForGabor> l)
        {
            this.bank = l;
        }


        public GaborFilter GetGaborFilterFromBank(float angle)
        {
            GaborFilter gb = new GaborFilter();
            for (int i = 0; i < bank.Count - 1; i++)
            {
                if (angle > bank[i].theta && angle < bank[i + 1].theta)
                {
                    if (Math.Abs(angle - bank[i].theta) < Math.Abs(angle - bank[i + 1].theta))
                    {
                        bank[i].SetGaborFilter(gb);
                    }
                    else
                    {
                        bank[i + 1].SetGaborFilter(gb);
                    }
                    break;
                }
                else if (angle > bank[bank.Count - 1].theta)
                {
                    bank[bank.Count - 1].SetGaborFilter(gb);
                }
            }
            return gb;
        }


    }
}
