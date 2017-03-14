using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Windoes_Size_Project
{
        
        public class Basketball : TexturedPrimitive
        {
            // Change current position by this amount
            private const float kIncreaseRate = 1.001f;
            private Vector2 kInitSize = new Vector2(5, 5);
            private const float kFinalSize = 15f;

            public Basketball() : base("Basketball")
            {
                mPosition = Camera.RandomPosition(true);
                mSize = kInitSize;
            }

            public bool UpdateAndExplode()
            {
                mSize *= kIncreaseRate;
                return mSize.X > kFinalSize;
            }

        }

}