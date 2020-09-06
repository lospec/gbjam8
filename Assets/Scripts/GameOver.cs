using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
	private void Awake()
	{
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Keyboard.current.enterKey.wasPressedThisFrame)
		{
			Restart();
		}
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
