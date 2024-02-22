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
    //public Transform Handle;
    //public Transform scatterplotVisParent;
    public VRVisualisation OriginVisualisation;

    [HideInInspector]
    public float valueFromWatcherX;
    [HideInInspector]
    public float valueFromWatcherY;

    // Start is called before the first frame update
    void Start()
    {
        
           
    }

    // Update is called once per frame
    void Update()
    {
        valueFromWatcherX = FindObjectOfType<PositionWatcher>().dis;
        valueFromWatcherY = FindObjectOfType<PositionWatcherY>().disY;
        //Debug.Log(valueFromWatcher);
    }
    //scatterplot0 cut max
    //Scatterplot0
    public void partitionVis0()
    {
        OriginVisualisation.DataScalingEventPartition0(InteractHandle, valueFromWatcherX, valueFromWatcherY, OriginVisualisation);

    }
    //Scatterplot01
    public void partitionVis01()
    {
        OriginVisualisation.DataScalingEventPartition01(InteractHandle, valueFromWatcherX, valueFromWatcherY, OriginVisualisation);
    }
    //
    public void partitionVis02()
    {
        OriginVisualisation.DataScalingEventPartition02(InteractHandle, valueFromWatcherX, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVis03()
    {
        //X
        OriginVisualisation.DataScalingEventPartition03(InteractHandle, valueFromWatcherX, valueFromWatcherY, OriginVisualisation);
    }


    //Y 1 11 12 13 update Data
    public void partitionVisY1()
    {
        OriginVisualisation.DataScalingEventPartitionY1(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVisY11()
    {
        OriginVisualisation.DataScalingEventPartitionY11(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVisY12()
    {
        OriginVisualisation.DataScalingEventPartitionY12(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVisY13()
    {
        OriginVisualisation.DataScalingEventPartitionY13(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }

    //Y 2 21 22 23 update Data
    public void partitionVisY2()
    {
        OriginVisualisation.DataScalingEventPartitionY2(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVisY21()
    {
        OriginVisualisation.DataScalingEventPartitionY21(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVisY22()
    {
        OriginVisualisation.DataScalingEventPartitionY22(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
    public void partitionVisY23()
    {
        OriginVisualisation.DataScalingEventPartitionY23(InteractHandle, valueFromWatcherY, OriginVisualisation);
    }
}