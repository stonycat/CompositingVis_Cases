using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Utilities.Scales
{
    public class Scale : INumbericScale
    {
        static public float Linear(float input)
        {
            return input;
        }

        static public float Divide100(float input)
        {
            return input / 100;
        }

        static public float Log(float input)
        {
            return Mathf.Log(input + 1);
        }

        static public float Sqrt(float input)
        {
            return Mathf.Sqrt(input);
        }

        static public float Pow(float input)
        {
            return Mathf.Pow(input, 1.0f / 3.0f);
        }

        static public float Power2(float input)
        {
            return input * input;
        }

        public float DomainMin { get; private set; }
        public float DomainMax { get; private set; }
        public float RangeMin { get; private set; }
        public float RangeMax { get; private set; }

        public Func<float, float> TransferFunc { get; private set; }

        public float TransDomainMin { get; private set; }
        public float TransDomainMax { get; private set; }

        private float unit;

        public Scale(
            float domainMin, float domainMax, 
            float rangeMin, float rangeMax, 
            Func<float, float> transfer = null
        )
        {
            this.DomainMin = domainMin;
            this.DomainMax = domainMax;

            this.RangeMin = rangeMin;
            this.RangeMax = rangeMax;

            this.TransferFunc = transfer;
            if (this.TransferFunc == null)
            {
                this.TransferFunc = Linear;
            }

            this.TransDomainMin = this.TransferFunc(this.DomainMin);
            this.TransDomainMax = this.TransferFunc(this.DomainMax);

            this.unit = (this.RangeMax - this.RangeMin) / (this.TransDomainMax - this.TransDomainMin);
        }

        public float Project(float input)
        {
            var transInput = this.TransferFunc(input);
            return this.RangeMin + this.unit * (transInput - this.TransDomainMin);
        }
    }
}
