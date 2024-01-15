using Assets.Scripts.Model.db;
using Assets.Scripts.MonoBehaviors.Dynamic;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Exp
{
    public class VisRenderer : MonoBehaviour
    {
        private const string DataDir = "./Data/Exp/Questions/";

        public GameObject EUObj;
        public GameObject USObj;

        public VisController EUMagic;
        public VisController EUSideBySide;
        public VisController EUButtonChange;

        public VisController USMagic;
        public VisController USSideBySide;
        public VisController USButtonChange;

        public VisController CurrentMap;
        public bool IsVisShown => this.gameObject.activeSelf;

        private void Start()
        {
            var propName = "newDensity";

            EUMagic.MagicMap.Map.PropName = propName;
            EUSideBySide.SideBySideChoropleth.Map.PropName = propName;
            EUSideBySide.SideBySideChoropleth.Map.PropName = propName;
            EUSideBySide.SideBySideChoropleth.Map.PropName = propName;
            EUButtonChange.ButtonChange.Map.PropName = propName;

            USMagic.MagicMap.Map.PropName = propName;
            USSideBySide.SideBySideChoropleth.Map.PropName = propName;
            USSideBySide.SideBySideChoropleth.Map.PropName = propName;
            USSideBySide.SideBySideChoropleth.Map.PropName = propName;
            USButtonChange.ButtonChange.Map.PropName = propName;

            var defaultColor = new Color(0.7450981f, 0.7294118f, 0.854902f);

            EUMagic.MagicMap.Map.DefaultMapColor = defaultColor;
            EUSideBySide.SideBySideChoropleth.Map.DefaultMapColor = defaultColor;
            EUSideBySide.SideBySideChoropleth.Map.DefaultMapColor = defaultColor;
            EUSideBySide.SideBySideChoropleth.Map.DefaultMapColor = defaultColor;
            EUButtonChange.ButtonChange.Map.DefaultMapColor = defaultColor;

            USMagic.MagicMap.Map.DefaultMapColor = defaultColor;
            USSideBySide.SideBySideChoropleth.Map.DefaultMapColor = defaultColor;
            USSideBySide.SideBySideChoropleth.Map.DefaultMapColor = defaultColor;
            USSideBySide.SideBySideChoropleth.Map.DefaultMapColor = defaultColor;
            USButtonChange.ButtonChange.Map.DefaultMapColor = defaultColor;


            //this.ShowVis(GeoName.EU, VisType.SideBySide);
        }

        //private void Update()
        //{
        //    //var a = this.CurrentMap.LookAtProgress;
        //    Debug.Log(this.CurrentMap.LookAtProgress);
        //}

        private void ShowVis()
        {
            this.gameObject.SetActive(true);
            this.CurrentMap.ResetPosition();
        }

        public void ShowVis(GeoName country, VisType vis)
        {
            switch (country)
            {
                case GeoName.EU:
                    this.EUObj.SetActive(true);
                    this.USObj.SetActive(false);

                    switch (vis)
                    {
                        case VisType.Magic:
                            this.EUMagic.gameObject.SetActive(true);
                            this.EUSideBySide.gameObject.SetActive(false);
                            this.EUButtonChange.gameObject.SetActive(false);

                            this.CurrentMap = this.EUMagic;
                            break;
                        case VisType.SideBySide:
                            this.EUMagic.gameObject.SetActive(false);
                            this.EUSideBySide.gameObject.SetActive(true);
                            this.EUButtonChange.gameObject.SetActive(false);

                            this.CurrentMap = this.EUSideBySide;
                            break;
                        case VisType.ButtonChange:
                            this.EUMagic.gameObject.SetActive(false);
                            this.EUSideBySide.gameObject.SetActive(false);
                            this.EUButtonChange.gameObject.SetActive(true);

                            this.CurrentMap = this.EUButtonChange;
                            break;
                    }
                    break;
                case GeoName.US:
                    this.EUObj.SetActive(false);
                    this.USObj.SetActive(true);

                    switch (vis)
                    {
                        case VisType.Magic:
                            this.USMagic.gameObject.SetActive(true);
                            this.USSideBySide.gameObject.SetActive(false);
                            this.USButtonChange.gameObject.SetActive(false);

                            this.CurrentMap = this.USMagic;
                            break;
                        case VisType.SideBySide:
                            this.USMagic.gameObject.SetActive(false);
                            this.USSideBySide.gameObject.SetActive(true);
                            this.USButtonChange.gameObject.SetActive(false);

                            this.CurrentMap = this.USSideBySide;
                            break;
                        case VisType.ButtonChange:
                            this.USMagic.gameObject.SetActive(false);
                            this.USSideBySide.gameObject.SetActive(false);
                            this.USButtonChange.gameObject.SetActive(true);

                            this.CurrentMap = this.USButtonChange;
                            break;
                    }
                    break;
            }
            this.ShowVis();
        }

        public void HideVis()
        {
            this.gameObject.SetActive(false);
        }

        public void DrawQuestion(Question q, bool highlighted = true)
        {
            var dataFile = $"{DataDir}{q.FileName}.csv";
            if (this.CurrentMap.MagicMap != null)
            {
                this.CurrentMap.MagicMap.Map.DrawMap(dataFile);
                this.CurrentMap.MagicMap.BaseMap.DrawMap(dataFile, false, false);
                this.CurrentMap.MagicMap.Map.ClearHighlighted();
            }

            if (this.CurrentMap.SideBySideChoropleth != null)
            {
                this.CurrentMap.SideBySideChoropleth.Map.DrawMap(dataFile);
                this.CurrentMap.SideBySideChoropleth.Map.ClearHighlighted();
            }
            if (this.CurrentMap.SideBySidePrism != null)
            {
                this.CurrentMap.SideBySidePrism.Map.DrawMap(dataFile);
                this.CurrentMap.SideBySidePrism.BaseMap.DrawMap(dataFile, false, false);
                this.CurrentMap.SideBySidePrism.Map.ClearHighlighted();
            }
            if (this.CurrentMap.SideBySideBars != null)
            {
                this.CurrentMap.SideBySideBars.Map.DrawMap(dataFile);
                this.CurrentMap.SideBySideBars.Map.ClearHighlighted();
                this.CurrentMap.SideBySideBars.Progress = 0.99f;
                this.CurrentMap.SideBySideBars.Progress = 1f;
            }


            if (this.CurrentMap.ButtonChange != null)
            {
                this.CurrentMap.ButtonChange.Map.DrawMap(dataFile);
                this.CurrentMap.ButtonChange.BaseMap.DrawMap(dataFile, false, false);
                this.CurrentMap.ButtonChange.Map.ClearHighlighted();
            }


            if (highlighted)
            {
                var targets = q.Targets.Split(new string[] { ";;" }, StringSplitOptions.None);
                switch (q.Task)
                {
                    case TaskType.Region:
                        if (this.CurrentMap.MagicMap != null)
                        {
                            this.CurrentMap.MagicMap.Map.HightlightBlue(targets);
                        }


                        if (this.CurrentMap.SideBySideChoropleth != null)
                        {
                            this.CurrentMap.SideBySideChoropleth.Map.HightlightBlue(targets);
                        }
                        if (this.CurrentMap.SideBySidePrism != null)
                        {
                            this.CurrentMap.SideBySidePrism.Map.HightlightBlue(targets);
                        }
                        if (this.CurrentMap.SideBySideBars != null)
                        {
                            this.CurrentMap.SideBySideBars.Map.HightlightBlue(targets);
                        }


                        if (this.CurrentMap.ButtonChange != null)
                        {
                            this.CurrentMap.ButtonChange.Map.HightlightBlue(targets);
                        } 
                        break;
                }
            }
        }
        
    }

}
