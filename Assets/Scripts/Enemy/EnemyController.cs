using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyStat enemyStat;

        public Transform Player { get; set; }

        [SerializeField] protected Rigidbody2D body;
        [SerializeField] protected Animator animator;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        private List<MethodInfo> _trackedRoutines = new List<MethodInfo>();

        protected Vector2 velocity;

        public EnemyStat EnemyStat => enemyStat;
        protected Vector2 DirectionToPlayer =>
            (Player.position - transform.position).normalized;
        protected float DistanceToPlayer =>
            Vector2.Distance(Player.position, transform.position);


        protected virtual void Start()
        {
        }

        protected void Initialize(Transform player)
        {
            Player = player;
        }

        protected void InvokeWhen(Action action, Func<bool> predicate, float rate,
            bool tracked = true)
        {
            Func<Coroutine> routine = () =>
                StartCoroutine(Repeat(action, predicate, rate));
            if (!tracked)
            {
                routine.Invoke();
                return;
            }

            StartCoroutine(Track(routine));
        }

        private IEnumerator Track(Func<Coroutine> routine)
        {
            if (_trackedRoutines.Contains(routine.Method))
            {
                yield break;
            }

            _trackedRoutines.Add(routine.Method);

            yield return routine.Invoke();
            _trackedRoutines.Remove(routine.Method);
        }

        private static IEnumerator Repeat(Action action, Func<bool>
                predicate,
            float repeatRate)
        {
            var rate = new WaitForSeconds(repeatRate);
            while (!predicate.Invoke())
            {
                yield return rate;
            }

            action.Invoke();
        }
    }
}