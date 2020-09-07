using System;
using UnityEngine;

namespace Weapon.Hook
{
    public class HookAimGraphics : MonoBehaviour
    {
        public float aimLineDistance = 0f;

        [SerializeField] private GrapplingGun grapplingGun = default;
        [SerializeField] private LineRenderer aimLine = default;

        private void Start()
        {
            GrapplingGun.OnHookRetracted += GrappleHookRetracted;
            GrapplingGun.OnPullEnded += GrapplePullEnded;
            GrapplingGun.OnHookShot += GrappleHookShot;
        }

        private void Update()
        {
            var aim = grapplingGun.AimInput.normalized;
            var d = aimLineDistance;

            if (d <= 0f) d = Mathf.Infinity;
            var hit = Physics2D.Raycast(grapplingGun.HookOrigin,
                grapplingGun.AimInput, d);

            if (hit) d = hit.distance;
            else
                d = aimLineDistance <= 0f
                    ? grapplingGun.maxShootDistance
                    : aimLineDistance;

            aimLine.SetPosition(0, grapplingGun.HookOrigin);
            aimLine.SetPosition(1, grapplingGun.HookOrigin + aim * d);
        }

        private void OnDisable()
        {
            aimLine.enabled = false;
        }

        private void OnDestroy()
        {
            GrapplingGun.OnHookRetracted -= GrappleHookRetracted;
            GrapplingGun.OnPullEnded -= GrapplePullEnded;
            GrapplingGun.OnHookShot -= GrappleHookShot;
        }

        #region Callbacks

        private void GrappleHookRetracted() =>
            EnableAimLine();

        private void GrapplePullEnded(bool arrivedAtTarget, Collider2D targetObject, Collider2D collidedObject) =>
            EnableAimLine();
        
        private void GrappleHookShot(float speed, Vector2 target, Transform hook, Action finishShooting) =>
            DisableAimLine();

        #endregion

        private void DisableAimLine()
        {
            aimLine.enabled = false;
            enabled = false;
        }

        private void EnableAimLine()
        {
            aimLine.enabled = true;
            enabled = true;
        }
    }
}