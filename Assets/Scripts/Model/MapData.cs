using CsvHelper;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class MapPolygon
    {
        public List<Vector3> Outer;
        public List<List<Vector3>> Holes;
    }

    /// <summary>
    /// This class is only used for Json De-Serilisation
    /// </summary>
    public class DummyCentroid
    {
        public float lat;
        public float lng;
    }


    public class MapData
    {
        public Dictionary<string, List<MapPolygon>> Name2Polygons;
        public Dictionary<string, Vector3> Name2GeoPoint;
        public Dictionary<string, string> Name2Abbr;

        public MapData()
        {

        }


        public void LoadGeoJson(string geoJsonFilePath)
        {
            this.Name2Polygons = new Dictionary<string, List<MapPolygon>>();

            var mapFeatures = JsonConvert.DeserializeObject<FeatureCollection>(
                File.ReadAllText(geoJsonFilePath)
            );

            var index = 0;
            foreach (var f in mapFeatures.Features)
            {
                if (f.Geometry == null) continue;
                object thisName = index.ToString();
                index++;

                var hasV = f.Properties.TryGetValue("name", out thisName);
                if (!hasV)
                    f.Properties.TryGetValue("NAME", out thisName);

                //if((string)thisName == "New South Wales")
                //    Debug.Log("shit");

                List<MapPolygon> thisMapPolygon = new List<MapPolygon>();
                var thisGeoType = f.Geometry.GetType();
                if (thisGeoType == typeof(MultiPolygon))
                {
                    var thisG = (MultiPolygon)f.Geometry;
                    foreach (var g in thisG.Coordinates)
                    {
                        thisMapPolygon.Add(this.ProcessPolygon(g));
                    }
                }
                else if (thisGeoType == typeof(Polygon))
                {
                    thisMapPolygon.Add(
                        this.ProcessPolygon(
                            (Polygon)f.Geometry
                        )
                    );
                }
                else
                {
                    Debug.LogWarning($"Not supported GeoJson Feature: {thisGeoType}.");
                    continue;
                }

                this.Name2Polygons.Add((string)thisName, thisMapPolygon);
            }
        }

        public void LoadCentroid(string centroidFilePath)
        {
            this.Name2GeoPoint = new Dictionary<string, Vector3>();

            using (TextReader textReader = new StreamReader(centroidFilePath))
            {
                var header = textReader.ReadLine();
                var csv = new CsvReader(textReader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
                while (csv.Read())
                {
                    var thisName = csv.GetField<string>(0).Replace("\"", "");
                    var lat = csv.GetField<float>(1);
                    var lon = csv.GetField<float>(2);
                    this.Name2GeoPoint.Add(thisName, new Vector3(lon, lat));
                }
            }
            foreach (string s in  this.Name2GeoPoint.Keys)
            {
                Debug.Log(s + ' ' + this.Name2GeoPoint[s]);
            }
        }

        public void LoadAbbr(string abbrFilePath)
        {
            this.Name2Abbr = new Dictionary<string, string>();
            if (!File.Exists(abbrFilePath))
            {
                Debug.LogWarning($"{abbrFilePath} does not exist.");
            }
            using (TextReader textReader = new StreamReader(abbrFilePath))
            {
                var header = textReader.ReadLine();
                var csv = new CsvReader(textReader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
                while (csv.Read())
                {
                    var thisName = csv.GetField<string>(0).Replace("\"", "");
                    var thisAbbr = csv.GetField<string>(1).Replace("\"", "");
                    this.Name2Abbr.Add(thisName, thisAbbr);
                }
            }
        }

        private MapPolygon ProcessPolygon(Polygon p)
        {
          
            var result = new MapPolygon();
            var index = 0;
            foreach(var ele in p.Coordinates)
            {
                var thisLine = new List<Vector3>();
                foreach (var c in ele.Coordinates)
                {
                    //Debug.Log(c.Longitude);
                    //Debug.Log(c.Latitude);
                    //Debug.Log(c.Altitude);
                    //Debug.Log("============================");

                    thisLine.Add(
                        new Vector3(
                            (float)c.Longitude,
                            (float)c.Latitude,
                            (float)(c.Altitude.HasValue ? c.Altitude : 0)
                        )
                    );
                }

                if (index == 0)
                {
                    result.Outer = thisLine;
                }
                else
                {
                    if (result.Holes == null)
                        result.Holes = new List<List<Vector3>>();
                    result.Holes.Add(thisLine);
                }

                index++;
            }
            return result;
        }
    }
}

