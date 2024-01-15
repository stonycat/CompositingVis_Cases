using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities.Scales
{
    public class DivergingNumbericScale : INumbericScale
    {
        private readonly float zero = 0;
        private Scale scale;
        public DivergingNumbericScale
        (
            float domainMin, float domainMax,
            float rangeMin, float rangeMax,
            Func<float, float> transfer = null
        )
        {
            var max = Mathf.Max(Mathf.Abs(domainMin), Mathf.Abs(domainMax));
            this.scale = new Scale(
                this.zero, max, rangeMin, rangeMax,
                transfer
            );
        }

        public float Project(float input)
        {
            if (input < 0)
                return -this.scale.Project(-input);
            else
                return this.scale.Project(input);
        }
    }

    public class OneDirNegative : INumbericScale
    {

        private readonly float zero = 0;
        private Scale posScale;
        private Scale negScale;

        public OneDirNegative
        (
            float domainMin, float domainMax,
            float rangeMin, float rangeMax,
            Func<float, float> transfer = null
        )
        {
            var max = Mathf.Max(Mathf.Abs(domainMin), Mathf.Abs(domainMax));
            var mid = (rangeMin + rangeMax) / 2;

            this.negScale = new Scale(
                this.zero, max, rangeMin, mid,
                transfer
            );

            this.posScale = new Scale(
                this.zero, max, mid, rangeMax,
                transfer
            );
        }

        public float Project(float input)
        {
            if (input < 0)
            {
                return this.negScale.Project(-input);
            }
            else
            {
                return this.posScale.Project(input);
            }
        }
    }

    public class DivergingColorMapper : IColorScale
    {
        private readonly float zero = 0;
        private ColorMapper positiveMapper;
        private ColorMapper negtiveMapper;

        public DivergingColorMapper
        (
            float domainMin, float domainMax,
            Color32[] colors,
            Func<float, float> transfer = null
        )
        {
            var max = Mathf.Max(Mathf.Abs(domainMin), Mathf.Abs(domainMax));
            if (colors.Length % 2 == 0) throw new ArgumentException("Diverging colors need odd number of colors as input.");

            var count = colors.Length / 2 + 1;

            var positiveColors = new Color32[count];
            for (var i = 0; i < count; i++)
            {
                positiveColors[i] = colors[count - 1 - i];
            }
            this.positiveMapper = new ColorMapper(
                this.zero, max, positiveColors, transfer
            );

            var negtiveColors = new Color32[count];
            for (var i = 0; i < count; i++)
            {
                negtiveColors[i] = colors[count - 1 + i];
            }
            this.negtiveMapper = new ColorMapper(
                this.zero, max, negtiveColors, transfer
            );
        }


        public Color32 Project(float input)
        {
            if(input < 0)
            {
                return this.negtiveMapper.Project(-input);
            }
            else
            {
                return this.positiveMapper.Project(input);
            }
        }
    }
}
