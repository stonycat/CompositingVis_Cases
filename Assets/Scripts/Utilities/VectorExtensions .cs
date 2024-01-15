using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class VectorExtensions
    {

        /// <summary>
        /// Returns the signed angle between this 2D vector and another.
        /// (This is unlike Vector2.Angle, which always returns the
        /// absolute value of the angle.)
        /// </summary>
        /// <returns>The signed angle, in degrees, from A to B.</returns>
        /// <param name="a">Vector this was called on.</param>
        /// <param name="b">Vector to measure the angle to.</param>
        public static float SignedAngleTo(this Vector2 a, Vector2 b)
        {
            return Mathf.Atan2(a.x * b.y - a.y * b.x, a.x * b.x + a.y * b.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns the signed angle between this vector and the +X axis.
        /// </summary>
        /// <returns>The signed angle, reprenting the direction of this in degrees.</returns>
        /// <param name="a">Vector this was called on.</param>
        public static float SignedAngle(this Vector2 a)
        {
            return Mathf.Atan2(a.y, a.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns the signed angle between this 3D vector and another,
        /// with respect to some orthogonal "up" vector.  If looking in
        /// the "up" direction, then + angles are counter-clockwise.
        /// </summary>
        /// <returns>The signed angle, in degrees, from A to B.</returns>
        /// <param name="a">Vector this was called on.</param>
        /// <param name="b">Vector to measure the angle to.</param>
        public static float SignedAngleTo(this Vector3 a, Vector3 b, Vector3 up)
        {
            return Mathf.Atan2(
                Vector3.Dot(up.normalized, Vector3.Cross(a, b)),
                Vector3.Dot(a, b)) * Mathf.Rad2Deg;
        }
    }
}
