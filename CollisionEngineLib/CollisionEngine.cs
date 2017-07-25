using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using CollisionEngineLib.Objects;
using Microsoft.Xna.Framework;
using WorldMessengerLib;

namespace CollisionEngineLib
{
    public class CollisionEngine
    {
        private Dictionary<string, QuadTreePositionItem> Items = new Dictionary<string, QuadTreePositionItem>();        

        public CollisionEngine(QuadTree level, string name)
        {
            Name = name;
            Level = level;
            Sender = new Sender(StaticChannelNames.CollisionChannel, name);
            Sender.Connect();
        }
        private string Name { get; }
        private QuadTree Level { get; set; }
        private Sender Sender { get; }
        private bool _started { get; set; }
        //TODO: Implement Receiver for UpdatePosition
        public void Update()
        {
            if (_started)
            {
                CheckCollisions();
            }   
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        private void SendMessage(Collidable firstObject, Collidable secondObject)
        {
            Sender.SendMessage(new CollisionMessage {FirstObject = firstObject, SecondObject = secondObject, WorldName = Name});
        }

        private void CheckCollisions()
        {
            var firstItems = Items.Keys.ToList();
            var secondItems = firstItems;
            var ignoreList = new Dictionary<string, List<string>>();
            foreach (var item in firstItems)
            {
                foreach (var objectName in secondItems)
                {
                    if (Items[item].Parent.ColliderId == Items[objectName].Parent.ColliderId) continue;
                    var checkCollision = true;
                    if (ignoreList.ContainsKey(item))
                    {
                        if (ignoreList[item].Contains(objectName))
                        {
                            checkCollision = false;
                        }
                    }
                    if (!checkCollision) continue;

                    CheckCollision(item, objectName);
                    SendMessage(Items[item].Parent, Items[objectName].Parent);
                    if (ignoreList.ContainsKey(objectName))
                    {
                        ignoreList[objectName].Add(item);
                    }
                    else
                    {
                        ignoreList.Add(objectName, new List<string>{objectName});
                    }
                }
            }

        }
        public bool Add(QuadTreePositionItem item)
        {
            if (Items.ContainsKey(item.Parent.ColliderId)) return false;
            Items.Add(item.Parent.ColliderId, item);
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

        public bool Move(string name, Vector2 movement)
        {
            if (!Items.ContainsKey(name)) return false;
            Items[name].Position += movement;
            return Level.HeadNode.ItemMove(Items[name]);
        }

        public CollisionResponse CheckCollision(string firstObject, string secondObject)
        {
            
            if (!Items.ContainsKey(firstObject) || !Items.ContainsKey(secondObject)) return new CollisionResponse(false);
            var firstNode = Level.HeadNode.FindItemNode(Items[firstObject]);
            if (firstNode == null) return new CollisionResponse(false);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            // Resharper is told not to do this because I want the first to happen before the second. If it's not intersecting the node, then i don't care.

            if (!Items[secondObject].Rect.Intersects(firstNode.Rect).Collided) return new CollisionResponse(false);
            var collision = Items[firstObject].Rect.Intersects(Items[secondObject].Rect);
            if (!collision.Collided) return new CollisionResponse(false);

            return Polygon.PolygonCollision(Items[firstObject].Rect.Polygon, Items[secondObject].Rect.Polygon,new Vector(Vector2.Zero)).Intersect ? collision : new CollisionResponse(false);
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