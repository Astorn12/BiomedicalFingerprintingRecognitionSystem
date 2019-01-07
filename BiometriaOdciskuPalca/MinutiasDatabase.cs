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
        List<Tuple<string , MinutiaWektor>> mBase;
        string path;
        public MinutiasDatabase(string path)
        {
            this.path = path;
        }
        public void Load()
        {
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                mBase = (List<Tuple<string , MinutiaWektor>>)serializer.Deserialize(file, typeof(List<Tuple<string , MinutiaWektor>>));
            }
        }

        public void Save()
        {
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, mBase);
            }
        }






        public Boolean check(MinutiaWektor potentialy)
        {
            
            return false;
        }






        public void Add(MinutiaWektor wektor,string imageName)
        {
            this.mBase.Add(new Tuple<string, MinutiaWektor>(imageName,wektor));
            Save();
        }
    }
}
