using System;
using System.Collections.Generic;
using CollisionEngineLib.Objects;

namespace CollisionEngineLib
{
    [Serializable]
    public class CollisionData
    {
        public string Name { get; set; }
        public List<QuadTreePositionItem> Items { get; set; }
    }
}