using System;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(LineRenderer))]
    public class AimBeam : MonoBehaviour
    {
        private LineRenderer _line;

        private void Start()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = 2;
        }
    }
}