using Assets.Scripts.MonoBehaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarMoveAnimation : MonoBehaviour
{
    public StackedBarDraw BarChart;
    public Transform TargetTransform;

    private InteractableTest interactableStatus;
    private BoxCollider boxCollider;
    private bool inMap;

    // Start is called before the first frame update    
    void Start()
    {
        interactableStatus = GetComponent<InteractableTest>();
        Debug.Assert(interactableStatus.leftInteractor != null);
        Debug.Assert(interactableStatus.rightInteractor != null);
        interactableStatus.MovingThreshold = 1f;
        boxCollider = GetComponent<BoxCollider>();
        Debug.Assert(boxCollider != null);
        Debug.Assert(!boxCollider.enabled);
        // Only the upper half is trigger
        boxCollider.center = new Vector3(0, 0.75f, 0);
        boxCollider.size = new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetToTargetPosition(float barWidth, float barHeight)
    {
        Vector3 originalLocalScale = new Vector3(barWidth, barHeight, barWidth);
        Vector3 newLocalPosition = new Vector3(TargetTransform.localPosition.x, TargetTransform.localPosition.y, RenderingSettings.MinZOffset);
        Quaternion newRotation = Quaternion.Euler(90, 0, 0);
        StartCoroutine(AnimateTransform(TargetTransform.parent, newLocalPosition, originalLocalScale, newRotation, 1f, true));
    }

    public void SetToBarChartView(float barWidth, float barHeight, float barXPosition)
    {
        Vector3 originalLocalScale = new Vector3(barWidth, barHeight, 0.001f);
        Vector3 newLocalPosition = new Vector3(barXPosition, 0, 0);
        Quaternion newRotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(AnimateTransform(BarChart.transform.GetChild(2), newLocalPosition, originalLocalScale, newRotation, 0.5f, false));
    }

    private IEnumerator AnimateTransform(Transform newParent, Vector3 newLocalPosition, Vector3 newLocalScale, Quaternion newLocalRotation, float duration, bool isCompose)
    {
        float time = 0;
        transform.parent = newParent;
        Vector3 originalLocalPosition = transform.localPosition;
        Vector3 currentLocalScale = transform.localScale;
        Quaternion originalRotation = transform.localRotation;
        if (!isCompose)
        {
            boxCollider.enabled = false;
            inMap = false;
        }
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalLocalPosition, newLocalPosition, time / duration);
            transform.localScale = Vector3.Lerp(currentLocalScale, newLocalScale, time / duration);
            transform.localRotation = Quaternion.Lerp(originalRotation, newLocalRotation, time / duration);
            yield return null;
        }
        if (isCompose)
        {
            boxCollider.enabled = true;
            inMap = true;
        }
        if (!isCompose)
        {
            MeshRenderer meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            Color color = meshRenderer.material.color;
            color.a = 1f;
            meshRenderer.material.SetColor("_Color", color);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger " + name);
        MeshRenderer meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        Color color = meshRenderer.material.color;
        color.a = 0.5f;
        meshRenderer.material.SetColor("_Color", color);
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (inMap && interactableStatus.isMoving && interactableStatus.currentAttachInteractor != null)
        {
            Debug.Log("Grab " + name);
            inMap = false;
            BarChart.Decompose(interactableStatus.currentAttachInteractor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MeshRenderer meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        Color color = meshRenderer.material.color;
        color.a = 1f;
        meshRenderer.material.SetColor("_Color", color);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide " + name);
    }
}
