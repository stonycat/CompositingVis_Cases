
using System.Collections;
using System.Collections.Generic;
using IATK;
using UnityEngine;
using UnityEngine.XR;
using Tilia.Interactions.Controllables.LinearDriver;
using Tilia.Interactions.Interactables.Interactables;

public class CreateSmallMultiples : MonoBehaviour
{
    //handles
    public Transform HandleX;
    public Transform HandleY;

    public InteractableFacade InteractableHandleX;
    public InteractableFacade InteractableHandleY;

    private bool isLeftGrabbedY = false;
    private bool isRightGrabbedX = false;

    // Small multiples preset 5x3
    public Visualisation Xvis01;
    public Visualisation Xvis02;
    public Visualisation Xvis03;
    //public Visualisation Xvis04;

    public Visualisation Xvis1;
    public Visualisation Xvis11;
    public Visualisation Xvis12;
    public Visualisation Xvis13;
    //public Visualisation Xvis14;

    public Visualisation Xvis2;
    public Visualisation Xvis21;
    public Visualisation Xvis22;
    public Visualisation Xvis23;
    //public Visualisation Xvis24;

    private Vector3 lastPositionX;
    private Vector3 lastPositionY;
    // Start is called before the first frame update
    void Start()
    {
        Xvis01.SetActive(false);
        Xvis02.SetActive(false);
        Xvis03.SetActive(false);

        Xvis1.SetActive(false);
        Xvis11.SetActive(false);
        Xvis12.SetActive(false);
        Xvis13.SetActive(false);

        Xvis2.SetActive(false);
        Xvis21.SetActive(false);
        Xvis22.SetActive(false);
        Xvis23.SetActive(false);


        lastPositionX = HandleX.position;
        lastPositionY = HandleY.position;

        
    }


    // Update is called once per frame
    void Update()
    {
        // Bimanual interaction
        biExtendVis(HandleX, HandleY, InteractableHandleX, InteractableHandleY);
        Debug.Log("");
    }

    private void biExtendVis(Transform HandleX, Transform HandleY, InteractableFacade InteractableHandleX, InteractableFacade InteractableHandleY)
    {
        if (InteractableHandleX.IsGrabbed && InteractableHandleY.IsGrabbed)
        {
            if (HandleX.position.x > 0.2f && HandleY.position.y < 0.2f)
            {
                Xvis01.SetActive(true);
                Xvis02.SetActive(true);
                Xvis03.SetActive(true);
                //Xvis04.SetActive(true);

                Xvis1.SetActive(true);
                Xvis11.SetActive(true);
                Xvis12.SetActive(true);
                Xvis13.SetActive(true);

                Xvis2.SetActive(true);
                Xvis21.SetActive(true);
                Xvis22.SetActive(true);
                Xvis23.SetActive(true);
            }
            else
            {
                Xvis01.SetActive(false);
                Xvis02.SetActive(false);
                Xvis03.SetActive(false);
                //Xvis04.SetActive(true);

                Xvis1.SetActive(false);
                Xvis11.SetActive(false);
                Xvis12.SetActive(false);
                Xvis13.SetActive(false);

                Xvis2.SetActive(false);
                Xvis21.SetActive(false);
                Xvis22.SetActive(false);
                Xvis23.SetActive(false);
            }
        }

       
    }


    //private void DetectXControlVis(Transform HandleX)
    //{
    //    if (HandleX.position.x > 0.5f)
    //    {
    //        Xvis01.SetActive(true);
    //        if (HandleX.position.x < 0.85f) //0.4-0.7 1&2
    //        {
    //            Xvis02.SetActive(false);
    //            Xvis03.SetActive(false);
    //            Xvis04.SetActive(false);
    //        }
    //        else // 0.7-
    //        {
    //            Xvis02.SetActive(true);
    //            if (HandleX.position.x < 1.2f) // 0.7-1.0 1&2&3
    //            {
    //                Xvis03.SetActive(false);
    //                Xvis03.SetActive(false);
    //            }
    //            else // 1.0-
    //            {
    //                Xvis03.SetActive(true);
    //                if (HandleX.position.x < 1.4f) // 1.0-1.3
    //                {
    //                    Xvis04.SetActive(false);
    //                }
    //                else
    //                {
    //                    Xvis04.SetActive(true);
    //                }
    //            }

    //        }
    //    }
    //    else //X back to initial position
    //    {
    //        Xvis01.SetActive(false);
    //        Xvis02.SetActive(false);
    //        Xvis03.SetActive(false);
    //        Xvis04.SetActive(false);
    //    }
    //}

    //private void DetectYControlVis(Transform HandleY)
    //{
    //    if (HandleY.position.y < 0.2f)
    //    {
    //        Xvis1.SetActive(true);
    //        Xvis11.SetActive(true);
    //        Xvis12.SetActive(true);
    //        Xvis13.SetActive(true);
    //        Xvis14.SetActive(true);
    //        if (HandleY.position.y > -0.01f)
    //        {
    //            Xvis2.SetActive(false);
    //            Xvis21.SetActive(false);
    //            Xvis22.SetActive(false);
    //            Xvis23.SetActive(false);
    //            Xvis24.SetActive(false);
    //        }
    //        else if(HandleY.position.y < -0.2f)
    //        {
    //            Xvis2.SetActive(true);
    //            Xvis21.SetActive(true);
    //            Xvis22.SetActive(true);
    //            Xvis23.SetActive(true);
    //            Xvis24.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        Xvis1.SetActive(false);
    //        Xvis11.SetActive(false);
    //        Xvis12.SetActive(false);
    //        Xvis13.SetActive(false);
    //        Xvis14.SetActive(false);

    //        Xvis2.SetActive(false);
    //        Xvis21.SetActive(false);
    //        Xvis22.SetActive(false);
    //        Xvis23.SetActive(false);
    //        Xvis24.SetActive(false);
    //    }
    //}
}
