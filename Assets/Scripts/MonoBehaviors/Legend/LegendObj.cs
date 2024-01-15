using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.MonoBehaviors.Legend
{
    public class LegendObj : MonoBehaviour
    {
        public enum LegendPosition
        {
            Top,
            Bottom,
            Left,
            Right
        }

        public LegendPosition MountingPosition;

        public ThematicMapObj MapObj;
        public Material GradientM;

        public GameObject ColorStripObj;
        public GameObject OtherVisualObj;

        public GameObject FlatTextObj;
        public GameObject ElevatedTextObj;

        private void Update()
        {
            if (this.gameObject.activeSelf == false) return;
            if (Camera.main == null) return;
            if (MapObj.MapType == ThematicType.Choropleth) return;

            var cameraP = MapObj.gameObject.transform.InverseTransformPoint(Camera.main.transform.position);
            cameraP.z = 0;

            var thisP = MapObj.gameObject.transform.InverseTransformPoint(gameObject.transform.position);
            thisP.z = 0;

            var direction = (cameraP - thisP).normalized;
            var angle = Vector3.SignedAngle(new Vector3(1, 0, 0), direction, new Vector3(0, 0, 1));
            gameObject.transform.localEulerAngles = new Vector3(0, 0, angle + 90);
        }

        private void Start()
        {
            float topMargin = 0, bottomMargin = 0, leftMargin = 0, rightMargin = 0;
            switch(MapObj.GeoName)
            {
                case GeoName.UK:
                    topMargin = 0.55f;
                    bottomMargin = -0.55f;

                    leftMargin = 0.35f;
                    rightMargin = -0.35f;
                    break;
                case GeoName.US:
                    topMargin = 0.4f;
                    bottomMargin = -0.4f;

                    leftMargin = 0.55f;
                    rightMargin = -0.55f;
                    break;
            }
            

            switch (this.MountingPosition)
            {
                case LegendPosition.Top:
                    this.transform.localPosition = new Vector3(0, topMargin, 0);
                    break;
                case LegendPosition.Bottom:
                    this.transform.localPosition = new Vector3(0, bottomMargin, 0);
                    break;

                case LegendPosition.Left:
                    this.transform.localPosition = new Vector3(leftMargin, 0, 0);
                    break;
                case LegendPosition.Right:
                    this.transform.localPosition = new Vector3(rightMargin, 0, 0);
                    break;
            }
            //this.transform.localPosition += new Vector3(0, 1, 1);
            //Quaternion rotation = Quaternion.Euler(new Vector3(180, 0, 0));
            //this.transform.localRotation = rotation;

            this.InitialVisualObj();

            var textRenderers = ElevatedTextObj.GetComponentsInChildren<MeshRenderer>();
            foreach(var render in textRenderers)
            {
                render.material.renderQueue = 2800;
            }
        }

        public void InitialVisualObj()
        {
            this.OtherVisualObj.SetActive(true);
            this.ColorStripObj.SetActive(false);
            switch (MapObj.MapType)
            {
                case ThematicType.Choropleth:
                    this.ElevatedTextObj.SetActive(false);
                    this.FlatTextObj.SetActive(true);
                    this.ColorStripObj.SetActive(true);

                    break;
                
                case ThematicType.Prism:
                    ElevatedTextObj.SetActive(true);
                    FlatTextObj.SetActive(false);

                    var cylinder = OtherVisualObj.AddComponent<MapRegionCircle>();
                    this.MapObj.ElevatedMapMaterial.color = this.MapObj.DefaultMapColor;
                    cylinder.DrawCircleWithHeight(Vector3.zero, Mathf.Sqrt(this.MapObj.DefaultArea / Mathf.PI), this.MapObj.MaxHeight, this.MapObj.ElevatedMapMaterial);
                    cylinder.gameObject.transform.localPosition = Vector3.zero;

                    var thisRenderer = cylinder.gameObject.GetComponent<MeshRenderer>();
                    thisRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    thisRenderer.receiveShadows = false;
                    thisRenderer.material.renderQueue = (int)RenderQueue.Geometry;
                    break;

                case ThematicType.ColoredPrism:
                    ElevatedTextObj.SetActive(true);
                    FlatTextObj.SetActive(false);

                    var colorCylinder = OtherVisualObj.AddComponent<MapRegionCircle>();
                    colorCylinder.DrawCircleWithHeight(Vector3.zero, Mathf.Sqrt(this.MapObj.DefaultArea / Mathf.PI), this.MapObj.MaxHeight, this.GradientM);
                    colorCylinder.gameObject.transform.localPosition = Vector3.zero;

                    var colorRenderer = colorCylinder.gameObject.GetComponent<MeshRenderer>();
                    colorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    colorRenderer.receiveShadows = false;
                    colorRenderer.material.renderQueue = (int)RenderQueue.Geometry;
                    break;
            }
        }
    }
}
