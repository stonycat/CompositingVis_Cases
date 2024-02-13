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
        //Debug.Log("initial:" + lastPosition.x);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("handle positionX:" + handleXWatch.position.x);
        
        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.35f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.6f)
        {
            onPositionChange1.Invoke(handleXWatch);
        }

        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.65f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.9f)
        {
            onPositionChange2.Invoke(handleXWatch);
        }

        if (Vector3.Distance(lastPosition, handleXWatch.position) > 1.0f)
        {
            onPositionChange3.Invoke(handleXWatch);
        }


    }
}
