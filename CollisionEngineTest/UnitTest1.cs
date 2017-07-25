using CollisionEngineLib;
using CollisionEngineLib.Objects;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CollisionEngineTest
{
    [TestFixture]
    public class UnitTest1
    {
        [TestCase(TestName = "Adding a new QuadTreePosition Item works")]
        public void AddQuadTreePositionItem()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var positionItem = new QuadTreePositionItem(new Collidable(), new Vector2(10, 10), new Vector2(0, 0));
            Assert.IsTrue(collisionEngine.Add(positionItem)); 
        }

        [TestCase(TestName = "Removing an item works")]
        public void RemoveQuadTreePositionItem()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var positionItem = new QuadTreePositionItem(new Collidable(), new Vector2(10, 10), new Vector2(0, 0));
            collisionEngine.Add(positionItem);
            Assert.IsTrue(collisionEngine.Remove(positionItem.Parent.ColliderId));
        }

        [TestCase(TestName = "Moving an item works correctly")]
        public void MoveQuadTreePositionItem()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var positionItem = new QuadTreePositionItem(new Collidable(), new Vector2(10, 10), new Vector2(1, 1));
            collisionEngine.Add(positionItem);

            var movePosition = new Vector2(5,5);
            Assert.IsTrue(collisionEngine.Move(positionItem.Parent.ColliderId, movePosition));
            var position = collisionEngine.GetPoisitionOfItem(positionItem.Parent.ColliderId);
            
            Assert.AreEqual(15, position.X);
            Assert.AreEqual(15, position.Y);
        }

        [TestCase(TestName = "Two objects colliding reports back true")]
        public void CollideTwoObjects_CheckTheirCollision()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(10, 10));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(50, 50), new Vector2(10,10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.ColliderId, new Vector2(45, 45));
            Assert.IsTrue(collisionEngine.CheckCollision(firstItem.Parent.ColliderId, secondItem.Parent.ColliderId).Collided);
        }

        [TestCase(TestName = "Two objects not colliding returns false")]
        public void PlaceTwoObjects_MakeSureNotColliding()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(10, 10));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            Assert.IsFalse(collisionEngine.CheckCollision(firstItem.Parent.ColliderId, secondItem.Parent.ColliderId).Collided);
        }

        [TestCase(TestName = "When an object collides from the West of another object, collision ")]
        public void CollideFromWest_CorrectResponse()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(10, 3));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.ColliderId, new Vector2(45, 53));
            var response = collisionEngine.CheckCollision(secondItem.Parent.ColliderId, firstItem.Parent.ColliderId);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.West));
        }
        [TestCase(TestName = "When an object collides from the North of another object, collision ")]
        public void CollideFromNorth_CorrectResponse()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(3, 10));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.ColliderId, new Vector2(53, 45));
            var response = collisionEngine.CheckCollision(secondItem.Parent.ColliderId, firstItem.Parent.ColliderId);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.North));
        }
        [TestCase(TestName = "When an object collides from the East of another object, collision ")]
        public void CollideFromEast_CorrectResponse()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(10, 3));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.ColliderId, new Vector2(55, 53));
            var response = collisionEngine.CheckCollision(secondItem.Parent.ColliderId, firstItem.Parent.ColliderId);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.East));
        }
        [TestCase(TestName = "When an object collides from the North of another object, collision ")]
        public void CollideFromSouth_CorrectResponse()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(3, 10));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.ColliderId, new Vector2(53, 55));
            var response = collisionEngine.CheckCollision(secondItem.Parent.ColliderId, firstItem.Parent.ColliderId);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.South));
        }

        [TestCase(
            TestName = "When an the first object is inside the second object, 'Surround' response will be returned")]
        public void CollideFromSurround_CorrectResponse()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(1, 1));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            var response = collisionEngine.CheckCollision(firstItem.Parent.ColliderId, secondItem.Parent.ColliderId);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.Surround));
        }
        [TestCase(TestName = "When an the first object has the second object inside of it , 'Inside' response will be returned")]
        public void CollideFromInside_CorrectResponse()
        {
            var collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1), "Test");
            var firstItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(1, 1));
            var secondItem = new QuadTreePositionItem(new Collidable(), new Vector2(0, 0), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            var response = collisionEngine.CheckCollision(secondItem.Parent.ColliderId, firstItem.Parent.ColliderId);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.Inside));
        }

    }
}
