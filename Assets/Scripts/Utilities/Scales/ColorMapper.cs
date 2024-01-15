using UnityEngine;

using System;
using System.Collections.Generic;
using Assets.Scripts.Utilities.Scales;

namespace Assets.Scripts.Utilities.Scales
{
    public class ColorMapper : IColorScale
    {
        private float inMin;
        private float inMax;
        private float interval;
        private int maxLength;
        private Func<float, float> transf;

        private List<Dictionary<string, Scale>> scalers;
        public ColorMapper(float min, float max, Color32[] colors, Func<float, float> transf = null)
        {
            if(transf == null)
            {
                transf = Scale.Linear;
            }

            this.transf = transf;

            this.inMin = transf(min);
            this.inMax = transf(max);

            if (float.IsInfinity(this.inMin))
            {
                this.inMin = 0;
            }

            var colorLength = colors.Length;
            this.maxLength = colorLength - 1;

            var interval = (this.inMax - this.inMin) / this.maxLength;
            this.interval = interval;

            this.scalers = new List<Dictionary<string, Scale>>();

            for(var i = 0; i < this.maxLength; i++)
            {
                var startIn = this.inMin + i * interval;
                var endIn = this.inMin + (i + 1) * interval;

                var startC = colors[i];
                var endC = colors[i + 1];

                var thisScalers = new Dictionary<string, Scale>();

                var RScaler = new Scale(startIn, endIn, startC.r, endC.r, transf);
                var GScaler = new Scale(startIn, endIn, startC.g, endC.g, transf);
                var BScaler = new Scale(startIn, endIn, startC.b, endC.b, transf);

                thisScalers.Add("r", RScaler);
                thisScalers.Add("g", GScaler);
                thisScalers.Add("b", BScaler);

                this.scalers.Add(thisScalers);
            }
        }

        public Color32 Project(float input)
        {
            //Debug.Log($"==========================");
            //Debug.Log($"{input}");
            input = this.transf(input);
            if (float.IsInfinity(input))
            {
                input = 0;
            }

            int index = (int)((input - this.inMin) / this.interval);
            if(index < 0 || index > (this.maxLength-1))
            {
                if(Mathf.Abs(input - this.inMax) < 0.0001)
                {
                    index -= 1;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            var scaler = this.scalers[index];
            var r = scaler["r"].Project(input);
            var g = scaler["g"].Project(input);
            var b = scaler["b"].Project(input);

            //Debug.Log($"{input}: {r}, {g}, {b}");

            return new Color32((byte)r, (byte)g, (byte)b, 255);
        }

    }
}
