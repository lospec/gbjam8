using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    // SCRIPT IS FOR TESTING! DO NOT USE FOR RELEASE BUILD
    public class KillEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private GameObject explosion;

        public void Explode()
        {
            var dissolve = GetComponentInChildren<Dissolve>();
            dissolve.enabled = true;
            Instantiate(explosion, transform.position,
                Quaternion.identity);
        }
    }
}