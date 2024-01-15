using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public class Choroplth2PrismMorpher : BaseMorpher
    {
        public ThematicMapObj ColoredPrism { get; private set; }
        private GameObject mapObj;
        private float startZScale = -1;


        public override float Progress {
           set {
                base.Progress = value;
                if (!this._isProgressChanged) return;

                var newZScale = (1 - this.startZScale) * this.Progress + this.startZScale;
                this.mapObj.transform.localScale = new Vector3(1, 1, newZScale);

                foreach(var label in this.ColoredPrism.name2Label.Values)
                {
                    label.SetOffset(RenderingSettings.MinZOffset * 4 / newZScale / 0.3f);
                }
            }
        }

        public Choroplth2PrismMorpher(ThematicMapObj coloredPrism)
        {
            this.ColoredPrism = coloredPrism;
            this.mapObj = this.ColoredPrism.gameObject.transform.parent.gameObject;
            this.startZScale = this.mapObj.transform.localScale.z;
        }
    }
}
