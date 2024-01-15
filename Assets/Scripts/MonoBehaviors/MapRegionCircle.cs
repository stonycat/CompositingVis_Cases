using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MapRegionCircle : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        private float radius;
        private float height;
        private Vector3 centerPosition;

        private Vector3 positiveP;
        private Vector3 negativeP;

        private MeshFilter borderMeshFilter;
        private MeshRenderer borderMeshRenderer;

        private float borderThickness = 0.0005f;

        private GameObject labelObj;
        private TextMeshPro textScript;

        private void Start()
        {
            this.positiveP = gameObject.transform.position;
            this.positiveP.z += RenderingSettings.MinZOffset * 2;

            this.negativeP = positiveP;
            this.negativeP.z -= (RenderingSettings.MinZOffset * 4);

            if (height < 0) gameObject.transform.position = this.negativeP;
            else gameObject.transform.position = this.positiveP;
        }

        public void UpdateRadius(float radius)
        {
            if (Mathf.Abs(radius - this.radius) < Number.epsilon) return;
            this.radius = radius;

            var borderVertices = new Vector3[GeometryHelper.CIRCLR_COUNT * 2];

            var unityVertices = new Vector3[GeometryHelper.CIRCLR_COUNT + 1];
            var position = this.centerPosition;
            unityVertices[0] = position;
            for (var i = 0; i < GeometryHelper.CIRCLR_COUNT; i++)
            {
                unityVertices[i + 1] = position + new Vector3(GeometryHelper.cosArray[i] * radius, GeometryHelper.sinArray[i] * radius);
                borderVertices[i] = unityVertices[i + 1];
                borderVertices[i + GeometryHelper.CIRCLR_COUNT] = position + new Vector3(GeometryHelper.cosArray[i] * (radius + this.borderThickness), GeometryHelper.sinArray[i] * (radius + this.borderThickness));
            }

            var unityMesh = this.meshFilter.mesh;
            unityMesh.vertices = unityVertices;
            this.meshFilter.mesh = unityMesh;

            var borderMesh = this.borderMeshFilter.mesh;
            borderMesh.vertices = borderVertices;
            this.borderMeshFilter.mesh = borderMesh;
        }

        public void UpdateRenderringQ(int renderringQ)
        {
            this.meshRenderer.material.renderQueue = renderringQ;
            this.borderMeshRenderer.material.renderQueue = renderringQ;
        }

        public void UpdateHeight(float height)
        {
            if (Mathf.Abs(height - this.height) < Number.epsilon) return;
            this.height = height;

            if (height < 0) gameObject.transform.position = this.negativeP;
            else gameObject.transform.position = this.positiveP;

            this.meshRenderer.material.SetFloat("_Height", height);
        }

        public void UpdateColor(Color color)
        {
            this.meshRenderer.material.SetColor("_Color", color);
            this.meshRenderer.material.color = color;
        }

        public void DrawCircle(Vector3 position, float radius, Material inM, int renderringQ = 3001, Material borderM = null, float border = 0.0005f)
        {
            this.centerPosition = position;
            this.radius = radius;
            this.borderThickness = border;

            if (this.meshFilter == null)
                this.meshFilter = GetComponent<MeshFilter>();
            if (this.meshRenderer == null)
                this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshRenderer.material.CopyPropertiesFromMaterial(inM);
            this.meshRenderer.material.SetInt("_ZWrite", 0);
            this.meshRenderer.material.renderQueue = renderringQ;

            var unityVertices = new Vector3[GeometryHelper.CIRCLR_COUNT + 1];
            var unityTriangles = new int[GeometryHelper.CIRCLR_COUNT * 3];

            unityVertices[0] = position;
            for (var i = 0; i < GeometryHelper.CIRCLR_COUNT; i++)
            {
                unityVertices[i + 1] = position + new Vector3(GeometryHelper.cosArray[i] * radius, GeometryHelper.sinArray[i] * radius);

                unityTriangles[i * 3 + 2] = i + 1;
                unityTriangles[i * 3 + 1] = 0;
                if (i + 2 == unityVertices.Length)
                    unityTriangles[i * 3] = 1;
                else
                    unityTriangles[i * 3] = i + 2;
            }

            var unityMesh = new UnityEngine.Mesh();
            unityMesh.vertices = unityVertices;
            unityMesh.triangles = unityTriangles;
            this.meshFilter.mesh = unityMesh;

            if (borderM != null)
            {
                var borderObj = new GameObject("Border");
                borderObj.transform.parent = this.gameObject.transform;
                borderObj.transform.localPosition = Vector3.zero;

                this.borderMeshFilter = borderObj.AddComponent<MeshFilter>();
                this.borderMeshRenderer = borderObj.AddComponent<MeshRenderer>();
                this.borderMeshRenderer.material.CopyPropertiesFromMaterial(borderM);
                this.borderMeshRenderer.material.SetInt("_ZWrite", 0);
                this.borderMeshRenderer.material.renderQueue = renderringQ;

                var borderVertices = new Vector3[GeometryHelper.CIRCLR_COUNT * 2];
                var borderTriangles = new int[GeometryHelper.CIRCLR_COUNT * 3 * 2];
                for (var i = 0; i < GeometryHelper.CIRCLR_COUNT; i++)
                {
                    borderVertices[i] = unityVertices[i + 1];
                    borderVertices[i + GeometryHelper.CIRCLR_COUNT] = position + new Vector3(GeometryHelper.cosArray[i] * (radius + border), GeometryHelper.sinArray[i] * (radius + border));

                    if (i != (GeometryHelper.CIRCLR_COUNT - 1))
                    {
                        borderTriangles[i * 6] = i + 1;
                        borderTriangles[i * 6 + 1] = i;
                        borderTriangles[i * 6 + 2] = i + GeometryHelper.CIRCLR_COUNT;

                        borderTriangles[i * 6 + 3] = i + 1;
                        borderTriangles[i * 6 + 4] = i + GeometryHelper.CIRCLR_COUNT;
                        borderTriangles[i * 6 + 5] = i + 1 + GeometryHelper.CIRCLR_COUNT;
                    }
                    else
                    {
                        borderTriangles[i * 6] = 0;
                        borderTriangles[i * 6 + 1] = GeometryHelper.CIRCLR_COUNT - 1;
                        borderTriangles[i * 6 + 2] = 2 * GeometryHelper.CIRCLR_COUNT - 1;

                        borderTriangles[i * 6 + 3] = 0;
                        borderTriangles[i * 6 + 4] = 2 * GeometryHelper.CIRCLR_COUNT - 1;
                        borderTriangles[i * 6 + 5] = GeometryHelper.CIRCLR_COUNT;

                        //unityTriangles[i * 6 + sideIndex + 2] = GeometryHelper.CIRCLR_COUNT;
                        //unityTriangles[i * 6 + sideIndex + 1] = GeometryHelper.CIRCLR_COUNT + 2;
                        //unityTriangles[i * 6 + sideIndex] = 1;

                        //unityTriangles[i * 6 + sideIndex + 5] = GeometryHelper.CIRCLR_COUNT + 2;
                        //unityTriangles[i * 6 + sideIndex + 4] = GeometryHelper.CIRCLR_COUNT;
                        //unityTriangles[i * 6 + sideIndex + 3] = 2 * GeometryHelper.CIRCLR_COUNT + 1;
                    }
                }

                var borderMesh = new UnityEngine.Mesh();
                borderMesh.vertices = borderVertices;
                borderMesh.triangles = borderTriangles;
                this.borderMeshFilter.mesh = borderMesh;
            }

        }


        public void DrawCircleWithHeight(Vector3 position, float radius, float height, Material inM)
        {
            this.centerPosition = position;
            this.radius = radius;
            
            if (this.meshFilter == null)
                this.meshFilter = GetComponent<MeshFilter>();
            if (this.meshRenderer == null)
                this.meshRenderer = GetComponent<MeshRenderer>();

            this.meshRenderer.material.CopyPropertiesFromMaterial(inM);
            this.meshRenderer.material.shader = inM.shader;

            if (this.meshRenderer.material.HasProperty("_Color"))
            {
                this.meshRenderer.material.SetColor("_Color", inM.color);
            }
            this.UpdateHeight(height);
            this.meshRenderer.material.renderQueue = 3100;

            var vertexCount = GeometryHelper.CIRCLR_COUNT + 1;
            var unityVertices = new Vector3[vertexCount * 3];
            var unityTriangles = new int[GeometryHelper.CIRCLR_COUNT * 3 * 2 * 2 + GeometryHelper.CIRCLR_COUNT * 3];

            var unityUV1 = new Vector2[vertexCount * 3];
            var unityUV = new Vector2[vertexCount * 3];

            unityVertices[0] = position;
            unityVertices[GeometryHelper.CIRCLR_COUNT + 1] = position + new Vector3(0, 0, height);
            unityVertices[GeometryHelper.CIRCLR_COUNT * 2 + 2] = position + new Vector3(0, 0, height);

            unityUV1[0] = new Vector2(0.1f, 0);
            unityUV1[GeometryHelper.CIRCLR_COUNT + 1] = new Vector2(0.6f, 0);
            unityUV1[GeometryHelper.CIRCLR_COUNT * 2 + 2] = new Vector2(0.6f, 0);

            unityUV[0] = new Vector2(0.5f, 0);
            unityUV[GeometryHelper.CIRCLR_COUNT + 1] = new Vector2(0.5f, 1);
            unityUV[GeometryHelper.CIRCLR_COUNT * 2 + 2] = new Vector2(0.5f, 1);

            var sideIndex = GeometryHelper.CIRCLR_COUNT * 3 * 3;
            for (var i = 0; i < GeometryHelper.CIRCLR_COUNT; i++)
            {
                unityVertices[i + 1] = position + new Vector3(GeometryHelper.cosArray[i] * radius, GeometryHelper.sinArray[i] * radius);
                unityVertices[i + 2 + GeometryHelper.CIRCLR_COUNT] = position + new Vector3(GeometryHelper.cosArray[i] * radius, GeometryHelper.sinArray[i] * radius) + new Vector3(0, 0, height);
                unityVertices[i + 3 + GeometryHelper.CIRCLR_COUNT * 2] = unityVertices[i + 2 + GeometryHelper.CIRCLR_COUNT];

                unityUV1[i + 1] = new Vector2(0.1f, 0);
                unityUV1[i + 2 + GeometryHelper.CIRCLR_COUNT] = new Vector2(0.6f, 0);
                unityUV1[i + 3 + GeometryHelper.CIRCLR_COUNT * 2] = new Vector2(0.7f, 0);

                unityUV[i + 1] = new Vector2(0.5f, 0);
                unityUV[i + 2 + GeometryHelper.CIRCLR_COUNT] = new Vector2(0.5f, 1);
                unityUV[i + 3 + GeometryHelper.CIRCLR_COUNT * 2] = new Vector2(0.5f, 1);

                unityTriangles[i * 3] = i + 1;
                unityTriangles[i * 3 + 1] = 0;
                if (i + 2 == (GeometryHelper.CIRCLR_COUNT + 1))
                    unityTriangles[i * 3 + 2] = 1;
                else
                    unityTriangles[i * 3 + 2] = i + 2;

                unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3 + 2] = i + 2 + GeometryHelper.CIRCLR_COUNT;
                unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3 + 1] = GeometryHelper.CIRCLR_COUNT + 1;
                if (i + GeometryHelper.CIRCLR_COUNT + 3 == (GeometryHelper.CIRCLR_COUNT + 1) * 2)
                    unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3] = GeometryHelper.CIRCLR_COUNT + 2;
                else
                    unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3] = i + GeometryHelper.CIRCLR_COUNT + 3;


                unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3 * 2 + 2] = i + 3 + GeometryHelper.CIRCLR_COUNT * 2;
                unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3 * 2 + 1] = GeometryHelper.CIRCLR_COUNT * 2 + 2;
                if (i + GeometryHelper.CIRCLR_COUNT * 2 + 4 == (GeometryHelper.CIRCLR_COUNT + 1) * 3)
                    unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3 * 2] = GeometryHelper.CIRCLR_COUNT * 2 + 3;
                else
                    unityTriangles[i * 3 + GeometryHelper.CIRCLR_COUNT * 3 * 2] = i + GeometryHelper.CIRCLR_COUNT * 2 + 4;


                if (i != (GeometryHelper.CIRCLR_COUNT - 1))
                {
                    unityTriangles[i * 6 + sideIndex] = i + 2;
                    unityTriangles[i * 6 + sideIndex + 1] = i + GeometryHelper.CIRCLR_COUNT + 2;
                    unityTriangles[i * 6 + sideIndex + 2] = i + 1;

                    unityTriangles[i * 6 + sideIndex + 3] = i + GeometryHelper.CIRCLR_COUNT + 2;
                    unityTriangles[i * 6 + sideIndex + 4] = i + 2;
                    unityTriangles[i * 6 + sideIndex + 5] = i + GeometryHelper.CIRCLR_COUNT + 3;
                }
                else
                {
                    unityTriangles[i * 6 + sideIndex + 2] = GeometryHelper.CIRCLR_COUNT;
                    unityTriangles[i * 6 + sideIndex + 1] = GeometryHelper.CIRCLR_COUNT + 2;
                    unityTriangles[i * 6 + sideIndex] = 1;

                    unityTriangles[i * 6 + sideIndex + 5] = GeometryHelper.CIRCLR_COUNT + 2;
                    unityTriangles[i * 6 + sideIndex + 4] = GeometryHelper.CIRCLR_COUNT;
                    unityTriangles[i * 6 + sideIndex + 3] = 2 * GeometryHelper.CIRCLR_COUNT + 1;
                }
            }

            var unityMesh = new UnityEngine.Mesh();

            unityMesh.vertices = unityVertices;
            unityMesh.triangles = unityTriangles;
            unityMesh.SetUVs(1, unityUV1.ToList());

            unityMesh.uv = unityUV;

            this.meshFilter.mesh = unityMesh;
        }
    }
}
