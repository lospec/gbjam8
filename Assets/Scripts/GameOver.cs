using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreHud;
    [SerializeField] private TextMeshProUGUI gameOverScore;
    
    private void Start()
    {
        gameOverScore.text = scoreHud.text.Replace("\n", "");
    }
    
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