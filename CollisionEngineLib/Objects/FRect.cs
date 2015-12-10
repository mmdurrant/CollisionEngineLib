using System;
using Microsoft.Xna.Framework;

namespace CollisionEngineLib.Objects
{
[Serializable]
    public struct FRect
    {
        #region Properties

        /// <summary>
        /// The top left of this rectangle
        /// </summary>
        private Vector2 topLeft;

        /// <summary>
        /// The bottom right of this rectangle
        /// </summary>
        private Vector2 bottomRight;

        /// <summary>
        /// Gets the top left of this rectangle
        /// </summary>
        public Vector2 TopLeft
        {
            get { return topLeft; }
            set { topLeft = value; }
        }

        /// <summary>
        /// Gets the top right of this rectangle
        /// </summary>
        public Vector2 TopRight
        {
            get { return new Vector2(bottomRight.X, topLeft.Y); }
            set
            {
                bottomRight.X = value.X;
                topLeft.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets the bottom right of this rectangle
        /// </summary>
        public Vector2 BottomRight
        {
            get { return bottomRight; }
            set { bottomRight = value; }
        }

        /// <summary>
        /// Gets the bottom left of this rectangle
        /// </summary>
        public Vector2 BottomLeft
        {
            get { return new Vector2(topLeft.X, bottomRight.Y); }
            set
            {
                topLeft.X = value.X;
                bottomRight.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets the top of this rectangle
        /// </summary>
        public float Top
        {
            get { return TopLeft.Y; }
            set { topLeft.Y = value; }
        }

        /// <summary>
        /// Gets the left of this rectangle
        /// </summary>
        public float Left
        {
            get { return TopLeft.X; }
            set { topLeft.X = value; }
        }

        /// <summary>
        /// Gets the bottom of this rectangle
        /// </summary>
        public float Bottom
        {
            get { return BottomRight.Y; }
            set { bottomRight.Y = value; }
        }

        /// <summary>
        /// Gets the right of this rectangle
        /// </summary>
        public float Right
        {
            get { return BottomRight.X; }
            set { bottomRight.X = value; }
        }

        /// <summary>
        /// Gets the width of this rectangle
        /// </summary>
        public float Width
        {
            get { return bottomRight.X - topLeft.X; }
        }

        /// <summary>
        /// Gets the height of this rectangle
        /// </summary>
        public float Height
        {
            get { return bottomRight.Y - topLeft.Y; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Floating-point rectangle constructor
        /// </summary>
        /// <param name="topleft">The top left point of the rectangle</param>
        /// <param name="bottomright">The bottom right point of the rectangle</param>
        public FRect(Vector2 topleft, Vector2 bottomright)
        {
            topLeft = topleft;
            bottomRight = bottomright;
        }

        /// <summary>
        /// Floating-point rectangle constructor
        /// </summary>
        /// <param name="top">The top of the rectangle</param>
        /// <param name="left">The left of the rectangle</param>
        /// <param name="bottom">The bottom of the rectangle</param>
        /// <param name="right">The right of the rectangle</param>
        public FRect(float top, float left, float bottom, float right)
        {
            topLeft = new Vector2(left, top);
            bottomRight = new Vector2(right, bottom);
        }

        #endregion

        #region Intersection testing functions

        /// <summary>
        /// Checks if this rectangle contains a point
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <returns>Whether or not this rectangle contains the point</returns>
        public bool Contains(Vector2 point)
        {
            return (topLeft.X <= point.X && bottomRight.X >= point.X &&
                    topLeft.Y <= point.Y && bottomRight.Y >= point.Y);
        }

        /// <summary>
        /// Checks if this rectangle intersects another rectangle
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>Whether or not this rectangle intersects the other</returns>
        public bool Intersects(FRect rect)
        {
            return (TopLeft.X < rect.BottomRight.X &&
                    BottomRight.X > rect.TopLeft.X &&
                    TopLeft.Y < rect.BottomRight.Y &&
                    BottomRight.Y > rect.TopLeft.X);
        }

        #endregion
    }
}