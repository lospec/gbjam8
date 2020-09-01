using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolChanger : MonoBehaviour
{
    public LevelEditor.Tool tool;
    
    public void ChangeTool()
    {
        LevelEditor.Instance.SetTool(tool);
    }
}
