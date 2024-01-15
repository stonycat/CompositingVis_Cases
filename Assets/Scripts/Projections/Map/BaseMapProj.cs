using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Projections.Map
{
    public abstract class BaseMapProj : IProj
    {
        private Rotation rotation;
        private float radius;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoRotation"></param>
        /// <param name="radius">Half of map height</param>
        public BaseMapProj(Vector3 geoRotation)
        {
            this.rotation = new Rotation(
               GeometryHelper.Degree2Radian(geoRotation.x),
               GeometryHelper.Degree2Radian(-geoRotation.y),
               GeometryHelper.Degree2Radian(geoRotation.z)
            );
        }

        abstract protected Vector3 ForwardImpl(Vector3 geoP);
        abstract protected Vector3 InvertImpl(Vector3 geoP);

        public Vector3 Forward(Vector3 geoP)
        {
            geoP.x = -geoP.x;
            var rotated = this.rotation.Forward(geoP);
            return this.ForwardImpl(rotated);
        }
        public Vector3 Invert(Vector3 coordinate)
        {
            var p1 = this.InvertImpl(coordinate);
            return this.rotation.Invert(p1);
        }
    }
}
