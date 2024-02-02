using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSM_Partition : MonoBehaviour
{

    public Transform HandleX;

    public Visualisation OriginVisualisation;
    public Visualisation Xvis01;
    public Visualisation Xvis02;
    public Visualisation Xvis03;
    public Visualisation Xvis04;

    private Vector3 lastPositionX;

    // Start is called before the first frame update
    void Start()
    {
        Xvis01.SetActive(false);
        Xvis02.SetActive(false);
        Xvis03.SetActive(false);
        Xvis04.SetActive(false);

        lastPositionX = HandleX.position;
        Debug.Log("initial position X:" + HandleX.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPositionX != HandleX.position)
        {
            //Debug.Log("X handle moved:" + HandleX.position.x);
            lastPositionX = HandleX.position;
        }

        DetectXPartitionVis(HandleX);
    }

    private void DetectXPartitionVis(Transform HandleX)
    {
        if (HandleX.position.x > 0.5f)
        {
            OriginVisualisation.xDimension.minScale = 0.001f;
            OriginVisualisation.xDimension.maxScale = 0.5f;


            Xvis01.xDimension.maxScale = 0.99f;
            Xvis01.xDimension.minScale = 0.5f;
            Xvis01.SetActive(true);

        }
        else //X back to initial position
        {
            Xvis01.SetActive(false);
            Xvis02.SetActive(false);
        }
    }
}
