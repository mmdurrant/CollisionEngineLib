using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using CollisionEngineLib.Objects;
using Microsoft.Xna.Framework;

namespace CollisionEngineLib
{
    public class CollisionEngine
    {
        private Dictionary<string, QuadTreePositionItem> Items = new Dictionary<string, QuadTreePositionItem>();
        private Dictionary<string, Dictionary<string, CollisionResponse>> collisionList = new Dictionary<string, Dictionary<string, CollisionResponse>>();
        public CollisionEngine(QuadTree level)
        {
            Level = level;
        }

        private QuadTree Level { get; set; }

        public void Update()
        {
            foreach (string item in Items.Keys.ToList())
            {
                Dictionary<string,CollisionResponse> list = CheckCollisionsWithObject(item);
                if (list.Count <= 0) continue;
                switch (collisionList.ContainsKey(item))
                {
                    case true:
                        foreach (string collidedObject in list.Keys)
                        {
                            switch (collisionList[item].ContainsKey(collidedObject))
                            {
                                case true:
                                    collisionList[item][collidedObject] = list[collidedObject];
                                    break;
                                case false:
                                    collisionList[item].Add(collidedObject, list[collidedObject]);
                                    break;
                            }
                        }
                        break;
                    case false:
                        collisionList.Add(item, list);
                        break;
                }
            }

        }

        private Dictionary<string, CollisionResponse> CheckCollisionsWithObject(string objectName)
        {
            List<string> items = Items.Keys.ToList();
            Dictionary<string, CollisionResponse> returnResponse = new Dictionary<string, CollisionResponse>();
            foreach (string item in items)
            {
                CollisionResponse response = CheckCollision(objectName, item);
                returnResponse.Add(item, response);
            }
            return returnResponse;
        }
        public bool Add(QuadTreePositionItem item)
        {
            if (Items.ContainsKey(item.Parent.Name)) return false;
            Items.Add(item.Parent.Name, item);
            Level.Insert(item);
            return true;
        }

        public bool Remove(string name)
        {
            if (!Items.ContainsKey(name)) return false;
            Level.HeadNode.RemoveItem(name);
            Items.Remove(name);
            return true;
        }

        public bool Move(string name, Vector2 position)
        {
            if (!Items.ContainsKey(name)) return false;
            Items[name].Position = position;
            return Level.HeadNode.ItemMove(Items[name]);
        }

        public CollisionResponse CheckCollision(string firstObject, string secondObject)
        {
            
            if (!Items.ContainsKey(firstObject) || !Items.ContainsKey(secondObject)) return new CollisionResponse(false);
            QuadTreeNode firstNode = Level.HeadNode.FindItemNode(Items[firstObject]);
            if (firstNode == null) return new CollisionResponse(false);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            // Resharper is told not to do this because I want the first to happen before the second. If it's not intersecting the node, then i don't care.
            if (!Items[secondObject].Rect.Intersects(firstNode.Rect).Collided) return new CollisionResponse(false);
            return Items[firstObject].Rect.Intersects(Items[secondObject].Rect);
        }

        public CollisionResponse CheckCollisionResponse(string firstObject, string secondObject)
        {
            if (!Items.ContainsKey(firstObject) || !Items.ContainsKey(secondObject))
                return new CollisionResponse(false);
            if (!collisionList.ContainsKey(firstObject)) return new CollisionResponse(false);
            return !collisionList[firstObject].ContainsKey(secondObject) ? new CollisionResponse(false) : collisionList[firstObject][secondObject];
        }

        public void ClearLevel(Vector2 size, int maxItems)
        {
            Level = new QuadTree(size, maxItems);
            Items = new Dictionary<string, QuadTreePositionItem>();
        }

        public Vector2 GetPoisitionOfItem(string name)
        {
            if (!Items.ContainsKey(name)) throw new Exception("Item does not exist");
            return Items[name].Position;
        }

        public bool DoesItemExist(string name)
        {
            return Items.ContainsKey(name);
        }
    }
}