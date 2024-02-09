using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestNodeListener : MonoBehaviour
{
    private int _closest;
    public event OnVariableChangeDelegate OnVariableChange;
    public delegate void OnVariableChangeDelegate(int newVal);
    public int ClosestNode
    {
        get {  return _closest; }
        set
        {
            if (_closest != value && value != -1)
            {
                OnVariableChange(value);
            }
            _closest = value;
        }
    }
}
