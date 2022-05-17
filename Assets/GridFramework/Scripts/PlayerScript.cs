using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private readonly Grid.EntityType _entityType = Grid.EntityType.PLAYER;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Grid.EntityType GetType()
    {
        return _entityType;
    }
}
