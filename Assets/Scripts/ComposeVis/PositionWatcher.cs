using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionWatcher : MonoBehaviour
{
   
    public Transform handleXWatch;
    public Transform scatterplotVisParent;
    private float initialRelativeDistance;
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
        initialRelativeDistance = Vector3.Distance(handleXWatch.position, scatterplotVisParent.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("handle positionX:" + handleXWatch.position.x);
        //listenXMove();
        float currentRelativeDistance = Vector3.Distance(handleXWatch.position, scatterplotVisParent.position);
        float dis = Mathf.Abs(currentRelativeDistance - initialRelativeDistance);
        //if (Mathf.Abs(currentRelativeDistance - initialRelativeDistance) > 0.01f)
        //{
        //    Debug.Log("Relative Distance has changed");
        //}
        //else
        //{
        //    Debug.Log("Relative Distance remains unchanged");
        //}
        if (dis > 0.01f && dis < 0.35f)
        {
            onPositionChange1.Invoke(handleXWatch);
        }
        if (dis > 0.35f && dis < 0.65f)
        {
            onPositionChange2.Invoke(handleXWatch);
        }
        if (dis > 0.65f)
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
