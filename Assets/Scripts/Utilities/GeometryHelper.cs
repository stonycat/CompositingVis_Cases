using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class Number
    {
        static public float epsilon = 0.00001f;
        static public float pi = Mathf.PI;
        static public float halfPi = Mathf.PI / 2;
        static public float quarterPi = Mathf.PI / 4;
        static public float tau = Mathf.PI * 2;
    }

    public static class GeometryHelper
    {
        public const int CIRCLR_COUNT = 36;
        static public float[] cosArray = new float[CIRCLR_COUNT];
        static public float[] sinArray = new float[CIRCLR_COUNT];

        static GeometryHelper()
        {
            for (var i = 0; i < CIRCLR_COUNT; i++)
            {
                var thisAngle = Number.pi * 2 / CIRCLR_COUNT * i;
                cosArray[i] = Mathf.Cos(thisAngle);
                sinArray[i] = Mathf.Sin(thisAngle);
            }
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        public static bool RayHitBox(Transform obj, Vector3 size, out Vector3 hitP)
        {
            var cameraP = Camera.main.transform.position;
            var cameraF = Camera.main.transform.forward;

            var tmpP = obj.InverseTransformPoint(cameraP);
            tmpP.z *= obj.localScale.z;
            var tmpDir = obj.InverseTransformDirection(cameraF);
            var tmpRay = new Ray(tmpP, tmpDir);

            var tmpBound = new Bounds(Vector3.zero, size);

            if(tmpBound.IntersectRay(tmpRay, out float tmpDis))
            {
                hitP = tmpP + tmpDis * tmpDir;
                return true;
            }
            else
            {
                hitP = new Vector3(1000, 1000, 1000);
                return false;
            }
        }

        public static void AddCurvePoints(ref List<Vector3> points, Vector3 p0, Vector3 p1, Vector3 p2, int count = 100)
        {
            var unit = 1.0f / count;

            for (var i = 0; i <= count; i++)
            {
                var t = i * unit;
                points.Add(GetPoint(p0, p1, p2, t));
            }
        }


        public static Vector3 GetSPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Slerp(Vector3.Slerp(p0, p1, t), Vector3.Slerp(p1, p2, t), t);
        }

        public static void AddCurveSPoints(ref List<Vector3> points, Vector3 p0, Vector3 p1, Vector3 p2, int count = 100)
        {
            var unit = 1.0f / count;

            for (var i = 0; i <= count; i++)
            {
                var t = i * unit;
                points.Add(GetSPoint(p0, p1, p2, t));
            }
        }

        public static Vector3 InterpolateGeoLocations(Vector3 p1, Vector3 p2, float t)
        {
            var phi1 = Degree2Radian(p1[1]);
            var lambad1 = Degree2Radian(p1[0]);

            var phi2 = Degree2Radian(p2[1]);
            var lambad2 = Degree2Radian(p2[0]);

            var sinPhi1 = Mathf.Sin(phi1);
            var cosPhi1 = Mathf.Cos(phi1);
            var sinLambad1 = Mathf.Sin(lambad1);
            var cosLambad1 = Mathf.Cos(lambad1);

            var sinPhi2 = Mathf.Sin(phi2);
            var cosPhi2 = Mathf.Cos(phi2);
            var sinLambad2 = Mathf.Sin(lambad2);
            var cosLambad2 = Mathf.Cos(lambad2);

            // distance between points
            var deltaPhi = phi2 - phi1;
            var deltaLambda = lambad2 - lambad1;

            return InterpolateGeoLocations(
                sinPhi1, cosPhi1, sinLambad1, cosLambad1,
                sinPhi2, cosPhi2, sinLambad2, cosLambad2,
                deltaPhi, deltaLambda, 
                t
            );
        }

        public static Vector3 InterpolateGeoLocations(float sinPhi1, float cosPhi1, float sinLambad1, float cosLambad1, 
                                                      float sinPhi2, float cosPhi2, float sinLambad2, float cosLambad2,
                                                      float deltaPhi, float deltaLambda,
                                                      float t)
        {
            var a = Mathf.Sin(deltaPhi / 2) * Mathf.Sin(deltaPhi / 2)
                         + cosPhi1 * cosPhi2 * Mathf.Sin(deltaLambda / 2) * Mathf.Sin(deltaLambda / 2);

            var theta = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

            var A = Mathf.Sin((1 - t) * theta) / Mathf.Sin(theta);
            var B = Mathf.Sin(t * theta) / Mathf.Sin(theta);

            var x = A * cosPhi1 * cosLambad1 + B * cosPhi2 * cosLambad2;
            var y = A * cosPhi1 * sinLambad1 + B * cosPhi2 * sinLambad2;
            var z = A * sinPhi1 + B * sinPhi2;

            var phi3 = Mathf.Atan2(z, Mathf.Sqrt(x * x + y * y));
            var lambad3 = Mathf.Atan2(y, x);

            return new Vector3((Radian2Degree(lambad3) + 540) % 360 - 180, Radian2Degree(phi3)); // normalise lon to −180..+180°
        }

        public static void AddCurvePoints(ref List<Vector3> geoPoints, Vector3 geoStart, Vector3 geoEnd, int count = 200)
        {
            var phi1 = Degree2Radian(geoStart[1]);
            var lambad1 = Degree2Radian(geoStart[0]);

            var phi2 = Degree2Radian(geoEnd[1]);
            var lambad2 = Degree2Radian(geoEnd[0]);

            var sinPhi1 = Mathf.Sin(phi1);
            var cosPhi1 = Mathf.Cos(phi1);
            var sinLambad1 = Mathf.Sin(lambad1);
            var cosLambad1 = Mathf.Cos(lambad1);

            var sinPhi2 = Mathf.Sin(phi2);
            var cosPhi2 = Mathf.Cos(phi2);
            var sinLambad2 = Mathf.Sin(lambad2);
            var cosLambad2 = Mathf.Cos(lambad2);

            // distance between points
            var deltaPhi = phi2 - phi1;
            var deltaLambda = lambad2 - lambad1;


            var unit = 1.0f / count;

            for (var i = 0; i <= count; i++)
            {
                var t = i * unit;
                var thisP = InterpolateGeoLocations(
                    sinPhi1, cosPhi1, sinLambad1, cosLambad1,
                    sinPhi2, cosPhi2, sinLambad2, cosLambad2,
                    deltaPhi, deltaLambda,
                    t
                );
                geoPoints.Add(thisP);
            }
        }


        public static float radians = Mathf.PI / 180.0f;
        public static float degerees = 180.0f / Mathf.PI;

        public static float Degree2Radian(float angle)
        {
            return angle * radians;
        }

        public static float Radian2Degree(float angle)
        {
            return angle * degerees;
        }

        public static Vector3 Degree2Radian(Vector3 p)
        {
            return p * radians;
        }

        public static Vector3 Radian2Degree(Vector3 p)
        {
            return p * degerees;
        }

        static public Vector3 Forward3DPoint(Vector3 lonlat, bool isInside = false)
        {
            var f = 0;

            var lon = lonlat[0];
            var lat = lonlat[1];
            var alt = lonlat[2];

            var ls = Mathf.Atan(Mathf.Pow((1 - f), 2) * Mathf.Tan(lat));

            var x = Mathf.Cos(ls) * Mathf.Cos(lon) + alt * Mathf.Cos(lat) * Mathf.Cos(lon);
            var y = Mathf.Cos(ls) * Mathf.Sin(lon) + alt * Mathf.Cos(lat) * Mathf.Sin(lon);
            var z = Mathf.Sin(ls) + alt * Mathf.Sin(lat);

            if (isInside)
            {
                x = -x;
            }

            return new Vector3(x, z, y);
        }

        static public Vector3 Invert3DPoint(Vector3 coor, bool isInside = false)
        {
            var x = coor.x;
            var y = coor.y;
            var z = coor.z;

            if (isInside)
            {
                x = -x;
            }
            var r = Mathf.Sqrt(x * x + y * y + z * z);
            //var alt = r - radius;
            var alt = 0;

            var lon = Mathf.Atan2(z, x);
            var lat = Mathf.Asin(y / r);
            return new Vector3(lon, lat, alt);
        }

        public static int FindClosest(Vector3 input, List<Vector3> set)
        {
            var index = 0;
            var minDis = Vector3.Distance(input, set[index]);
            for (var i = 1; i < set.Count; i++)
            {
                var thisDis = Vector3.Distance(input, set[i]);
                if (thisDis < minDis)
                {
                    minDis = thisDis;
                    index = i;
                }
            }
            return index;
        }

        public static Bounds GetBounds(List<Vector3> points)
        {
            if (points.Count == 0)
                return new Bounds();
            if (points.Count == 1)
                return new Bounds(points[0], Vector3.zero);

            var min = Vector3.Min(points[0], points[1]);
            var max = Vector3.Max(points[0], points[1]);

            for(var i = 2; i < points.Count; i++)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            return new Bounds((max + min) / 2, (max - min));
        }

        public static float NormalizeLon(float lon)
        {
            return (lon + 540.0f) % 360.0f - 180.0f;
        }

        public static Vector3 GetGeoMidPoint(Vector3 v1, Vector3 v2)
        {
            var lambda1 = Degree2Radian(v1[0]);
            var phi1 = Degree2Radian(v1[1]);

            var lambda2 = Degree2Radian(v2[0]);
            var phi2 = Degree2Radian(v2[1]);

            var lonDiff = lambda2 - lambda1;
            var bx = Mathf.Cos(phi2) * Mathf.Cos(lonDiff);
            var by = Mathf.Cos(phi2) * Mathf.Sin(lonDiff);

            var temp = Mathf.Cos(phi1) + bx;
            var phi3 = Mathf.Atan2(
                Mathf.Sin(phi1) + Mathf.Sin(phi2),
                Mathf.Sqrt(temp * temp + by * by)
            );
            var lambda3 = lambda1 + Mathf.Atan2(by, temp);
            return new Vector3(NormalizeLon(Radian2Degree(lambda3)), Radian2Degree(phi3));
        }

        public static float GeoDistance(Vector3 p1, Vector3 p2)
        {
            var rlat1 = Degree2Radian(p1[0]);
            var rlat2 = Degree2Radian(p2[0]);
            
            var rtheta  = Degree2Radian(p2[1] - p1[1]);
            
            var dist =
                Mathf.Sin(rlat1) * Mathf.Sin(rlat2) + Mathf.Cos(rlat1)*
                Mathf.Cos(rlat2) * Mathf.Cos(rtheta);
            dist = Mathf.Acos(dist);
            dist = dist * 180 / Mathf.PI;
            dist = dist * 60 * 1.1515f;
            return dist;
        }
    }
}
