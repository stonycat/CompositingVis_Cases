using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionWatcher : MonoBehaviour
{

    public Transform handleXWatch;
    
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

        //listenYMove();

        listenXMove();
        
    }

    public void listenXMove()
    {
        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.05f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.6f)
        {
            onPositionChange1.Invoke(handleXWatch);
        }

        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.4f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.9f)
        {
            onPositionChange2.Invoke(handleXWatch);
        }

        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.8f)
        {
            onPositionChange3.Invoke(handleXWatch);
        }
    }


    //public void listenYMove()
    //{
    //    if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.1f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.6f)
    //    {
    //        onPositionChange1.Invoke(handleXWatch);
    //    }

    //    if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.3f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.9f)
    //    {
    //        onPositionChange2.Invoke(handleXWatch);
    //    }

    //    //if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.8f)
    //    //{
    //    //    onPositionChange3.Invoke(handleXWatch);
    //    //}
    //}
}
