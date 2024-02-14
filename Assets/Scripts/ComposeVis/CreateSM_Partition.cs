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

    public LinearDriveFacade InteractHandle;
    public Transform Handle;
    public VRVisualisation OriginVisualisation;

    //public float startPartition;
    //public float endPartition;
    //public AbstractVisualisation AbXvis;
    //public Axis AbXaxis;
    //public DimensionFilter dim;
    //private AttributeFilter attribute1;

    //private Vector3 lastPositionX;
    //public UnityEvent<Transform> onPositionChange1;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Intial HandleX position" + Handle.position.x);
        Debug.Log("Intial HandleY position" + Handle.position.y);


    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Intial HandleX move" + HandleX.position.x);
    }
    //scatterplot0 cut max
    public void partitionVis0()
    {
        //OriginVisualisation.doit(InteractHandleX.transform.position.x, 1.0f);
        OriginVisualisation.DataScalingEventPartition0(InteractHandle, Handle.position.x);

    }
    public void partitionVis01()
    {
        OriginVisualisation.DataScalingEventPartition01(InteractHandle, Handle.position.x);
    }
    public void partitionVis02()
    {
        OriginVisualisation.DataScalingEventPartition02(InteractHandle, Handle.position.x);
    }

    //Y update Data
    public void partitionVisY0()
    {
        //OriginVisualisation.doit(InteractHandleX.transform.position.x, 1.0f);
        OriginVisualisation.DataScalingEventPartitionY0(InteractHandle, Handle.position.y);

    }

}