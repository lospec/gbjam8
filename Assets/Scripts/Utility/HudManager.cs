using UnityEngine;

namespace Utility
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private HUDCounterHandler hearts;
        [SerializeField] private HUDCounterHandler ammo;
        [SerializeField] private HUDScoreHandler score;
        
        public HUDCounterHandler HeartCounter => hearts;
        public HUDCounterHandler AmmoCounter => ammo;
        public HUDScoreHandler Score => score;
    }
}