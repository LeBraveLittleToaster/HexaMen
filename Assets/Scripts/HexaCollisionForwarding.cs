using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexaCollisionForwarding : MonoBehaviour
{
    
    [SerializeField] private Hex hexRef;
    private Tools _currentTool = Tools.ADD;

    private void Awake()
    {
        ToolsScript.onToolChanged += OnToolChanged;
    }

    private void OnToolChanged(Tools tool)
    {
        _currentTool = tool;
    }
    
    private void OnMouseDown()
    {
        if (!Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject()) return;
        switch (_currentTool)
        {
            case Tools.ADD:
                hexRef.AddLayer();
                break;
            case Tools.SUBTRACT:
                hexRef.SubtractLayer();
                break;
            case Tools.PLACE_PLAYER:
                hexRef.PlaceOrRemoveEntity(EntityType.PLAYER);
                break;
            case Tools.PLACE_ENEMY:
                hexRef.PlaceOrRemoveEntity(EntityType.ENEMY);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}