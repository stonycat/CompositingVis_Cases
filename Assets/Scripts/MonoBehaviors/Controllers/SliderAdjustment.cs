using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Controllers
{
    public class SliderAdjustment : MonoBehaviour
    {
        public Assets.Scripts.MonoBehaviors.Exp.CanvasRenderer canvasRenderer;
        //public VRTK.VRTK_ControllerEvents ControllerEvents;

        private void Start()
        {
            //ControllerEvents.TouchpadReleased += ControllerEvents_TouchpadReleased;
        }

        private void ControllerEvents_TouchpadReleased(object sender)
        {
            if (canvasRenderer._sliderObj.activeSelf == false) return;

            //if (e.touchpadAxis.x < 0)
            //{
            //    canvasRenderer.DeductSliderValue();
            //}
            //else
            //{
            //    canvasRenderer.AddSliderValue();
            //}
        }
    }
}
