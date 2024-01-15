using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using CsvHelper;
using System.Dynamic;

namespace Assets.Scripts.Utilities
{
    static public class DebugFileWriter
    {
        public class TmpVertex
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }

        static List<TmpVertex> convert2dynamic(IEnumerable<Vector3> vertices)
        {
            var records = new List<TmpVertex>();

            foreach(var v in vertices)
            {
                var record = new TmpVertex
                {
                    X = v.x,
                    Y = v.y,
                    Z = v.z
                };
                records.Add(record);
            }
            return records;
        }

        static public void WriteVertices(string baseName, MapPolygon mapPolygon)
        {
            var outerPath = $"./Debug/{baseName}_outer.csv";
            using (var textWriter = new StreamWriter(outerPath))
            {
                var csv = new CsvWriter(textWriter, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
                csv.WriteRecords(convert2dynamic(mapPolygon.Outer));
            }

            if(mapPolygon.Holes != null && mapPolygon.Holes.Count != 0)
            {
                var index = 0;
                foreach(var hole in mapPolygon.Holes)
                {
                    var holePath = $"./Debug/{baseName}_hole_{index}.csv";
                    using (var textWriter = new StreamWriter(holePath))
                    {
                        var csv = new CsvWriter(textWriter, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
                        csv.WriteRecords(convert2dynamic(mapPolygon.Holes[index]));
                    }
                    index++;
                }
            }

        }
        static public void WriteVertices(string baseName, IEnumerable<Vector3> vertices)
        {
            var outerPath = $"./Debug/{baseName}_mesh.csv";
            using (var textWriter = new StreamWriter(outerPath))
            {
                var csv = new CsvWriter(textWriter, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
                csv.WriteRecords(convert2dynamic(vertices));
            }
        }
    }
}
