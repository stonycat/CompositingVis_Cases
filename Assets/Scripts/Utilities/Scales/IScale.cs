using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities.Scales
{
    public interface INumbericScale
    {
        float Project(float input);
    }

    public interface IColorScale
    {
        Color32 Project(float input);
    }
}
