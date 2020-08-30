using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrapplePrototype
{
    public class DebugPanel : MonoBehaviour
    {
        Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                canvas.enabled = !canvas.enabled;
            }
        }
    }
}
