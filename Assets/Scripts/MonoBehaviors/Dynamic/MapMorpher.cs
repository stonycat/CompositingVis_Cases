using Assets.Scripts.MonoBehaviors.Legend;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public enum MorphingState
    {
        Choropleth,
        Choropleth2Prism,
        Prism,
        Prism2Bars,
        Bars,
        Bars2Front,
        Front
    }

    public class MapMorpher : MonoBehaviour, IMorpher
    {
        public ThematicMapObj Map;
        public ThematicMapObj BaseMap;
        public MorphingLegend Legend;

        private float startProgress = 0;

        public float startChoropleth2Prism = 20.0f / 90.0f;
        public float endChoropleth2Prism = 45.0f / 90.0f;
        
        public float startPrism2Bars = 75.0f / 90.0f;
        public float endPrism2Bars = 80.0f / 90.0f;
        
        public float startBar2Front = 82.0f / 90.0f;
        public float endBar2Front = 90.0f / 90.0f;
        
        public float endProgress = 1.0f;

        public Choroplth2PrismMorpher Choropleth2Prism;
        public Prism2BarsMorpher Prism2Bars;
        public Bars2FrontMorpher Bars2Front;

        public GameObject MapObj
        {
            get
            {
                return this.Map.transform.parent.gameObject;
            }
        }

        public MorphingState CurrentState
        {
            get; private set;
        }

        public void Morph2Next()
        {

        }

        public float TiltedAngle
        {
            get
            {
                var mapForward = this.MapObj.transform.forward;
                var up = Vector3.up;
                var result = Vector3.Angle(mapForward, up);

                var cameraForwrd = Camera.main.transform.forward;
                cameraForwrd.y = 0;
                cameraForwrd.Normalize();
                mapForward.y = 0;

                var mapOnCamera = Vector3.Project(mapForward, cameraForwrd);
                if (mapOnCamera.x / cameraForwrd.x > 0)
                    result = result + 90;
                else
                    result = 90 - result;
                return result;

                //var cameraRight = Camera.main.transform.right;
                //cameraRight.y = 0;
                //cameraRight.Normalize();
                //var mapDir = this.MapObj.transform.InverseTransformDirection(cameraRight);
                //mapDir.z = 0;
                //mapDir.Normalize();
                //mapDir = this.MapObj.transform.TransformDirection(mapDir);
                //mapDir.y = 0;
                //mapDir.Normalize();

                //var mapForward = this.MapObj.transform.forward;
                //var cameraForwrd = Camera.main.transform.forward;

                //var cameraProj = Vector3.ProjectOnPlane(cameraForwrd, Camera.main.transform.right);
                //var mapProj = Vector3.ProjectOnPlane(mapForward, Camera.main.transform.right);

                //return 180 - Vector3.Angle(cameraProj, mapProj);
            }
        }

        public float SubProgress { get; private set; }

        private float _progress = 0.0f;
        public float Progress
        {
            get
            {
                return this._progress;
            }
            set
            {
                var tmp = value;
                tmp = tmp > 1 ? 1 : tmp;
                tmp = tmp < 0 ? 0 : tmp;
                
                if (Mathf.Abs(tmp - this.Progress) < Mathf.Epsilon)
                {
                    return;
                }
                this._progress = tmp;

                var thisCurrentProgress = this.Progress;
                if (thisCurrentProgress < startProgress)
                {
                    this.SubProgress = 0;
                    this.Bars2Front.Progress = 0;
                    this.Prism2Bars.Progress = 0;
                    this.Choropleth2Prism.Progress = 0;
                    this.Legend.Progress = 0;

                    this.CurrentState = MorphingState.Choropleth;
                    this.BaseMap.gameObject.SetActive(false);
                }
                else if (thisCurrentProgress >= this.startProgress && thisCurrentProgress < this.startChoropleth2Prism)
                {
                    // keep Choropleth
                    this.SubProgress = 1;
                    this.Bars2Front.Progress = 0;
                    this.Prism2Bars.Progress = 0;
                    this.Choropleth2Prism.Progress = 0;
                    this.Legend.Progress = 0;

                    this.CurrentState = MorphingState.Choropleth;
                    this.BaseMap.gameObject.SetActive(false);
                }
                else if (thisCurrentProgress >= this.startChoropleth2Prism && thisCurrentProgress < this.endChoropleth2Prism)
                {
                    // from Choropleth to Prism
                    this.SubProgress = (thisCurrentProgress - this.startChoropleth2Prism) / (this.endChoropleth2Prism - this.startChoropleth2Prism);

                    this.Bars2Front.Progress = 0;
                    this.Prism2Bars.Progress = 0;
                    this.Choropleth2Prism.Progress = this.SubProgress;

                    this.CurrentState = MorphingState.Choropleth2Prism;

                    var legendStart = 0.5f;
                    var legendEnd = 1f;
                    var legendProgress = 0f;
                    if(this.SubProgress > 0.5f)
                    {
                        legendProgress = (this.SubProgress - legendStart) / (legendEnd - legendStart);
                    }
                    this.Legend.Progress = Mathf.Min(legendProgress / 2, 0.5f);
                    this.BaseMap.gameObject.SetActive(true);
                }
                else if (thisCurrentProgress >= this.endChoropleth2Prism && thisCurrentProgress < this.startPrism2Bars)
                {
                    // keep Prism
                    this.SubProgress = 1;
                    this.Bars2Front.Progress = 0;
                    this.Prism2Bars.Progress = 0;
                    this.Choropleth2Prism.Progress = 1;
                    this.Legend.Progress = 0.5f;

                    this.CurrentState = MorphingState.Prism;
                    this.BaseMap.gameObject.SetActive(true);
                }
                else if (thisCurrentProgress >= this.startPrism2Bars && thisCurrentProgress < this.endPrism2Bars)
                {
                    // from Prism to bars
                    this.SubProgress = (thisCurrentProgress - this.startPrism2Bars) / (this.endPrism2Bars - this.startPrism2Bars);

                    this.Bars2Front.Progress = 0;
                    this.Prism2Bars.Progress = this.SubProgress;
                    this.Choropleth2Prism.Progress = 1;
                    this.Legend.Progress = 0.5f;

                    this.CurrentState = MorphingState.Prism2Bars;
                    this.BaseMap.gameObject.SetActive(true);
                }
                else if (thisCurrentProgress >= this.endPrism2Bars && thisCurrentProgress < this.startBar2Front)
                {
                    // keep bars
                    this.SubProgress = 1;
                    this.Bars2Front.Progress = 0;
                    this.Prism2Bars.Progress = 1;
                    this.Choropleth2Prism.Progress = 1;
                    this.Legend.Progress = 0.5f;

                    this.CurrentState = MorphingState.Bars;
                    this.BaseMap.gameObject.SetActive(true);
                }
                else if (thisCurrentProgress >= this.startBar2Front && thisCurrentProgress < this.endBar2Front)
                {
                    // moving bars to front
                    this.SubProgress = (thisCurrentProgress - this.startBar2Front) / (this.endBar2Front - this.startBar2Front);

                    this.Bars2Front.Progress = this.SubProgress;
                    this.Prism2Bars.Progress = 1;
                    this.Choropleth2Prism.Progress = 1;
                    this.Legend.Progress = 0.5f + this.SubProgress / 2;

                    this.CurrentState = MorphingState.Bars2Front;
                    this.BaseMap.gameObject.SetActive(false);
                }
                else if (thisCurrentProgress >= this.endPrism2Bars && thisCurrentProgress < this.endProgress)
                {
                    // keep front bars
                    this.SubProgress = 1;
                    this.Bars2Front.Progress = 1;
                    this.Prism2Bars.Progress = 1;
                    this.Choropleth2Prism.Progress = 1;
                    this.Legend.Progress = 1;

                    this.CurrentState = MorphingState.Front;
                    this.BaseMap.gameObject.SetActive(false);
                }
                else
                {
                    // keep front bars
                    this.Choropleth2Prism.Progress = 1;
                    this.Prism2Bars.Progress = 1;
                    this.Bars2Front.Progress = 1;
                    this.Legend.Progress = 1;

                    this.SubProgress = 1;
                    this.CurrentState = MorphingState.Front;
                    this.BaseMap.gameObject.SetActive(false);
                }
            }
        }

        private void Start()
        {
            var propName = "newDensity";
            this.Map.PropName = propName;
            this.BaseMap.PropName = propName;

            this.Map.FuncType = TransferFuncType.Linear;
            this.BaseMap.FuncType = TransferFuncType.Linear;
            

            this.Map.DrawMap($"./Data/{this.Map.GeoName}-Data.csv");
            this.BaseMap.DrawMap($"./Data/{this.Map.GeoName}-Data.csv", false, false);
            this.BaseMap.gameObject.SetActive(false);
            this.MapObj.transform.localScale = new Vector3(1, 1, 0.001f);
            this.BaseMap.gameObject.transform.localPosition = new Vector3(0, 0, -RenderingSettings.MinZOffset);

            var barWidth = 0.015f;
            if (this.Map.GeoName == GeoName.EU)
                barWidth /= 1.8f;

            this.Choropleth2Prism = new Choroplth2PrismMorpher(this.Map);
            this.Prism2Bars = new Prism2BarsMorpher(this.Map, barWidth);
            this.Bars2Front = new Bars2FrontMorpher(this.Map, barWidth);
            if (this.Map.GeoName == GeoName.EU)
            {
                this.Bars2Front.EndLabelScale = 0.25f;
                this.Bars2Front.BarLabelMargin = 0.01f;
                this.Bars2Front.LabelLevelMargin = 0.008f;
                this.Bars2Front.BarMarginRate = 1.1f;
            }
            this.Progress = 0;
            this.IsInitialed = true;
        }

        public void UpdateFrontDirection()
        {
            var cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();
            var mapDir = this.MapObj.transform.InverseTransformDirection(cameraRight);
            mapDir.z = 0;
            mapDir.Normalize();

            this.Bars2Front.UpdateDirection(mapDir);
            this.Legend.UpdateLegendIndex();
        }

        public bool IsInitialed = false;
        private void Update()
        {
            //if(this._isInitialed)
            //{
            //    var tmpProgress = this.TiltedAngle / 90.0f;
            //    this.Progress = tmpProgress;
            //}

            if(this.CurrentState == MorphingState.Bars || this.CurrentState == MorphingState.Front || this.CurrentState == MorphingState.Bars2Front)
            {
                var bars = this.Prism2Bars.name2BarMorphers.Values;
                foreach(var bar in bars)
                {
                    var centroid = bar.CentroidP;
                    var worldCentroid = bar.MapSubRegion.gameObject.transform.TransformPoint(centroid);
                    var currentFacing = bar.MapSubRegion.gameObject.transform.up;
                    currentFacing.y = 0;
                    currentFacing.Normalize();

                    var cameraP = Camera.main.transform.position;
                    cameraP.y = 0;
                    //var tmpCentroid = this.MapObj.transform.position;
                    var tmpCentroid = worldCentroid;
                    tmpCentroid.y = 0;
                    var thisDir = (cameraP - tmpCentroid).normalized;

                    var angle = Vector3.SignedAngle(currentFacing, thisDir, new Vector3(0, 1, 0));
                    bar.MapSubRegion.gameObject.transform.RotateAround(worldCentroid, this.MapObj.transform.forward, angle);
                }

                if(this.CurrentState == MorphingState.Bars)
                {
                    this.UpdateFrontDirection();
                }
            }

            if (IsInitialed || Camera.main == null) return;
            //TransUtility.InitialMap(this.MapObj, Camera.main.transform, 0);
            
        }
    }
}
