using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class KillEnemy : MonoBehaviour
    {
        [SerializeField] private Dissolve enemyDissolve;
        [SerializeField] private GameObject explosion;

        public void Explode()
        {
            enemyDissolve.enabled = true;
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
    }
}