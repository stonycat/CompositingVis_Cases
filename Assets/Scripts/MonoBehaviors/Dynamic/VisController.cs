using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public enum VisType
    {
        Magic,
        SideBySide,
        ButtonChange
    }

    public class VisController : MonoBehaviour
    {
        public MapMorpher MagicMap;

        public MapMorpher SideBySideChoropleth;
        public MapMorpher SideBySidePrism;
        public MapMorpher SideBySideBars;

        public MapMorpher ButtonChange;

        public VisType Vis;
        //public VRTK.VRTK_ControllerEvents ControllerEvents;

        private float prismProgress = -1;

        private void Start()
        {
            if (this.Vis == VisType.ButtonChange)
            {
                this.prismProgress = (this.ButtonChange.endChoropleth2Prism + this.ButtonChange.startPrism2Bars) / 2.0f;
                //ControllerEvents.TouchpadPressed += ControllerEvents_TouchpadPressed;
            }
        }

        public float LookAtProgress
        {
            get
            {
                switch(this.Vis)
                {
                    case VisType.Magic:
                        return this.MagicMap.Progress;
                    case VisType.ButtonChange:
                        return this.ButtonChange.Progress;
                    case VisType.SideBySide:
                        if (Camera.main == null) return -1;
                        
                        var isC = GeometryHelper.RayHitBox(
                            this.SideBySideChoropleth.MapObj.transform, 
                            new Vector3(0.7f * 2, 0.7f * 2, 0.001f), 
                            out Vector3 cP
                        );

                        var isP = GeometryHelper.RayHitBox(
                            this.SideBySidePrism.MapObj.transform,
                            new Vector3(0.7f * 2, 0.7f * 2, 0.5f),
                            out Vector3 pP
                        );

                        var isB = GeometryHelper.RayHitBox(
                            this.SideBySideBars.MapObj.transform,
                            new Vector3(0.7f * 2, 0.001f, 0.5f * 2),
                            out Vector3 bP
                        );

                        if (isC && !isP && !isB) return this.SideBySideChoropleth.Progress;
                        if (!isC && isP && !isB) return this.SideBySidePrism.Progress;
                        if (!isC && !isP && isB) return this.SideBySideBars.Progress;

                        float disC = Mathf.Infinity, disP = Mathf.Infinity, disB = Mathf.Infinity;
                        if (isC) disC = cP.x * cP.x + cP.y * cP.y;
                        if (isP)
                        {
                            var disP1 = pP.x * pP.x + pP.y * pP.y;
                            var disP2 = pP.z * pP.z + pP.y * pP.y;
                            var disP3 = pP.x * pP.x + pP.z * pP.z;

                            disP = Mathf.Min(new float[] { disP1, disP2, disP3 });
                        }
                        if(isB) disB = bP.x * bP.x + bP.z * bP.z;


                        if (isC && isP && !isB)
                        {
                            if(disC < disP)
                                return this.SideBySideChoropleth.Progress;
                            else
                                return this.SideBySidePrism.Progress;
                        }
                        if (isC && !isP && isB)
                        {
                            if (disC < disB)
                                return this.SideBySideChoropleth.Progress;
                            else
                                return this.SideBySideBars.Progress;
                        }
                        if (!isC && isP && isB)
                        {
                            if (disP < disB)
                                return this.SideBySidePrism.Progress;
                            else
                                return this.SideBySideBars.Progress;
                        }

                        if (isC && isP && isB)
                        {
                            if (disC < disP && disC < disB) return this.SideBySideChoropleth.Progress;
                            if (disP < disC && disP < disB) return this.SideBySidePrism.Progress;
                            if (disB < disP && disB < disC) return this.SideBySideBars.Progress;
                        }
                        return -1;
                }
                return -1;
            }
        }


        private void move2Choropleth(MapMorpher morpher)
        {
            //TransUtility.InitialMap(morpher.MapObj, Camera.main.transform, 0.6f, 0, 0);
            morpher.Progress = 0;
        }

        private void move2Prism(MapMorpher morpher)
        {
            //TransUtility.InitialMap(morpher.MapObj, Camera.main.transform, 0.6f, morpher.startPrism2Bars * 90f, 0);
            morpher.Progress = (morpher.endChoropleth2Prism + morpher.startPrism2Bars) / 2.0f;
        }

        private void move2Bars(MapMorpher morpher)
        {
            morpher.UpdateFrontDirection();
            //TransUtility.InitialMap(morpher.MapObj, Camera.main.transform, 0.6f, 90, 0);
            morpher.Progress = 1;
        }

        private void ControllerEvents_TouchpadPressed(object sender)
        {
            //    if (ControllerEvents.touchpadPressed)
            //    {
            //        var p = ControllerEvents.GetTouchpadAxis();
            //        // left click, go back
            //        if (p.x < 0)
            //        {
            //            if(Mathf.Abs(this.ButtonChange.Progress) < Mathf.Epsilon)
            //            {
            //                this.move2Bars(this.ButtonChange);
            //            }
            //            else if (Mathf.Abs(this.ButtonChange.Progress - this.prismProgress) < Mathf.Epsilon)
            //            {
            //                this.move2Choropleth(this.ButtonChange);
            //            }
            //            else if(Mathf.Abs(this.ButtonChange.Progress - 1) < Mathf.Epsilon)
            //            {
            //                this.move2Prism(this.ButtonChange);
            //            }
            //        }
            //        // right click, go forward
            //        else
            //        {
            //            if (Mathf.Abs(this.ButtonChange.Progress) < Mathf.Epsilon)
            //            {
            //                this.move2Prism(this.ButtonChange);
            //            }
            //            else if (Mathf.Abs(this.ButtonChange.Progress - this.prismProgress) < Mathf.Epsilon)
            //            {
            //                this.move2Bars(this.ButtonChange);
            //            }
            //            else if (Mathf.Abs(this.ButtonChange.Progress - 1) < Mathf.Epsilon)
            //            {
            //                this.move2Choropleth(this.ButtonChange);
            //            }
            //        }
            //    }
        }

        public void ResetPosition()
        {
            if (Camera.main == null) return;
            switch(this.Vis)
            {
                case VisType.Magic:
                    TransUtility.InitialMap(this.MagicMap.MapObj, Camera.main.transform, 0.6f, 0, 0);
                    this.MagicMap.Progress = 0;
                    break;
                case VisType.SideBySide:
                    TransUtility.InitialMap(this.SideBySideChoropleth.MapObj, Camera.main.transform, 0.9f, 0, -80);
                    TransUtility.InitialMap(this.SideBySidePrism.MapObj, Camera.main.transform, 0.9f, this.SideBySidePrism.startPrism2Bars * 90f, 0);
                    TransUtility.InitialMap(this.SideBySideBars.MapObj, Camera.main.transform, 0.9f, 90, 80);

                    this.SideBySideBars.Bars2Front.UpdateDirection(-Vector3.right);
                    this.SideBySideBars.Legend.UpdateLegendIndex();

                    this.SideBySideChoropleth.Progress = 0;
                    this.SideBySidePrism.Progress = (this.SideBySidePrism.endChoropleth2Prism + this.SideBySidePrism.startPrism2Bars) / 2.0f;
                    this.SideBySideBars.Progress = 1;
                    break;
                case VisType.ButtonChange:
                    TransUtility.InitialMap(this.ButtonChange.MapObj, Camera.main.transform, 0.6f, 0, 0);
                    this.ButtonChange.Progress = 0;
                    break;
            }

        }

        private bool _isInitialed = false;
        private void Update()
        {
            if (this._isInitialed)
            {
                if (this.Vis == VisType.Magic)
                {
                    var tmpProgress = this.MagicMap.TiltedAngle / 90.0f;
                    this.MagicMap.Progress = tmpProgress;
                }
            }

            if (_isInitialed || Camera.main == null) return;
            switch (this.Vis)
            {
                case VisType.Magic:
                    if (!this.MagicMap.IsInitialed) return;
                    break;
                case VisType.SideBySide:
                    if (!this.SideBySideChoropleth.IsInitialed) return;
                    if (!this.SideBySidePrism.IsInitialed) return;
                    if (!this.SideBySideBars.IsInitialed) return;
                    break;
                case VisType.ButtonChange:
                    if (!this.ButtonChange.IsInitialed) return;
                    break;
            }
            
            this._isInitialed = true;
        }
    }
}
