using Assets.Scripts.Model;
using Assets.Scripts.Projections;
using Assets.Scripts.Projections.Map;
using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Scales;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors
{
    public class ThematicMapObj : MonoBehaviour
    {
        public GeoName GeoName = GeoName.US;
        public ThematicType MapType = ThematicType.Choropleth;

        public string PropName = "Area";


        public List<MapSubRegionObj> SubRegionObjs;

        public Material MapBoundaryMaterial;
        public Material MapBoundaryBlueMaterial;
        public Material MapBoundaryGreenMaterial;

        public Material FlatMapMaterial;
        public Material ElevatedMapMaterial;
        public Material BarMaterial;
        public Material CircleMaterial;
        public Material CircleBorderMaterial;

        public Color DefaultMapColor = new Color(0.6f, 0.6f, 0.6f);
        public Color DefaultBarColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);

        //public new Rigidbody rigidbody;

        public float MinHeight = 0.000f;
        public float MaxHeight = 0.2f;

        public float MaxArea = 0.0001f * 2;
        public float MinArea = 0.0001f / 40000;

        public float DefaultArea = 0.0001f;

        public TransferFuncType FuncType = TransferFuncType.Linear;

        public PaletteName ColorName = PaletteName.YlOrBr;

        private NumericData numericData;
        private MapData mapData;

        public Dictionary<string, List<MapSubRegionObj>> name2SubRegions = new Dictionary<string, List<MapSubRegionObj>>();
        public Dictionary<string, MapRegionCircle> name2Circle = new Dictionary<string, MapRegionCircle>();
        public Dictionary<string, MapRegionLabel> name2Label = new Dictionary<string, MapRegionLabel>();

        private Dictionary<string, List<float>> barChartData;

        private INumbericScale HeightScaler;
        private INumbericScale AreaScaler;
        private IColorScale ColorScaler;

        private IProj proj;
        public FitMapProj FitMapProj;

        private MapData mapBoundaryData;
        private FitMapProj fitMapBoundaryProj;

        private List<string> highlightedRegions;

        private bool isInitialled = false;

        public Vector3 CameraRightInMap
        {
            get
            {
                var cameraRight = Camera.main.transform.right;
                cameraRight.y = 0;
                cameraRight.Normalize();
                var MapObj = this.gameObject.transform.parent;
                var direction = MapObj.InverseTransformDirection(cameraRight);
                direction.z = 0;
                direction.Normalize();

                return direction;
            }
        }
        void Start()
        {
            this.DrawMap($"{Settings.DataDir}{this.GeoName.ToString()}-Data.csv");
            // this.UpdateMapData($"{Settings.DataDir}{this.GeoName.ToString()}-Data-Test.csv");
            transform.parent.parent.parent.GetComponent<Map>().Init(barChartData);
            
        }

        private void Doit()
        {
            var defaultLineWidth = 0.0005f;
            var geoString = this.GeoName.ToString();
            if (this.GeoName == GeoName.US)
            {
                defaultLineWidth *= 3;
            }
            this.mapData = new MapData();
            this.mapData.LoadGeoJson($"{Settings.DataDir}{geoString}-Map.json");
            this.mapData.LoadCentroid($"{Settings.DataDir}{geoString}-Centroid.csv");
            this.mapData.LoadAbbr($"{Settings.DataDir}{geoString}-Abbr.csv");
            this.mapBoundaryData = new MapData();
            this.mapBoundaryData.LoadGeoJson($"{Settings.DataDir}{geoString}-Map-Boundary.json");

            this.proj = new Hammer(Settings.Location2geoCenter[geoString]);
            this.FitMapProj = new FitMapProj(this.proj, this.mapData);
            this.FitMapProj.FitExtent(Settings.MapSize);

            this.fitMapBoundaryProj = new FitMapProj(this.proj, this.mapBoundaryData);
            this.fitMapBoundaryProj.FitExtent(Settings.MapSize);
            this.SetupScales($"{Settings.DataDir}{this.GeoName}-Data.csv");
            Action<MapSubRegionObj, MapPolygon, Color32, float> subRegionAction = null;
            Action<MapRegionCircle, Vector3, int, float, float> RegionAction = null;
            Action<MapRegionLabel, string, Vector3, float> LabelAction = null;
            subRegionAction = (MapSubRegionObj thisMapRef, MapPolygon thisPoly, Color32 c, float h) =>
            {
                this.FlatMapMaterial.color = c;
                this.MapBoundaryMaterial.renderQueue = 2900;
                this.MapBoundaryBlueMaterial.renderQueue = 3000;
                this.MapBoundaryGreenMaterial.renderQueue = 3000;

                thisMapRef.DrawMapRegion(thisPoly, this.FlatMapMaterial, this.MapBoundaryMaterial, defaultLineWidth);
                thisMapRef.SetHighlightBoundaryMaterials(this.MapBoundaryBlueMaterial, this.MapBoundaryGreenMaterial);
            };

            RegionAction = (MapRegionCircle objRef, Vector3 p, int rank, float h, float r) =>
            { };
            LabelAction = (MapRegionLabel objRef, string name, Vector3 p, float h) =>
            {
                float txtScale = 1;
                float outter = 0.1f;
                if (this.GeoName == GeoName.EU)
                {
                    txtScale = 0.3f;
                    outter = 0.3f;
                }
                objRef.DrawLabel(name, p, 0, txtScale, outter);
            };
            this.name2SubRegions.Clear();
            this.name2Circle.Clear();
            foreach (var item in FitMapProj.projMapData.Name2Polygons)
            {
                var thisName = item.Key;
                var thisPolygons = item.Value;

                GameObject thisPart = null;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).name == thisName)
                    {
                        thisPart = transform.GetChild(i).gameObject;
                    }
                }
                //var thisPart = new GameObject(thisName);
                //thisPart.transform.parent = this.gameObject.transform;
                //thisPart.transform.localPosition = Vector3.zero;
                //thisPart.transform.localRotation = Quaternion.identity;

                //var subRegionParent = new GameObject("SubRegions");
                //subRegionParent.transform.parent = thisPart.transform;
                //subRegionParent.transform.localPosition = Vector3.zero;
                //subRegionParent.transform.localRotation = Quaternion.identity;

                var thisV = this.numericData.GetValue(thisName);

                var thisH = this.HeightScaler.Project(thisV);
                var thisC = this.ColorScaler.Project(thisV);
                var thisR = this.AreaScaler.Project(thisV);

                //Debug.Log($"{thisName}: {thisR}");

                var index = 0;
                var subRegions = new List<MapSubRegionObj>();
                foreach (var polygon in thisPolygons)
                {
                    //Debug.Log("Draw " + thisName + "_" + index);
                    var subRegionName = $"{thisName}_{index}";
                    //var thisPoly = new GameObject($"{thisName}_{index}");
                    var thisPoly = SubRegionObjs[0];
                    for (var i = 1; i < SubRegionObjs.Count; i++)
                    {
                        if (SubRegionObjs[i].name == subRegionName)
                        {
                            thisPoly = SubRegionObjs[i];
                            break;
                        }
                    }
                    index++;
                    //thisPoly.transform.parent = subRegionParent.transform;
                    //thisPoly.transform.localPosition = Vector3.zero;
                    //thisPoly.transform.localRotation = Quaternion.identity;


                    var thisRegion = thisPoly.GetComponent<MapSubRegionObj>();
                    subRegions.Add(thisRegion);

                    subRegionAction(thisRegion, polygon, thisC, thisH);
                }
                this.name2SubRegions.Add(thisName, subRegions);

                var thisP = FitMapProj.projMapData.Name2GeoPoint[thisName];
                //var circleObj = new GameObject("Circle");
                //circleObj.transform.parent = thisPart.transform;
                //var mapPartCircle = circleObj.AddComponent<MapRegionCircle>();
                //this.name2Circle.Add(thisName, mapPartCircle);

                var rank = this.numericData.GetRank(thisName);
                //RegionAction(mapPartCircle, thisP, rank, thisH, Mathf.Sqrt(thisR / Mathf.PI));

                if (LabelAction != null)
                {
                    var labelObj = new GameObject("Label");
                    labelObj.transform.parent = thisPart.transform;
                    labelObj.transform.localPosition = Vector3.zero;
                    //labelObj.transform.localScale = new Vector3(1, 1, 1);

                    var mapLabel = labelObj.AddComponent<MapRegionLabel>();
                    this.name2Label.Add(thisName, mapLabel);

                    LabelAction(mapLabel, this.mapData.Name2Abbr[thisName], thisP, thisH);
                }
            }
            this.SetupColliders();
        }

        void SetupColliders()
        {
            var colliders = new GameObject("0-Colliders");
            colliders.transform.parent = gameObject.transform;
            colliders.transform.localRotation = Quaternion.identity;
            colliders.transform.localPosition = Vector3.zero;

            foreach (var item in fitMapBoundaryProj.projMapData.Name2Polygons)
            {
                var thisName = item.Key;
                var thisPolygons = item.Value;

                var thisPart = new GameObject(thisName);
                thisPart.transform.parent = colliders.transform;
                thisPart.transform.localRotation = Quaternion.identity;
                thisPart.transform.localPosition = Vector3.zero;

                var subRegionParent = new GameObject("SubRegions");
                subRegionParent.transform.parent = thisPart.transform;
                subRegionParent.transform.localRotation = Quaternion.identity;
                subRegionParent.transform.localPosition = Vector3.zero;
                
                var index = 0;
                foreach (var polygon in thisPolygons)
                {
                    var thisPoly = new GameObject($"{thisName}_{index}");
                    index++;
                    thisPoly.transform.parent = subRegionParent.transform;

                    var thisRegion = thisPoly.AddComponent<MapSubRegionObj>();
                    thisRegion.DrawMapRegion(polygon);

                    var convexCalculator = thisPoly.AddComponent<NonConvexMeshCollider>();
                    convexCalculator.Calculate();

                    thisPoly.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

        void SetupScales(string fileName)
        {
            this.numericData = new NumericData(fileName);
            if (this.PropName == "") throw new ArgumentException("Property of the numberic data needs to be assigned.");
            this.numericData.PropName = this.PropName;

            var thisMin = this.numericData.GetMin();
            var thisMax = this.numericData.GetMax();

            Func<float, float> func = Scale.Sqrt;
            switch (this.FuncType)
            {
                case TransferFuncType.Linear:
                    func = Scale.Linear;
                    break;
                case TransferFuncType.Sqrt:
                    func = Scale.Sqrt;
                    break;
                case TransferFuncType.Log:
                    func = Scale.Log;
                    break;
            }

            if (thisMax > 0 && thisMin >= 0)
            {
                //thisMin = 0;
                this.HeightScaler = new Scale(thisMin, thisMax, this.MinHeight, this.MaxHeight, func);
                this.AreaScaler = new Scale(thisMin, thisMax, this.MinArea, this.MaxArea, func);
                switch(this.ColorName)
                {
                    case PaletteName.YlGn:
                        this.ColorScaler = new ColorMapper(thisMin, thisMax, ColorBrewerPalette.YlGn(9).colors, func);
                        break;
                    case PaletteName.YlOrBr:
                        this.ColorScaler = new ColorMapper(thisMin, thisMax, ColorBrewerPalette.YlOrBr(9).colors, func);
                        break;
                }
            }
            else if (thisMax > 0 && thisMin < 0)
            {
                this.HeightScaler = new DivergingNumbericScale(thisMin, thisMax, this.MinHeight, this.MaxHeight, func);
                this.AreaScaler = new OneDirNegative(thisMin, thisMax, this.MinArea, this.MaxArea, func);
                this.ColorScaler = new DivergingColorMapper(thisMin, thisMax, ColorBrewerPalette.RdYlGn(11).colors, func);
            }
        }

        private List<string> highlighted = new List<string>();
        public void ClearHighlighted()
        {
            foreach(var r in highlighted)
            {
                this.UnHighlight(r);
            }
            highlighted.Clear();
        }

        private void UnHighlight(string name)
        {
            if (this.name2SubRegions.ContainsKey(name))
            {
                var regions = this.name2SubRegions[name];
                foreach (var r in regions)
                {
                    r.HighlightNone();
                }
                this.name2Label[name].HighlightNone();
            }
            else
            {
                Debug.LogError($"{name} does not contain in the map");
            }
        }

        public void HightlightBlue(string name)
        {
            if(this.name2SubRegions.ContainsKey(name))
            {
                this.highlighted.Add(name);
                var regions = this.name2SubRegions[name];
                foreach(var r in regions)
                {
                    r.HighlightBlue();
                }

                if (this.name2Label.ContainsKey(name))
                {
                    this.name2Label[name].HighlightBlue();
                }
            }
            else
            {
                Debug.LogError($"{name} does not contain in the map");
            }
        }
        
        public void HightlightGreen(string name)
        {
            if (this.name2SubRegions.ContainsKey(name))
            {
                this.highlighted.Add(name);
                var regions = this.name2SubRegions[name];
                foreach (var r in regions)
                {
                    r.HighlightGreen();
                }

                if (this.name2Label.ContainsKey(name))
                {
                    this.name2Label[name].HighlightGreen();
                }
            }
            else
            {
                Debug.LogError($"{name} does not contain in the map");
            }
        }

        public void HightlightBlue(IEnumerable<string> names)
        {
            foreach(var n in names)
            {
                this.HightlightBlue(n);
            }
        }


        public void HightlightGreen(IEnumerable<string> names)
        {
            foreach (var n in names)
            {
                this.HightlightGreen(n);
            }
        }

        private void UpdateMapData(string fileName)
        {
            this.SetupScales(fileName);

            Action<MapSubRegionObj, Color32, float> subRegionAction = null;
            Action<MapRegionCircle, int, float, float> RegionAction = null;
            Action<MapRegionLabel, float> LabelAction = null;

            switch (this.MapType)
            {
                case ThematicType.Choropleth:
                    subRegionAction = (MapSubRegionObj thisMapRef, Color32 c, float h) =>
                    {

                        thisMapRef.UpdateColor(c);
                    };

                    RegionAction = (MapRegionCircle objRef, int rank, float h, float r) =>{};
                    break;
                case ThematicType.ElevatedBars:
                    subRegionAction = (MapSubRegionObj thisMapRef, Color32 c, float h) =>{};

                    RegionAction = (MapRegionCircle objRef, int rank, float h, float r) =>
                    {
                        objRef.UpdateHeight(h);
                    };
                    break;
                case ThematicType.Prism:
                    subRegionAction = (MapSubRegionObj thisMapRef, Color32 c, float h) =>
                    {
                        thisMapRef.UpdateHeight(h);
                    };

                    RegionAction = (MapRegionCircle objRef, int rank, float h, float r) =>{};
                    break;
                case ThematicType.ColoredPrism:
                    subRegionAction = (MapSubRegionObj thisMapRef, Color32 c, float h) =>
                    {
                        thisMapRef.UpdateHeight(h);
                        thisMapRef.UpdateColor(c);
                    };

                    RegionAction = (MapRegionCircle objRef, int rank, float h, float r) =>{};

                    LabelAction = (MapRegionLabel objRef, float h) =>
                    {
                        objRef.RawHeight = h;
                        objRef.SetOffset(RenderingSettings.MinZOffset * (1 / (this.gameObject.transform.lossyScale.z / 0.3f)) * 4);
                    };
                    break;
                case ThematicType.PropotionalSymbols:
                    subRegionAction = (MapSubRegionObj thisMapRef, Color32 c, float h) =>{};

                    RegionAction = (MapRegionCircle objRef, int rank, float h, float r) =>
                    {
                        objRef.UpdateRadius(r);
                        objRef.UpdateRenderringQ(3001 + (rank < 0 ? 0 : rank));
                    };
                    break;
            }

            foreach (var item in FitMapProj.projMapData.Name2Polygons)
            {
                var thisName = item.Key;
                var thisPolygons = item.Value;

                var thisV = this.numericData.GetValue(thisName);

                var thisH = this.HeightScaler.Project(thisV);
                var thisC = this.ColorScaler.Project(thisV);
                var thisR = this.AreaScaler.Project(thisV);

                var subRegions = this.name2SubRegions[thisName];

                var index = 0;
                foreach (var polygon in thisPolygons)
                {
                    var subRegion = subRegions[index];
                    index++;

                    subRegionAction(subRegion, thisC, thisH);
                }

                var region = this.name2Circle[thisName];
                var rank = this.numericData.GetRank(thisName);
                RegionAction(region, rank, thisH, Mathf.Sqrt(thisR / Mathf.PI));

                if(this.name2Label.ContainsKey(thisName))
                {
                    var label = this.name2Label[thisName];
                    LabelAction(label, thisH);
                }
            }
        }

        public void DrawMap(string fileName, bool setColliders = true, bool drawText = true)
        {
            if (isInitialled)
            {
                this.UpdateMapData(fileName);
                Debug.Log("DrawMap returned");
                return;
            }
            Debug.Log("DrawMap not returned");
            this.isInitialled = true;

            //this.transform.localPosition = new Vector3(0, 1, 1);
            this.transform.localPosition = Vector3.zero;
            //Quaternion rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            this.transform.localRotation = Quaternion.identity;
            //this.transform.localRotation = rotation;

            var defaultLineWidth = 0.0005f;
            var geoString = this.GeoName.ToString();
            if (this.GeoName == GeoName.US)
            {
                defaultLineWidth *= 3;
            }
            
            Debug.Log("Initializing mapData");
            this.mapData = new MapData();
            this.mapData.LoadGeoJson($"{Settings.DataDir}{geoString}-Map.json");
            this.mapData.LoadCentroid($"{Settings.DataDir}{geoString}-Centroid.csv");
            this.mapData.LoadAbbr($"{Settings.DataDir}{geoString}-Abbr.csv");
            Debug.Log("Initializing mapData complete");

            this.mapBoundaryData = new MapData();
            this.mapBoundaryData.LoadGeoJson($"{Settings.DataDir}{geoString}-Map-Boundary.json");

            this.proj = new Hammer(Settings.Location2geoCenter[geoString]);
            this.FitMapProj = new FitMapProj(this.proj, this.mapData);
            this.FitMapProj.FitExtent(Settings.MapSize);

            this.fitMapBoundaryProj = new FitMapProj(this.proj, this.mapBoundaryData);
            this.fitMapBoundaryProj.FitExtent(Settings.MapSize);

            Debug.Log("SetupScale executing! File name is " + fileName);
            this.SetupScales(fileName);
            Debug.Log("SetupScale executed!");
            Action<MapSubRegionObj, MapPolygon, Color32, float> subRegionAction = null;
            Action<MapRegionCircle, Vector3, int, float, float> RegionAction = null;
            Action<MapRegionLabel, string, Vector3, float> LabelAction = null;
            Debug.Log("MapType is " + this.MapType);
            switch (this.MapType)
            {
                case ThematicType.Choropleth:
                    subRegionAction = (MapSubRegionObj thisMapRef, MapPolygon thisPoly, Color32 c, float h) =>
                    {
                        this.FlatMapMaterial.color = c;
                        this.MapBoundaryMaterial.renderQueue = 2900;
                        this.MapBoundaryBlueMaterial.renderQueue = 3000;
                        this.MapBoundaryGreenMaterial.renderQueue = 3000;

                        thisMapRef.DrawMapRegion(thisPoly, this.FlatMapMaterial, this.MapBoundaryMaterial, defaultLineWidth);
                        thisMapRef.SetHighlightBoundaryMaterials(this.MapBoundaryBlueMaterial, this.MapBoundaryGreenMaterial);
                    };

                    RegionAction = (MapRegionCircle objRef, Vector3 p, int rank, float h, float r) =>
                    {};
                    LabelAction = (MapRegionLabel objRef, string name, Vector3 p, float h) =>
                    {
                        float txtScale = 1;
                        float outter = 0.1f;
                        if (this.GeoName == GeoName.EU)
                        {
                            txtScale = 0.3f;
                            outter = 0.3f;
                        }
                        objRef.DrawLabel(name, p, 0, txtScale, outter);
                    };
                    break;
                case ThematicType.ElevatedBars:
                    subRegionAction = (MapSubRegionObj thisMapRef, MapPolygon thisPoly, Color32 c, float h) =>
                    {
                        this.FlatMapMaterial.color = this.DefaultMapColor;
                        thisMapRef.DrawMapRegion(thisPoly, this.FlatMapMaterial, this.MapBoundaryMaterial);
                    };

                    RegionAction = (MapRegionCircle objRef, Vector3 p, int rank, float h, float r) =>
                    {
                        this.BarMaterial.color = this.DefaultBarColor;
                        
                        objRef.DrawCircleWithHeight(p, Mathf.Sqrt(this.DefaultArea / Mathf.PI), h, this.BarMaterial);
                    };
                    break;
                case ThematicType.Prism:
                    subRegionAction = (MapSubRegionObj thisMapRef, MapPolygon thisPoly, Color32 c, float h) =>
                    {
                        this.ElevatedMapMaterial.color = this.DefaultMapColor;
                        this.MapBoundaryMaterial.renderQueue = 2900;
                        this.MapBoundaryBlueMaterial.renderQueue = 3000;
                        this.MapBoundaryGreenMaterial.renderQueue = 3000;

                        thisMapRef.DrawMapRegion(thisPoly, this.ElevatedMapMaterial, h, this.MapBoundaryMaterial, defaultLineWidth);
                        thisMapRef.SetHighlightBoundaryMaterials(this.MapBoundaryBlueMaterial, this.MapBoundaryGreenMaterial);
                    };

                    RegionAction = (MapRegionCircle objRef, Vector3 p, int rank, float h, float r) =>
                    {};
                    break;
                case ThematicType.ColoredPrism:
                    subRegionAction = (MapSubRegionObj thisMapRef, MapPolygon thisPoly, Color32 c, float h) =>
                    {
                        this.ElevatedMapMaterial.color = c;
                        this.MapBoundaryMaterial.renderQueue = 2900;
                        this.MapBoundaryBlueMaterial.renderQueue = 3000;
                        this.MapBoundaryGreenMaterial.renderQueue = 3000;

                        thisMapRef.DrawMapRegion(thisPoly, this.ElevatedMapMaterial, h, this.MapBoundaryMaterial, defaultLineWidth);
                        thisMapRef.SetHighlightBoundaryMaterials(this.MapBoundaryBlueMaterial, this.MapBoundaryGreenMaterial);
                    };

                    RegionAction = (MapRegionCircle objRef, Vector3 p, int rank, float h, float r) =>
                    {};

                    LabelAction = (MapRegionLabel objRef, string name, Vector3 p, float h) =>
                    {
                        float txtScale = 1;
                        float outter = 0.1f;
                        if (this.GeoName == GeoName.EU)
                        {
                            txtScale = 0.3f;
                            outter = 0.3f;
                        }
                        objRef.DrawLabel(name, p, h, txtScale, outter);
                    };
                    break;
                case ThematicType.PropotionalSymbols:
                    subRegionAction = (MapSubRegionObj thisMapRef, MapPolygon thisPoly, Color32 c, float h) =>
                    {
                        this.FlatMapMaterial.color = this.DefaultMapColor;
                        thisMapRef.DrawMapRegion(thisPoly, this.FlatMapMaterial, this.MapBoundaryMaterial);
                    };

                    RegionAction = (MapRegionCircle objRef, Vector3 p, int rank, float h, float r) =>
                    {
                        this.CircleMaterial.color = this.DefaultBarColor;
                        this.CircleBorderMaterial.color = this.DefaultMapColor;
                        objRef.DrawCircle(p, r, this.CircleMaterial, 3001 + (rank < 0 ? 0 : rank), this.CircleBorderMaterial);
                    };
                    break;
            }

            this.name2SubRegions.Clear();
            this.name2Circle.Clear();

            barChartData = new Dictionary<string, List<float>>();

            foreach (var item in FitMapProj.projMapData.Name2Polygons)
            {
                var thisName = item.Key;
                var thisPolygons = item.Value;

                var thisPart = new GameObject(thisName);
                thisPart.transform.parent = this.gameObject.transform;
                thisPart.transform.localPosition = Vector3.zero;
                thisPart.transform.localRotation = Quaternion.identity;

                var subRegionParent = new GameObject("SubRegions");
                subRegionParent.transform.parent = thisPart.transform;
                subRegionParent.transform.localPosition = Vector3.zero;
                subRegionParent.transform.localRotation = Quaternion.identity;

                var thisV = this.numericData.GetValue(thisName);
                barChartData.Add(thisName, new List<float> { thisV });

                var thisH = this.HeightScaler.Project(thisV);
                var thisC = this.ColorScaler.Project(thisV);
                var thisR = this.AreaScaler.Project(thisV);

                var index = 0;
                var subRegions = new List<MapSubRegionObj>();
                foreach (var polygon in thisPolygons)
                {
                    var thisPoly = new GameObject($"{thisName}_{index}");
                    Debug.Log("Draw " + thisName + "_" + index);
                    index++;
                    thisPoly.transform.parent = subRegionParent.transform;
                    thisPoly.transform.localPosition = Vector3.zero;
                    thisPoly.transform.localRotation = Quaternion.identity;


                    var thisRegion = thisPoly.AddComponent<MapSubRegionObj>();
                    subRegions.Add(thisRegion);

                    subRegionAction(thisRegion, polygon, thisC, thisH);
                }
                this.name2SubRegions.Add(thisName, subRegions);

                var thisP = FitMapProj.projMapData.Name2GeoPoint[thisName];
                var circleObj = new GameObject("Circle");
                circleObj.transform.parent = thisPart.transform;
                var mapPartCircle = circleObj.AddComponent<MapRegionCircle>();
                this.name2Circle.Add(thisName, mapPartCircle);

                var rank = this.numericData.GetRank(thisName);
                RegionAction(mapPartCircle, thisP, rank, thisH, Mathf.Sqrt(thisR / Mathf.PI));

                if(LabelAction != null && drawText)
                {
                    var labelObj = new GameObject("Label");
                    labelObj.transform.parent = thisPart.transform;
                    labelObj.transform.localPosition = Vector3.zero;
                    //labelObj.transform.localScale = new Vector3(1, 1, 1);

                    var mapLabel = labelObj.AddComponent<MapRegionLabel>();
                    this.name2Label.Add(thisName, mapLabel);
                    
                    LabelAction(mapLabel, this.mapData.Name2Abbr[thisName], thisP, thisH);
                }
            }
            if(setColliders)
                this.SetupColliders();
        }

        public void SetLabelHight(Dictionary<string, float> barHeights)
        {
            foreach (var (name, label) in name2Label)
            {
                label.ChangeHeight(barHeights[name]);
            }
        }

        public void ResetLabelHight()
        {
            Action<MapRegionLabel, string, Vector3, float> LabelAction = (MapRegionLabel objRef, string name, Vector3 p, float h) =>
            {
                float txtScale = 1;
                float outter = 0.1f;
                if (this.GeoName == GeoName.EU)
                {
                    txtScale = 0.3f;
                    outter = 0.3f;
                }
                objRef.DrawLabel(name, p, 0, txtScale, outter);
            };
            foreach (var (name, label) in name2Label)
            {
                LabelAction(label, mapData.Name2Abbr[name], FitMapProj.projMapData.Name2GeoPoint[name], 0);
            }
        }
    }
}
