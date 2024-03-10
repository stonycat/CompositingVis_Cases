using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PCPView : MonoBehaviour
{
    
    public GameObject ScatterPlotInteractableTemplate;
    public GameObject ScatterPrefab;
    public GameObject leftAxis;
    public GameObject rightAxis;
    public Dictionary<string, GameObject> points;
    public Dictionary<string, Line> lines;

    private GameObject ScatterPlotInteractable;
    private GameObject ScatterPlot;
    private int numPoints
    {
        get
        {
            return numPointsInner;
        }
        set
        {
            if (numPointsInner > 0 && value == 0)
            {
                ScatterPlotInteractable.Destroy();
                foreach (var (name, line) in lines)
                {
                    line.point = points[name];
                    line.isCurve = true;
                }
                points.Clear();
                plotCreated = false;
            }
            numPointsInner = value;
        }
    }

    private int numPointsInner;
    private bool plotCreated;

    public void CreateViewPlot()
    {
        if (plotCreated) { return; }
        ScatterPlotInteractable = Instantiate(ScatterPlotInteractableTemplate);
        ScatterPlotInteractable.transform.parent = transform;
        ScatterPlotInteractable.transform.localPosition = new Vector3(0, 1.2f, 0);
        ScatterPlotInteractable.transform.localRotation = Quaternion.identity;
        ScatterPlot = ScatterPlotInteractable.transform.GetChild(0).GetChild(1).gameObject;
        GameObject ScatterPlotYAxis = Instantiate(leftAxis);
        ScatterPlotYAxis.transform.parent = ScatterPlot.transform;
        ScatterPlotYAxis.transform.localPosition = Vector3.zero;
        ScatterPlotYAxis.transform.localRotation = Quaternion.identity;
        GameObject ScatterPlotXAxis = Instantiate(rightAxis);
        ScatterPlotXAxis.transform.parent = ScatterPlot.transform;
        ScatterPlotXAxis.transform.localPosition = Vector3.zero;
        ScatterPlotXAxis.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, 180, 90);// * Quaternion.Euler(0, 0, 90);
        ResetAxisLabel(ScatterPlotXAxis);
        ScatterPlotInteractable.transform.localScale = 0.5f * Vector3.one;
        DrawPoints(ScatterPlotXAxis, ScatterPlotYAxis);
        ScatterPlotInteractable.SetActive(true);
        plotCreated = true;
    }

    private void DrawPoints(GameObject ScatterPlotXAxis, GameObject ScatterPlotYAxis)
    {
        Transform attrLabelX = ScatterPlotXAxis.transform.Find("AxisLabels");
        Transform attrLabelY = ScatterPlotYAxis.transform.Find("AxisLabels");
        Dictionary<string, float> pointX = new Dictionary<string, float>();
        for (int i = 0; i < attrLabelX.childCount; i++)
        {
            Transform child = attrLabelX.GetChild(i);
            if (child.name.Contains("BaseLabel")) continue;
            pointX.Add(child.name, child.localPosition.y);
        }
        for (int i = 0; i < attrLabelY.childCount;i++)
        {
            Transform child = attrLabelY.GetChild(i);
            if (child.name.Contains("BaseLabel")) continue;
            GameObject point = Instantiate(ScatterPrefab);
            point.name = child.name;
            point.transform.parent = ScatterPlot.transform;
            point.transform.localPosition = new Vector3(pointX[child.name], child.localPosition.y, 0);
            point.transform.localScale = 0.02f * Vector3.one;
            point.SetActive(true);
            points.Add(point.name, point);
        }
        numPoints = points.Count;
    }

    private void ResetAxisLabel(GameObject axis)
    {
        Transform axisTransfrom = axis.transform;
        Transform attrLabel = axisTransfrom.Find("AttributeLabel");
        attrLabel.localRotation *= Quaternion.Euler(0, 180, 180);
        attrLabel.localPosition -= new Vector3(0.05f, 0, 0);
        Transform axisLabel = axisTransfrom.Find("AxisLabels");
        for (int i = 0; i < axisLabel.childCount; i++)
        {
            Transform child = axisLabel.GetChild(i);
            if (child.name != "BaseLabel(Clone)") continue;
            Transform text = child.GetChild(0);
            text.localRotation *= Quaternion.Euler(0, 180, 90);
            text.localPosition -= new Vector3(0.02f, -0.045f, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PCPGeneratedPlot" && !other.transform.parent.parent.GetComponent<InteractableTest>().interactable.IsGrabbed)
        {
            foreach (GameObject point in  points.Values)
            {
                StartCoroutine(AnimateToAxis(point));
            }
        }
    }

    private IEnumerator AnimateToAxis(GameObject point)
    {
        Transform scatterTransform = leftAxis.transform.GetChild(3).Find(point.name);
        point.transform.parent = transform;
        Vector3 originalPos = point.transform.localPosition;
        Vector3 targetPos = transform.InverseTransformPoint(scatterTransform.position);
        float time = 0;
        float duration = 0.5f;
        while (time < duration)
        {
            time += Time.deltaTime;
            point.transform.localPosition = Vector3.Lerp(originalPos, targetPos, time / duration);
            yield return null;
        }
        numPoints--;
    }

    // Start is called before the first frame update
    void Start()
    {
        plotCreated = false;
        points = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Line line in lines.Values)
        {
            line.RenderLine();
        }
    }
}
