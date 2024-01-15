using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public class LineRendererMorpher : BaseMorpher
    {
        public LineRenderer Renderer;
        public Vector3[] origins;
        public Vector3[] targets;


        public override float Progress
        {
            set
            {
                base.Progress = value;
                if (!this._isProgressChanged) return;

                //if(Mathf.Abs(1 - this.Progress) < Mathf.Epsilon)
                //{
                //    this.Renderer.gameObject.SetActive(false);
                //}
                //else
                //{
                //    this.Renderer.gameObject.SetActive(true);
                //}
                
                var currentPs = new Vector3[this.Renderer.positionCount];
                for(var i = 0; i < this.Renderer.positionCount; i++)
                {
                    currentPs[i] = Vector3.Lerp(this.origins[i], this.targets[i], this.Progress);
                }
                this.Renderer.SetPositions(currentPs);
            }
        }

        
        public void UpdateTargets(Vector3[] targets)
        {
            this.targets = targets;
        }

        public LineRendererMorpher(LineRenderer renderer)
        {
            this.Renderer = renderer;
            this.origins = new Vector3[this.Renderer.positionCount];
            renderer.GetPositions(this.origins);
        }
    }
}
