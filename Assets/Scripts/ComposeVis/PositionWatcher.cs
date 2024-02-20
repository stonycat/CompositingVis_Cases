using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionWatcher : MonoBehaviour
{
    public Transform scatterplotVisParent;
    public Transform HandleX;
    private float initialRelativeDistance;

    [HideInInspector]
    public float dis;

    public Transform handleXWatch;
    public UnityEvent<Transform> onPositionChange1;
    public UnityEvent<Transform> onPositionChange2;
    public UnityEvent<Transform> onPositionChange3;
    //public Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        //lastPosition = HandleX.position;
        initialRelativeDistance = Vector3.Distance(HandleX.position, scatterplotVisParent.position);
    }

    // Update is called once per frame
    void Update()
    {
        //
        //listenXMove();
        float currentRelativeDistance = Vector3.Distance(HandleX.position, scatterplotVisParent.position);
        dis = Mathf.Abs(currentRelativeDistance - initialRelativeDistance);
        //Debug.Log("handle watcher:" + dis);


        if (dis > 0.01f && dis < 0.31f)
        {
            onPositionChange1.Invoke(handleXWatch);
        }
        if (dis > 0.31f && dis < 0.6f)
        {
            onPositionChange2.Invoke(handleXWatch);
        }
        if (dis > 0.6f && dis < 0.89f)
        {
            onPositionChange3.Invoke(handleXWatch);
        }
    }

    //public void listenXMove()
    //{
    //    if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.1f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.4f)
    //    {
    //        onPositionChange1.Invoke(handleXWatch);
    //    }

    //    if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.4f && Vector3.Distance(lastPosition, handleXWatch.position) < 0.8f)
    //    {
    //        onPositionChange2.Invoke(handleXWatch);
    //    }

    //    if (Vector3.Distance(lastPosition, handleXWatch.position) > 0.8f)
    //    {
    //        onPositionChange3.Invoke(handleXWatch);
    //    }
    //    //Vector3 localPosition = scatterplotVisParent.InverseTransformPoint(handleXWatch.position);
    //    //if (Mathf.Abs(localPosition.x) > 0.05f && Mathf.Abs(localPosition.x) < 0.4f)
    //    //{
    //    //    onPositionChange1.Invoke(handleXWatch);
    //    //}

    //    //if (Mathf.Abs(localPosition.x) > 0.4f && Mathf.Abs(localPosition.x) < 0.8f)
    //    //{
    //    //    onPositionChange2.Invoke(handleXWatch);
    //    //}

    //    //if (Mathf.Abs(localPosition.x) > 0.8f)
    //    //{
    //    //    onPositionChange3.Invoke(handleXWatch);
    //    //}
    //}

}
