using System;
using Microsoft.Xna.Framework;

namespace CollisionEngineLib.Objects
{
    /// <summary>
    /// A position item in a quadtree
    /// </summary>
    /// <typeparam name="T">The type of the QuadTree item's parent</typeparam>
    [Serializable]
    public class QuadTreePositionItem
    {
        #region Events and Event Handlers

        /// <summary>
        /// A movement handler delegate
        /// </summary>
        /// <param name="positionItem">The item that fired the event</param>
        public delegate bool MoveHandler(QuadTreePositionItem positionItem);

        /// <summary>
        /// A destruction handler delegate - fired when the item is destroyed
        /// </summary>
        /// <param name="positionItem">The item that fired the event</param>
        public delegate bool DestroyHandler(QuadTreePositionItem positionItem);

        /// <summary>
        /// Event handler for the move action
        /// </summary>
        public event MoveHandler Move;

        /// <summary>
        /// Event handler for the destroy action
        /// </summary>
        public event DestroyHandler Destroy;

        /// <summary>
        /// Handles the move event
        /// </summary>
        protected void OnMove(Vector2 offSet)
        {
            // Update rectangles
            rect.TopLeft = position - (size * .5f);
            rect.BottomRight = position + (size * .5f);
            rect.Polygon.Offset(offSet);
            // Call event handler
            Move?.Invoke(this);
        }

        /// <summary>
        /// Handles the destroy event
        /// </summary>
        protected void OnDestroy()
        {
            Destroy?.Invoke(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The center position of this item
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Gets or sets the center position of this item
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                var offSet = value - position;
                if (value.X < 0)
                {
                    offSet.X *= -1;
                }
                if (value.Y < 0)
                {
                    offSet.Y *= -1;
                }
                position = value;
                OnMove(offSet);
            }
        }

        /// <summary>
        /// The size of this item
        /// </summary>
        private Vector2 size;

        /// <summary>
        /// Gets or sets the size of this item
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                rect.TopLeft = position - (size / 2f);
                rect.BottomRight = position + (size / 2f);
                OnMove(Vector2.Zero);
            }
        }

        /// <summary>
        /// The rectangle containing this item
        /// </summary>
        private FRect rect;

        /// <summary>
        /// Gets a rectangle containing this item
        /// </summary>
        public FRect Rect
        {
            get { return rect; }
        }

        /// <summary>
        /// The parent of this item
        /// </summary>
        /// <remarks>The Parent accessor is used to gain access to the item controlling this position item</remarks>
        private Collidable parent;

        /// <summary>
        /// Gets the parent of this item
        /// </summary>
        public Collidable Parent
        {
            get { return parent; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a position item in a QuadTree
        /// </summary>
        /// <param name="parent">The parent of this item</param>
        /// <param name="position">The position of this item</param>
        /// <param name="size">The size of this item</param>
        public QuadTreePositionItem(Collidable parent, Vector2 position, Vector2 size)
        {
            this.rect = new FRect(position.Y, position.X, position.Y + size.Y, position.X + size.X);
            this.parent = parent;
            this.position = position;
            this.size = size;
            OnMove(Vector2.Zero);
        }

        public QuadTreePositionItem(Collidable parent, Vector2 position, Polygon polygon)
        {
            this.rect = new FRect(polygon);
            this.parent = parent;
            this.position = position;
            this.size = rect.BottomRight;
            OnMove(Vector2.Zero);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Destroys this item and removes it from the QuadTree
        /// </summary>
        public void Delete()
        {
            OnDestroy();
        }

        #endregion
    }
}