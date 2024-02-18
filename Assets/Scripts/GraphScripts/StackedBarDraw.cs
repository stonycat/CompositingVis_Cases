using Assets.Scripts.MonoBehaviors;
using IATK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Tilia.Interactions.Interactables.Interactors;
using TMPro;
using UnityEngine;
using Zinnia.Tracking.Collision.Active.Operation.Extraction;

public class StackedBarDraw : MonoBehaviour
{
    public TextAsset data;
    public Material[] materials;
    public bool isStacked;
    public bool useDifferentMaterialEachBar;
    public bool isMoving;
    public GameObject MovingTarget;
    public Dictionary<string, List<float>> attr;
    public Dictionary<string, float> barHeights;
    public Dictionary<string, float> barXPosition;
    public float MinX = -0.5f;
    public float MaxX = 0.5f;

    private int numAttr;
    private int numNode;
    private int StepC;
    private int StepL;
    private List<float> XLinspace;
    private List<BarMoveAnimation> barAnimation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Loading()
    {
        LoadData();
        ProcessData();
    }

    public void LoadingFromDict(Dictionary<string, List<float>> barChartData)
    {
        LoadDataFromDict(barChartData);
        ResetData();
    }

    public void CreateChart()
    {
        Draw();
    }

    private void LoadData()
    {
        if (data == null || attr != null) return;
        attr = new Dictionary<string, List<float>>();
        string[] lines = data.text.Split("\n");
        numAttr = int.Parse(lines[0].Trim());
        numNode = int.Parse(lines[1].Trim());
        for (int i = 0; i < numAttr; i++)
        {
            string attrName = lines[i * (numNode + 1) + 2].Trim();
            attr.Add(attrName, new List<float>());
            for (int j = 1; j <= numNode; j++)
            {
                string v = lines[i * (numNode + 1) + j + 2].Trim();
                attr[attrName].Add(float.Parse(v));
            }
        }
        XLinspace = new List<float>(numNode);
        Debug.Assert(XLinspace.Capacity == numNode);
        for (int i = 0; i < numNode; i++)
        {
            XLinspace.Add(MinX + (MaxX - MinX) * i / numNode);
        }
    }

    private void LoadDataFromDict(Dictionary<string, List<float>> data)
    {
        attr = data;
    }

    public void ResetData()
    {
        numNode = 1;
        numAttr = attr.Keys.Count;
        XLinspace = new List<float>(numAttr);
        Debug.Assert(XLinspace.Capacity == numAttr);
        for (int i = 0; i < numAttr; i++)
        {
            XLinspace.Add(MinX + (MaxX - MinX) * i / numAttr);
        }
        float largest = 0;
        foreach (string s in attr.Keys)
        {
            if (attr[s][0] > largest)
            {
                largest = attr[s][0];
            }
        }
        float mag = Mathf.Floor(Mathf.Log10(largest));
        StepL = (int)Mathf.Pow(10, mag);
        StepC = (int)Mathf.Ceil(largest / StepL);
        Debug.Log(StepL);
        Debug.Log(StepC);
    }

    private void ProcessData()
    {
        if (data == null && attr == null) return;
        float largest = 0;
        for (int i = 0; i < numNode; i++)
        {
            float total = 0;
            foreach (string s in attr.Keys)
            {
                total += attr[s][i];
                Debug.Log(attr[s][i]);
            }
            if (total > largest)
            {
                largest = total;
            }
        }
        float mag = Mathf.Floor(Mathf.Log10(largest));
        StepL = (int)Mathf.Pow(10, mag);
        StepC = (int)Mathf.Ceil(largest / StepL);
    }

    private void drawYAxis()
    {
        GameObject y = transform.GetChild(1).gameObject;
        GameObject yAxisLabels = y.transform.GetChild(3).gameObject;
        GameObject BaseLabel = yAxisLabels.transform.GetChild(0).gameObject;
        for (int i = 0; i <= StepC; i++)
        {
            GameObject label = Instantiate(BaseLabel, BaseLabel.transform.position, BaseLabel.transform.rotation);
            label.transform.parent = yAxisLabels.transform;
            label.transform.localPosition = new Vector3(0, i / (float)StepC, 0);
            label.transform.GetChild(0).GetComponent<TextMeshPro>().text = (i * StepL).ToString();
            label.SetActive(true);
        }
    }

    private void drawXAxis()
    {
        GameObject x = transform.GetChild(0).gameObject;
        GameObject xAxisLabels = x.transform.GetChild(3).gameObject;
        GameObject BaseLabel = xAxisLabels.transform.GetChild(0).gameObject;
        int idx = 0;
        foreach (string s in attr.Keys)
        {
            GameObject label = Instantiate(BaseLabel, BaseLabel.transform.position, BaseLabel.transform.rotation);
            label.transform.parent = xAxisLabels.transform;
            label.transform.localPosition = new Vector3(0, (idx + 0.5f) / (float)numAttr, 0);
            label.transform.GetChild(0).GetComponent<TextMeshPro>().text = s;
            label.SetActive(true);
            idx++;
        }
    }

    private void Draw()
    {
        if (data == null && attr == null) return;
        
        if (isStacked)
        {
            // draw Y axis
            drawYAxis();

            // draw View
            GameObject view = transform.GetChild(2).gameObject;
            GameObject BarWrap = view.transform.GetChild(0).gameObject;
            for (int i = 0; i < numNode; i++)
            {
                float val = 0;
                int idx = 0;
                foreach (string s in attr.Keys)
                {
                    GameObject bar = Instantiate(BarWrap, Vector3.zero, Quaternion.identity);
                    bar.transform.parent = view.transform;
                    bar.transform.localPosition = new Vector3(XLinspace[i], val / (StepL * StepC), 0);
                    val += attr[s][i];
                    bar.transform.localScale = new Vector3(1f / numNode, (float)attr[s][i] / (StepL * StepC), BarWrap.transform.localScale.z);
                    bar.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[idx];
                    bar.SetActive(true);
                    idx++;
                }
            }
        }
        else if (useDifferentMaterialEachBar)
        {
            // draw Y axis
            drawYAxis();

            // draw X axis
            drawXAxis();

            // draw View
            barHeights = new Dictionary<string, float>();
            GameObject view = transform.GetChild(2).gameObject;
            GameObject BarWrap = view.transform.GetChild(0).gameObject;
            int idx = 0;
            foreach (string s in attr.Keys)
            {
                Debug.Assert(attr[s].Count == 1);
                GameObject bar = Instantiate(BarWrap, Vector3.zero, Quaternion.identity);
                bar.transform.parent = view.transform;
                bar.transform.localPosition = new Vector3(XLinspace[idx], 0, 0);
                bar.transform.localScale = new Vector3(1f / numAttr, (float)attr[s][0] / (StepL * StepC), BarWrap.transform.localScale.z);
                bar.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[idx];
                bar.SetActive(true);
                idx++;
            }
        }
        else
        {
            // draw Y axis
            drawYAxis();

            // draw X axis
            drawXAxis();

            // draw View
            barHeights = new Dictionary<string, float>();
            barXPosition = new Dictionary<string, float>();
            GameObject view = transform.GetChild(2).gameObject;
            GameObject BarWrap = view.transform.GetChild(0).gameObject;
            int idx = 0;
            foreach (string s in attr.Keys)
            {
                Debug.Assert(attr[s].Count == 1);
                GameObject bar = Instantiate(BarWrap, Vector3.zero, Quaternion.identity);
                bar.name = s;
                bar.transform.parent = view.transform;
                bar.transform.localPosition = new Vector3(XLinspace[idx], 0, 0);
                bar.transform.localScale = new Vector3(1f / numAttr, attr[s][0] / (StepL * StepC), BarWrap.transform.localScale.z);
                bar.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[0];
                bar.SetActive(true);
                barHeights.Add(s, attr[s][0] / (StepL * StepC));
                barXPosition.Add(s, XLinspace[idx]);
                idx++;
            }
        }
    }

    public void AddBarMoveComponent(List<Transform> target)
    {
        barAnimation = new List<BarMoveAnimation>();
        GameObject view = transform.GetChild(2).gameObject;
        Debug.Assert(target.Count + 1 == view.transform.childCount);
        for (int i = 0; i < target.Count; i++)
        {
            Debug.Log(target[i].gameObject.name);
            Transform BarWrapTransform = view.transform.Find(target[i].gameObject.name);
            Debug.Assert(BarWrapTransform != null);
            GameObject BarWrap = BarWrapTransform.gameObject;
            BarMoveAnimation animation = BarWrap.AddComponent<BarMoveAnimation>();
            animation.BarChart = this;
            animation.TargetTransform = target[i].Find("Label");
            barAnimation.Add(animation);
        }
    }

    public void Compose()
    {
        foreach (BarMoveAnimation animation in barAnimation)
        {
            animation.SetToTargetPosition(0.005f, barHeights[animation.name]);
        }
        transform.parent.parent.gameObject.SetActive(false);
    }

    public void Decompose(GameObject currentInteractor)
    {
        MovingTarget.GetComponent<ThematicMapObj>().ResetLabelHight();
        GameObject interactable = transform.parent.parent.gameObject;
        currentInteractor.GetComponent<InteractorFacade>().Ungrab();
        interactable.gameObject.SetActive(true);
        foreach (BarMoveAnimation animation in barAnimation)
        {
            animation.SetToBarChartView(1f / numAttr, barHeights[animation.name], barXPosition[animation.name]);
        }
        interactable.GetComponent<InteractableTest>().SetGrabOffset(0);
        interactable.GetComponent<InteractableTest>().interactable.Grab(currentInteractor);
    }
}
