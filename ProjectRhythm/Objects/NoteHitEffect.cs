using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    class NoteHitEffect
    {
        public Texture2D texture;
        public BoundingRectangle Bounds;
        private Rectangle sheetlocation;

        public bool AnimationDone;

        public NoteHitEffect( Texture2D t, float x, float y )
        {
            texture = t;
            Bounds.Width = 258;
            Bounds.Height = 170;
            Bounds.X = x;
            Bounds.Y = y;

            sheetlocation = new Rectangle( 0, 0, 258, 170 );
        }

        public void Update()
        {
            int x = sheetlocation.X;
            x += 258;
            if ( x >= 3870 )
            {
                AnimationDone = true;
            }
            else
            {
                sheetlocation.X = x;
            }
        }

        public void Draw( SpriteBatch sb )
        {
            sb.Draw( texture, Bounds, sheetlocation, Color.White );
        }
    }
}
