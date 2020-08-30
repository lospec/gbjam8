using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrapplePrototype
{
    public class PlayerInputIteration1 : MonoBehaviour
    {
        public float direction = 1f;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        public float moveSpeed = 8f;
        public float grappleSpeed = 20f;

        private float holdTime = 0f;

        private bool usingGrapple => holdTime >= .25f || playerGrappling.grappling;
        private PlayerGrappling playerGrappling;
        private Rigidbody2D rigid;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            playerGrappling = GetComponent<PlayerGrappling>();

            if (playerGrappling == null)
            {
                Debug.LogErrorFormat("PlayerGrappling not found on player {0}", name);
            }
        }

        private void Update()
        {
            HandleAction();

            if (usingGrapple) GetComponent<SpriteRenderer>().color = new Color(.9f, 1f, 0f);
            else GetComponent<SpriteRenderer>().color = new Color(0f, .88f, 1f);
        }

        private void FixedUpdate()
        {
            HandleMove();
        }

        private void HandleMove()
        {
            if (usingGrapple) return;

            float x = 0f;

            if (Input.GetKey(KeyCode.RightArrow)) x++;
            if (Input.GetKey(KeyCode.LeftArrow)) x--;

            if (Mathf.Abs(x) > 0f)
            {
                direction = Mathf.Sign(x);
                rigid.MovePosition(rigid.position + new Vector2(x, 0f) * moveSpeed * Time.deltaTime);
            }
        }

        private void HandleAction()
        {
            if (playerGrappling.grappling) return;

            if (Input.GetKeyDown(KeyCode.X))
                holdTime = 0f;

            if (Input.GetKey(KeyCode.X))
                holdTime += Time.deltaTime;

            if (Input.GetKeyUp(KeyCode.X))
            {
                if (usingGrapple) Grapple();
                //else Shoot();

                holdTime = 0f;
            }
        }

        private void Grapple()
        {
            float x = direction;
            float y = 0f;

            if (Input.GetKey(KeyCode.UpArrow)) y++;
            if (Input.GetKey(KeyCode.DownArrow)) y--;

            playerGrappling.StartGrappling(new Vector2(x, y));
        }
    }
}
