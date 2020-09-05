using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public static class EventValidator
    {
        private static void ConnectEvent<T>(UnityEvent<T> @event, UnityAction<T> action)
        {
            for (var i = 0; i < @event.GetPersistentEventCount(); i++)
            {
                if (ReferenceEquals(@event.GetPersistentTarget(i), action.Target) &&
                    @event.GetPersistentMethodName(i) == action.Method.Name)
                {
                    UnityEventTools.RegisterPersistentListener(@event, i, action);
                    return;
                }
            }

            UnityEventTools.AddPersistentListener(@event, action);
        }

        public static void ValidatePlayerHud(HudManager hud, GameObject player)
        {
            if (!hud || !player)
            {
                return;
            }

            ConnectEvent(player.GetComponent<PlayerHealth>().OnHealthSet,
                hud.HealthBar.SetHUDHealth);
            ConnectEvent(
                player.GetComponent<EntityScore>().OnScoreSet,
                hud.Score.SetHUDScore);
        }
    }
}