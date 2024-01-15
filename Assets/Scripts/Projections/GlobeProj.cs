using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.Projections
{
    public class GlobeProj : IProj
    {
        private bool isIniside;
        private float radius;
        public GlobeProj(float radius, bool isIniside=false)
        {
            this.radius = radius;
            this.isIniside = isIniside;
        }

        public Vector3 Forward(Vector3 geoP)
        {
            return GeometryHelper.Forward3DPoint(GeometryHelper.Degree2Radian(geoP), this.isIniside) * this.radius;
        }

        public Vector3 Invert(Vector3 coordinate)
        {
            coordinate /= this.radius;
            return GeometryHelper.Radian2Degree(GeometryHelper.Invert3DPoint(coordinate, this.isIniside));
        }
    }
}
