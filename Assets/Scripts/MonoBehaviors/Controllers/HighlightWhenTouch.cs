using Assets.Scripts.MonoBehaviors.Exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//using VRTK;

namespace Assets.Scripts.MonoBehaviors.Controllers
{
    //[RequireComponent(typeof(VRTK_InteractTouch))]
    public class HighlightWhenTouch : MonoBehaviour
    {
        //private VRTK_InteractTouch toucher;
        //private VRTK_ControllerEvents events;
        private bool isInitial = false;

        private Transform triggerTransform;
        private Transform touchpadTransform;
        private Transform bodyTransform;

        public Material Highlighted;
        public Material TriggerM;
        public Material TouchpadM;
        private Material unHighlighted;

        private Transform modelObj;
        private MeshRenderer[] meshrenderers;

        public bool isTouched = false;

        //public VRTK_Pointer Pointer;
        public VisRenderer Vis;

        private void Update()
        {
            if (this.isInitial)
            {
                if (this.Vis == null) return;

                //if (Vis.IsVisShown)
                //    Pointer.enabled = false;
                //else
                //    Pointer.enabled = true;
                return;
            }

            var parentObj = gameObject.transform.parent;
            if (parentObj == null) return;

            this.modelObj = parentObj.Find("Model");
            if (this.modelObj == null) return;

            this.meshrenderers = this.modelObj.GetComponentsInChildren<MeshRenderer>();

            this.triggerTransform = this.modelObj.Find("trigger");
            if (this.triggerTransform == null) return;

            this.touchpadTransform = this.modelObj.Find("trackpad");
            if (this.triggerTransform == null) return;

            this.bodyTransform = this.modelObj.Find("body");
            if (this.bodyTransform == null) return;

            var thisMeshRenderer = this.triggerTransform.GetComponent<MeshRenderer>();
            if (thisMeshRenderer == null) return;

            this.unHighlighted = this.bodyTransform.GetComponent<MeshRenderer>().material;

            if (this.TriggerM != null)
                this.triggerTransform.GetComponent<MeshRenderer>().material = this.TriggerM;
            else if (this.Highlighted != null)
                this.triggerTransform.GetComponent<MeshRenderer>().material = this.Highlighted;

            if (this.TouchpadM != null)
                this.touchpadTransform.GetComponent<MeshRenderer>().material = this.TouchpadM;
            else if (this.Highlighted != null)
                this.touchpadTransform.GetComponent<MeshRenderer>().material = this.Highlighted;

            this.isInitial = true;
        }

        private void Start()
        {
            //this.toucher = GetComponent<VRTK_InteractTouch>();
            //this.events = GetComponent<VRTK_ControllerEvents>();

            //this.toucher.ControllerStartTouchInteractableObject += ToucherOnControllerStartTouchInteractableObject;
            //this.toucher.ControllerStartUntouchInteractableObject += ToucherOnControllerStartUntouchInteractableObject;

            //this.events.TriggerPressed += Events_TriggerPressed;
            //this.events.TriggerReleased += Events_TriggerReleased;
        }

        private void Events_TriggerReleased(object sender)
        {
            foreach (var m in this.meshrenderers)
            {
                m.enabled = true;
            }
        }

        private void Events_TriggerPressed(object sender)
        {
            if(this.isTouched)
            {
                foreach (var m in this.meshrenderers)
                {
                    m.enabled = false;
                }
            }
        }

        private void ToucherOnControllerStartUntouchInteractableObject(object sender)
        {
            if (!this.isInitial) return;
            this.bodyTransform.GetComponent<MeshRenderer>().material = this.unHighlighted;
            this.isTouched = false;
        }

        private void ToucherOnControllerStartTouchInteractableObject(object sender)
        {
            if (!this.isInitial) return;

            if (this.Highlighted != null)
                this.bodyTransform.GetComponent<MeshRenderer>().material = this.Highlighted;
            this.isTouched = true;
        }
    }
}
