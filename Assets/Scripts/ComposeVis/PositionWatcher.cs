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
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.3f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.6f)
        {
            onPositionChange1.Invoke(handleXWatch);
        }

        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.6f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.9f)
        {
            onPositionChange2.Invoke(handleXWatch);
        }

        if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.9f)
        {
            onPositionChange3.Invoke(handleXWatch);
        }


    }
}
