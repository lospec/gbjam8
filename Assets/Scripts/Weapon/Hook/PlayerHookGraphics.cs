using UnityEngine;

namespace Weapon.Hook
{
    public class PlayerHookGraphics : MonoBehaviour
    {
        [SerializeField] GrapplingGun playerHook;
        [SerializeField] LineRenderer aimLine;

        public float aimLineDistance = 0f;

        private void FixedUpdate()
        {
            if (playerHook.ShowHook)
            {
                aimLine.enabled = false;
                return;
            }

            aimLine.enabled = true;

            Vector2 aim = playerHook.Aim.normalized;
            float d = aimLineDistance;

            if (d <= 0f) d = Mathf.Infinity;
            RaycastHit2D hit = Physics2D.Raycast(playerHook.HookOrigin,
                playerHook.Aim, d);

            if (hit) d = hit.distance;
            else
                d = aimLineDistance <= 0f
                    ? playerHook.maxHookDistance
                    : aimLineDistance;

            aimLine.SetPosition(0, playerHook.HookOrigin);
            aimLine.SetPosition(1, playerHook.HookOrigin + aim * d);
        }

        private void OnDisable()
        {
            aimLine.enabled = false;
        }
    }
}