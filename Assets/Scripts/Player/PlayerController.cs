using System;
using UnityEngine;

namespace Player
{
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
                        _spriteRenderer.flipX = true;
                        break;
                    case Direction.E:
                        _spriteRenderer.flipX = false;
                        break;
                }

                _playerDirection = value;
            }
            get => _playerDirection;
        }

        [SerializeField] private SpriteRenderer _spriteRenderer;

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