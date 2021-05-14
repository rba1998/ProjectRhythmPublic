using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    class AlbumArt
    {
        Texture2D art;
        public BoundingRectangle Bounds;

        public AlbumArt( Texture2D t, float x, float y )
        {
            art = t;

            Bounds.X = x;
            Bounds.Y = y;
            Bounds.Width = 250;
            Bounds.Height = 250;
        }

        public void Update()
        {

        }

        public void Draw( SpriteBatch sb )
        {
            sb.Draw(art, Bounds, Color.White);
        }
    }
}
