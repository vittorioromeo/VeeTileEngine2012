#region
using System;
using System.Collections.Generic;
using System.Linq;
using SFMLStart.Utilities;

#endregion

namespace VeeTileEngine2012
{
    public class Field
    {
        private readonly TileManager _tileManager;

        public Field()
        {
            _tileManager = new TileManager(this);
            NullTile = new Tile(_tileManager, -1, -1);

            TurnActions = new SortedList<int, Action> {{0, () => _tileManager.NextTurn()}};
        }

        public int Width { get { return _tileManager.Width; } }
        public int Height { get { return _tileManager.Height; } }
        public Action<Entity> OnEntityAdded { get; set; }
        public Action OnLoad { get; set; }
        public Action OnLoadChecks { get; set; }
        public SortedList<int, Action> TurnActions { get; set; }
        public Tile NullTile { get; private set; }

        public void Clear(int mWidth, int mHeight)
        {
            _tileManager.Clear(mWidth, mHeight);
            TurnActions = new SortedList<int, Action> {{0, () => _tileManager.NextTurn()}};
        }

        public Tile[,] GetTiles() { return _tileManager.Tiles; }
        public Tile GetTile(int mX, int mY) { return !IsTileValid(mX, mY) ? null : _tileManager.GetTile(mX, mY); }
        public Tile GetTileOrNullTile(int mX, int mY) { return !IsTileValid(mX, mY) ? NullTile : _tileManager.GetTile(mX, mY); }
        public Entity GetEntityOnTop(int mX, int mY) { return GetTile(mX, mY).OrderedEntities.Last(); }
        public IEnumerable<Entity> GetEntities() { return _tileManager.Repository.GetEntities(); }
        public IEnumerable<Entity> GetEntitiesByTag(string mTag) { return _tileManager.Repository.GetEntities(mTag); }
        public IEnumerable<Entity> GetEntitiesByComponent(Type mType) { return _tileManager.Repository.GetEntitiesByComponent(mType); }
        public IEnumerable<Component> GetComponents(Type mType) { return _tileManager.Repository.GetComponents(mType); }
        public IEnumerable<Entity> GetEntities(int mX, int mY) { return GetTileOrNullTile(mX, mY).Repository.GetEntities(); }
        public IEnumerable<Entity> GetEntitiesByTag(int mX, int mY, string mTag) { return GetTileOrNullTile(mX, mY).Repository.GetEntities(mTag); }
        public IEnumerable<Entity> GetEntitiesByComponent(int mX, int mY, Type mType) { return GetTileOrNullTile(mX, mY).Repository.GetEntitiesByComponent(mType); }
        public IEnumerable<Component> GetComponents(int mX, int mY, Type mType) { return GetTileOrNullTile(mX, mY).Repository.GetComponents(mType); }
        public bool IsTileValid(int mX, int mY) { return _tileManager.IsValid(mX, mY); }
        public bool HasEntityByTag(string mTag) { return GetEntitiesByTag(mTag).Any(); }
        public bool HasEntityByTag(int mX, int mY, string mTag, Func<Entity, bool> mCondition = null) { return GetTileOrNullTile(mX, mY).Repository.GetEntities(mTag).Any(x => mCondition == null || mCondition.Invoke(x)); }
        public bool HasEntityByComponent(int mX, int mY, Type mType, Func<Entity, bool> mCondition = null) { return GetTileOrNullTile(mX, mY).Repository.GetEntitiesByComponent(mType).Any(x => mCondition == null || mCondition.Invoke(x)); }

        public void AddEntity(Entity mEntity) { OnEntityAdded.SafeInvoke(mEntity); }

        public void NextTurn()
        {
            foreach (var action in TurnActions)
                action.Value.SafeInvoke();
        }
    }
}