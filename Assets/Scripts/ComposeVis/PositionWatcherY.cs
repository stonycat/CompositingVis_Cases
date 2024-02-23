using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionWatcherY : MonoBehaviour
{
    public Transform originY;
    public Transform HandleY;
    private float initialRelativeDistance;
    [HideInInspector]
    public float disY;


    public Transform handleYWatch;
    public UnityEvent<Transform> onPositionChange1;
    public UnityEvent<Transform> onPositionChange2;
    //public UnityEvent<Transform> onPositionChange3;
    //public Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        //lastPosition = handleYWatch.position;
        initialRelativeDistance = Vector3.Distance(HandleY.position, originY.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("distanceHandleYVis:" + Vector3.Distance(lastPosition, handleYWatch.position));

        float currentRelativeDistance = Vector3.Distance(HandleY.position, originY.position);
        disY = Mathf.Abs(currentRelativeDistance - initialRelativeDistance);

        Debug.Log("handleY watcher:" + disY);

        if (disY > 0.01f && disY < 0.23f)
        {
            onPositionChange1.Invoke(handleYWatch);
        }
        else if (disY > 0.23f && disY < 0.48f)
        {
            onPositionChange2.Invoke(handleYWatch);
        }
    }

    //public void listenYMove()
    //{
        
    //    if (Vector3.Distance(lastPosition, handleYWatch.position) > 0.05f && Vector3.Distance(lastPosition, handleYWatch.position) < 0.35f)
    //    {
    //        onPositionChange1.Invoke(handleYWatch);
    //    }

    //    if (Vector3.Distance(lastPosition, handleYWatch.position) > 0.35f && Vector3.Distance(lastPosition, handleYWatch.position) < 0.65f)
    //    {
    //        onPositionChange2.Invoke(handleYWatch);
    //    }

    //    //if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.8f)
    //    //{
    //    //    onPositionChange3.Invoke(handleXWatch);
    //    //}
    //}
}
