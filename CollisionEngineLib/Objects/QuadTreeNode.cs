using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CollisionEngineLib.Objects
{
    [Serializable]
    public class QuadTreeNode
    {
        #region Delegates

        /// <summary>
        /// World resize delegate
        /// </summary>
        /// <param name="newSize">The new world size</param>
        public delegate void ResizeDelegate(FRect newSize);

        #endregion

        #region Properties

        /// <summary>
        /// The rectangle of this node
        /// </summary>
        protected FRect rect;

        /// <summary>
        /// Gets the rectangle of this node
        /// </summary>
        public FRect Rect
        {
            get { return rect; }
            protected set { rect = value; }
        }

        /// <summary>
        /// The maximum number of items in this node before partitioning
        /// </summary>
        protected int MaxItems;

        /// <summary>
        /// Whether or not this node has been partitioned
        /// </summary>
        protected bool IsPartitioned;

        /// <summary>
        /// The parent node
        /// </summary>
        protected QuadTreeNode ParentNode;

        /// <summary>
        /// The top left node
        /// </summary>
        protected QuadTreeNode TopLeftNode;

        /// <summary>
        /// The top right node
        /// </summary>
        protected QuadTreeNode TopRightNode;

        /// <summary>
        /// The bottom left node
        /// </summary>
        protected QuadTreeNode BottomLeftNode;

        /// <summary>
        /// The bottom right node
        /// </summary>
        protected QuadTreeNode BottomRightNode;

        /// <summary>
        /// The items in this node
        /// </summary>
        protected Dictionary<string, QuadTreePositionItem> Items;

        /// <summary>
        /// Resize the world
        /// </summary>
        /// <param name="newSize">The new world size</param>
        protected ResizeDelegate WorldResize;

        #endregion

        #region Initialization

        /// <summary>
        /// QuadTreeNode constructor
        /// </summary>
        /// <param name="parentNode">The parent node of this QuadTreeNode</param>
        /// <param name="rect">The rectangle of the QuadTreeNode</param>
        /// <param name="maxItems">Maximum number of items in the QuadTreeNode before partitioning</param>
        public QuadTreeNode(QuadTreeNode parentNode, FRect rect, int maxItems)
        {
            ParentNode = parentNode;
            Rect = rect;
            MaxItems = maxItems;
            IsPartitioned = false;
            Items = new Dictionary<string, QuadTreePositionItem>();
        }

        /// <summary>
        /// QuadTreeNode constructor
        /// </summary>
        /// <param name="rect">The rectangle of the QuadTreeNode</param>
        /// <param name="maxItems">Maximum number of items in the QuadTreeNode before partitioning</param>
        /// <param name="worldResize">The function to return the size</param>
        public QuadTreeNode(FRect rect, int maxItems, ResizeDelegate worldResize)
        {
            ParentNode = null;
            Rect = rect;
            MaxItems = maxItems;
            WorldResize = worldResize;
            IsPartitioned = false;
            Items = new Dictionary<string, QuadTreePositionItem>();
        }

        #endregion

        #region Insertion methods

        /// <summary>
        /// Insert an item in this node
        /// </summary>
        /// <param name="item">The item to insert</param>
        public void Insert(QuadTreePositionItem item)
        {
            // If partitioned, try to find child node to add to
            if (InsertInChild(item)) return;
            item.Destroy += ItemDestroy;
            item.Move += ItemMove;
            Items.Add(item.Parent.ColliderId, item);

            // Check if this node needs to be partitioned
            if (!IsPartitioned && Items.Count > MaxItems)
            {
                Partition();
            }
        }

        /// <summary>
        /// Inserts an item into one of this node's children
        /// </summary>
        /// <param name="item">The item to insert in a child</param>
        /// <returns>Whether or not the insert succeeded</returns>
        protected bool InsertInChild(QuadTreePositionItem item)
        {
            if (!IsPartitioned) return false;

            if (TopLeftNode.ContainsRect(item.Rect))
                TopLeftNode.Insert(item);
            else if (TopRightNode.ContainsRect(item.Rect))
                TopRightNode.Insert(item);
            else if (BottomLeftNode.ContainsRect(item.Rect))
                BottomLeftNode.Insert(item);
            else if (BottomRightNode.ContainsRect(item.Rect))
                BottomRightNode.Insert(item);

            else return false; // insert in child failed

            return true;
        }

        /// <summary>
        /// Pushes an item down to one of this node's children
        /// </summary>
        /// <param name="i">The index of the item to push down</param>
        /// <returns>Whether or not the push was successful</returns>
        public bool PushItemDown(string i)
        {
            if (!InsertInChild(Items[i])) return false;
            RemoveItem(i);
            return true;
        }

        /// <summary>
        /// Push an item up to this node's parent
        /// </summary>
        /// <param name="i">The index of the item to push up</param>
        public void PushItemUp(string i)
        {
            var m = Items[i];

            RemoveItem(i);
            ParentNode.Insert(m);
        }

        /// <summary>
        /// Repartitions this node
        /// </summary>
        protected void Partition()
        {
            // Create the nodes
            var midPoint = Vector2.Divide(Vector2.Add(Rect.TopLeft, Rect.BottomRight), 2.0f);

            TopLeftNode = new QuadTreeNode(this, new FRect(Rect.TopLeft, midPoint), MaxItems);
            TopRightNode = new QuadTreeNode(this, new FRect(new Vector2(midPoint.X, Rect.Top), new Vector2(Rect.Right, midPoint.Y)), MaxItems);
            BottomLeftNode = new QuadTreeNode(this, new FRect(new Vector2(Rect.Left, midPoint.Y), new Vector2(midPoint.X, Rect.Bottom)), MaxItems);
            BottomRightNode = new QuadTreeNode(this, new FRect(midPoint, Rect.BottomRight), MaxItems);

            IsPartitioned = true;

            // Try to push items down to child nodes
            foreach (var item in Items.Keys.ToList())
            {
                PushItemDown(item);
            }
        }

        #endregion

        #region Query methods

        /// <summary>
        /// Gets a list of items containing a specified point
        /// </summary>
        /// <param name="point">The point</param>
        /// <remarks>ItemsFound is assumed to be initialized, and will not be cleared</remarks>
        public List<QuadTreePositionItem> GetItems(Vector2 point)
        {
            var itemsFound = new List<QuadTreePositionItem>();
            // test the point against this node
            if (!Rect.Contains(point)) return itemsFound;
            // test the point in each item
            itemsFound.AddRange(from item in Items where item.Value.Rect.Contains(point) select item.Value);
            //itemsFound.AddRange(Items.Where(item => item.Value.Rect.Contains(Point)));

            // query all subtrees
            if (!IsPartitioned) return itemsFound;
            TopLeftNode.GetItems(point);
            TopRightNode.GetItems(point);
            BottomLeftNode.GetItems(point);
            BottomRightNode.GetItems(point);
            return itemsFound;
        }

        /// <summary>
        /// Gets a list of items intersecting a specified rectangle
        /// </summary>
        /// <param name="inRect">The rectangle</param>        
        /// <remarks>ItemsFound is assumed to be initialized, and will not be cleared</remarks>
        public List<QuadTreePositionItem> GetItems(FRect inRect)
        {
            var itemsFound = new List<QuadTreePositionItem>();
            // test the point against this node
            if (!Rect.Intersects(inRect).Collided) return itemsFound;
            // test the point in each item
            itemsFound.AddRange(from item in Items where item.Value.Rect.Intersects(inRect).Collided select item.Value);

            // query all subtrees
            if (!IsPartitioned) return itemsFound;
            TopLeftNode.GetItems(inRect);
            TopRightNode.GetItems(inRect);
            BottomLeftNode.GetItems(inRect);
            BottomRightNode.GetItems(inRect);
            return itemsFound;
        }

        /// <summary>
        /// Gets a list of all items within this node
        /// </summary>
        public List<QuadTreePositionItem> GetAllItems()
        {
            var itemsFound = Items.Select(item => item.Value).ToList();

            // query all subtrees
            if (!IsPartitioned) return itemsFound;
            itemsFound.AddRange(TopLeftNode.GetAllItems());
            itemsFound.AddRange(TopRightNode.GetAllItems());
            itemsFound.AddRange(BottomLeftNode.GetAllItems());
            itemsFound.AddRange(BottomRightNode.GetAllItems());
            return itemsFound;
        }

        /// <summary>
        /// Finds the node containing a specified item
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The node containing the item</returns>
        public QuadTreeNode FindItemNode(QuadTreePositionItem item)
        {
            if (Items.ContainsKey(item.Parent.ColliderId)) return this;

            if (!IsPartitioned) return null;
            QuadTreeNode n = null;

            // Check the nodes that could contain the item
            if (TopLeftNode.ContainsRect(item.Rect))
            {
                n = TopLeftNode.FindItemNode(item);
            }
            if (n == null &&
                TopRightNode.ContainsRect(item.Rect))
            {
                n = TopRightNode.FindItemNode(item);
            }
            if (n == null &&
                BottomLeftNode.ContainsRect(item.Rect))
            {
                n = BottomLeftNode.FindItemNode(item);
            }
            if (n == null &&
                BottomRightNode.ContainsRect(item.Rect))
            {
                n = BottomRightNode.FindItemNode(item);
            }

            return n;
        }

        #endregion

        #region Destruction

        /// <summary>
        /// Destroys this node
        /// </summary>
        public void Destroy()
        {
            // Destroy all child nodes
            if (IsPartitioned)
            {
                TopLeftNode.Destroy();
                TopRightNode.Destroy();
                BottomLeftNode.Destroy();
                BottomRightNode.Destroy();

                TopLeftNode = null;
                TopRightNode = null;
                BottomLeftNode = null;
                BottomRightNode = null;
            }

            // Remove all items
            Items = new Dictionary<string, QuadTreePositionItem>();
        }

        /// <summary>
        /// Removes an item from this node
        /// </summary>
        /// <param name="item">The item to remove</param>
        public bool RemoveItem(string item)
        {
            // Find and remove the item
            if (!Items.ContainsKey(item)) return false;

            Items[item].Move -= ItemMove;
            Items[item].Destroy -= ItemDestroy;
            return Items.Remove(item);
        }
        /*
         * This function is useless. Why would I ever need to do this?
        protected void RemoveItem(int i)
        {
            if (i >= Items.Count) return;
            Items[i].Move -= new QuadTreePositionItem.MoveHandler(ItemMove);
            Items[i].Destroy -= new QuadTreePositionItem.DestroyHandler(ItemDestroy);
            Items.RemoveAt(i);
        }
        */
        #endregion

        #region Observer methods

        /// <summary>
        /// Handles item movement
        /// </summary>
        /// <param name="item">The item that moved</param>
        public bool ItemMove(QuadTreePositionItem item)
        {
            // Find the item
            if (Items.ContainsKey(item.Parent.ColliderId))
            {
                // Try to push the item down to the child
                if (PushItemDown(item.Parent.ColliderId)) return true;
                // otherwise, if not root, push up
                if (ParentNode != null)
                {
                    PushItemUp(item.Parent.ColliderId);
                }
                else if (!ContainsRect(item.Rect))
                {
                    WorldResize(new FRect(
                        Vector2.Min(Rect.TopLeft, item.Rect.TopLeft) * 2,
                        Vector2.Max(Rect.BottomRight, item.Rect.BottomRight) * 2));
                }
                return true;
            }
            // this node doesn't contain that item, stop notifying it about it
            item.Move -= ItemMove;
            return false;
        }

        /// <summary>
        /// Handles item destruction
        /// </summary>
        /// <param name="item">The item that is being destroyed</param>
        public bool ItemDestroy(QuadTreePositionItem item)
        {
            return RemoveItem(item.Parent.ColliderId);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Tests whether this node contains a rectangle
        /// </summary>
        /// <param name="inRect">The rectangle to test</param>
        /// <returns>Whether or not this node contains the specified rectangle</returns>
        public bool ContainsRect(FRect inRect)
        {
            return (inRect.TopLeft.X >= Rect.TopLeft.X &&
                    inRect.TopLeft.Y >= Rect.TopLeft.Y &&
                    inRect.BottomRight.X <= Rect.BottomRight.X &&
                    inRect.BottomRight.Y <= Rect.BottomRight.Y);
        }

        #endregion
    }
}