using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        private Direction _playerDirection = Direction.E;
        public Direction PlayerDirection
        {
            set
            {
                switch (value)
                {
                    case Direction.W:
                        spriteRenderer.flipX = true;
                        break;
                    case Direction.E:
                        spriteRenderer.flipX = false;
                        break;
                }
                _playerDirection = value;
            }
            get => _playerDirection;
        }

        public SpriteRenderer spriteRenderer;

        private void Update()
        {
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput != 0)
            {
                PlayerDirection = horizontalInput >= 0 ? Direction.E : Direction.W;
            }
        }
    }
}