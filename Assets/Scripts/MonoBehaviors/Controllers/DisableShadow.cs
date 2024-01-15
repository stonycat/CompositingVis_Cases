using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Controllers
{
    public class DisableShadow : MonoBehaviour
    {
        bool isInitial = false;
        private void Start()
        {
            
        }

        private void Update()
        {
            if (this.isInitial) return;

            var parentObj = gameObject.transform.parent;
            if (parentObj == null) return;
            var modelObj = parentObj.Find("Model");
            if (modelObj == null) return;

            for (var i = 0; i < modelObj.childCount; i++)
            {
                var childObj = modelObj.GetChild(i);
                var meshRenderer = childObj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    this.isInitial = true;
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    meshRenderer.receiveShadows = false;
                }
            }


        }
    }
}
