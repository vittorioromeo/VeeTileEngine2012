#region
using System;
using System.Collections.Generic;

#endregion

namespace VeeTileEngine2012
{
    [Serializable]
    public class Entity
    {
        private static int _lastUID = -1;

        private readonly Dictionary<Type, Component> _componentDictionary;
        private readonly List<Component> _components;
        private readonly HashSet<string> _tags;
        [NonSerialized] private Field _field;
        private bool _isOutOfField;
        private bool _isUpdated;
        [NonSerialized] private Tile _tile;
        [NonSerialized] private TileManager _tileManager;
        private int _uID;

        public Entity(Tile mTile, bool mIsUpdated = true)
        {
            _lastUID++;
            _uID = _lastUID;

            _components = new List<Component>();
            _componentDictionary = new Dictionary<Type, Component>();
            _tags = new HashSet<string>();

            Tile = mTile;
            TileManager = mTile.TileManager;
            Field = TileManager.Field;

            TileManager.AddEntity(this, mIsUpdated);
            Tile.AddEntity(this);

            IsAlive = true;
            _isUpdated = mIsUpdated;
            Layer = 0;
            UpdateOrder = 0;
        }

        public Field Field { get { return _field; } private set { _field = value; } }
        private TileManager TileManager { get { return _tileManager; } set { _tileManager = value; } }
        private Tile Tile { get { return _tile; } set { _tile = value; } }
        public int X { get { return Tile.X; } }
        public int Y { get { return Tile.Y; } }
        public int Layer { get; set; }
        public bool IsAlive { get; private set; }

        public bool IsOutOfField
        {
            get { return _isOutOfField; }
            set
            {
                _isOutOfField = value;

                if (value)
                    Tile.RemoveEntity(this);
                else
                    Tile.AddEntity(this);
            }
        }

        public int UpdateOrder { get; set; }

        public bool Move(int mX, int mY)
        {
            if (!IsAlive || IsOutOfField) return false;
            //if (!Field.IsTileValid(mX, mY)) return false;

            var tempTile = Field.GetTileOrNullTile(mX, mY);
            if (tempTile == Field.NullTile) return false;

            Tile.RemoveEntity(this);
            tempTile.AddEntity(this);
            Tile = tempTile;

            return true;
        }

        private void AddTag(string mTag)
        {
            _tags.Add(mTag);

            Tile.Repository.TagAdded(this, mTag);
            TileManager.Repository.TagAdded(this, mTag);
        }

        public void AddTags(params string[] mTags) { foreach (var tag in mTags) AddTag(tag); }

        private void RemoveTag(string mTag)
        {
            _tags.Remove(mTag);

            Tile.Repository.TagRemoved(this, mTag);
            TileManager.Repository.TagRemoved(this, mTag);
        }

        public void RemoveTags(params string[] mTags) { foreach (var tag in mTags) RemoveTag(tag); }
        public bool HasTag(string mTag) { return _tags.Contains(mTag); }
        internal IEnumerable<string> GetTags() { return _tags; }

        private void AddComponent(Component mComponent)
        {
            mComponent.Entity = this;

            var componentType = mComponent.GetType();

            _components.Add(mComponent);

            if (!_componentDictionary.ContainsKey(componentType)) _componentDictionary.Add(componentType, mComponent);
            else _componentDictionary[componentType] = mComponent;

            Tile.Repository.ComponentAdded(this, mComponent);
            TileManager.Repository.ComponentAdded(this, mComponent);

            mComponent.Added();
        }

        public void AddComponents(params Component[] mComponents) { foreach (var component in mComponents) AddComponent(component); }

        public void RemoveComponent(Component mComponent)
        {
            var componentType = mComponent.GetType();

            _components.Remove(mComponent);

            _componentDictionary.Remove(componentType);

            Tile.Repository.ComponentRemoved(this, mComponent);
            TileManager.Repository.ComponentRemoved(this, mComponent);

            mComponent.Removed();
        }

        public T GetComponent<T>() where T : Component
        {
            if (_componentDictionary.ContainsKey(typeof (T)))
                return (T) _componentDictionary[typeof (T)];

            return null;
        }

        internal IEnumerable<Component> GetComponents() { return _components; }

        public void NextTurn()
        {
            var componentsToUpdate = new List<Component>();
            foreach (var component in _components)
            {
                component.Refresh();
                componentsToUpdate.Add(component);
            }
            foreach (var component in componentsToUpdate)
                component.NextTurn();
        }

        public void Destroy()
        {
            TileManager.RemoveEntity(this, _isUpdated);
            Tile.RemoveEntity(this);

            IsAlive = false;
            _isUpdated = false;

            foreach (var component in new List<Component>(_components)) RemoveComponent(component);
        }
    }
}