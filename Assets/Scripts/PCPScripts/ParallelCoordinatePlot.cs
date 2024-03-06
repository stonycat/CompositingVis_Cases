using Mapbox.Directions;
using Mapbox.Examples.Voxels;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using Zinnia.Data.Type;
using static UnityEngine.GraphicsBuffer;

public class ParallelCoordinatePlot : MonoBehaviour
{
    public TextAsset DataSource;
    public GameObject ControllableAxisPrefab;
    public GameObject ScatterPrefab;
    public GameObject ScatterPlotInteractableTemplate;
    public Material LineMaterial;
    public Material LineSelectedMaterial;

    private Dictionary<string, List<float>> data;
    private List<Transform> axesObj;
    //private List<List<Line>> lines;
    private List<PCPView> views;
    private string itemName;

    private const float AxesDistanceScalingFactor = 0.5f;
    private const float AxisLabelScalingFactor = 0.98f;
    public class Coordinate
    {
        public float x, y;
        public Coordinate(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Coordinate()
        {

        }
    }
    public class NumericAxisBound
    {
        public float Max = -Mathf.Infinity;
        public float Min = Mathf.Infinity;

        public NumericAxisBound(float max, float min)
        {
            Max = max;
            Min = min;
        }
        public NumericAxisBound()
        {

        }
    }
    private class NumericAxisSteps
    {
        public int StepL = 0;
        public int StepC = 0;
    }
    private Dictionary<string, NumericAxisBound> numericAxisBounds;
    private Dictionary<string, int> axesIds;
    private GrabbingAxes grabManager;
    // Start is called before the first frame update
    void Start()
    {
        data = new Dictionary<string, List<float>>();
        numericAxisBounds = new Dictionary<string, NumericAxisBound>();
        axesObj = new List<Transform>();
        //lines = new List<List<Line>>();
        views = new List<PCPView>();
        axesIds = new Dictionary<string, int>();
        grabManager = new GrabbingAxes();
        grabManager.parent = this;
        LoadDataFromCSV(DataSource);
        DrawItemAxis(itemName);
        DrawOtherAxes();
        DrawLines();
    }

    private class GrabbingAxes : ParallelCoordinatePlot
    {
        public ParallelCoordinatePlot parent;
        public GameObject LeftAxis
        {
            get { return leftAxis; }
            set
            {
                leftAxis = value;
                if (value != null)
                {
                    leftId = parent.axesIds[leftAxis.name];
                    Num++;
                }
                else
                {
                    Num--;
                }
            }
        }
        public GameObject RightAxis
        {
            get { return rightAxis; }
            set
            {
                rightAxis = value;
                if (value != null)
                {
                    rightId = parent.axesIds[rightAxis.name];
                    Num++;
                }
                else
                {
                    Num--;
                }
            }
        }

        private GameObject leftAxis;
        private GameObject rightAxis;
        private int leftId;
        private int rightId;
        private int Num
        {
            get
            {
                return num;
            }
            set
            {
                if (value >= 2)
                {
                    parent.LineToCurveBetween(leftId, rightId, false);
                }
                else if (num >= 2 && value < 2)
                {
                    parent.LineToCurveBetween(leftId, rightId, true);
                }
                num = value;
            }
        }
        private int num;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //foreach (List<Line> view in lines)
        //{
        //    foreach (Line line in view)
        //    {
        //        line.RenderLine();
        //    }
        //}
    }

    private void LoadDataFromCSV(TextAsset source)
    {
        if (source == null) return;
        string[] lines = source.text.Split('\n');
        string[] headline = lines[0].Split(',');
        itemName = headline[0];
        for (int i = 1; i < headline.Length; i++)
        {
            numericAxisBounds[headline[i]] = new NumericAxisBound();
        }
        foreach (string line in new ArraySegment<string>(lines, 1, lines.Length - 1))
        {
            if (line.Trim().Length == 0) continue;
            string[] item = line.Trim().Split(',');
            data.Add(item[0], new List<float>());
            int idx = 1;
            while (idx < item.Length)
            {
                float val = float.Parse(item[idx]);
                data[item[0]].Add(val);
                if (val > numericAxisBounds[headline[idx]].Max)
                {
                    numericAxisBounds[headline[idx]].Max = val;
                }
                if (val < numericAxisBounds[headline[idx]].Min)
                {
                    numericAxisBounds[headline[idx]].Min = val;
                }
                idx++;
            }
        }
    }

    private void DrawItemAxis(string itemName)
    {
        DrawFactorAxis(itemName, data.Keys.ToList(), 0);
        axesIds.Add(itemName, 0);
    }

    private void DrawOtherAxes()
    {
        int x = 1;
        foreach (var (key, val) in numericAxisBounds)
        {
            NumericAxisSteps steps = DrawNumericAxis(key, val, x * AxesDistanceScalingFactor);
            DrawDataPoints(x, steps);
            axesIds.Add(key, x);
            x += 1;
        }

    }

    private void DrawDataPoints(int x, NumericAxisSteps steps)
    {
        Transform axisTransfrom = axesObj[x];
        Transform axisLabelsTransform = axisTransfrom.GetChild(3);
        foreach (var (itemName, stats) in data)
        {
            GameObject point = new GameObject(itemName);
            point.transform.parent = axisLabelsTransform;
            point.transform.localPosition = new Vector3(0, stats[x - 1] * AxisLabelScalingFactor / (steps.StepL * steps.StepC), 0);
        }
    }

    private void DrawFactorAxis(string AxisName, List<string> AxisLabels, float x)
    {
        GameObject itemAxis = Instantiate(ControllableAxisPrefab, Vector3.zero, Quaternion.identity);
        itemAxis.name = AxisName;
        itemAxis.transform.parent = transform;
        itemAxis.transform.localPosition = new Vector3(x, 0, 0);
        Transform axisTransform = itemAxis.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        axesObj.Add(axisTransform);
        GameObject attrLabel = axisTransform.GetChild(2).gameObject;
        Debug.Assert(attrLabel.GetComponent<TextMeshPro>() != null);
        attrLabel.GetComponent<TextMeshPro>().SetText(AxisName);
        GameObject axisLabelsObj = axisTransform.GetChild(3).gameObject;
        GameObject BaseLabel = axisLabelsObj.transform.GetChild(0).gameObject;
        int len = AxisLabels.Count;
        Debug.Log(len);
        for (int i = 0; i < len; i++)
        {
            GameObject label = Instantiate(BaseLabel, BaseLabel.transform.position, BaseLabel.transform.rotation);
            label.transform.parent = axisLabelsObj.transform;
            label.transform.localPosition = new Vector3(0, (float)i * AxisLabelScalingFactor / len, 0);
            label.transform.GetChild(0).GetComponent<TextMeshPro>().text = AxisLabels[i];
            label.SetActive(true);
            label.name = AxisLabels[i];
        }
        itemAxis.SetActive(true);
    }

    private NumericAxisSteps GetAxisSteps(NumericAxisBound bound)
    {
        // each list element should be nonnegative
        float max = bound.Max;
        if (bound.Min < 0)
        {
            throw new ArgumentOutOfRangeException("List element should be non-negative but the current min is " + bound.Min);
        }
        float mag = Mathf.Floor(Mathf.Log10(max));
        NumericAxisSteps step = new NumericAxisSteps();
        step.StepL = (int)Mathf.Pow(10, mag);
        step.StepC = (int)Mathf.Ceil(max / step.StepL);
        return step;
    }

    private NumericAxisSteps DrawNumericAxis(string AxisName, NumericAxisBound bound, float x)
    {
        NumericAxisSteps steps = GetAxisSteps(bound);
        GameObject itemAxis = Instantiate(ControllableAxisPrefab, Vector3.zero, Quaternion.identity);
        itemAxis.name = AxisName;
        itemAxis.transform.parent = transform;
        itemAxis.transform.localPosition = new Vector3(x, 0, 0);
        Transform axisTransform = itemAxis.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1);
        axesObj.Add(axisTransform);
        GameObject attrLabel = axisTransform.GetChild(2).gameObject;
        Debug.Assert(attrLabel.GetComponent<TextMeshPro>() != null);
        attrLabel.GetComponent<TextMeshPro>().SetText(AxisName);
        GameObject axisLabelsObj = axisTransform.GetChild(3).gameObject;
        GameObject BaseLabel = axisLabelsObj.transform.GetChild(0).gameObject;
        for (int i = 0; i <= steps.StepC; i++)
        {
            GameObject label = Instantiate(BaseLabel, BaseLabel.transform.position, BaseLabel.transform.rotation);
            label.transform.parent = axisLabelsObj.transform;
            label.transform.localPosition = new Vector3(0, (float)i * AxisLabelScalingFactor / steps.StepC, 0);
            label.transform.GetChild(0).GetComponent<TextMeshPro>().text = (i * steps.StepL).ToString();
            label.SetActive(true);
        }
        itemAxis.SetActive(true);
        return steps;
    }

    private void DrawLines()
    {
        for (int i = 1; i < axesObj.Count; i++)
        {
            Transform itemAxisTicks1 = axesObj[i - 1].GetChild(3);
            Transform itemAxisTicks2 = axesObj[i].GetChild(3);
            GameObject view = new GameObject(transform.GetChild(i).name + "-" + transform.GetChild(i + 1).name);
            BoxCollider bc = view.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.center = new Vector3(0, 0.5f, 0);
            bc.size = new Vector3(AxesDistanceScalingFactor, 1, 0.01f);
            view.transform.parent = transform;
            float x = (transform.GetChild(i).localPosition.x + transform.GetChild(i + 1).localPosition.x) / 2;
            view.transform.localPosition = new Vector3(x, 0, 0);
            PCPView viewComp = view.AddComponent<PCPView>();
            viewComp.ScatterPlotInteractableTemplate = ScatterPlotInteractableTemplate;
            viewComp.ScatterPrefab = ScatterPrefab;
            viewComp.leftAxis = axesObj[i - 1].gameObject;
            viewComp.rightAxis = axesObj[i].gameObject;
            viewComp.lines = new Dictionary<string, Line>();
            //lines.Add(new List<Line>());
            foreach (var (item, stats) in data)
            {
                GameObject line = new GameObject(item);
                line.transform.parent = view.transform;
                Line l = line.AddComponent<Line>();
                LineRenderer lr = line.AddComponent<LineRenderer>();
                Debug.Assert(itemAxisTicks1.Find(item) != null);
                Debug.Assert(itemAxisTicks2.Find(item) != null);
                l.SetEnds(itemAxisTicks1.Find(item), itemAxisTicks2.Find(item));
                l.material = LineMaterial;
                l.selectedMaterial = LineSelectedMaterial;
                l.ScatterPrefab = ScatterPrefab;
                l.parentView = viewComp;
                l.PCP = this;
                l.level = 1;
                l.SetLineRenderer(lr);
                l.RenderLine();
                //lines[i - 1].Add(l);
                viewComp.lines.Add(item, l);
            }
            views.Add(viewComp);
        }
    }

    public void OnAxisGrabbed(GameObject grabbedAxis)
    {
        if (grabManager.LeftAxis == null)
        {
            grabManager.LeftAxis = grabbedAxis;
        }
        else if (grabManager.RightAxis == null)
        {
            grabManager.RightAxis = grabbedAxis;
        }
    }
    public void OnAxisUngrabbed(GameObject grabbedAxis)
    {
        if (grabManager.LeftAxis == grabbedAxis)
        {
            grabManager.LeftAxis = null;
        }
        else if (grabManager.RightAxis == grabbedAxis)
        {
            grabManager.RightAxis = null;
        }
    }

    public NumericAxisBound GetCurveParameters(Line line, float c)
    {
        const float scaling = 0.7f;
        float x1 = line.transform.parent.InverseTransformPoint(line.startObj.position).x;
        float x2 = line.transform.parent.InverseTransformPoint(line.endObj.position).x;
        float fx1 = line.transform.parent.InverseTransformPoint(line.startObj.position).y;
        float fx2 = line.transform.parent.InverseTransformPoint(line.endObj.position).y;
        float p = (fx2 < 1) ? (fx2 * c * x2 + (1 - fx2 * c) * x1) : scaling * (x2 * c + x1 * (1 - c));
        //float p = (x1 + x2) / 2;
        float fp = (fx2 < 1) ? fx1 : (1 - (1 - fx1) * 0.5f);
        float dfp = (fx1 - fx2) * c / (x1 - x2);
        line.GetCurve(x1, x2, p, fx1, fx2, fp, dfp);
        return new NumericAxisBound(p, fp);
    }

    private void LineToCurveBetween(int leftId, int rightId, bool visibility)
    {
        if (leftId == rightId)
        {
            return;
        }
        if (leftId > rightId)
        {
            int temp = leftId;
            leftId = rightId;
            rightId = temp;
        }
        if (rightId - leftId == 1)
        {
            foreach (Line line in views[leftId].lines.Values)
            {
                line.IsGrabbed = !visibility;
            }
        }
        else
        {
            // MDS - abandoned feature
            //Matrix<double> dataMtx = Matrix<double>.Build.Dense(data.Keys.Count, rightId - leftId);
            //for (int i = 0; i < rightId - leftId; i++)
            //{
            //    int j = 0;
            //    foreach (string key in data.Keys)
            //    {
            //        dataMtx[j, i] = data[key][i + leftId];
            //        j++;
            //    }
            //}
            //ClassicalMDS.ClassicalMDS cmds = new ClassicalMDS.ClassicalMDS(1);
            //var res = cmds.FitTransform(dataMtx, "euclidean");
            //Debug.Log("log matrix " + res);
        }
    }

    public float GetAxesDistance()
    {
        return AxesDistanceScalingFactor;
    }
}
