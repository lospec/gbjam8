using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuTab : MonoBehaviour
{
	private void Update()
	{
		if (new[]
		{
			Keyboard.current.anyKey
		}.Any(control => control.wasPressedThisFrame))
		{
			GoToMainMenuScene();
		}
	}

	public void GoToMainMenuScene()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
