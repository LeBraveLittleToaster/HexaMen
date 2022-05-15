using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolHighlighter : MonoBehaviour
{
    [SerializeField] private Button toolAdd;
    [SerializeField] private Button toolSubtract;
    [SerializeField] private Button toolEnemy;
    [SerializeField] private Button toolPlayer;

    [SerializeField] private Color baseButtonColor;
    [SerializeField] private Color hightlightButtonColor;

    private Button _highlightedButton;

    private void Awake()
    {
        ToolsScript.onToolChanged += OnToolChanged;
    }

    private void Start()
    {
        OnToolChanged(Tools.ADD);
    }

    private void OnToolChanged(Tools tool)
    {
        
        _highlightedButton = tool switch
        {
            Tools.ADD => toolAdd,
            Tools.SUBTRACT => toolSubtract,
            Tools.PLACE_PLAYER => toolEnemy,
            Tools.PLACE_ENEMY => toolPlayer,
            _ => throw new ArgumentOutOfRangeException(nameof(tool), tool, null)
        };

    }
}
