using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Circles
{
    public interface IObjectInteractions
    {
        public void OnTap();

        public void OnDoubleTap();

        public void OnWiping();
    }
}