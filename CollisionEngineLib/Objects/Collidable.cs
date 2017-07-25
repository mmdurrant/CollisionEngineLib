using System;

namespace CollisionEngineLib.Objects
{
    [Serializable]
    public class Collidable
    {
        public Collidable()
        {
            ColliderId = Guid.NewGuid().ToString();
        }
        public string ColliderId { get; }

    }
}