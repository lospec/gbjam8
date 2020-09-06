using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Restart();
        }
    }

    private void OnValidate()
    {
        gameObject.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}