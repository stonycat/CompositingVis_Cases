using Assets.Scripts.MonoBehaviors.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Legend
{
    public class MorphingLegend : MonoBehaviour, IMorpher
    {
        public MapMorpher Morpher;

        public LegendController Left;
        public LegendController Right;
        public LegendController Top;
        public LegendController Bottom;

        private List<GameObject> LRTBObjs = new List<GameObject>();
        private List<Vector3> LRTBFlatPostions = new List<Vector3>();
        private List<Vector3> LRTBFlatRotations = new List<Vector3>();
        private List<LegendController> LRTBControllers = new List<LegendController>();

        private List<Vector3> LRTBVerticalPostions = new List<Vector3>();
        private List<Vector3> LRTBVerticalRotations = new List<Vector3>();


        private float _progress = 0.0f;
        public float Progress
        {
            get
            {
                return this._progress;
            }
            set
            {
                var pre = this.Progress;
                this._progress = value > 1 ? 1 : value;
                this._progress = this._progress < 0 ? 0 : this._progress;

                if (Mathf.Abs(pre - this.Progress) < Mathf.Epsilon)
                {
                    return;
                }
                //this.gameObject.transform.parent = null;
                //this.gameObject.transform.localScale = Vector3.one;
                //this.gameObject.transform.parent = this.Morpher.MapObj.transform;

                this.Left.TextState = LegendTextState.Left;
                this.Right.TextState = LegendTextState.Right;
                this.Top.TextState = LegendTextState.Top;
                this.Bottom.TextState = LegendTextState.Bottom;

                var flat2verticalProgress = Mathf.Min(this.Progress, 0.5f) * 2;
                for (var i = 0; i < this.LRTBObjs.Count; i++)
                {
                    var obj = this.LRTBObjs[i];
                    obj.transform.localPosition = Vector3.Lerp(this.LRTBFlatPostions[i], this.LRTBVerticalPostions[i], flat2verticalProgress);
                    obj.transform.localEulerAngles = Vector3.Lerp(this.LRTBFlatRotations[i], this.LRTBVerticalRotations[i], flat2verticalProgress);
                    if(flat2verticalProgress == 1)
                        this.billboard(obj);
                }

                for (var i = 0; i < this.LRTBObjs.Count; i++)
                {
                    var obj = this.LRTBObjs[i];
                    obj.transform.localPosition = this.LRTBVerticalPostions[i];
                    obj.transform.localScale = Vector3.one;
                }

                if (this.Progress >= 0.5f)
                {
                    var moveProgress = Mathf.Min(this.Progress - 0.5f, 0.5f) * 2;
                    this.Left.TextState = LegendTextState.Right;
                    this.Right.TextState = LegendTextState.Right;
                    this.Top.TextState = LegendTextState.Right;
                    this.Bottom.TextState = LegendTextState.Right;

                    for (var i = 0; i < this.LRTBObjs.Count; i++)
                    {
                        var obj = this.LRTBObjs[i];
                        if (this.MatchRightIndex != -1 && i == this.MatchRightIndex)
                        {
                            var VP = this.LRTBVerticalPostions[MatchRightIndex];
                            var thisEndP = this.Morpher.Bars2Front.endP + this.Morpher.Bars2Front.OrderDirection * this.Morpher.Bars2Front.BarWidth * 3;
                            thisEndP.z = VP.z;
                            obj.transform.localPosition = Vector3.Lerp(VP, thisEndP, moveProgress);
                        }
                        else if(this.MatchLeftIndex != -1 && i == this.MatchLeftIndex)
                        {
                            this.LRTBControllers[MatchLeftIndex].TextState = LegendTextState.Left;
                            var VP = this.LRTBVerticalPostions[MatchLeftIndex];
                            var thisEndP = this.Morpher.Bars2Front.StartP - this.Morpher.Bars2Front.OrderDirection * this.Morpher.Bars2Front.BarWidth * 3;
                            thisEndP.z = VP.z;
                            obj.transform.localPosition = Vector3.Lerp(VP, thisEndP, moveProgress);
                        }
                        else
                        {
                            var tmpProgress = Mathf.Min(moveProgress * 4, 1);
                            obj.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.000001f, 0.000001f, 0.000001f), tmpProgress);
                            if(moveProgress > 0.9f)
                            {
                                obj.SetActive(false);
                            }
                            else
                            {
                                obj.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    this.ResetText();
                }
            }
        }

        private int MatchRightIndex = -1;
        private int MatchLeftIndex = -1;
        public void UpdateLegendIndex()
        {
            var minRightDis = Mathf.Infinity;
            var minLeftDis = Mathf.Infinity;
            var thisEndP = this.Morpher.Bars2Front.endP;
            var thisStartP = this.Morpher.Bars2Front.StartP;
            for (var i = 0; i < this.LRTBObjs.Count; i++)
            {
                var obj = this.LRTBObjs[i];
                var thisLocalP = obj.transform.localPosition;
                thisLocalP.z = 0;

                var thisDis = Vector3.Distance(thisEndP, thisLocalP);
                if (thisDis < minRightDis)
                {
                    minRightDis = thisDis;
                    this.MatchRightIndex = i;
                }

                thisDis = Vector3.Distance(thisStartP, thisLocalP);
                if (thisDis < minLeftDis)
                {
                    minLeftDis = thisDis;
                    this.MatchLeftIndex = i;
                }
            }
        }

        private void ResetText()
        {
            this.Left.TextState = LegendTextState.Left;
            this.Right.TextState = LegendTextState.Right;
            this.Top.TextState = LegendTextState.Top;
            this.Bottom.TextState = LegendTextState.Bottom;

            for (var i = 0; i < this.LRTBObjs.Count; i++)
            {
                var obj = this.LRTBObjs[i];
                obj.gameObject.transform.localScale = Vector3.one;
                obj.gameObject.SetActive(true);
            }
        }

        private void Start()
        {
            this.LRTBObjs.Add(this.Left.gameObject);
            this.LRTBObjs.Add(this.Right.gameObject);
            this.LRTBObjs.Add(this.Top.gameObject);
            this.LRTBObjs.Add(this.Bottom.gameObject);

            this.LRTBControllers.Add(this.Left);
            this.LRTBControllers.Add(this.Right);
            this.LRTBControllers.Add(this.Top);
            this.LRTBControllers.Add(this.Bottom);

            for (var i = 0; i < this.LRTBObjs.Count; i++)
            {
                var obj = this.LRTBObjs[i];
                var thisP = obj.transform.localPosition;
                var thisR = obj.transform.localEulerAngles;

                this.LRTBFlatPostions.Add(thisP);
                this.LRTBFlatRotations.Add(thisR);

                thisP.z = 0.1f;
                this.LRTBVerticalPostions.Add(thisP);

                if(i < 2)
                {
                    thisR.x = 90;
                }
                else
                {
                    thisR.y = 90;
                }
                this.LRTBVerticalRotations.Add(thisR);
            }
            this.ResetText();
        }

        private void billboard(GameObject legendObj)
        {
            var MapObj = this.Morpher.MapObj;
            var worldP = legendObj.transform.position;
            var currentFacing = legendObj.transform.forward;

            currentFacing.y = 0;
            currentFacing.Normalize();

            var cameraP = Camera.main.transform.position;
            cameraP.y = 0;
            var tmpCentroid = worldP;
            tmpCentroid.y = 0;
            var thisDir = (cameraP - tmpCentroid).normalized;

            var angle = Vector3.SignedAngle(currentFacing, thisDir, new Vector3(0, 1, 0));
            legendObj.transform.RotateAround(worldP, MapObj.transform.forward, angle);
        }

        private void Update()
        {
            if (this.Progress >= 0.5f)
            {
                for (var i = 0; i < this.LRTBObjs.Count; i++)
                {
                    var obj = this.LRTBObjs[i];
                    this.billboard(obj);
                }
            }
        }

    }
}
