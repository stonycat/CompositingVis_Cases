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
                onVariableChange();
            }
            isGrabbed = value; 
        }
    }

    private bool isGrabbed;
    public event OnVariableChangeDelegate onVariableChange;
    public delegate void OnVariableChangeDelegate();
}
