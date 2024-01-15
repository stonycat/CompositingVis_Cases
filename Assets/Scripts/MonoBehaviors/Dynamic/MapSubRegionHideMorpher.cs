using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public class MapSubRegionHideMorpher : BaseMorpher
    {
        public MapSubRegionObj MapSubRegion { get; private set; }
        public LineRendererMorpher TopLineMorpher { get; private set; }
        public LineRendererMorpher BaseLineMorpher { get; private set; }

        public LineRendererMorpher TopBlueLineMorpher { get; private set; }
        public LineRendererMorpher BaseBlueLineMorpher { get; private set; }

        public override float Progress
        {
            set
            {
                base.Progress = value;
                if (!this._isProgressChanged) return;

                var shaderProgress = this.Progress / 0.5f;
                shaderProgress = shaderProgress > 1 ? 1 : shaderProgress;

                this.MapSubRegion.meshRenderer.material.SetFloat("_Progress", shaderProgress);
                this.TopLineMorpher.Progress = shaderProgress;
                this.BaseLineMorpher.Progress = shaderProgress;
                this.TopBlueLineMorpher.Progress = shaderProgress;
                this.BaseBlueLineMorpher.Progress = shaderProgress;

                if (Mathf.Abs(this.Progress - 0.5f) < Mathf.Epsilon)
                {
                    this.MapSubRegion.gameObject.SetActive(false);
                }
                else
                {
                    this.MapSubRegion.gameObject.SetActive(true);
                }
            }
        }


        public MapSubRegionHideMorpher(MapSubRegionObj mapSubRegion, Vector2 centroidP)
        {
            this.MapSubRegion = mapSubRegion;

            var mesh = this.MapSubRegion.meshFilter.mesh;
            var uv2 = new Vector3[mesh.vertexCount];

            for(var i = 0; i < mesh.vertexCount; i++)
            {
                uv2[i] = centroidP;
            }
            mesh.SetUVs(2, uv2.ToList());
            this.MapSubRegion.meshFilter.mesh = mesh;


            this.TopLineMorpher = new LineRendererMorpher(this.MapSubRegion.rawTopRenderer);
            this.BaseLineMorpher = new LineRendererMorpher(this.MapSubRegion.rawBaseRenderer);

            this.TopBlueLineMorpher = new LineRendererMorpher(this.MapSubRegion.blueTopRenderer);
            this.BaseBlueLineMorpher = new LineRendererMorpher(this.MapSubRegion.blueBaseRenderer);

            this.TopLineMorpher.UpdateTargets(uv2);
            this.BaseLineMorpher.UpdateTargets(uv2);

            this.TopBlueLineMorpher.UpdateTargets(uv2);
            this.BaseBlueLineMorpher.UpdateTargets(uv2);
        }
    }
}
