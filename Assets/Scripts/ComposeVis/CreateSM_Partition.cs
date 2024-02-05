using IATK;
using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Controllables.LinearDriver;
using UnityEngine;
using UnityEngine.XR;

public class CreateSM_Partition : MonoBehaviour
{

    public Transform HandleX;
    public LinearDriveFacade InteractHandleX;

    public VRVisualisation OriginVisualisation;
    public VRVisualisation Xvis01;
    public VRVisualisation Xvis02;
    public VRVisualisation Xvis03;

    //public AbstractVisualisation AbXvis;
    public Axis AbXaxis;
    //public DimensionFilter dim;
    //private AttributeFilter attribute1;

    private Vector3 lastPositionX;

    // Start is called before the first frame update
    void Start()
    {
        Xvis01.SetActive(false);
        Xvis02.SetActive(false);
        Xvis03.SetActive(false);

        lastPositionX = HandleX.position;
        //Debug.Log("initial position X:" + HandleX.position.x);

        //using my own handle
        //OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, false, 0.01f, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        if (lastPositionX != HandleX.position)
        {
            Debug.Log("X handle moved:" + HandleX.position.x);
            lastPositionX = HandleX.position;
        }

        DetectXPartitionVis(HandleX);
    }

    private void DetectXPartitionVis(Transform HandleX)
    {
        if (HandleX.position.x > 0.85f)
        {
            //OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, false, 0.01f, 0.5f);
            Xvis01.SetActive(true);
            //Xvis01.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, 0.5f, 1.0f);

            if (HandleX.position.x > 1.25f)
            {
                //OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, false, 0.01f, 0.33f);
                //Xvis01.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, 0.33f, 0.66f);
                Xvis02.SetActive(true);
                //Xvis02.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, 0.66f, 1.0f);
                if (HandleX.position.x > 1.65f)
                {
                    ///OriginVisualisation.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, false, 0.01f, 0.25f);
                    //Xvis01.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, 0.25f, 0.5f);
                    //Xvis02.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, 0.5f, 0.75f);
                    Xvis03.SetActive(true);
                    //Xvis03.HalfScalingEventPartition(InteractHandleX, AbXaxis.AxisDirection, 0.75f, 1.0f);
                }
                else
                {
                    Xvis03.SetActive(false);
                }
            }
            else
            {
                Xvis02.SetActive(false);
                Xvis03.SetActive(false);
            }

        }
        else //X back to initial position
        {
            Xvis01.SetActive(false);
            Xvis02.SetActive(false);
            Xvis03.SetActive(false);
        }
    }
}
