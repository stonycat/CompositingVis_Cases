using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Interactables.Interactables;
using UnityEngine;

public class BendSmallMultiples : MonoBehaviour
{
    public InteractableFacade RightHandBend;
    public InteractableFacade LeftHandBend;

    public Transform SmallMultiples01;
    public Transform SmallMultiples02;
    public Transform SmallMultiples03;
    public Transform SmallMultiples04;
    public Transform SmallMultiples05;   
    public Transform SmallMultiples06;
    public Transform SmallMultiples07;
    public Transform SmallMultiples08;
    public Transform SmallMultiples09;
    public Transform SmallMultiples10;
    public Transform SmallMultiples11;
    public Transform SmallMultiples12;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RightHandBend.IsGrabbed && LeftHandBend.IsGrabbed)
        {
            float distance = Vector3.Distance(RightHandBend.transform.position, LeftHandBend.transform.position);
            ManipulateSmallMultiples(distance);
        }
    }

    public void ManipulateSmallMultiples(float longth)
    {

    }
}
