using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class VRTKHelper
    {
        private static GameObject LeftControllerObj;
        private static GameObject RightControllerObj;

        static VRTKHelper()
        {
            var playerObj = Camera.main.transform.parent;
            Transform leftObj = null;
            Transform rightObj = null;

            if(playerObj.name == "[CameraRig]")
            {
                leftObj = playerObj.transform.Find("Controller (left)");
                rightObj = playerObj.transform.Find("Controller (right)");
            }
            else if(playerObj.name == "VRSimulatorCameraRig")
            {
                leftObj = playerObj.transform.Find("LeftHand");
                rightObj = playerObj.transform.Find("RightHand");
            }

            if(leftObj != null) LeftControllerObj = leftObj.Find("LeftController").gameObject;
            if(rightObj != null) RightControllerObj = rightObj.Find("RightController").gameObject;
        }

        public static void EnableAllPointerGrab()
        {
            //ProcessPointers((pointer) => pointer.interactWithObjects = true);
        }

        public static void DisableAllPointerGrab()
        {
            //ProcessPointers((pointer) => pointer.interactWithObjects = false);
        }

        //public static List<VRTK.VRTK_Pointer> ProcessPointers(Action<VRTK.VRTK_Pointer> action = null)
        //{
        //    var result = new List<VRTK.VRTK_Pointer>();
        //    if(LeftControllerObj != null)
        //    {
        //        var leftPointer = LeftControllerObj.GetComponent<VRTK.VRTK_Pointer>();
        //        if (leftPointer != null)
        //        {
        //            action(leftPointer);
        //            result.Add(leftPointer);
        //        }
        //    }

        //    if (RightControllerObj != null)
        //    {
        //        var rightPointer = RightControllerObj.GetComponent<VRTK.VRTK_Pointer>();
        //        if (rightPointer != null)
        //        {
        //            action(rightPointer);
        //            result.Add(rightPointer);
        //        }
        //    }
        //    return result;
        //}


    }
}
