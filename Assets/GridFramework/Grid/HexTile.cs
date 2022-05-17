using System.Collections.Generic;

namespace Grid
{
    public class HexTile : IHexTile
    {
        private readonly Position _position;
        private int _layerHeight;
        private IEntity _entity;

        public HexTile(Position position, int layerHeight)
        {
            _position = position;
            _layerHeight = layerHeight;
        }

        public Position GetPosition()
        {
            return _position;
        }

        public void AddLayer()
        {
            _layerHeight += 1;
        }

        public void SubtractLayer()
        {
            _layerHeight -= 1;
        }

        public int GetLayerHeight()
        {
            return _layerHeight;
        }

        public void SetLayerHeight(int layerHeight)
        {
            _layerHeight = layerHeight;
        }

        public bool PlaceEntity(IEntity entity)
        {
            if (_entity != null) return false;
            _entity = entity;
            return true;
        }

        public IEntity RemoveEntity()
        {
            var previousEntity = _entity;
            _entity = null;
            return previousEntity;
        }

        protected bool Equals(HexTile other)
        {
            return Equals(_position, other._position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HexTile)obj);
        }

        public override int GetHashCode()
        {
            return (_position != null ? _position.GetHashCode() : 0);
        }
    }
}