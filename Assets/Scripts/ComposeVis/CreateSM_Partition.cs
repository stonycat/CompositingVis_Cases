using IATK;
using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Controllables.LinearDriver;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class CreateSM_Partition : MonoBehaviour
{

    public LinearDriveFacade InteractHandleX;
    public VRVisualisation OriginVisualisation;

    //public float startPartition;
    //public float endPartition;
    //public AbstractVisualisation AbXvis;
    public Axis AbXaxis;
    //public DimensionFilter dim;
    //private AttributeFilter attribute1;

    //private Vector3 lastPositionX;
    //public UnityEvent<Transform> onPositionChange1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //scatterplot0 cut max
    //public void partitionVis0()
    //{
    //    //OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, false, 0.01f, 0.5f);
    //}
    //public void partitionVis01()
    //{
    //    OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, true, 0.01f, 0.33f);
    //}

}