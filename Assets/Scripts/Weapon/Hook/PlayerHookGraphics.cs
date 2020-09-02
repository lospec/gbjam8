using System;
using UnityEngine;

namespace Weapon.Hook
{
    public class PlayerHookGraphics : MonoBehaviour
    {
        public float aimLineDistance = 0f;

        [SerializeField] private GrapplingGun grapplingGun;
        [SerializeField] private LineRenderer aimLine;


        private void Start()
        {
            GrapplingGun.OnHookRetracted += EnableAimLine;
            GrapplingGun.OnPullEnded += EnableAimLine;
            GrapplingGun.OnHookShot += DisableAimLine;
        }


        private void Update()
        {
            var aim = grapplingGun.Aim.normalized;
            var d = aimLineDistance;

            if (d <= 0f) d = Mathf.Infinity;
            var hit = Physics2D.Raycast(grapplingGun.HookOrigin,
                grapplingGun.Aim, d);

            if (hit) d = hit.distance;
            else
                d = aimLineDistance <= 0f
                    ? grapplingGun.maxHookDistance
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
            GrapplingGun.OnHookRetracted -= EnableAimLine;
            GrapplingGun.OnPullEnded -= EnableAimLine;
            GrapplingGun.OnHookShot -= DisableAimLine;
        }

        private void DisableAimLine(float arg1, Vector2 arg2, Transform arg3,
            Action arg4)
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