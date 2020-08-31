using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hook.Prototype
{
    public class PlayerHookGraphics : MonoBehaviour
    {
        [SerializeField] PlayerHookA playerHook;
        [SerializeField] LineRenderer grappleLine;
        [SerializeField] LineRenderer aimLine;

        public float aimLineDistance = 0f;

        private void FixedUpdate()
        {
            if (playerHook.ShowHook)
            {
                grappleLine.enabled = true;
                aimLine.enabled = false;

                grappleLine.SetPosition(0, playerHook.HookOrigin);
                grappleLine.SetPosition(1, playerHook.HookPosition);
            }
            else
            {
                grappleLine.enabled = false;
                aimLine.enabled = true;

                Vector2 aim = playerHook.Aim.normalized;
                float d = aimLineDistance;

                if (d <= 0f) d = Mathf.Infinity;
                RaycastHit2D hit = Physics2D.Raycast(playerHook.HookOrigin, playerHook.Aim, d);

                if (hit) d = hit.distance;
                else d = aimLineDistance <= 0f ? playerHook.maxHookDistance : aimLineDistance;

                aimLine.SetPosition(0, playerHook.HookOrigin);
                aimLine.SetPosition(1, playerHook.HookOrigin + aim * d);
            }
        }

        private void OnDisable()
        {
            grappleLine.enabled = false;
            aimLine.enabled = false;
        }
    }
}
