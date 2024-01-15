using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public class Bars2FrontMorpher : BaseMorpher
    {
        public ThematicMapObj ColoredPrism { get; private set; }
        public Dictionary<string, Vector3> Name2Target { get; private set; }
        public Dictionary<string, Vector3> Name2LabelTarget { get; private set; }
        public float BarWidth { get; private set; }

        private float startLabelScale = 1;
        public float EndLabelScale = 0.35f;
        public float BarLabelMargin = 0.015f;
        public float LabelLevelMargin = 0f;
        public float BarMarginRate = 1.5f;

        public override float Progress
        {
            set
            {
                base.Progress = value;
                if (!this._isProgressChanged) return;

                this._progress = Ease.easeInOutCubic(this._progress);
                foreach (var nT in this.Name2Target)
                {
                    var n = nT.Key;
                    var t = nT.Value;

                    var thisT = Vector3.Lerp(Vector3.zero, t - this.ColoredPrism.FitMapProj.projMapData.Name2GeoPoint[n], this.Progress);
                    var thisObj = this.ColoredPrism.name2SubRegions[n][0].gameObject.transform.parent;
                    thisObj.transform.localPosition = thisT;

                    var thisLabel = this.ColoredPrism.name2Label[n];
                    var originLabelP = this.ColoredPrism.FitMapProj.projMapData.Name2GeoPoint[n];
                    originLabelP.z = thisLabel.RawHeight;

                    var newP = Vector3.Lerp(originLabelP, this.Name2LabelTarget[n], this.Progress);
                    thisLabel.transform.localPosition = newP;

                    var newScale = Mathf.Lerp(this.startLabelScale, this.EndLabelScale, this.Progress);
                    thisLabel.transform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
        }

        public Vector3 OrderDirection;
        public Vector3 StartP;
        public Vector3 endP;
        public void UpdateDirection(Vector3 direction)
        {
            //Debug.Log($"{direction.x}, {direction.y}, {direction.z}");

            var MapObj = this.ColoredPrism.gameObject.transform.parent;
            var names = this.ColoredPrism.name2SubRegions.Keys.ToList();

            this.OrderDirection = direction;
            
            var avgCentroid = Vector3.zero;
            foreach (var n in names)
            {
                avgCentroid += (this.ColoredPrism.FitMapProj.projMapData.Name2GeoPoint[n]);
            }
            avgCentroid /= names.Count;

            
            //this.dirObj.transform.localPosition = avgCentroid + direction * 0.5f;

            names = names.OrderBy(n =>
            {
                var centroidP = this.ColoredPrism.FitMapProj.projMapData.Name2GeoPoint[n];
                var projP = Vector3.Project(centroidP - avgCentroid, this.OrderDirection) + avgCentroid;
                return Camera.main.transform.InverseTransformPoint(this.ColoredPrism.transform.TransformPoint(projP)).x;
            }).ToList();
            

            var marginWidth = this.BarWidth * this.BarMarginRate;
            var length = marginWidth * (names.Count / 2);
            this.StartP = avgCentroid - marginWidth * (names.Count / 2.0f - 0.5f) * this.OrderDirection;
            this.endP = this.StartP + marginWidth * this.OrderDirection * (names.Count - 1);

            //this.centerObj.transform.localPosition = this.endP + this.OrderDirection * 3 * this.BarWidth;
            var up = new Vector3(0, 0, 1);
            for (var i = 0; i < names.Count; i++)
            {
                var vMargin = i % 2;
                var thisP = this.StartP + marginWidth * i * this.OrderDirection;
                var labelP = thisP - this.BarLabelMargin * up - vMargin * up * this.LabelLevelMargin;

                if (this.Name2Target.ContainsKey(names[i]))
                {
                    this.Name2Target[names[i]] = thisP;
                    this.Name2LabelTarget[names[i]] = labelP;
                }
                else
                {
                    this.Name2Target.Add(names[i], thisP);
                    this.Name2LabelTarget.Add(names[i], labelP);
                }
            }
        }

        //GameObject centerObj;
        //GameObject dirObj;

        public Bars2FrontMorpher(ThematicMapObj coloredPrism, float barWidth)
        {
            this.ColoredPrism = coloredPrism;
            this.Name2Target = new Dictionary<string, Vector3>();
            this.Name2LabelTarget = new Dictionary<string, Vector3>();
            this.BarWidth = barWidth;

            this.startLabelScale = this.ColoredPrism.name2Label.Values.First().transform.localScale.x;

            //this.centerObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //this.centerObj.transform.parent = this.ColoredPrism.transform.parent;
            //this.centerObj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            //this.dirObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //this.dirObj.transform.parent = this.ColoredPrism.transform.parent;
            //this.dirObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
}
