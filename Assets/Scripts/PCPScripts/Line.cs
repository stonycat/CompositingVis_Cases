using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Tilia.Interactions.Interactables.Interactables;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

public class Line : ParallelCoordinatePlot
{
    public ParallelCoordinatePlot PCP;
    public PCPView parentView;
    public int level;
    public Transform startObj;
    public Transform endObj;
    public Material material;
    public Material selectedMaterial;
    public bool IsGrabbed;
    public GameObject point;
    public bool isCurve;

    private LineRenderer lineRenderer;
    private Vector4 curve;
    private bool isReadyToCurve
    {
        get
        {
            return isReadyToCurveInner;
        }
        set
        {
            if (!isReadyToCurveInner && value)
            {
                parentView.CreateViewPlot();
            } else if (isReadyToCurve && !value)
            {
                isCurve = false;
                point.Destroy();
            }
            isReadyToCurveInner = value;
        }
    }
    private float t;
    private bool isReadyToCurveInner;

    private const int resolution = 100;
    private const float duration = 1f;

    public void SetEnds(Transform startObj, Transform endObj)
    {
        this.startObj = startObj;
        this.endObj = endObj;
        IsGrabbed = false;
        isReadyToCurve = false;
        isCurve = false;
    }

    public void SetLineRenderer(LineRenderer lineRenderer)
    {
        this.lineRenderer = lineRenderer;
        this.lineRenderer.material = material;
        this.lineRenderer.startWidth = 0.001f;
        this.lineRenderer.endWidth = 0.001f;
        //point = new GameObject(gameObject.name + "_scatterpoint"); //Instantiate(PCP.ScatterPrefab, Vector3.zero, Quaternion.identity);
        //point.transform.parent = transform.parent;
        //Vector3 startObjPos = transform.parent.InverseTransformPoint(startObj.position);
        //point.transform.localPosition = startObjPos;
        //point.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //point.SetActive(false);
    }

    private float f(Vector4 par, float t)
    {
        return par.x * t * t * t + par.y * t * t + par.z * t + par.w;
    }

    public void RenderLine()
    {
        if (!isCurve)
        {
            if (!isReadyToCurve)
            {
                lineRenderer.material = material;
            }
            else
            {
                lineRenderer.material = selectedMaterial;
            }
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startObj.position);
            lineRenderer.SetPosition(1, endObj.position);
        } else
        {
            lineRenderer.material = material;
            AnimateCurve();
        }
    }

    private void AnimateCurve()
    {
        if (t < duration) t += Time.deltaTime; else t = duration;
        NumericAxisBound pointXY = PCP.GetCurveParameters(this, t / duration);
        lineRenderer.positionCount = resolution;
        float x1 = transform.parent.InverseTransformPoint(startObj.position).x;
        float x2 = transform.parent.InverseTransformPoint(endObj.position).x;
        if (endObj.localPosition.y < 1)
        {
            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / resolution;
                float y = f(curve, (1 - t) * x1 + t * x2);
                if (y > 5 || y < -5)
                {
                    lineRenderer.positionCount = 0;
                    break;
                }
                Vector3 pos = transform.parent.TransformPoint(new Vector3((1 - t) * x1 + t * x2, y, 0));
                lineRenderer.SetPosition(i, pos);
            }
            point.transform.localPosition = new Vector3(pointXY.Max, pointXY.Min, 0);
            point.SetActive(true);
        } else
        {
            lineRenderer.positionCount = 0;
        }
    }

    public void GetCurve(float x1, float x2, float p, float fx1, float fx2, float fp, float dfp)
    {
        if (fx1 == 0 && fx2 == 0)
        {
            curve = Vector4.zero;
            return;
        }
        Vector4 row1 = new Vector4(3 * p * p, 2 * p, 1, 0);
        Vector4 row2 = new Vector4(p * p * p, p * p, p, 1);
        Vector4 row3 = new Vector4(x1 * x1 * x1, x1 * x1, x1, 1);
        Vector4 row4 = new Vector4(x2 * x2 * x2, x2 * x2, x2, 1);
        Matrix4x4 mat = new Matrix4x4(row1, row2, row3, row4);
        Vector4 vec = new Vector4(dfp, fp, fx1, fx2);
        curve = mat.transpose.inverse * vec;
    }

    // Start is called before the first frame update
    void Start()
    {
        curve = Vector4.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsGrabbed)
        {
            float x1 = transform.parent.InverseTransformPoint(startObj.position).x;
            float x2 = transform.parent.InverseTransformPoint(endObj.position).x;
            t = (Mathf.Abs(x1 - x2) > level * PCP.GetAxesDistance() + 0.03f && !isReadyToCurve) ? 0 : t;
            isReadyToCurve = (Mathf.Abs(x1 - x2) > level * PCP.GetAxesDistance() + 0.03f);
        }
    }
}
