using Assets.Scripts.Model;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Meshing.Algorithm;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MapSubRegionObj : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public LineRenderer rawTopRenderer;
        public LineRenderer blueTopRenderer;
        public LineRenderer greenTopRenderer;

        public LineRenderer rawBaseRenderer;
        public LineRenderer blueBaseRenderer;
        public LineRenderer greenBaseRenderer;

        private GameObject rawBoundaryObj;
        private GameObject blueBoundaryObj;
        private GameObject greenBoundaryObj;

        public void SetHighlightBoundaryMaterials(Material blueM, Material greenM)
        {
            var blueRenders = this.blueBoundaryObj.GetComponentsInChildren<LineRenderer>();
            foreach (var bR in blueRenders)
            {
                bR.sharedMaterial = blueM;
                bR.startWidth = bR.startWidth * 3;
                bR.endWidth = bR.endWidth * 3;
            }

            //var greenRenders = this.greenBoundaryObj.GetComponentsInChildren<LineRenderer>();
            //foreach (var gR in greenRenders)
            //{
            //    gR.sharedMaterial = greenM;
            //    gR.startWidth = gR.startWidth * 3;
            //    gR.endWidth = gR.endWidth * 3;
            //}

            this.HighlightNone();
        }

        public void HighlightBlue()
        {
            this.rawBoundaryObj.SetActive(false);
            this.blueBoundaryObj.SetActive(true);
            //this.greenBoundaryObj.SetActive(false);

            //if(this.isInitialedFlat)
            //{
            //    var baselineObj = this.blueBoundaryObj.transform.Find("BaseBoundary");
            //    baselineObj.gameObject.SetActive(false);
            //}
        }

        public void HighlightGreen()
        {
            this.rawBoundaryObj.SetActive(false);
            this.blueBoundaryObj.SetActive(false);
            //this.greenBoundaryObj.SetActive(true);

            //if (this.isInitialedFlat)
            //{
            //    var baselineObj = this.greenBoundaryObj.transform.Find("BaseBoundary");
            //    baselineObj.gameObject.SetActive(false);
            //}
        }

        public void HighlightNone()
        {
            this.rawBoundaryObj.SetActive(true);
            this.blueBoundaryObj.SetActive(false);
            //this.greenBoundaryObj.SetActive(false);

            //if (this.isInitialedFlat)
            //{
            //    var baselineObj = this.rawBoundaryObj.transform.Find("BaseBoundary");
            //    baselineObj.gameObject.SetActive(false);
            //}
        }

        void Start()
        {
        }

        void Update()
        {

        }

        public void DrawBoundary(MapPolygon mapPolygon, Material lineM, float width, float height = Mathf.Infinity)
        {
            var boundariesObj = new GameObject("Boundaries");
            boundariesObj.transform.parent = gameObject.transform;
            boundariesObj.transform.localPosition = Vector3.zero;
            boundariesObj.transform.localRotation = Quaternion.identity;

            this.rawBoundaryObj = new GameObject("Raw");
            this.rawBoundaryObj.transform.parent = boundariesObj.transform;
            rawBoundaryObj.transform.localPosition = Vector3.zero;
            rawBoundaryObj.transform.localRotation = Quaternion.identity;

            var boundaryObj = new GameObject("TopBoundary");
            boundaryObj.transform.parent = this.rawBoundaryObj.transform;
            boundaryObj.transform.localPosition = Vector3.zero;
            boundaryObj.transform.localRotation = Quaternion.identity;

            if (height == Mathf.Infinity) height = 0;
            boundaryObj.transform.localPosition = new Vector3(0, 0, height + RenderingSettings.MinZOffset);

            this.rawTopRenderer = boundaryObj.AddComponent<LineRenderer>();
            this.rawTopRenderer.useWorldSpace = false;
            this.rawTopRenderer.loop = true;
            this.rawTopRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            this.rawTopRenderer.positionCount = mapPolygon.Outer.Count;
            this.rawTopRenderer.SetPositions(mapPolygon.Outer.ToArray());

            this.rawTopRenderer.startWidth = width;
            this.rawTopRenderer.endWidth = width;

            this.rawTopRenderer.sharedMaterial = lineM;

            var baseBoundary = Instantiate(boundaryObj, this.rawBoundaryObj.transform);
            baseBoundary.name = "BaseBoundary";
            baseBoundary.transform.localPosition = new Vector3(0, 0, -RenderingSettings.MinZOffset);
            baseBoundary.transform.localRotation = Quaternion.identity;
            this.rawBaseRenderer = baseBoundary.GetComponent<LineRenderer>();

            //this.greenBoundaryObj = Instantiate(this.rawBoundaryObj, boundariesObj.transform);
            //this.greenBoundaryObj.name = "Green";
            //this.greenBoundaryObj.transform.localPosition = new Vector3(0, 0, 0);
            //greenBoundaryObj.transform.Find("TopBoundary").transform.localPosition = new Vector3(0, 0, height + RenderingSettings.MinZOffset * 2);
            //greenBoundaryObj.transform.Find("BaseBoundary").transform.localPosition = new Vector3(0, 0, -RenderingSettings.MinZOffset * 2);

            this.blueBoundaryObj = Instantiate(this.rawBoundaryObj, boundariesObj.transform);
            this.blueBoundaryObj.transform.localPosition = new Vector3(0, 0, 0);
            this.blueBoundaryObj.name = "Blue";
            var topBlue = blueBoundaryObj.transform.Find("TopBoundary");
            topBlue.transform.localPosition = new Vector3(0, 0, height + RenderingSettings.MinZOffset * 2);
            this.blueTopRenderer = topBlue.GetComponent<LineRenderer>();
            var baseBlue = blueBoundaryObj.transform.Find("BaseBoundary");
            baseBlue.transform.localPosition = new Vector3(0, 0, -RenderingSettings.MinZOffset * 2);
            this.blueBaseRenderer = baseBlue.GetComponent<LineRenderer>();
        }

        private IMesh TriangulatePolygon(MapPolygon mapPolygon)
        {
            var polygon = new Polygon();
            var outerContour = new Vertex[mapPolygon.Outer.Count];
            for (var i = 0; i < mapPolygon.Outer.Count; i++)
            {
                var thisP = mapPolygon.Outer[i];
                outerContour[i] = new Vertex(thisP.x, thisP.y, 1);
            }
            polygon.Add(new Contour(outerContour, 1));


            if (mapPolygon.Holes != null)
            {
                for (var i = 0; i < mapPolygon.Holes.Count; i++)
                {
                    var thisH = mapPolygon.Holes[i];
                    var holeContour = new Vertex[thisH.Count];

                    for (var j = 0; j < thisH.Count; j++)
                    {
                        var thisP = thisH[j];
                        holeContour[j] = new Vertex(thisP.x, thisP.y, i + 2);
                    }
                    polygon.Add(new Contour(holeContour, i + 2), true);
                }
            }

            var options = new ConstraintOptions()
            {
                ConformingDelaunay = true,
                SegmentSplitting = 2
            };
            var quality = new QualityOptions()
            {
                //MinimumAngle = 5
                //MinimumAngle = 2
            };

            // var algorithm = new Dwyer();
            // var mesh = polygon.Triangulate(options, quality, algorithm);

            var mesh = polygon.Triangulate(options, quality);
            return mesh;
        }

        bool isInitialedFlat = false;

        public void UpdateColor(Color c)
        {
            this.meshRenderer.material.color = c;
            this.meshRenderer.material.SetColor("_Color", c);
        }

        public void UpdateHeight(float h)
        {
            this.meshRenderer.material.SetFloat("_Height", h);

            (this.rawBoundaryObj.transform.Find("TopBoundary")).transform.localPosition = new Vector3(0, 0, h + RenderingSettings.MinZOffset);
            (this.blueBoundaryObj.transform.Find("TopBoundary")).transform.localPosition = new Vector3(0, 0, h + RenderingSettings.MinZOffset);
            //(this.greenBoundaryObj.transform.Find("TopBoundary")).transform.localPosition = new Vector3(0, 0, h + RenderingSettings.MinZOffset);
        }

        public void DrawMapRegion(MapPolygon mapPolygon)
        {
            if (this.meshFilter == null)
                this.meshFilter = GetComponent<MeshFilter>();
            if (this.meshRenderer == null)
                this.meshRenderer = GetComponent<MeshRenderer>();

            var mesh = this.TriangulatePolygon(mapPolygon);

            var triangles = mesh.Triangles;
            var vertices = mesh.Vertices;

            var unityVertices = new Vector3[vertices.Count];
            var unityTriangles = new int[triangles.Count * 3];

            var index = 0;
            foreach (var v in vertices)
            {
                unityVertices[index] = new Vector3(
                    (float)v.X,
                    (float)v.Y
                );
                index++;
            }

            index = 0;
            foreach (var t in triangles)
            {
                unityTriangles[index * 3] = t.GetVertexID(0);
                unityTriangles[index * 3 + 1] = t.GetVertexID(1);
                unityTriangles[index * 3 + 2] = t.GetVertexID(2);

                index++;
            }
            var unityMesh = new UnityEngine.Mesh();

            unityMesh.vertices = unityVertices;
            unityMesh.triangles = unityTriangles;

            this.meshFilter.mesh = unityMesh;
        }

        public void DrawMapRegion(MapPolygon mapPolygon, Material inM)
        {
            this.DrawMapRegion(mapPolygon);
            this.meshRenderer.material.CopyPropertiesFromMaterial(inM);
            //this.meshRenderer.material.renderQueue = 3000;
        }

        public void DrawMapRegion(MapPolygon mapPolygon, Material inM, Material lineM, float lineWidth = 0.0005f)
        {
            this.isInitialedFlat = true;

            this.DrawBoundary(mapPolygon, lineM, lineWidth);
            this.DrawMapRegion(mapPolygon, inM);

            //var colliderHelper = this.GetComponent<NonConvexMeshCollider>();
            //colliderHelper.Calculate();
        }

        public void DrawMapRegion(MapPolygon mapPolygon, Material inM, float height, Material lineM, float lineWidth = 0.0005f)
        {
            this.isInitialedFlat = false;

            this.DrawBoundary(mapPolygon, lineM, lineWidth, height);

            if (this.meshFilter == null)
                this.meshFilter = GetComponent<MeshFilter>();
            if (this.meshRenderer == null)
                this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshRenderer.material.CopyPropertiesFromMaterial(inM);
            this.meshRenderer.material.shader = inM.shader;

            this.meshRenderer.material.SetColor("_Color", inM.color);
            this.meshRenderer.material.SetFloat("_Height", height);

            //this.meshRenderer.receiveShadows = false;
            //this.meshRenderer.material.renderQueue = 2000;

            //if (mapPolygon.Holes != null)
            //    Debug.Log(this.name);
            //DebugFileWriter.WriteVertices(this.name, mapPolygon);

            var mesh = this.TriangulatePolygon(mapPolygon);

            var triangles = mesh.Triangles;
            var vertices = mesh.Vertices;

            var unityVertices = new Vector3[vertices.Count * 2];
            var unityTriangles = new int[triangles.Count * 3 * 2 + vertices.Count * 6];
            var unityUV1 = new Vector2[vertices.Count * 2];

            var index = 0;
            var indexOffset01 = triangles.Count * 3;
            var indexOffset02 = indexOffset01 * 2;
            foreach (var v in vertices)
            {
                unityVertices[index] = new Vector3(
                    (float)v.X,
                    (float)v.Y
                );
                unityUV1[index] = new Vector2(0.1f, 0);

                unityVertices[index + vertices.Count] = new Vector3(
                    (float)v.X,
                    (float)v.Y,
                    height
                );
                unityUV1[index + vertices.Count] = new Vector2(0.6f, 0);

                index++;
            }

            //DebugFileWriter.WriteVertices(this.name, unityVertices);

            index = 0;
            foreach (var v in vertices)
            {
                if (index < (vertices.Count - 1))
                {
                    unityTriangles[indexOffset02 + index * 6] = index + 1;
                    unityTriangles[indexOffset02 + index * 6 + 1] = index + vertices.Count;
                    unityTriangles[indexOffset02 + index * 6 + 2] = index;

                    unityTriangles[indexOffset02 + index * 6 + 3] = index + vertices.Count;
                    unityTriangles[indexOffset02 + index * 6 + 4] = index + 1;
                    unityTriangles[indexOffset02 + index * 6 + 5] = index + vertices.Count + 1;
                }
                index++;
            }

            index -= 1;
            unityTriangles[indexOffset02 + index * 6] = 0;
            unityTriangles[indexOffset02 + index * 6 + 1] = vertices.Count * 2 - 1;
            unityTriangles[indexOffset02 + index * 6 + 2] = vertices.Count - 1;

            unityTriangles[indexOffset02 + index * 6 + 3] = vertices.Count * 2 - 1;
            unityTriangles[indexOffset02 + index * 6 + 4] = 0;
            unityTriangles[indexOffset02 + index * 6 + 5] = vertices.Count;

            index = 0;
            foreach (var t in triangles)
            {
                unityTriangles[index * 3] = t.GetVertexID(2);
                unityTriangles[index * 3 + 1] = t.GetVertexID(1);
                unityTriangles[index * 3 + 2] = t.GetVertexID(0);

                unityTriangles[index * 3 + indexOffset01] = t.GetVertexID(0) + vertices.Count;
                unityTriangles[index * 3 + 1 + indexOffset01] = t.GetVertexID(1) + vertices.Count;
                unityTriangles[index * 3 + 2 + indexOffset01] = t.GetVertexID(2) + vertices.Count;

                index++;
            }
            var unityMesh = new UnityEngine.Mesh();
            unityMesh.vertices = unityVertices;
            unityMesh.triangles = unityTriangles;
            unityMesh.SetUVs(1, unityUV1.ToList());

            this.meshFilter.mesh = unityMesh;

            //var colliderHelper = this.GetComponent<NonConvexMeshCollider>();
            //colliderHelper.Calculate();
        }
    }
}
