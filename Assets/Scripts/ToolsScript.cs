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
            Debug.Log("Hello: " + 1);
            SetTool(0);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Debug.Log("Hello: " + 2);
            SetTool(1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Debug.Log("Hello: " + 3);
            SetTool(2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            Debug.Log("Hello: " + 4);
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