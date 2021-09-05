using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNGExecutable
{
    /// <summary>
    /// Маппер
    /// </summary>
    public class Mapper
    {
        public Mapper(int iD, long startPoint, long stopPoint, long size, string file)
        {
            ID = iD;
            StartPoint = startPoint;
            StopPoint = stopPoint;
            Size = size;
            File = file;
        }
        public Mapper(string CSVData)
        {
            ID = int.Parse(CSVData.Split(';')[0]);
            StartPoint = long.Parse(CSVData.Split(';')[1]);
            StopPoint = long.Parse(CSVData.Split(';')[2]);
            Size = long.Parse(CSVData.Split(';')[3]);
            File = CSVData.Split(';')[4];
        }

        public int ID
        {
            get;
            set;
        }
        public long StartPoint
        {
            get;
            set;
        }
        public long StopPoint
        {
            get;
            set;
        }
        public long Size
        {
            get;
            set;
        }
        public string File
        {
            get;
            set;
        }
    }

    public static class MapperController
    {
        public static void Save(this Mapper[] Mappers, string Path)
        {
            string tmp = "";
            for (int i = 0; i < Mappers.Length; i++)
            {
                tmp += Mappers[i].ID + ";" + Mappers[i].StartPoint + ";" + Mappers[i].StopPoint + ";" + Mappers[i].Size + ";" + Mappers[i].File + "\n";
            }
            File.WriteAllText(Path, tmp);
        }
        public static Mapper[] Load(this string Path)
        {
            string tmp = File.ReadAllText(Path);
            var tt = new List<Mapper>();
            var lines = tmp.Split('\n');
            foreach (var item in lines)
                tt.Add(new Mapper(item));
            return tt.ToArray();
        }
    }
}
