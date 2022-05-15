using System;
using UnityEngine;

public class ToolsScript : MonoBehaviour
{
    private Tools _currentTool = Tools.ADD;

    public delegate void OnToolChanged(Tools tool);

    public static event OnToolChanged onToolChanged;

    public void SetTool(int toolId)
    {
        _currentTool = toolId switch
        {
            0 => Tools.ADD,
            1 => Tools.SUBTRACT,
            2 => Tools.PLACE_PLAYER,
            3 => Tools.PLACE_ENEMY,
            _ => _currentTool
        };

        onToolChanged?.Invoke(_currentTool);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SetTool(0);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SetTool(1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SetTool(2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SetTool(3);
        }
    }
}

public enum Tools
{
    ADD,
    SUBTRACT,
    PLACE_PLAYER,
    PLACE_ENEMY
}