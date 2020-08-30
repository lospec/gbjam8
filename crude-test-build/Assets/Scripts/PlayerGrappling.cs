using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrapplePrototype
{
    public class PlayerGrappling : MonoBehaviour
    {
        public float PullSpeed { get => pullSpeed; set => pullSpeed = value; }

        public float pullSpeed = 20f;
        public float checkDistance = .1f;

        [SerializeField] LineRenderer grappleLineRenderer = null;

        public bool grappling = false;
        public Vector2 target;

        private Rigidbody2D rigid;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!grappling)
            {
                grappleLineRenderer.enabled = false;
                return;
            }


            grappleLineRenderer.enabled = true;
            grappleLineRenderer.SetPosition(0, transform.position);
            grappleLineRenderer.SetPosition(1, target);
        }

        private void FixedUpdate()
        {
            if (!grappling) return;

            Vector2 dir = (target - rigid.position).normalized;
            Vector2 vel = dir * pullSpeed;
            rigid.MovePosition(rigid.position + vel * Time.deltaTime);

            RaycastHit2D[] colliders = new RaycastHit2D[1];
            if (rigid.Cast(dir, colliders, checkDistance) > 0)
            {
                Debug.Log("DONE GRAPPLING");
                grappling = false;
            }

            if (rigid.OverlapPoint(target))
            {
                Debug.Log("DONE GRAPPLING");
                grappling = false;
            }
        }

        public void StartGrappling(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
            if (hit)
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log(hitObject.name);
                if (hitObject.CompareTag("Tile"))
                {
                    MoveTowards(hit.point);
                }
            }
        }

        public void MoveTowards(Vector2 target)
        {
            if (grappling) return;

            grappling = true;
            this.target = target;
        }
    }
}
