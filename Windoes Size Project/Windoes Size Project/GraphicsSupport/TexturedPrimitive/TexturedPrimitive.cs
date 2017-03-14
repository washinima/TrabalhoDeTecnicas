using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windoes_Size_Project
{
    /// <summary>
    /// TexturedPrimitive class
    /// </summary>
    public class TexturedPrimitive
    {
        // Support for drawing the image
        protected Texture2D mImage;     // The UWB-JPG.jpg image to be loaded
        public Vector2 mPosition;    // Center position of image        ***public porque na classe Game1 no método draw, não conseguia aceder a esta variável quando é protected***
        protected Vector2 mSize;        // Size of the image to be drawn
        public Vector2 MinBound { get { return mPosition - (0.5f * mSize); } }
        public Vector2 MaxBound { get { return mPosition + (0.5f * mSize); } }
        /// Accessors to the camera window bounds

        /// <summary>
        /// Constructor of TexturePrimitive
        /// </summary>
        /// <param name="imageName">name of the image to be loaded as texture.</param>
        /// <param name="position">top left pixel position of the texture to be drawn</param>
        /// <param name="size">width/height of the texture to be drawn</param>
        public TexturedPrimitive(String imageName, Vector2 position, Vector2 size)
        {
            mImage = Game1.sContent.Load<Texture2D>(imageName);
            mPosition = position;
            mSize = size;
        }

        public TexturedPrimitive(String imageName)
        {
            mImage = Game1.sContent.Load<Texture2D>(imageName);
        }

        /// <summary>
        /// Allows the primitive object to update its state
        /// </summary>
        /// <param name="deltaTranslate">Amount to change the position of the primitive. 
        ///                              Value of 0 says position is not changed.</param>
        /// <param name="deltaScale">Amount to change of the scale the primitive. 
        ///                          Value of 0 says size is not changed.</param>
        public void Update(Vector2 deltaTranslate, Vector2 deltaScale)
        {
            mPosition += deltaTranslate;
            mSize += deltaScale;
        }

        /// <summary>
        /// Draw the primitive
        /// </summary>
        public void Draw()
        {
            // Defines location and size of the texture
            Rectangle destRect = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)mSize.X, (int)mSize.Y);
            // Defines where and size of the texture to show
            destRect = Camera.ComputePixelRectangle(mPosition, mSize);
            Game1.sSpriteBatch.Draw(mImage, destRect, Color.White);
        }

        public bool PrimitivesTouches(TexturedPrimitive otherPrim)
        {
            Vector2 v = mPosition - otherPrim.mPosition;
            float dist = v.Length();
            return (dist < ((mSize.X / 2f) + (otherPrim.mSize.X / 2f)));
        }
    }
}
