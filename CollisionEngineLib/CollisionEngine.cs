﻿using System;
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
        public CollisionEngine(QuadTree level)
        {
            Level = level;
        }

        private QuadTree Level { get; set; }

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