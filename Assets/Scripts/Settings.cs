 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Settings
    {
        public static string DataDir = "./Data/";
        public static Dictionary<string, Vector3> Location2geoCenter;
        public static Vector2 MapSize = new Vector2(1, 1);
        
        static Settings()
        {
            Location2geoCenter = new Dictionary<string, Vector3>();
            Location2geoCenter.Add("US", new Vector3(-96f, 39f));
            Location2geoCenter.Add("AU", new Vector3(132.555f, -25.666f));
            Location2geoCenter.Add("UK", new Vector3(-1f, 52.5f));
            Location2geoCenter.Add("EU", new Vector3(11.33f, 20.05f));
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
