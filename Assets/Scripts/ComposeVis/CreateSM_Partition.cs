using IATK;
using Mapbox.Unity.Map.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Controllables.LinearDriver;
using TriangleNet.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using static Zinnia.Data.Operation.Extraction.TransformDirectionExtractor;

public class CreateSM_Partition : MonoBehaviour
{

    public LinearDriveFacade InteractHandleX;
    public Transform HandleX;
    public VRVisualisation OriginVisualisation;

    //public float startPartition;
    public float endPartition;
    //public AbstractVisualisation AbXvis;
    public Axis AbXaxis;
    //public DimensionFilter dim;
    //private AttributeFilter attribute1;

    //private Vector3 lastPositionX;
    //public UnityEvent<Transform> onPositionChange1;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Intial HandleX position" + HandleX.position.x);



    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Intial HandleX move" + HandleX.position.x);
    }
    //scatterplot0 cut max
    public void partitionVis0()
    {
        //OriginVisualisation.doit(InteractHandleX.transform.position.x, 1.0f);
        OriginVisualisation.DataScalingEventPartition0(InteractHandleX, HandleX.position.x);

    }
    //public void partitionVis01()
    //{
    //    OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, true, 0.01f, 0.33f);
    //}

}