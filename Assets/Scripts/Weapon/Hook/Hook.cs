using System;
using System.Collections;
using UnityEngine;

namespace Weapon.Hook
{
    [RequireComponent(typeof(LineRenderer))]
    public class Hook : MonoBehaviour
    {
        [SerializeField] private int ropeResolution = 100;
        [SerializeField] private float waveAmplitude = 2;
        [SerializeField] private int waveFrequency = 2;
        [SerializeField] private int waveOscillation = 4;
        [SerializeField] private float waveStrengthDistanceModifier = 10;
        private Coroutine _retractRoutine = default;


        private Coroutine _shootRoutine = default;
        private LineRenderer Line { get; set; }

        private void Start()
        {
            Line = GetComponent<LineRenderer>();
            Line.useWorldSpace = true;
            Line.positionCount = 0;
            enabled = false;
            GrapplingGun.HookShot += GrapplingGunOnHookShot;
            GrapplingGun.HookRetract += GrapplingGunOnHookRetract;
            GrapplingGun.EndPull += GrapplingGunOnEndPull;
        }


        private void Update()
        {
            Line.SetPosition(0, transform.position);
        }

        private void OnDestroy()
        {
            GrapplingGun.HookShot -= GrapplingGunOnHookShot;
            GrapplingGun.HookRetract -= GrapplingGunOnHookRetract;
            GrapplingGun.EndPull -= GrapplingGunOnEndPull;
        }

        private void GrapplingGunOnEndPull()
        {
            Line.enabled = false;
        }


        private void GrapplingGunOnHookShot(float speed, Vector2 target, Transform hook,
            Action callBack)
        {
            hook.SetParent(null, true);
            Line.enabled = true;
            if (_shootRoutine != null) StopCoroutine(_shootRoutine);
            if (_retractRoutine != null) StopCoroutine(_retractRoutine);
            _shootRoutine = StartCoroutine(ShootHook(speed, target, hook, callBack));
        }


        private void GrapplingGunOnHookRetract(float speed, Transform hook,
            Action callBack)
        {
            _retractRoutine = StartCoroutine(RetractHook(speed, hook, callBack));
        }

        private IEnumerator ShootHook(float speed, Vector2 target, Transform hook,
            Action callBack)
        {
            enabled = false;
            Line.positionCount = ropeResolution;
            var d = Vector2.Distance(hook.position, target);
            Vector2 hookPosition;
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime * speed / d;

                SetRopePoints(transform.position, target, t, d, out hookPosition);
                hook.position = hookPosition;
                yield return null;
            }

            SetRopePoints(transform.position, target, 1f, d, out hookPosition);

            hook.position = hookPosition;
            Line.Simplify(1f);
            enabled = true;
            callBack.Invoke();
        }

        private IEnumerator RetractHook(float speed, Transform hook, Action callBack)
        {
            var t = 0f;
            var startPosition = hook.position;
            var d = Vector2.Distance(startPosition, transform.position);
            while (t <= 1f)
            {
                t += Time.deltaTime * speed / d;
                hook.position = Vector2.Lerp(startPosition, transform.position, t);
                Line.SetPosition(1, hook.position);
                yield return null;
            }

            hook.position = Vector2.Lerp(startPosition, transform.position, 1f);
            Line.enabled = false;
            enabled = false;
            callBack.Invoke();
        }

        private void SetRopePoints(Vector2 startPoint, Vector2 targetPoint, float
            t, float distance, out Vector2 ropeEnd)
        {
            var angle = GetAngle(targetPoint - startPoint);
            ropeEnd = Vector2.Lerp(startPoint, targetPoint, t);
            var length = Vector2.Distance(startPoint, ropeEnd);
            for (var i = 0; i < ropeResolution; i++)
            {
                var xPos = (float) i / ropeResolution * length;
                var reversePercent = 1 - t;
                var amplitude = Mathf.Log(distance / waveStrengthDistanceModifier + 1) *
                                Mathf.Sin(reversePercent * waveOscillation * Mathf.PI) *
                                ((1f -
                                  (float) i / ropeResolution) * waveAmplitude);
                var yPos =
                    Mathf.Sin((float) waveFrequency * i / ropeResolution * 2 *
                              Mathf.PI *
                              reversePercent) * amplitude;

                var p = new Vector2(xPos + startPoint.x, yPos + startPoint.y);
                var pos = Rotate(p, startPoint, angle);
                Line.SetPosition(i, pos);
            }
        }

        private static float GetAngle(Vector2 target)
        {
            return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        }

        private static Vector2 Rotate(Vector2 point, Vector2 pivot, float angle)
        {
            var dir = point - pivot;
            dir = Quaternion.Euler(0, 0, angle) * dir;
            point = dir + pivot;
            return point;
        }
    }
}