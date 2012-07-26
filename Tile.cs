#region
using System.Collections.Generic;

#endregion

namespace VeeTileEngine2012
{
    public class Tile
    {
        internal Tile(TileManager mTileManager, int mX, int mY)
        {
            TileManager = mTileManager;
            X = mX;
            Y = mY;

            OrderedEntities = new List<Entity>();
            Repository = new Repository();
        }

        internal TileManager TileManager { get; private set; }
        internal List<Entity> OrderedEntities { get; private set; }
        internal Repository Repository { get; private set; }

        internal int X { get; private set; }
        internal int Y { get; private set; }

        internal void AddEntity(Entity mEntity)
        {
            OrderedEntities.Add(mEntity);
            OrderedEntities.Sort((a, b) => a.Layer.CompareTo(b.Layer));

            Repository.AddEntity(mEntity);
        }

        internal void RemoveEntity(Entity mEntity)
        {
            OrderedEntities.Remove(mEntity);
            Repository.RemoveEntity(mEntity);
        }
    }
}