using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Legend
{
    public enum LegendTextState
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }

    public class LegendController : MonoBehaviour
    {
        public GameObject HTop;
        public GameObject HBottom;
        public GameObject VLeft;
        public GameObject VRight;

        public GameObject TickLeft;
        public GameObject TickRight;

        public void HideAll()
        {
            if (this.HTop != null) this.HTop.SetActive(false);
            if (this.HBottom != null) this.HBottom.SetActive(false);
            if (this.VLeft != null) this.VLeft.SetActive(false);
            if (this.VRight != null) this.VRight.SetActive(false);

            if (this.TickLeft != null) this.TickLeft.SetActive(false);
            if (this.TickRight != null) this.TickRight.SetActive(false);
        }

        private LegendTextState textStat = LegendTextState.Top;
        public LegendTextState TextState
        {
            get
            {
                return this.textStat;
            }
            set
            {
                this.textStat = value;

                this.HideAll();
                switch (this.TextState)
                {
                    case LegendTextState.Top:
                        this.HTop.SetActive(true);
                        this.TickLeft.SetActive(true);
                        break;
                    case LegendTextState.Left:
                        this.VLeft.SetActive(true);
                        this.TickLeft.SetActive(true);
                        break;
                    case LegendTextState.Bottom:
                        this.HBottom.SetActive(true);
                        this.TickRight.SetActive(true);
                        break;
                    case LegendTextState.Right:
                        this.VRight.SetActive(true);
                        this.TickRight.SetActive(true);
                        break;
                }
            }
        }

    }
}
