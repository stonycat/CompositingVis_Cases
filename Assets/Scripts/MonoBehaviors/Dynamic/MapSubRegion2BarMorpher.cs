using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public class MapSubRegion2BarMorpher : BaseMorpher
    {
        public MapSubRegionObj MapSubRegion { get; private set; }
        public LineRendererMorpher TopLineMorpher { get; private set; }
        public LineRendererMorpher BaseLineMorpher { get; private set; }

        public LineRendererMorpher TopBlueLineMorpher { get; private set; }
        public LineRendererMorpher BaseBlueLineMorpher { get; private set; }

        public Vector3 CentroidP { get; private set; }
        public float BarWidth { get; private set; }

        public override float Progress
        {
            set
            {
                base.Progress = value;

                if (this.Progress < 1)
                {
                    this.MapSubRegion.gameObject.transform.localPosition = Vector3.zero;
                    this.MapSubRegion.gameObject.transform.localRotation = Quaternion.identity;
                }
                if (!this._isProgressChanged) return;

                this.MapSubRegion.meshRenderer.material.SetFloat("_Progress", this.Progress);
                this.TopLineMorpher.Progress = this.Progress;
                this.BaseLineMorpher.Progress = this.Progress;

                this.TopBlueLineMorpher.Progress = this.Progress;
                this.BaseBlueLineMorpher.Progress = this.Progress;
            }
        }

        private void InitialDirection()
        {
            var direction = Vector3.right;
            var mesh = this.MapSubRegion.meshFilter.mesh;
            var uv2 = new Vector3[mesh.vertexCount];
            var vertices = mesh.vertices;
            
            var lP = -1.0f;
            var lN = Mathf.Infinity;

            var large = -1.0f;
            var small = Mathf.Infinity;

            for (var i = 0; i < mesh.vertexCount; i++)
            {
                var currentP = vertices[i];
                uv2[i] = Vector3.Project(currentP - this.CentroidP, direction);
                var thisX = uv2[i].x / direction.x;

                if (thisX >= 0)
                {
                    lP = Mathf.Max(lP, thisX);
                }
                else
                {
                    lN = Mathf.Min(lN, thisX);
                }

                large = Mathf.Max(large, thisX);
                small = Mathf.Min(small, thisX);
            }

            var scaleP = lP / (this.BarWidth / 2);
            var scaleN = (-lN) / (this.BarWidth / 2);

            var offset = 0f;

            if(lP < 0 || float.IsInfinity(lN))
            {
                var thisS = (large - small) / this.BarWidth;
                scaleP = thisS;
                scaleN = thisS;

                // all negative
                offset = ((large + small) / 2) / thisS;

                // all positive
                if (lP < 0)
                {
                    offset = -offset;
                }
            }
            
            
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                if (uv2[i].x >= 0)
                    uv2[i] = ((uv2[i].x) / scaleP - offset) * direction + this.CentroidP;
                else
                    uv2[i] = ((uv2[i].x) / scaleN + offset) * direction + this.CentroidP;
            }

            mesh.SetUVs(2, uv2.ToList());

            //DebugFileWriter.WriteVertices(this.MapSubRegion.name, uv2);

            this.MapSubRegion.meshFilter.mesh = mesh;

            this.TopLineMorpher.UpdateTargets(uv2);
            this.BaseLineMorpher.UpdateTargets(uv2);

            this.TopBlueLineMorpher.UpdateTargets(uv2);
            this.BaseBlueLineMorpher.UpdateTargets(uv2);
        }

        public MapSubRegion2BarMorpher(MapSubRegionObj mapSubRegion, Vector3 centroidP, float width)
        {
            this.MapSubRegion = mapSubRegion;
            this.CentroidP = centroidP;
            this.BarWidth = width;

            this.TopLineMorpher = new LineRendererMorpher(this.MapSubRegion.rawTopRenderer);
            this.BaseLineMorpher = new LineRendererMorpher(this.MapSubRegion.rawBaseRenderer);

            this.TopBlueLineMorpher = new LineRendererMorpher(this.MapSubRegion.blueTopRenderer);
            this.BaseBlueLineMorpher = new LineRendererMorpher(this.MapSubRegion.blueBaseRenderer);

            this.InitialDirection();
        }
    }
}
