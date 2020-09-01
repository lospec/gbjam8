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
            LevelEditor.Instance.StartPlayMode();
            text.text = "STOP";
        }
        else
        {
            LevelEditor.Instance.EndPlayMode();
            text.text = "TEST";
        }
    }
}
