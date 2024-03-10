using Assets.Scripts.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class FitMapProj : IProj
    {
        private float _scale = 1;
        private Vector3 _offset;

        public Vector2 Size;
        public IProj proj;
        public MapData rawMapData;
        public MapData projMapData;

        public FitMapProj(IProj proj, MapData rawMapData)
        {
            this.proj = proj;
            this.rawMapData = rawMapData;
        }

        
        public void FitExtent(Vector2 size)
        {
            this.Size = size;

            this.projMapData = new MapData();
            this.projMapData.Name2Polygons = new Dictionary<string, List<MapPolygon>>();

            var maxP = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var minP = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (var item in this.rawMapData.Name2Polygons)
            {
                var key = item.Key;
                var value = item.Value;

                var newPolygons = new List<MapPolygon>();

                foreach (var p in value)
                {
                    var thisP = new MapPolygon
                    {
                        Outer = new List<Vector3>()
                    };

                    foreach (var outerP in p.Outer)
                    {
                        var projP = this.proj.Forward(outerP);
                        thisP.Outer.Add(projP);

                        maxP = Vector3.Max(maxP, projP);
                        minP = Vector3.Min(minP, projP);
                    }

                    if (p.Holes != null)
                    {
                        thisP.Holes = new List<List<Vector3>>();
                        foreach (var hole in p.Holes)
                        {
                            var thisHole = new List<Vector3>();
                            foreach (var holeP in hole)
                            {
                                var projP = this.proj.Forward(holeP);
                                thisHole.Add(projP);

                                maxP = Vector3.Max(maxP, projP);
                                minP = Vector3.Min(minP, projP);
                            }
                            thisP.Holes.Add(thisHole);
                        }
                    }

                    newPolygons.Add(thisP);
                }
                this.projMapData.Name2Polygons.Add(key, newPolygons);
            }

            var rawSize = new Vector2(maxP.x - minP.x, maxP.y - minP.y);
            this._scale = Mathf.Min(size.x / rawSize.x, size.y / rawSize.y);
            this._offset = (maxP + minP) / 2;

            foreach (var item in this.projMapData.Name2Polygons)
            {
                var value = item.Value;
                foreach (var p in value)
                {
                    for(var i = 0; i < p.Outer.Count; i++)
                    {
                        p.Outer[i] -= this._offset;
                        p.Outer[i] *= this._scale;
                    }

                    if (p.Holes != null)
                    {
                        foreach (var hole in p.Holes)
                        {
                            for (var i = 0; i < hole.Count; i++)
                            {
                                hole[i] -= this._offset;
                                hole[i] *= this._scale;
                            }

                        }
                    }
                }
            }

            if(this.rawMapData.Name2GeoPoint != null)
            {
                this.projMapData.Name2GeoPoint = new Dictionary<string, Vector3>();
                foreach (var key in this.rawMapData.Name2GeoPoint.Keys)
                {
                    var p = this.rawMapData.Name2GeoPoint[key];
                    var newP = this.Forward(p);

                    this.projMapData.Name2GeoPoint.Add(key, newP);
                }
                foreach (var s in projMapData.Name2GeoPoint.Keys)
                {
                    Debug.Log(s + " " + projMapData.Name2GeoPoint[s]);
                }
            }
        }

        public Vector3 Forward(Vector3 geoP)
        {
            if (this.projMapData == null)
            {
                Debug.LogWarning("Map has not been fitted into extent before use!");
                return geoP;
            }

            var newP = this.proj.Forward(geoP);
            newP -= this._offset;
            newP *= this._scale;

            return newP;
        }

        public Vector3 Invert(Vector3 coordinate)
        {
            if (this.projMapData == null)
            {
                Debug.LogWarning("Map has not been fitted into extent before use!");
                return coordinate;
            }

            var newP = coordinate / this._scale;
            newP += this._offset;

            return this.proj.Invert(newP);
        }
    }
}
