#region
using System.Collections.Generic;
using System.Linq;

#endregion

namespace VeeTileEngine2012
{
    public class TileManager
    {
        private bool _requiresSort;

        internal TileManager(Field mField)
        {
            Field = mField;
            EntitiesToUpdate = new List<Entity>();
            Repository = new Repository();
        }

        internal Field Field { get; private set; }
        private List<Entity> EntitiesToUpdate { get; set; }
        internal Repository Repository { get; private set; }
        internal Tile[,] Tiles { get; private set; }
        internal int Width { get; private set; }
        internal int Height { get; private set; }

        internal void Clear(int mWidth, int mHeight)
        {
            Width = mWidth;
            Height = mHeight;

            Tiles = new Tile[mWidth,mHeight];
            EntitiesToUpdate.Clear();
            Repository.Clear();

            for (var iY = 0; iY < Height; iY++) for (var iX = 0; iX < Width; iX++) Tiles[iX, iY] = new Tile(this, iX, iY);
        }

        internal Tile GetTile(int mX, int mY) { return Tiles[mX, mY]; }
        internal bool IsValid(int mX, int mY) { return mX > -1 && mX < Width && mY > -1 && mY < Height; }

        internal void AddEntity(Entity mEntity, bool mIsUpdated)
        {
            if (mIsUpdated)
            {
                EntitiesToUpdate.Add(mEntity);
                _requiresSort = true;
            }

            Repository.AddEntity(mEntity);
        }

        internal void RemoveEntity(Entity mEntity, bool mIsUpdated)
        {
            if (mIsUpdated) EntitiesToUpdate.Remove(mEntity);

            Repository.RemoveEntity(mEntity);
        }

        internal void NextTurn()
        {
            if (_requiresSort) EntitiesToUpdate = new List<Entity>(EntitiesToUpdate.OrderBy(x => x.UpdateOrder)); // EntitiesToUpdate.Sort((a,b) => a.UpdateOrder.CompareTo(b.UpdateOrder));
            _requiresSort = false;

            foreach (var entity in new List<Entity>(EntitiesToUpdate))
                if (entity.IsAlive && !entity.IsOutOfField) entity.NextTurn();
        }
    }
}