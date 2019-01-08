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






        public Boolean check(MinutiaWektor potentialy)
        {

            return false;
        }






        public void Add(MinutiaWektor wektor, string imageName)
        {
            this.mBase.Add(new DatabaseElement(imageName, wektor));
            Save();
        }


        public List<Tuple<DatabaseElement,float>> CheckList(MinutiaWektor wektor)
        {

            List<Tuple<DatabaseElement, float>> result = new List<Tuple<DatabaseElement, float>>();
            MinutiaWektorComperer comperer = new MinutiaWektorComperer(12);

            foreach(var item in mBase)
            {
                Tuple<bool, float> compereResult = comperer.Compere(item.MinutiaesWektor, wektor);

                if (compereResult.Item1)
                {
                    result.Add(new Tuple<DatabaseElement, float>(item, compereResult.Item2));
                }

            }



            return result;
        }
    }
}
