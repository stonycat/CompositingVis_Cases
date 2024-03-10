using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarCopyObjectInteractable : MonoBehaviour
{
    public GameObject origin;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Colllisssssdsds");
        //collision.transform.localScale = 0.3f * Vector3.one ;
    }
}
