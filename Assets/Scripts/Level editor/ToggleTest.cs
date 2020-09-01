using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTest : MonoBehaviour
{
    public Text text;
    private bool isPlaying = false;
    
    public void TogglePlayMode()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
        {
            Time.timeScale = 1;
            text.text = "STOP";
        }
        else
        {
            Time.timeScale = 0;
            text.text = "TEST";
        }
    }
}
