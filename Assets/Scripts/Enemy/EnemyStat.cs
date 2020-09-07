using System;
using UnityEngine.Serialization;

namespace Enemy
{
    [Serializable]
    public struct EnemyStat
    {
        public string enemyName;
        public int maxHealth;
        public int moveSpeed;
        public int damage;
    }
}