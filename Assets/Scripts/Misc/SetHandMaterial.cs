using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHandMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Material myMaterial = Resources.Load<Material>("Hand Material");
        GetComponent<SkinnedMeshRenderer>().materials[0] = myMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Material myMaterial = Resources.Load<Material>("ds");
        GetComponent<SkinnedMeshRenderer>().materials[0] = myMaterial;
    }
}
