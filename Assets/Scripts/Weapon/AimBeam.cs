using System;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(LineRenderer))]
    public class AimBeam : MonoBehaviour
    {
        private LineRenderer _line;
        [SerializeField] private float offset = 2f;

        public Vector2 Target { get; set; }
        public Vector2 InputDirection { get; set; }

        private void Start()
        {
            enabled = false;
        }

        private void Update()
        {
            _line.SetPosition(1, Target);
            var position = transform.position;
            _line.SetPosition(0,
                position + ((Vector3) Target - position).normalized * offset);
        }

        private void OnEnable()
        {
            if (!_line)
            {
                _line = GetComponent<LineRenderer>();
            }

            _line.enabled = true;
            _line.positionCount = 2;
        }

        private void OnDisable()
        {
            _line.enabled = false;
        }
    }
}