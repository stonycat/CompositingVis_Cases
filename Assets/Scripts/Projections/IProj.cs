using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Projections
{
    public interface IProj
    {
        /// <summary>
        /// return 3D point in unity3d
        /// </summary>
        /// <param name="geoP">x for longitude; y for latitude; z for altitude</param>
        /// <returns></returns>
        Vector3 Forward(Vector3 geoP);

        /// <summary>
        /// x for longitude; y for latitude; z for altitude
        /// </summary>
        /// <param name="coordinate">point in unity3D</param>
        /// <returns></returns>
        Vector3 Invert(Vector3 coordinate);
    }
}
