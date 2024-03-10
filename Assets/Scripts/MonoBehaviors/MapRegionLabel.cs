using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors
{
    [RequireComponent(typeof(TextMeshPro))]
    public class MapRegionLabel : MonoBehaviour
    {
        private TextMeshPro textScript;
        public float RawHeight;

        private Color thisBlue = new Color(0.2156862f, 0.4941176f, 0.7215686f);
        private Color thisGreen = new Color(0.3019608f, 0.6862745f, 0.2901961f);

        private void Start()
        {
            
        }

        public void HighlightNone()
        {
            this.textScript.color = Color.black;
        }


        public void HighlightBlue()
        {
            //this.gameObject.GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("OUTLINE_OFF");
            this.textScript.color = this.thisBlue;
        }

        public void HighlightGreen()
        {
            this.textScript.color = this.thisGreen;
        }

        private void Awake()
        {
            if(this.textScript == null)
            {
                this.textScript = this.gameObject.GetComponent<TextMeshPro>();
                this.textScript.text = "AA";
                this.textScript.fontSize = 0.3f;
                this.textScript.color = Color.black;
                this.textScript.alignment = TextAlignmentOptions.BottomGeoAligned;
                
                //this.textScript.outlineColor = Color.black;
                //this.gameObject.GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("OUTLINE_ON");
                //this.gameObject.GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("OUTLINE_OFF");

                var rect = this.gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0.08f, 0.03f);
                rect.pivot = new Vector2(0.5f, 0);
            }
        }

        public void DrawLabel(string content, Vector3 p, float height, float scale = 1, float outter = 0.1f)
        {
            this.RawHeight = height;
            this.textScript.text = content;

            p.y -= (this.textScript.fontSize / 10 / 2 * scale);
            this.gameObject.transform.localPosition = p;

            this.SetOffset(RenderingSettings.MinZOffset * (1 / (this.gameObject.transform.lossyScale.z / 0.3f)) * 4);

            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            this.textScript.outlineWidth = 0;
        }
        
        public void ChangeHeight(float height)
        {
            this.RawHeight = height;
            this.SetOffset(RenderingSettings.MinZOffset);
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
            if (this.textScript == null) return;
            if (Camera.main == null) return;

            this.textScript.transform.LookAt(this.textScript.transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
        }
    }
}
