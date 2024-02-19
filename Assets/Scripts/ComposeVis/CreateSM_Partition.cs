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
    public Transform scatterplotVisParent;
    private float initialRelativeDistance;

    public VRVisualisation OriginVisualisation;
    private float disCompute;
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
        //Debug.Log("Intial HandleY position" + Handle.position.y);
        initialRelativeDistance = Vector3.Distance(Handle.position, scatterplotVisParent.position);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Intial HandleX move" + HandleX.position.x);
        float currentRelativeDistance = Vector3.Distance(Handle.position, scatterplotVisParent.position);
        disCompute = Mathf.Abs(currentRelativeDistance - initialRelativeDistance);
        //Debug.Log(Mathf.Abs(currentRelativeDistance - initialRelativeDistance));
        //Debug.Log("HandleX:" + disCompute);
    }
    //scatterplot0 cut max
    //Scatterplot0
    public void partitionVis0()
    {
        OriginVisualisation.DataScalingEventPartition0(InteractHandle, disCompute, OriginVisualisation);

    }
    //Scatterplot01
    public void partitionVis01One()
    {
        OriginVisualisation.DataScalingEventPartition01(InteractHandle, disCompute, OriginVisualisation);
    }
    //public void partitionVis01Two()
    //{
    //    OriginVisualisation.DataScalingEventPartition01One(InteractHandle, disCompute, OriginVisualisation);
    //}
    //public void partitionVis01Three()
    //{
    //    OriginVisualisation.DataScalingEventPartition01One(InteractHandle, disCompute, OriginVisualisation);
    //}


    //
    public void partitionVis02()
    {
        OriginVisualisation.DataScalingEventPartition02(InteractHandle, disCompute, OriginVisualisation);
    }
    public void partitionVis03()
    {
        //X
        OriginVisualisation.DataScalingEventPartition02(InteractHandle, disCompute, OriginVisualisation);
    }


    //Y update Data
    //public void partitionVisY0()
    //{
    //    //OriginVisualisation.doit(InteractHandleX.transform.position.x, 1.0f);
    //    OriginVisualisation.DataScalingEventPartitionY0(InteractHandle, Handle.position.y);

    //}

}