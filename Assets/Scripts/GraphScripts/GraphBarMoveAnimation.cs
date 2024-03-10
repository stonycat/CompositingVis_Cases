using Assets.Scripts.MonoBehaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphBarMoveAnimation : MonoBehaviour
{
    public StackedBarDraw BarChart;
    public Transform TargetTransform;
    public void SetToTargetTransform(Vector3 newLocalScale, Node InNode)
    {
        Vector3 newLocalPosition = new Vector3(0, 0, 0);
        StartCoroutine(AnimateTransform(TargetTransform, newLocalPosition, newLocalScale, 2f, InNode));
    }

    public void SetToChartTransfrom(float barWidth, float barHeight, float barXPosition, float barYPosition)
    {
        Vector3 originalLocalScale = new Vector3(barWidth, barHeight, 0.001f);
        Vector3 newLocalPosition = new Vector3(barXPosition, barYPosition, 0);
        gameObject.SetActive(true);
        StartCoroutine(AnimateTransform(BarChart.transform.GetChild(2), newLocalPosition, originalLocalScale, 0.5f, null));
    }

    private IEnumerator AnimateTransform(Transform newParent, Vector3 newLocalPosition, Vector3 newLocalScale, float duration, Node InNode)
    {
        float time = 0;
        transform.parent = newParent;
        Vector3 originalLocalPosition = transform.localPosition;
        Vector3 currentLocalScale = transform.localScale;
        if (InNode != null)
        {
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(originalLocalPosition, newLocalPosition, time / duration);
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                yield return null;
            }
            InNode.SetObj();
            gameObject.SetActive(false);
        } else
        {
            Quaternion currentLocalRotation = transform.localRotation;
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(originalLocalPosition, newLocalPosition, time / duration);
                transform.localScale = Vector3.Lerp(currentLocalScale, newLocalScale, time / duration);
                transform.localRotation = Quaternion.Lerp(currentLocalRotation, Quaternion.identity, time / duration);
                yield return null;
            }
        }
    }
}
