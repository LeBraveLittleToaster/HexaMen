using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using TMPro;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color textHightlightColor = Color.red;
    [SerializeField] private float hexHeight;
    [SerializeField] private float textOffset = 0.25f;

    [SerializeField] private Transform hexPrefab;

    private Color _textBaseColor;
    private List<Transform> _layers;

    private void Awake()
    {
        _textBaseColor = text.color;
        _layers = new List<Transform> { hexPrefab };
    }

    public void SetHighlighted(bool isHighlighted)
    {
        text.color = isHighlighted ? textHightlightColor : _textBaseColor;
    }
    
    public TileScript SyncWithHexTile(IHexTile hexTile)
    {
        text.SetText(hexTile.GetPosition().X + ":" + hexTile.GetPosition().Y);
        SyncLayerHeight(hexTile.GetLayerHeight());
        return this;
    }

    private void SyncLayerHeight(int layerHeight)
    {
        if (layerHeight <= 1) return;
        var heightDiff = layerHeight - _layers.Count;
        if (heightDiff > 0)
        {
            for (var i = 0; i < heightDiff; i++)
            {
                AddLayer();
            }
        }
        else
        {
            for (var i = 0; i < -heightDiff; i++)
            {
                RemoveLayer();
            }
        }
    }

    private void RemoveLayer()
    {
        try
        {
            Destroy(_layers[^1].gameObject);
            _layers.RemoveAt(_layers.Count - 1);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.Log("");
        }
    }

    private void AddLayer()
    {
        var position = transform.position;
        var pos = new Vector3(position.x, hexHeight * _layers.Count, position.z);
        var instance = Instantiate(hexPrefab, pos, Quaternion.identity);
        instance.parent = this.transform;
        _layers.Add(instance);
        var posText = new Vector3(position.x, (hexHeight * _layers.Count) +textOffset, position.z);
        text.transform.position = posText;
    }
}