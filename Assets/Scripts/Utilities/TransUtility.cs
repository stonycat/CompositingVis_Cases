using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class TransUtility
    {
        private static GameObject Temp4TransMain;
        private static string transObjName = "temp4Trans";

        static TransUtility()
        {
            Temp4TransMain = GameObject.Find(transObjName);
            if (Temp4TransMain == null)
                Temp4TransMain = new GameObject(transObjName);
        }

        private static void ResetTrans(Transform trans)
        {
            trans.rotation = Quaternion.identity;
            trans.position = Vector3.zero;
            trans.localScale = new Vector3(1, 1, 1);
        }

        public static void ResetMain()
        {
            ResetTrans(Temp4TransMain.transform);
        }

        public static Transform GetMainTrans()
        {
            return Temp4TransMain.transform;
        }

        public static void InitialMap(GameObject mapObj, Transform cameraObj, float distance = 0.6f, float angle=45, float hAngle = 0)
        {
            var position = cameraObj.transform.position;
            var direction = cameraObj.transform.forward;

            direction.y = 0;
            direction.Normalize();

            var right = cameraObj.transform.right;
            right.y = 0;
            right.Normalize();

            var cameraUp = Vector3.Cross(direction, right);
            direction = Quaternion.AngleAxis(hAngle, cameraUp) * direction;

            var newP = position + direction * distance;
            mapObj.transform.position = newP;
            mapObj.transform.LookAt(position);
            mapObj.transform.Rotate(new Vector3(1, 0, 0), -angle);
            
            newP.y -= 0.1f;
            mapObj.transform.position = newP;
        }
    }
}
