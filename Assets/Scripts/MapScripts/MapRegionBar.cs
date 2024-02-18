using Assets.Scripts.MonoBehaviors;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapRegionBar : MonoBehaviour
{
    private GameObject bar;
    public float RawHeight;

    private Color thisBlue = new Color(0.2156862f, 0.4941176f, 0.7215686f);
    private Color thisGreen = new Color(0.3019608f, 0.6862745f, 0.2901961f);

    private void Start()
    {

    }

    public void DrawBar(GameObject originalBar, Vector3 p, float height, float scale = 1, float outter = 0.1f)
    {
        this.RawHeight = height;
        this.bar = Instantiate(originalBar, Vector3.zero, Quaternion.identity);

        //p.y -= (this.textScript.fontSize / 10 / 2 * scale);
        this.gameObject.transform.localPosition = p;

        this.SetOffset(RenderingSettings.MinZOffset * (1 / (this.gameObject.transform.lossyScale.z / 0.3f)) * 4);

        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        //this.textScript.outlineWidth = 0;
    }

    public void SetOffset(float offset)
    {
        //Debug.Log("=====================");
        //Debug.Log(offset);
        //Debug.Log(this.gameObject.transform.lossyScale.z);

        var p = this.gameObject.transform.localPosition;
        p.z = RawHeight + offset;
        this.gameObject.transform.localPosition = p;
    }

    private void Update()
    {
        //if (this.textScript == null) return;
        //if (Camera.main == null) return;

        //this.textScript.transform.LookAt(this.textScript.transform.position + Camera.main.transform.rotation * Vector3.forward,
        //        Camera.main.transform.rotation * Vector3.up);
    }
}
