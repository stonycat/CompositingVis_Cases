using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Projections.Map
{
    class NaturalEarth : BaseMapProj
    {
        public NaturalEarth(Vector3 geoRotation) : base(geoRotation)
        {
        }

        protected override Vector3 ForwardImpl(Vector3 geoP)
        {
            float lon = GeometryHelper.Degree2Radian(geoP[0]);
            float lat = GeometryHelper.Degree2Radian(geoP[1]);

            float lat2 = lat * lat;
            float lat4 = lat2 * lat2;

            float x = lon * (0.8707f - 0.131979f * lat2 + lat4 * (-0.013791f + lat4 * (0.003971f * lat2 - 0.001529f * lat4)));
            float y = lat * (1.007226f + lat2 * (0.015085f + lat4 * (-0.044475f + 0.028874f * lat2 - 0.005916f * lat4)));

            var result = new Vector3(x, y);
            return result;
        }

        const float MAX_Y = 0.8707f * 0.52f * Mathf.PI;
        const int MAX_ITERATIONS = 20;

        protected override Vector3 InvertImpl(Vector3 coordinate)
        {
            var xy = coordinate;
            float yc, tol, y2, y4, f, fder, lon;
            if (xy.y > MAX_Y || xy.y < -MAX_Y)
            {
                throw new Exception("Natural Earth invert, y out of range");
            }

            // Newton's method for the latitude
            yc = xy.y;
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                y2 = yc * yc;
                y4 = y2 * y2;
                f = (yc * (1.007226f + y2 * (0.015085f + y4 * (-0.044475f + 0.028874f * y2 - 0.005916f * y4)))) - xy.y;
                fder = 1.007226f + y2 * (0.015085f * 3.0f + y4 * (-0.044475f * 7.0f + 0.028874f * 9.0f * y2 - 0.005916f * 11.0f * y4));
                yc -= tol = f / fder;
                if (Mathf.Abs(tol) < Number.epsilon)
                {
                    break;
                }
            }

            // longitude
            y2 = yc * yc;
            lon = xy.x / (0.8707f + y2 * (-0.131979f + y2 * (-0.013791f + y2 * y2 * y2 * (0.003971f - 0.001529f * y2))));
            if (lon > Number.pi || lon < -Number.pi)
            {
                throw new Exception("Natural Earth invert, longitude out of range");
            }
            var result = new Vector3(lon, yc);
            return GeometryHelper.Radian2Degree(result);
        }
    }
}
