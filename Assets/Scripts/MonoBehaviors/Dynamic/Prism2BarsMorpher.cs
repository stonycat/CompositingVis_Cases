using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public class Prism2BarsMorpher : BaseMorpher
    {
        public ThematicMapObj ColoredPrism { get; private set; }
        public Dictionary<string, List<MapSubRegionHideMorpher>> name2HideMorphers;
        public Dictionary<string, MapSubRegion2BarMorpher> name2BarMorphers;

        public override float Progress
        {
            set
            {
                base.Progress = value;
                if (this.Progress == 0) this.UpdateDirection();
                if (!this._isProgressChanged) return;

                foreach(var map2Bar in this.name2BarMorphers.Values)
                {
                    map2Bar.Progress = this.Progress;
                }

                foreach(var mapHides in this.name2HideMorphers.Values)
                {
                    foreach(var mapHide in mapHides)
                    {
                        mapHide.Progress = this.Progress * 3;
                    }
                }
            }
        }

        public void UpdateDirection()
        {
            //var direction = this.ColoredPrism.CameraRightInMap;

            ////Debug.Log(cameraRight);
            ////Debug.Log(direction);
            ////Debug.Log("=======================");

            //foreach (var region in this.name2BarMorphers.Values)
            //{
            //    region.UpdateDirection(direction);
            //}
        }

        public Prism2BarsMorpher(ThematicMapObj coloredPrism, float widthEach)
        {
            this.ColoredPrism = coloredPrism;
            this.name2BarMorphers = new Dictionary<string, MapSubRegion2BarMorpher>();
            this.name2HideMorphers = new Dictionary<string, List<MapSubRegionHideMorpher>>();
            
            foreach (var subRegions in this.ColoredPrism.name2SubRegions)
            {
                var thisName = subRegions.Key;
                var regions = subRegions.Value;

                var largestRegionIndex = -1;
                var largestCount = -1;

                var thisIndex = 0;
                foreach (var r in regions)
                {
                    var thisCount = r.rawTopRenderer.positionCount;
                    if (thisCount > largestCount)
                    {
                        largestCount = thisCount;
                        largestRegionIndex = thisIndex;
                    }
                    thisIndex++;
                }

                var morphers = new List<MapSubRegionHideMorpher>();
                var centroidP = coloredPrism.FitMapProj.projMapData.Name2GeoPoint[thisName];
                for (var i = 0; i < regions.Count; i++)
                {
                    var r = regions[i];
                    if(i == largestRegionIndex)
                    {
                        name2BarMorphers.Add(thisName, new MapSubRegion2BarMorpher(r, centroidP, widthEach));
                    }
                    else
                    {
                        morphers.Add(new MapSubRegionHideMorpher(r, centroidP));
                    }

                }
                this.name2HideMorphers.Add(thisName, morphers);
            }
        }
    }
}
