using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionWatcher : MonoBehaviour
{

    public Transform handleXWatch;
    public Transform scatterplotVisParent;
    
    public UnityEvent<Transform> onPositionChange1;
    public UnityEvent<Transform> onPositionChange2;
    public UnityEvent<Transform> onPositionChange3;
    public Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = handleXWatch.position;
        //Debug.Log("initial X:" + lastPosition.x);
        //Debug.Log("initial Y:" + lastPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("handle positionX:" + handleXWatch.position.x);
        listenXMove();
        
    }

    public void listenXMove()
    {
        Vector3 localPosition = scatterplotVisParent.InverseTransformPoint(handleXWatch.position);
        if (Mathf.Abs(localPosition.x) > 0.05f && Mathf.Abs(localPosition.x) < 0.4f)
        {
            onPositionChange1.Invoke(handleXWatch);
        }

        if (Mathf.Abs(localPosition.x) > 0.4f && Mathf.Abs(localPosition.x) < 0.8f)
        {
            onPositionChange2.Invoke(handleXWatch);
        }

        if (Mathf.Abs(localPosition.x) > 0.8f)
        {
            onPositionChange3.Invoke(handleXWatch);
        }
    }

}
