using System;
using CollisionEngineLib;
using CollisionEngineLib.Objects;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;
using Assert = NUnit.Framework.Assert;

namespace CollisionEngineTest
{
    [TestFixture]
    public class UnitTest1
    {
        [TestCase(TestName = "Adding a new QuadTreePosition Item works")]
        public void AddQuadTreePositionItem()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem positionItem = new QuadTreePositionItem(new Collidable("new object"), new Vector2(10, 10), new Vector2(0, 0));
            Assert.IsTrue(collisionEngine.Add(positionItem)); 
        }

        [TestCase(TestName = "Removing an item works")]
        public void RemoveQuadTreePositionItem()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem positionItem = new QuadTreePositionItem(new Collidable("new object"), new Vector2(10, 10), new Vector2(0, 0));
            collisionEngine.Add(positionItem);
            Assert.IsTrue(collisionEngine.Remove(positionItem.Parent.Name));
        }

        [TestCase(TestName = "Moving an item works correctly")]
        public void MoveQuadTreePositionItem()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem positionItem = new QuadTreePositionItem(new Collidable("new object"), new Vector2(10, 10), new Vector2(0, 0));
            collisionEngine.Add(positionItem);

            Vector2 movePosition = new Vector2(5,5);
            Assert.IsTrue(collisionEngine.Move(positionItem.Parent.Name, movePosition));
            Vector2 position = collisionEngine.GetPoisitionOfItem(positionItem.Parent.Name);
            
            Assert.AreEqual(movePosition.X, position.X);
            Assert.AreEqual(movePosition.Y, position.Y);
        }

        [TestCase(TestName = "Two objects colliding reports back true")]
        public void CollideTwoObjects_CheckTheirCollision()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(10, 10));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10,10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.Name, new Vector2(45, 45));
            Assert.IsTrue(collisionEngine.CheckCollision(firstItem.Parent.Name, secondItem.Parent.Name).Collided);
        }

        [TestCase(TestName = "Two objects not colliding returns false")]
        public void PlaceTwoObjects_MakeSureNotColliding()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(10, 10));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            Assert.IsFalse(collisionEngine.CheckCollision(firstItem.Parent.Name, secondItem.Parent.Name).Collided);
        }

        [TestCase(TestName = "When an object collides from the West of another object, collision ")]
        public void CollideFromWest_CorrectResponse()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(10, 3));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.Name, new Vector2(45, 53));
            CollisionResponse response = collisionEngine.CheckCollision(secondItem.Parent.Name, firstItem.Parent.Name);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.West));
        }
        [TestCase(TestName = "When an object collides from the North of another object, collision ")]
        public void CollideFromNorth_CorrectResponse()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(3, 10));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.Name, new Vector2(53, 45));
            CollisionResponse response = collisionEngine.CheckCollision(secondItem.Parent.Name, firstItem.Parent.Name);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.North));
        }
        [TestCase(TestName = "When an object collides from the East of another object, collision ")]
        public void CollideFromEast_CorrectResponse()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(10, 3));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.Name, new Vector2(55, 53));  
            CollisionResponse response = collisionEngine.CheckCollision(secondItem.Parent.Name, firstItem.Parent.Name);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.East));
        }
        [TestCase(TestName = "When an object collides from the North of another object, collision ")]
        public void CollideFromSouth_CorrectResponse()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(3, 10));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.Name, new Vector2(53, 55));
            CollisionResponse response = collisionEngine.CheckCollision(secondItem.Parent.Name, firstItem.Parent.Name);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.South));
        }

        [TestCase(
            TestName = "When an the first object is inside the second object, 'Surround' response will be returned")]
        public void CollideFromSurround_CorrectResponse()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(1, 1));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(0, 0), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            CollisionResponse response = collisionEngine.CheckCollision(firstItem.Parent.Name, secondItem.Parent.Name);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.Surround));
        }
        [TestCase(TestName = "When an the first object has the second object inside of it , 'Inside' response will be returned")]
        public void CollideFromInside_CorrectResponse()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(1, 1));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(0, 0), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            CollisionResponse response = collisionEngine.CheckCollision(secondItem.Parent.Name, firstItem.Parent.Name);
            Assert.IsTrue(response.Sides.Count == 1);
            Assert.IsTrue(response.Sides.Contains(Direction.Inside));
        }

        [TestCase(TestName = "Using CheckCollisionResponse in junction with Update() will return the correct response")]
        public void CollisionHappens_Update_CheckForCollision()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(10, 10));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            collisionEngine.Move(firstItem.Parent.Name, new Vector2(45, 45));
            collisionEngine.Update();
            CollisionResponse response = collisionEngine.CheckCollisionResponse("first item", "second item");
            Assert.IsTrue(response.Collided);
        }

    }
}
