using Assets.Scripts.MonoBehaviors.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Controllers
{
    public class ControllerMorph : MonoBehaviour
    {
        public MapMorpher Morpher;
        //public VRTK.VRTK_ControllerEvents ControllerEvents;

        public GameObject MapObj
        {
            get
            {
                return this.Morpher.MapObj;
            }
        }

        private void Start()
        {
            //ControllerEvents.TouchpadPressed += ControllerEvents_TouchpadPressed;
        }

        private void Update()
        {
            //if(ControllerEvents.touchpadPressed)
            //{
            //    var p = ControllerEvents.GetTouchpadAxis();
            //    var local = this.Morpher.TiltedAngle;
            //    if (p.x < 0)
            //    {
            //        local -= 0.3f;
            //    }
            //    else
            //    {
            //        local += 0.3f;
            //    }
            //    this.Morpher.TiltedAngle = local;
            //}
        }
    }
}
