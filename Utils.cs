#region
using System;

#endregion

namespace VeeTileEngine2012
{
    public static class Utils
    {
        public static void SafeInvoke(this Action<Entity> mAction, Entity mEntity) { if (mAction != null) mAction(mEntity); }
    }
}