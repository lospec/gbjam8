using System.Collections;
using UnityEngine;

namespace Weapon.Hook
{
    [RequireComponent(typeof(LineRenderer))]
    public class Rope : MonoBehaviour
    {
        [SerializeField] private int resolution, waveFrequency, oscillation;
        [SerializeField] private float waveAmplitude, speed;

        public bool Active { get; private set; } = false;

        public LineRenderer Line { get; private set; }
        [SerializeField] private GrapplingGun gun;

        private Coroutine _ropeRoutine;

        private void Start()
        {
            Line = GetComponent<LineRenderer>();
            Line.useWorldSpace = true;
            Line.positionCount = 0;
            enabled = false;
        }

        private void Update()
        {
            Line.SetPosition(0, gun.HookOrigin);
            Line.SetPosition(1, gun.HookPosition);
        }


        public void StartConnect()
        {
            Line.enabled = true;

            if (_ropeRoutine != null)
            {
                StopCoroutine(_ropeRoutine);
            }
            
            _ropeRoutine = StartCoroutine(AnimateRopeConnect(gun.Target));
        }


        private IEnumerator AnimateRopeConnect(Vector2 target)
        {
            enabled = false;
            Active = true;
            Line.positionCount = resolution;
            var percent = 0f;
            while (percent <= 1f)
            {
                var currentPos = gun.HookOrigin;
                percent += Time.deltaTime * speed;
                SetPoints(currentPos, target, percent);
                yield return null;
            }

            SetPoints(gun.HookOrigin, gun.HookPosition, 1f);
            Line.Simplify(1f);
            enabled = true;
            Active = false;
        }

        private void SetPoints(Vector2 startPoint, Vector2 targetPoint, float percent)
        {
            var angle = GetAngle(targetPoint - startPoint);
            var ropeEnd = gun.HookPosition =
                Vector2.Lerp(startPoint, targetPoint, percent);
            var length = Vector2.Distance(startPoint, ropeEnd);
            for (var i = 0; i < resolution; i++)
            {
                var xPos = (float) i / resolution * length;
                var reversePercent = 1 - percent;
                var amplitutde =
                    Mathf.Sin(reversePercent * oscillation * Mathf.PI) * ((1f -
                        (float) i / resolution) * waveAmplitude);
                var yPos =
                    Mathf.Sin((float) waveFrequency * i / resolution * 2 * Mathf.PI *
                              reversePercent) * amplitutde;


                var p = new Vector2(xPos + startPoint.x, yPos + startPoint.y);
                var pos = Rotate(p, startPoint, angle);
                Line.SetPosition(i, pos);
            }
        }

        private float GetAngle(Vector2 target)
        {
            return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        }

        private Vector2 Rotate(Vector2 point, Vector2 pivot, float angle)
        {
            var dir = point - pivot;
            dir = Quaternion.Euler(0, 0, angle) * dir;
            point = dir + pivot;
            return point;
        }
    }
}