using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrapplePrototype
{
    public class DebugTextSetter : MonoBehaviour
    {
        public string text = "Player Value: {0}";
        public void UpdateValue(float val) => GetComponent<UnityEngine.UI.Text>().text = string.Format(text, val);
    }
}
