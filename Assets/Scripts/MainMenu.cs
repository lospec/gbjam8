using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void GoToMapScene()
	{
		SceneManager.LoadScene("MapScene");
	}
}
