using System;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    [SerializeField] private Transform hexPrefab;
    [SerializeField] private float hexPrefabHeight;
    [SerializeField] private Transform hexCollider;
    [SerializeField] private Transform enemyEntityPrefab;
    [SerializeField] private Transform playerEntityPrefab;

    private List<Transform> _layers;
    private Transform entity = null;
    private EntityType _entityType = EntityType.NONE;


    private void Start()
    {
        _layers = new List<Transform> {hexPrefab};
    }

    public void AddLayer()
    {
        if (entity != null) return;
        var position = transform.position;
        var pos = new Vector3(position.x, position.y + (_layers.Count - 1) * hexPrefabHeight,
            position.z);
        Debug.Log(pos + " LAYERCOUNT: " + _layers.Count);
        _layers.Add(Instantiate(hexPrefab, pos, Quaternion.identity));
        RefreshColliderPosition(pos);
    }

    public void SubtractLayer()
    {
        if (_layers.Count <= 1 || entity != null) return;
        var topLayer = _layers[^1];
        Destroy(topLayer.gameObject);
        _layers.RemoveAt(_layers.Count - 1);
        RefreshColliderPosition(_layers[^1].position);
    }

    public void PlaceOrRemoveEntity(EntityType entityType)
    {
        if (entity != null)
        {
            Destroy(entity.gameObject);
            _entityType = EntityType.NONE;
            return;
        }

        var position = transform.position;
        var pos = new Vector3(
            position.x,
            position.y + (_layers.Count * hexPrefabHeight),
            position.z
        );
        entity = entityType switch
        {
            EntityType.PLAYER => Instantiate(playerEntityPrefab, pos, Quaternion.identity),
            EntityType.ENEMY => Instantiate(enemyEntityPrefab, pos, Quaternion.identity),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };
        _entityType = entityType;
    }

    private void RefreshColliderPosition(Vector3 colliderPos)
    {
        hexCollider.transform.position = colliderPos;
    }

    public SerializedHex GetSerialized()
    {
        var sHex = new SerializedHex();
        sHex.layerHeight = _layers.Count;
        sHex.hexType = _entityType switch
        {
            EntityType.NONE => SerializedHexType.NONE,
            EntityType.PLAYER => SerializedHexType.PLAYER,
            EntityType.ENEMY => SerializedHexType.ENEMY,
            _ => throw new ArgumentOutOfRangeException()
        };
        return sHex;
    }
}