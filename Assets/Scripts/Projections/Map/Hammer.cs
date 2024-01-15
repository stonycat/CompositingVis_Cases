using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.Projections.Map
{
    public class Hammer : BaseMapProj
    {
        public Hammer(Vector3 geoRotation) : base(geoRotation)
        {
        }

        protected override Vector3 ForwardImpl(Vector3 geoP)
        {
            var lambda = GeometryHelper.Degree2Radian(geoP.x);
            var phi = GeometryHelper.Degree2Radian(geoP.y);

            var baseValue = Mathf.Sqrt(1 + Mathf.Cos(phi) * Mathf.Cos(lambda / 2));
            var x = (2 * Mathf.Cos(phi) * Mathf.Sin(lambda / 2)) / baseValue;
            var y = Mathf.Sin(phi) / baseValue;
            return new Vector3(x, y);
        }

        protected override Vector3 InvertImpl(Vector3 coordinate)
        {
            var input = coordinate;
            var tempGeo = input * Mathf.Sqrt(2);

            var x = tempGeo.x;
            var y = tempGeo.y;
            var z = Mathf.Sqrt(1 - (x / 4) * (x / 4) - (y / 2) * (y / 2));

            return new Vector3(
                GeometryHelper.Radian2Degree(2 * Mathf.Atan2(z * x, 2 * (2 * z * z - 1))),
                GeometryHelper.Radian2Degree(Mathf.Asin(z * y))
            );
        }
    }
}
