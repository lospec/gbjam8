using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    // SCRIPT IS FOR TESTING! DO NOT USE FOR RELEASE BUILD
    public class KillEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private Dissolve enemyDissolve;
        
        [SerializeField] private GameObject explosion;
        
        public void Explode()
        {
            enemyDissolve.enabled = true;
            Instantiate(explosion, enemyController.transform.position,
                Quaternion.identity);
        }
    }
}