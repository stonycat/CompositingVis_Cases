using System.Collections;
using System.Collections.Generic;
using IATK;
using UnityEngine;

public class CreateSmallMultiples : MonoBehaviour
{
    //handles
    public Transform handle;


    // Small multiples preset
    public Visualisation Xvis1;
    public Visualisation Xvis2;

    // Start is called before the first frame update
    void Start()
    {
        Xvis1.SetActive(false);
        Xvis2.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
