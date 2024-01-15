using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors
{
    public class InitialMapPos : MonoBehaviour
    {

        private bool _isInitialed = false;
        // Update is called once per frame
        void Update()
        {
            if (_isInitialed || Camera.main == null)
                return;
            if (_isInitialed == false)
            {
                TransUtility.InitialMap(gameObject, Camera.main.transform);
            }

            this._isInitialed = true;
        }
    }
}
