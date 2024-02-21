using Assets.Scripts.MonoBehaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject trackObjInteractable;
    public StackedBarDraw trackedBarChart;
    public ThematicMapObj mapObj;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(mapObj != null);
    }

    public void Init(Dictionary<string, List<float>> barChartData)
    {
        List<Transform> list = new List<Transform>();
        Transform map = transform.GetChild(0).GetChild(1).GetChild(0);
        foreach (string s in barChartData.Keys)
        {
            Transform region = map.transform.Find(s);
            Debug.Assert(region != null);
            list.Add(region);
        }
        trackedBarChart = trackObjInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>();
        trackedBarChart.MinX = -0.54f;
        trackedBarChart.MaxX = 0.46f;
        trackedBarChart.LoadingFromDict(barChartData);
        trackedBarChart.CreateChart();
        trackedBarChart.AddBarMoveComponent(list);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShowPreview();
        if (collision.gameObject.GetComponent<InteractableTest>().interactable.IsGrabbed == true)
        {
            return;
        }
        trackedBarChart.Compose();
        mapObj.SetLabelHight(trackedBarChart.barHeights);
        //GetComponent<BoxCollider>().isTrigger = true;
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
