using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Dynamic
{
    public abstract class BaseMorpher : IMorpher
    {
        protected float _progress = -1;
        protected bool _isProgressChanged = false;
        public virtual float Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                this._isProgressChanged = false;
                var tmp = value;
                tmp = tmp > 1 ? 1 : tmp;
                tmp = tmp < 0 ? 0 : tmp;

                if (Mathf.Abs(tmp - this.Progress) < Mathf.Epsilon)
                {
                    return;
                }
                this._progress = tmp;
                this._isProgressChanged = true;
            }
        }
    }
}
