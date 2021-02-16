using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    public class Note
    {
        Game game;
        public BoundingRectangle Bounds;
        Texture2D texture;

        public Note( Game g, Texture2D t )
        {
            game = g;
            texture = t;
            Bounds.Width = 716;
            Bounds.Height = 691;
            Bounds.X = 0;
            Bounds.Y = 1080 - 691;
        }

        public void Update( GameTime gt )
        {

        }

        public void Draw( SpriteBatch sb )
        {
            sb.Draw( texture, Bounds, Color.White );
        }
    }
}
