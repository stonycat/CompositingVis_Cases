using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class StackedBarDraw : MonoBehaviour
{
    public TextAsset data;
    public Material[] materials;

    private int numAttr;
    private int numNode;
    private int StepC;
    private int StepL;
    private Dictionary<string, List<int>> attr;
    private List<float> XLinspace;

    private const float MinX = -0.54f;
    private const float MaxX = 0.44f;
    StackedBarDraw()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        ProcessData();
        Draw();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadData()
    {
        if (data == null) return;
        attr = new Dictionary<string, List<int>>();
        string[] lines = data.text.Split("\n");
        numAttr = int.Parse(lines[0].Trim());
        numNode = int.Parse(lines[1].Trim());
        for (int i = 0; i < numAttr; i++)
        {
            string attrName = lines[i * (numNode + 1) + 2].Trim();
            attr.Add(attrName, new List<int>());
            for (int j = 1; j <= numNode; j++)
            {
                string v = lines[i * (numNode + 1) + j + 2].Trim();
                attr[attrName].Add(int.Parse(v));
            }
        }
        XLinspace = new List<float>(numNode);
        Debug.Assert(XLinspace.Capacity == numNode);
        for (int i = 0; i < numNode; i++)
        {
            XLinspace.Add(MinX + (MaxX - MinX) * i / numNode);
        }
    }

    private void ProcessData()
    {
        int largest = 0;
        for (int i = 0; i < numNode; i++)
        {
            int total = 0;
            foreach (string s in attr.Keys)
            {
                total += attr[s][i];
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

    private void Draw()
    {
        if (data == null) return;
        // draw Y axis
        GameObject y = transform.GetChild(1).gameObject;
        GameObject yAxisLabels = y.transform.GetChild(3).gameObject;
        GameObject BaseLabel = yAxisLabels.transform.GetChild(0).gameObject;
        for (int i = 0; i <= StepC; i++)
        {
            GameObject label = Instantiate(BaseLabel, Vector3.zero, Quaternion.identity);
            label.transform.parent = yAxisLabels.transform;
            label.transform.localPosition = new Vector3(0, i / (float)StepC, 0);
            label.transform.GetChild(0).GetComponent<TextMeshPro>().text = (i * StepL).ToString();
            label.SetActive(true);
        }

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
                bar.transform.localScale = new Vector3(BarWrap.transform.localScale.x, (float) attr[s][i] / (StepL * StepC), BarWrap.transform.localScale.z);
                bar.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[idx];
                bar.SetActive(true);
                idx++;
            }
        }
    }
}
