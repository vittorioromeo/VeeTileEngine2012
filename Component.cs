using System;

namespace VeeTileEngine2012
{
    [Serializable]
    public class Component
    {
        public Entity Entity { get; internal set; }
        protected Field Field { get { return Entity.Field; } }
        public int X { get { return Entity.X; } }
        public int Y { get { return Entity.Y; } }

        public virtual void NextTurn() { }
        public virtual void Added() { }
        public virtual void Removed() { }
        public virtual void Refresh() { }
    }
}