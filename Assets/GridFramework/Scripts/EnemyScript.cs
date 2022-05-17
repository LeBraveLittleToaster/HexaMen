using Grid;
using UnityEngine;

public class EnemyScript : MonoBehaviour, IEntity
{
    private readonly Grid.EntityType _entityType = Grid.EntityType.ENEMY;
    
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
