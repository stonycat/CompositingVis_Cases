using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Interactables.Interactables;
using TMPro;
using UnityEngine;

public class BendSmallMultiples : MonoBehaviour
{
    public InteractableFacade RightHandBend;
    public InteractableFacade LeftHandBend;

    public GameObject LeftHand;
    public GameObject RightHand;

    public Transform RightButton;
    public Transform LeftButton;

    public Transform SmallMultiples0;
    public Transform SmallMultiples01;
    public Transform SmallMultiples02;
    public Transform SmallMultiples03;
    public Transform SmallMultiples1;
    public Transform SmallMultiples11;   
    public Transform SmallMultiples12;
    public Transform SmallMultiples13;
    public Transform SmallMultiples2;
    public Transform SmallMultiples21;
    public Transform SmallMultiples22;
    public Transform SmallMultiples23;

    private float RightInitalPosition;
    private float LeftInitalPosition;
    private float RightMove;
    private float LeftMove;

    public Transform Target0;
    public Transform Target01;
    public Transform Target02;
    public Transform Target03;
    public Transform Target1;
    public Transform Target11;
    public Transform Target12;
    public Transform Target13;
    public Transform Target2;
    public Transform Target21;
    public Transform Target22;
    public Transform Target23;

    private float speed = 1.0f;
    private float rotateSpeed = 1.0f;
    private Quaternion targetLeftEulerAngles;
    private Quaternion targetRightEulerAngles;
    private Quaternion targetMidLeftEulerAngles;
    private Quaternion targetMidRightEulerAngles;
    private float duration = 6f;
    private float timeElapsed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //distance0 = Vector3.Distance(RightHandBend.transform.position, LeftHandBend.transform.position);
        //Debug.Log("right left dis:" + distance0);
        RightInitalPosition = RightButton.transform.position.x;
        LeftInitalPosition = LeftButton.transform.position.x;

        targetLeftEulerAngles = Quaternion.Euler(new Vector3(0, -35, 0));
        targetRightEulerAngles = Quaternion.Euler(new Vector3(0, 35, 0));

        targetMidLeftEulerAngles = Quaternion.Euler(new Vector3(0, -10, 0));
        targetMidRightEulerAngles = Quaternion.Euler(new Vector3(0, 10, 0));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.deltaTime);
        if (RightHandBend.IsGrabbed && LeftHandBend.IsGrabbed)
        {
            //Debug.Log("both grabbed");
            //distanceUpdate = Vector3.Distance(RightHandBend.transform.position, LeftHandBend.transform.position);
            RightMove = RightButton.position.x - RightInitalPosition;
            LeftMove = LeftButton.position.x - LeftInitalPosition;
            //Debug.Log(RightMove + " " + LeftMove);

            if (Mathf.Abs(RightMove) > 0.05f && Mathf.Abs(LeftMove) > 0.05f)
            {
                if(timeElapsed < duration)
                {
                    Debug.Log("animation");
                    //Move
                    SmallMultiples0.transform.position = Vector3.Lerp(SmallMultiples0.transform.position, Target0.position, timeElapsed / duration);
                    SmallMultiples01.transform.position = Vector3.Lerp(SmallMultiples01.transform.position, Target01.position, timeElapsed / duration);
                    SmallMultiples02.transform.position = Vector3.Lerp(SmallMultiples02.transform.position, Target02.position, timeElapsed / duration);
                    SmallMultiples03.transform.position = Vector3.Lerp(SmallMultiples03.transform.position, Target03.position, timeElapsed / duration);

                    SmallMultiples1.transform.position = Vector3.Lerp(SmallMultiples1.transform.position, Target1.position, timeElapsed / duration);
                    SmallMultiples11.transform.position = Vector3.Lerp(SmallMultiples11.transform.position, Target11.position, timeElapsed / duration);
                    SmallMultiples12.transform.position = Vector3.Lerp(SmallMultiples12.transform.position, Target12.position, timeElapsed / duration);
                    SmallMultiples13.transform.position = Vector3.Lerp(SmallMultiples13.transform.position, Target13.position, timeElapsed / duration);

                    SmallMultiples2.transform.position = Vector3.Lerp(SmallMultiples2.transform.position, Target2.position, timeElapsed / duration);
                    SmallMultiples21.transform.position = Vector3.Lerp(SmallMultiples21.transform.position, Target21.position, timeElapsed / duration);
                    SmallMultiples22.transform.position = Vector3.Lerp(SmallMultiples22.transform.position, Target22.position, timeElapsed / duration);
                    SmallMultiples23.transform.position = Vector3.Lerp(SmallMultiples23.transform.position, Target23.position, timeElapsed / duration);

                    //Rotate
                    SmallMultiples0.transform.rotation = Quaternion.Lerp(SmallMultiples0.transform.rotation, targetLeftEulerAngles, timeElapsed / duration);
                    //SmallMultiples01.transform.rotation = Quaternion.Lerp(SmallMultiples01.transform.rotation, targetMidLeftEulerAngles, timeElapsed / duration);
                    //SmallMultiples02.transform.rotation = Quaternion.Lerp(SmallMultiples02.transform.rotation, targetMidRightEulerAngles, timeElapsed / duration);
                    SmallMultiples03.transform.rotation = Quaternion.Lerp(SmallMultiples03.transform.rotation, targetRightEulerAngles, timeElapsed / duration);

                    SmallMultiples1.transform.rotation = Quaternion.Slerp(SmallMultiples1.transform.rotation, targetLeftEulerAngles, timeElapsed / duration);
                    //SmallMultiples11.transform.rotation = Quaternion.Slerp(SmallMultiples11.transform.rotation, targetMidLeftEulerAngles, timeElapsed / duration);
                    //SmallMultiples12.transform.rotation = Quaternion.Slerp(SmallMultiples12.transform.rotation, targetMidRightEulerAngles, timeElapsed / duration);
                    SmallMultiples13.transform.rotation = Quaternion.Slerp(SmallMultiples13.transform.rotation, targetRightEulerAngles, timeElapsed / duration);

                    SmallMultiples2.transform.rotation = Quaternion.Slerp(SmallMultiples2.transform.rotation, targetLeftEulerAngles, timeElapsed / duration);
                    //SmallMultiples21.transform.rotation = Quaternion.Slerp(SmallMultiples21.transform.rotation, targetMidLeftEulerAngles, timeElapsed / duration);
                    //SmallMultiples22.transform.rotation = Quaternion.Slerp(SmallMultiples22.transform.rotation, targetMidRightEulerAngles, timeElapsed / duration);
                    SmallMultiples23.transform.rotation = Quaternion.Slerp(SmallMultiples23.transform.rotation, targetRightEulerAngles, timeElapsed / duration);

                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    LeftHand.SetActive(false);
                    RightHand.SetActive(false);
                }
            }
        }       
    }
}
