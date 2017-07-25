using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CollisionEngineLib.Objects
{
    /// <summary>
    /// A QuadTree for partitioning a space into rectangles
    /// </summary>
    /// <typeparam name="T">The type of the QuadTree's items' parents</typeparam>
    /// <remarks>This QuadTree automatically resizes as needed</remarks>
    [Serializable]
    public class QuadTree
    {
        #region Properties

        /// <summary>
        /// The head node of the QuadTree
        /// </summary>
        public QuadTreeNode HeadNode;

        /// <summary>
        /// Gets the world rectangle
        /// </summary>
        public FRect WorldRect
        {
            get { return HeadNode.Rect; }
        }

        /// <summary>
        /// The maximum number of items in any node before partitioning
        /// </summary>
        protected int MaxItems;

        #endregion

        #region Initialization

        /// <summary>
        /// QuadTree constructor
        /// </summary>
        /// <param name="worldRect">The world rectangle for this QuadTree (a rectangle containing all items at all times)</param>
        /// <param name="maxItems">Maximum number of items in any cell of the QuadTree before partitioning</param>
        public QuadTree(FRect worldRect, int maxItems)
        {
            this.HeadNode = new QuadTreeNode(worldRect, maxItems, Resize);
            this.MaxItems = maxItems;
        }

        /// <summary>
        /// QuadTree constructor
        /// </summary>
        /// <param name="size">The size of the QuadTree (i.e. the bottom-right with a top-left of (0,0))</param>
        /// <param name="maxItems">Maximum number of items in any cell of the QuadTree before partitioning</param>
        /// <remarks>This constructor is for ease of use</remarks>
        public QuadTree(Vector2 size, int maxItems)
            : this(new FRect(Vector2.Zero, size), maxItems)
        {
            // Nothing extra to initialize
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an item into the QuadTree
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <remarks>Checks to see if the world needs resizing and does so if needed</remarks>
        public void Insert(QuadTreePositionItem item)
        {
            // check if the world needs resizing
            if (!HeadNode.ContainsRect(item.Rect))
            {
                Resize(new FRect(
                    Vector2.Min(HeadNode.Rect.TopLeft, item.Rect.TopLeft) * 2,
                    Vector2.Max(HeadNode.Rect.BottomRight, item.Rect.BottomRight) * 2));
            }

            HeadNode.Insert(item);
        }

        /// <summary>
        /// Inserts an item into the QuadTree
        /// </summary>
        /// <param name="parent">The parent of the new position item</param>
        /// <param name="position">The position of the new position item</param>
        /// <param name="size">The size of the new position item</param>
        /// <returns>A new position item</returns>
        /// <remarks>Checks to see if the world needs resizing and does so if needed</remarks>
        public QuadTreePositionItem Insert(Collidable parent, Vector2 position, Vector2 size)
        {
            var item = new QuadTreePositionItem(parent, position, size);

            // check if the world needs resizing
            if (!HeadNode.ContainsRect(item.Rect))
            {
                Resize(new FRect(
                    Vector2.Min(HeadNode.Rect.TopLeft, item.Rect.TopLeft) * 2,
                    Vector2.Max(HeadNode.Rect.BottomRight, item.Rect.BottomRight) * 2));
            }

            HeadNode.Insert(item);

            return item;
        }

        /// <summary>
        /// Resizes the Quadtree field
        /// </summary>
        /// <param name="newWorld">The new field</param>
        /// <remarks>This is an expensive operation, so try to initialize the world to a big enough size</remarks>
        public void Resize(FRect newWorld)
        {
            // Get all of the items in the tree
            var components = GetAllItems();

            // Destroy the head node
            HeadNode.Destroy();
            HeadNode = null;

            // Create a new head
            HeadNode = new QuadTreeNode(newWorld, MaxItems, Resize);

            // Reinsert the items
            foreach (var m in components)
            {
                HeadNode.Insert(m);
            }
        }

        #endregion

        #region Query methods

        /// <summary>
        /// Gets a list of items containing a specified point
        /// </summary>
        /// <param name="point">The point</param>
        public List<QuadTreePositionItem> GetItems(Vector2 point)
        {
            return HeadNode.GetItems(point);
        }

        /// <summary>
        /// Gets a list of items intersecting a specified rectangle
        /// </summary>
        /// <param name="inRect">The rectangle</param>        
        public List<QuadTreePositionItem> GetItems(FRect inRect)
        {
            return HeadNode.GetItems(inRect);
        }

        /// <summary>
        /// Get a list of all items in the quadtree
        /// </summary>
        public List<QuadTreePositionItem> GetAllItems()
        {
            return HeadNode.GetAllItems();
        }

        #endregion
    }
}