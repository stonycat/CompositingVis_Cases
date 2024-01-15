using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Projections.Map
{
    public class Robinson : BaseMapProj
    {
        public Robinson(Vector3 geoRotation) : base(geoRotation)
        {
        }

        protected override Vector3 ForwardImpl(Vector3 geoP)
        {
            var lambda = GeometryHelper.Degree2Radian(geoP.x);
            var phi = GeometryHelper.Degree2Radian(geoP.y);

            Vector3 lat2 = new Vector3(phi * phi, 0);
            Vector3 xy = Vector3.Scale(lat2, new Vector3(0.0104f, 0.0129f));
            xy += new Vector3(0.1450f, 0.0013f);
            xy = Vector3.Scale(xy, -lat2);
            xy += new Vector3(0.8507f, 0.9642f);
            xy = Vector3.Scale(xy, new Vector3(lambda, phi));
            return xy;
        }

        const float MAX_Y = 1.38615911291f;
        const int MAX_ITERATIONS = 20;

        protected override Vector3 InvertImpl(Vector3 coordinate)
        {
            var xy = coordinate;
            float yc, tol, y2, y4, f, fder, lon;

            if (xy.y > MAX_Y || xy.y < -MAX_Y)
            {
                throw new Exception("Robinson invert, y out of range");
            }

            yc = xy.y;
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                y2 = yc * yc;
                f = (yc * (0.9642f - y2 * (0.0013f + y2 * 0.0129f))) - xy.y;
                fder = 0.9642f - y2 * (0.0013f * 3.0f + y2 * 0.0129f * 5.0f);
                yc -= tol = f / fder;
                if (Mathf.Abs(tol) < Number.epsilon)
                {
                    break;
                }
            }

            y2 = yc * yc;
            lon = xy.x / (0.8507f - y2 * (0.1450f + y2 * 0.0104f));
            if (lon > Number.pi || lon < -Number.pi)
            {
                throw new Exception("Robinson invert, longitude out of range");
            }
            var result = new Vector3(lon, yc);
            return GeometryHelper.Radian2Degree(result);
        }
    }
}
