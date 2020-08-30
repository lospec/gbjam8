using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrapplePrototype
{
    public class PlayerInput : MonoBehaviour
    {
        public float direction = 1f;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        public float moveSpeed = 8f;
        public float grappleSpeed = 20f;

        [SerializeField] LineRenderer aimLineRenderer = null;

        //private float holdTime = 0f;

        private bool usingGrapple => Input.GetKey(KeyCode.X) || playerGrappling.grappling;
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
                aimLineRenderer.enabled = true;

            if (Input.GetKey(KeyCode.X))
            {
                Vector2 aim = GetAimDir();

                RaycastHit2D hit = Physics2D.Raycast(rigid.position, aim);
                if (hit)
                {
                    GameObject hitObject = hit.collider.gameObject;

                    if (hitObject.CompareTag("Tile"))
                    {
                        aimLineRenderer.enabled = true;
                        aimLineRenderer.SetPosition(0, rigid.position);
                        aimLineRenderer.SetPosition(1, hit.point);
                    }
                }
                else
                    aimLineRenderer.enabled = false;
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                aimLineRenderer.enabled = false;
                Grapple();
            }
        }

        private Vector2 GetAimDir()
        {
            Vector2 dir = Vector2.zero;

            if (Input.GetKey(KeyCode.UpArrow)) dir.y++;
            if (Input.GetKey(KeyCode.DownArrow)) dir.y--;

            if (Input.GetKey(KeyCode.RightArrow)) dir.x++;
            if (Input.GetKey(KeyCode.LeftArrow)) dir.x--;
            if (dir.y == 0f && dir.x == 0f) dir.x = direction;
            if (dir.x != 0f) direction = dir.x;

            return dir;
        }

        private void Grapple()
        {
            playerGrappling.StartGrappling(GetAimDir());
        }
    }
}
