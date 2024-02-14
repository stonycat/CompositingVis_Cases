using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public GameObject trackObjInteractable;
    // Start is called before the first frame update
    void Start()
    {
        List<Transform> list = new List<Transform>();
        Transform map = transform.GetChild(0).GetChild(1).GetChild(0);
        for (int i = 0; i < map.childCount; i++)
        {
            list.Add(map.GetChild(i));
            Debug.Log(map.GetChild(i).GetChild(0).GetChild(0).position);
        }
        trackObjInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().Loading();
        trackObjInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().CreateChart();
        trackObjInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().AddBarMoveComponent(list);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<InteractableTest>().interactable.IsGrabbed == true)
        {
            return;
        }
        ShowPreview();
    }

    private void OnCollisionExit(Collision collision)
    {
        HidePreview();
    }

    private void ShowPreview()
    {

    }

    private void HidePreview()
    {

    }
}
