namespace Grid
{
    public interface IHexTile 
    {
        Position GetPosition(); 
        void AddLayer();
        void SubtractLayer();
        int GetLayerHeight();

        void SetLayerHeight(int layerHeight);

        bool PlaceEntity(IEntity entity);
        IEntity RemoveEntity();
    }
}