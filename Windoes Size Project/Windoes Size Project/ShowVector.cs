using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windoes_Size_Project
{
    class ShowVector
    {
        protected static Texture2D sImage = null; // Singleton for the class
        private static float kLenToWidthRatio = 0.2f;

        static private void LoadImage()
        {
            if (null == sImage)
                ShowVector.sImage = Game1.sContent.Load<Texture2D>("RightArrow");
        }

        static public void DrawPointVector(Vector2 from, Vector2 dir)
        {
            #region Step 4b. Compute the angle to rotate
            float length = dir.Length();
            float theta = 0f;
            if (length > 0.001f)
            {
                dir /= length;
                theta = (float)Math.Acos((double)dir.X);
                if (dir.X < 0.0f)
                {
                    if (dir.Y > 0.0f)
                        theta = -theta;
                }
                else
                {
                    if (dir.Y > 0.0f)
                        theta = -theta;
                }
            }
            #endregion

            #region Step 4c. Draw Arrow
            // Define location and size of the texture
            Vector2 size = new Vector2(length, kLenToWidthRatio * length);
            Rectangle destRect = Camera.ComputePixelRectangle(from, size);

            // destRect is computed with respect to the "from" position, on the left-side of the texture
            // we only need to offset the reference in the y from top-left to middle-left
            Vector2 org = new Vector2(0f, ShowVector.sImage.Height / 2f);

            Game1.sSpriteBatch.Draw(ShowVector.sImage, destRect, null, Color.White, theta, org, SpriteEffects.None, 0f);
            #endregion

            #region Step 4d. Print status message
            String msg;
            msg = "Direction=" + dir + "\nSize=" + length;
            FontSupport.PrintStatusAt(from + new Vector2(2, 5), msg, Color.White);
            #endregion
        }

        static public void DrawFromTo(Vector2 from, Vector2 to)
        {
            DrawPointVector(from, to - from);
        }

        static public Vector2 RotateVectorByAngle(Vector2 v, float angleInRadian)
        {
            float sinTheta = (float)(Math.Sin((double)angleInRadian));
            float cosTheta = (float)(Math.Cos((double)angleInRadian));
            float x, y;
            x = cosTheta * v.X + sinTheta * v.Y;
            y = -sinTheta * v.X + cosTheta * v.Y;
            return new Vector2(x, y);
        }
    }
}
