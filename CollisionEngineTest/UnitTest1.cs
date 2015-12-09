using System;
using System.Security.Cryptography.X509Certificates;
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
            Assert.IsTrue(collisionEngine.CheckCollision(firstItem.Parent.Name, secondItem.Parent.Name));
        }

        [TestCase(TestName = "Two objects not colliding returns false")]
        public void PlaceTwoObjects_MakeSureNotColliding()
        {
            CollisionEngine collisionEngine = new CollisionEngine(new QuadTree(new Vector2(1000, 1000), 1));
            QuadTreePositionItem firstItem = new QuadTreePositionItem(new Collidable("first item"), new Vector2(0, 0), new Vector2(10, 10));
            QuadTreePositionItem secondItem = new QuadTreePositionItem(new Collidable("second item"), new Vector2(50, 50), new Vector2(10, 10));
            collisionEngine.Add(firstItem);
            collisionEngine.Add(secondItem);
            Assert.IsFalse(collisionEngine.CheckCollision(firstItem.Parent.Name, secondItem.Parent.Name));
        }
        

    }
}
