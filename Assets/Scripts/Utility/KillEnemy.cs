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

        private void Start()
        {
            (enemyController as FlyingEnemyController).Initialize(null,
                FlyingEnemyController.StartState.Flying);
        }

        public void Explode()
        {
            var dissolve = enemyController.GetComponentInChildren<Dissolve>();
            dissolve.enabled = true;
            Instantiate(explosion, enemyController.transform.position,
                Quaternion.identity);
        }
    }
}