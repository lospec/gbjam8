using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
	public void GoToMainMenuScene()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
