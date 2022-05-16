using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hex : MonoBehaviour
{
    [SerializeField] private Transform hexPrefab;
    [SerializeField] private float hexPrefabHeight;
    [SerializeField] private Transform hexCollider;
    [SerializeField] private Transform enemyEntityPrefab;
    [SerializeField] private Transform playerEntityPrefab;
    [SerializeField] private Color highlightColor;

    private List<Transform> _layers;
    private Transform _entity = null;
    private EntityType _entityType = EntityType.NONE;
    private int _column, _row;
    private Color _baseColor;


    private void Awake()
    {
        _layers = new List<Transform> {hexPrefab};
        _baseColor = _layers[^1].GetComponent<Renderer>().material.color;
    }

    public Hex Initiate(int column, int row)
    {
        _column = column;
        _row = row;
        return this;
    }
    
    public void ReInitiate(int column, int row, int height, EntityType entityType)
    {
        _column = column;
        _row = row;
        if (_entity != null)
        {
            Destroy(_entity.gameObject);
            _entityType = EntityType.NONE;
        }

        _entityType = entityType;
        if (_entityType != EntityType.NONE)
        {
            PlaceOrRemoveEntity(entityType);
        }

        var isFirst = true;
        foreach (var layer in _layers)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                Destroy(layer.gameObject);
            }
        }

        for (var i = 1; i < height; i++)
        {
            AddLayer();
        }
        RefreshColliderPosition();
    }

    public void AddLayer()
    {
        if (_entity != null) return;
        var position = transform.position;
        var pos = new Vector3(position.x, position.y + (_layers.Count) * hexPrefabHeight,
            position.z);
        var instance = Instantiate(hexPrefab, pos, Quaternion.identity);
        instance.parent = transform;
        _layers.Add(instance);
        RefreshColliderPosition();
    }

    public void SubtractLayer()
    {
        if (_layers.Count <= 1 || _entity != null) return;
        var topLayer = _layers[^1];
        Destroy(topLayer.gameObject);
        _layers.RemoveAt(_layers.Count - 1);
        RefreshColliderPosition();
    }

    public void PlaceOrRemoveEntity(EntityType entityType)
    {
        if (_entity != null)
        {
            Destroy(_entity.gameObject);
            _entityType = EntityType.NONE;
            return;
        }

        var position = transform.position;
        var pos = new Vector3(
            position.x,
            position.y + (_layers.Count * hexPrefabHeight),
            position.z
        );
        _entity = entityType switch
        {
            EntityType.PLAYER => Instantiate(playerEntityPrefab, pos, Quaternion.identity),
            EntityType.ENEMY => Instantiate(enemyEntityPrefab, pos, Quaternion.identity),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };
        _entity.parent = transform;
        _entityType = entityType;
    }

    private void RefreshColliderPosition()
    {
        hexCollider.transform.position = _layers[^1].position;
    }

    public SerializedHex GetSerialized()
    {
        var sHex = new SerializedHex
        {
            column = _column,
            row = _row,
            layerHeight = _layers.Count,
            hexType = _entityType switch
            {
                EntityType.NONE => SerializedHexType.NONE,
                EntityType.PLAYER => SerializedHexType.PLAYER,
                EntityType.ENEMY => SerializedHexType.ENEMY,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
        return sHex;
    }

    public void HightLightHex(bool isHighlighted)
    {
        _layers[^1].GetComponent<Renderer>().material.color = isHighlighted ? highlightColor : _baseColor;
    }
}