using UnityEngine;

namespace Utility
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private HUDHealthBar health;
        [SerializeField] private HUDScoreHandler score;
        
        public HUDHealthBar HealthBar => health;
        public HUDScoreHandler Score => score;
    }
}