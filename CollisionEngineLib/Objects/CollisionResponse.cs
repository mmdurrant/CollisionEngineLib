using System.Collections.Generic;

namespace CollisionEngineLib.Objects
{
    public class CollisionResponse
    {
        public CollisionResponse(bool collision)
        {
            Collided = collision;
        }
        public CollisionResponse(bool collision, List<Direction> directions )
        {
            Collided = collision;
            Sides = directions;
        }
        public List<Direction> Sides = new List<Direction>();
        public bool Collided;
    }
}