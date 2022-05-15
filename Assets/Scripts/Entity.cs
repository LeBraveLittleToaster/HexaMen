using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private EntityType _entityType;

    public void SetType(EntityType entityType)
    {
        _entityType = entityType;
    }
}

public enum EntityType
{
    NONE,
    PLAYER,
    ENEMY
}
