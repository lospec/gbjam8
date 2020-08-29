using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum Direction
	{
		RIGHT = 1,
		LEFT = -1
	}

	private Direction _playerDirection = Direction.RIGHT;
	public Direction PlayerDirection
	{
		set
		{
			SpriteRenderer.flipX = Direction.LEFT == value;
			_playerDirection = value;
		}
		get
		{
			return _playerDirection;
		}
	}

	public SpriteRenderer SpriteRenderer;

	private void Start()
	{
		SpriteRenderer = (SpriteRenderer)GetComponent("SpriteRenderer");
	}

	private void Update()
    {
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		if (horizontalInput != 0)
		{
			PlayerDirection = (Direction)horizontalInput;
			
		}
    }
}
