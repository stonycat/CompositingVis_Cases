using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGrabbedListener : MonoBehaviour
{
    public bool IsGrabbed
    {
        get
        {
            return isGrabbed;
        }
        set 
        { 
            if (!value && isGrabbed)
            {
                onVariableFalse();
            }
            if (value && !isGrabbed)
            {
                onVariableTrue();
            }
            isGrabbed = value; 
        }
    }

    private bool isGrabbed;
    public event OnVariableFalse onVariableFalse;
    public delegate void OnVariableFalse();
    public event OnTrueDelegate onVariableTrue;
    public delegate void OnTrueDelegate();
}
