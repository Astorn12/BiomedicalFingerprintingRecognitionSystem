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
        public List<DatabaseElement> mBase { get; set; }
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






        public List<Tuple<DatabaseElement, int,ModyficationElement,int>>  CheckWithDatabase(MinutiaWektor potential)
        {
            List<Tuple<DatabaseElement, int,ModyficationElement,int>> result;
            //potential = mBase[0].MinutiaesWektor;
            result = CheckList(potential);
            return result;
        }


        public void Add(MinutiaWektor wektor, string imageName)
        {
            this.mBase.Add(new DatabaseElement(imageName, wektor));
            Save();
        }


        public List<Tuple<DatabaseElement,int,ModyficationElement,int>> CheckList(MinutiaWektor wektor)
        {

            List<Tuple<DatabaseElement, int,ModyficationElement,int>> result = new List<Tuple<DatabaseElement, int,ModyficationElement,int>>();
            MinutiaWektorComperer comperer = new MinutiaWektorComperer(10,10,ImageSupporter.DegreeToRadian(15));

            foreach(var item in mBase)
            {
                Tuple<bool, int,ModyficationElement,int> compereResult = comperer.Compere(item.MinutiaesWektor, wektor);

                if (compereResult.Item1)
                {
                    result.Add(new Tuple<DatabaseElement, int,ModyficationElement,int>(item, compereResult.Item2,compereResult.Item3,compereResult.Item4));
                }

            }



            return result;
        }

        public void Clear()
        {
            mBase.Clear();
            Save();
            string[] filePaths = Directory.GetFiles(path.Replace("databse.json",""));
            foreach (string filePath in filePaths)
            {
                if (filePath.Contains(".png"))
                    File.Delete(filePath);
            }
        }

        
    }
}
