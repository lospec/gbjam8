using System;

namespace Enemy
{
    [Serializable]
    public struct EnemyStat
    {
        public string EnemyName { get; set; }
        public int MaxHealth { get; set; }
        public int MoveSpeed { get; set; }
    }
}