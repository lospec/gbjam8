using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private void Update()
    {
        if (new[]
        {
            Keyboard.current.anyKey, Mouse.current.leftButton, Mouse.current
                .rightButton
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