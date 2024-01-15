using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MonoBehaviors
{
    public enum GeoName // your custom enumeration
    {
        EU = 1,
        US = 2,
        UK = 3
    };

    public enum ThematicType
    {
        PropotionalSymbols,
        ElevatedBars,
        Prism,
        Choropleth,
        ColoredPrism,
        Bar
    };

    public enum TransferFuncType
    {
        Sqrt,
        Linear,
        Log
    }

    public enum PaletteName
    {
        YlGn,
        YlOrBr
    }

    static public class RenderingSettings
    {
        public static float MinZOffset = 0.0005f;
    }

}
