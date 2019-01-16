using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BiometriaOdciskuPalca
{
    class MinutiasDatabase
    {
        List<DatabaseElement> mBase { get; set; }
        string path { get; set; }

        public MinutiasDatabase(string path)
        {
            this.path = path;
            this.mBase = new List<DatabaseElement>();
        }
        public void Load()
        {

            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                mBase = (List<DatabaseElement>)serializer.Deserialize(file, typeof(List<DatabaseElement>));
            }
        }


        public void Save()
        {
            using (StreamWriter file = File.CreateText(path))
            {

                var json = JsonConvert.SerializeObject(mBase, Formatting.Indented);
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, mBase);
            }
        }






        public List<Tuple<DatabaseElement, int,ModyficationElement>>  CheckWithDatabase(MinutiaWektor potential)
        {
            List<Tuple<DatabaseElement, int,ModyficationElement>> result;
            //potential = mBase[0].MinutiaesWektor;
            result = CheckList(potential);
            return result;
        }


        public void Add(MinutiaWektor wektor, string imageName)
        {
            this.mBase.Add(new DatabaseElement(imageName, wektor));
            Save();
        }


        public List<Tuple<DatabaseElement,int,ModyficationElement>> CheckList(MinutiaWektor wektor)
        {

            List<Tuple<DatabaseElement, int,ModyficationElement>> result = new List<Tuple<DatabaseElement, int,ModyficationElement>>();
            MinutiaWektorComperer comperer = new MinutiaWektorComperer(12,10,ImageSupporter.DegreeToRadian(10));

            foreach(var item in mBase)
            {
                Tuple<bool, int,ModyficationElement> compereResult = comperer.Compere(item.MinutiaesWektor, wektor);

                if (compereResult.Item1)
                {
                    result.Add(new Tuple<DatabaseElement, int,ModyficationElement>(item, compereResult.Item2,compereResult.Item3));
                }

            }



            return result;
        }

        public void Clear()
        {
            mBase.Clear();
            Save();
        }
    }
}
