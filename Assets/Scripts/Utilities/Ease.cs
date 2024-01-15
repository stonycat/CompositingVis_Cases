using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utilities
{
    public static class Ease
    {
        public static float easeInOutCubic(float t)
        {
            return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
        }

        public static float easeInOutQuart(float t)
        {
            return t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
        }
    }
}
