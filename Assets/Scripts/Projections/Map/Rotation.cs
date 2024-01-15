using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Projections.Map
{
    public class Rotation
    {
        private float deltaLambda; // longitude
        private float deltaPhi; // latitude
        private float deltaGamma; // latitude

        private float cosDeltaPhi;
        private float sinDeltaPhi;

        private float cosDeltaGamma;
        private float sinDeltaGamma;

        private Vector3 rotationIdentity(Vector3 geoP)
        {
            var lambda = geoP.x;
            var phi = geoP.y;

            return new Vector3(lambda > Number.pi ? lambda - Number.tau : lambda < -Number.pi ? lambda + Number.tau : lambda, phi);
        }


        private Vector3 rotationIdentityInvert(Vector3 geoP)
        {
            return rotationIdentity(geoP); 
        }

        public Vector3 Forward(Vector3 coor)
        {
            //coor = coor / 180 * Number.pi;
            //return this.forward(coor) * 180 / Number.pi;
            return GeometryHelper.Radian2Degree(this.forward(GeometryHelper.Degree2Radian(coor)));
        }

        public Vector3 Invert(Vector3 coor)
        {
            //coor = coor / 180 * Number.pi;
            //return this.invert(coor) * 180 / Number.pi;
            return GeometryHelper.Radian2Degree(this.invert(GeometryHelper.Degree2Radian((coor))));
        }

        private Func<Vector3, Vector3> forward;
        private Func<Vector3, Vector3> invert;

        private Vector3 composeForward(Vector3 geoP)
        {
            var x = this.rotationLambda(geoP);
            return this.rotationPhiGamma(x);
        }

        private Vector3 composeInvert(Vector3 geoP)
        {
            var x = this.rotationPhiGammaInvert(geoP);
            var b = this.rotationLambdaInvert(x);
            return b;
        }

        public Rotation(float deltaLambda = 0, float deltaPhi = 0, float deltaGamma = 0)
        {
            if ((deltaLambda %= Number.tau) == 0)
            {
                if (deltaPhi != 0 || deltaGamma != 0)
                {
                    this.forward = this.rotationPhiGamma;
                    this.invert = this.rotationPhiGammaInvert;
                }
                else
                {
                    this.forward = this.rotationIdentity;
                    this.invert = this.rotationIdentityInvert;
                }
            }
            else
            {
                if (deltaPhi != 0 || deltaGamma != 0)
                {
                    this.forward = this.composeForward;
                    this.invert = this.composeInvert;
                }
                else
                {
                    this.forward = this.rotationLambda;
                    this.invert = this.rotationLambdaInvert;
                }
            }



            this.deltaLambda = deltaLambda;
            this.deltaPhi = deltaPhi;
            this.deltaGamma = deltaGamma;

            this.cosDeltaPhi = Mathf.Cos(this.deltaPhi);
            this.sinDeltaPhi = Mathf.Sin(this.deltaPhi);

            this.cosDeltaGamma = Mathf.Cos(this.deltaGamma);
            this.sinDeltaGamma = Mathf.Sin(this.deltaGamma);
        }

        private Vector3 rotationLambda(Vector3 geo)
        {
            var lambda = geo.x;
            lambda += this.deltaLambda;
            if (lambda > Mathf.PI)
            {
                return new Vector3(lambda - Number.tau, geo.y);
            }
            else if (lambda < -Mathf.PI)
            {
                return new Vector3(lambda + Number.tau, geo.y);
            }
            return new Vector3(lambda, geo.y);
        }

        private Vector3 rotationLambdaInvert(Vector3 geo)
        {
            this.deltaLambda = -this.deltaLambda;
            var result = this.rotationLambda(geo);
            this.deltaLambda = -this.deltaLambda;
            return result;
        }

        public Vector3 rotationPhiGamma(Vector3 geo)
        {
            var lambda = geo.x;
            var phi = geo.y;

            var cosPhi = Mathf.Cos(phi);
            var x = Mathf.Cos(lambda) * cosPhi;
            var y = Mathf.Sin(lambda) * cosPhi;
            var z = Mathf.Sin(phi);
            var k = z * this.cosDeltaPhi + x * this.sinDeltaPhi;

            return new Vector3(
                Mathf.Atan2(y * this.cosDeltaGamma - k * this.sinDeltaGamma, x * this.cosDeltaPhi - z * this.sinDeltaPhi),
                Mathf.Asin(k * this.cosDeltaGamma + y * this.sinDeltaGamma)
            );
        }

        public Vector3 rotationPhiGammaInvert(Vector3 geo)
        {
            var lambda = geo.x;
            var phi = geo.y;

            var cosPhi = Mathf.Cos(phi);
            var x = Mathf.Cos(lambda) * cosPhi;
            var y = Mathf.Sin(lambda) * cosPhi;
            var z = Mathf.Sin(phi);
            var k = z * this.cosDeltaGamma - y * this.sinDeltaGamma;

            return new Vector3(
                Mathf.Atan2(y * this.cosDeltaGamma + z * this.sinDeltaGamma, x * this.cosDeltaPhi + k * this.sinDeltaPhi),
                Mathf.Asin(k * this.cosDeltaPhi - x * this.sinDeltaPhi)
            );
        }
    }
}
