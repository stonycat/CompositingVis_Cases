using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinDistanceListener : MonoBehaviour
{
    private float _dist;
    public event OnVariableChangeDelegate OnVariableChange;
    public delegate void OnVariableChangeDelegate(float newVal);
    public float MinDist
    {
        get { return _dist; }
        set
        {
            if (_dist != value)
            {
                OnVariableChange(value);
            }
            _dist = value;
        }
    }
}
