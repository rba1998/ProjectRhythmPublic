using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectRhythm.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    class Overlay
    {
        Game game;
        RhythmGame rhythmgame;
        Texture2D texture;
        public BoundingRectangle Bounds;

        public Overlay( Game g, RhythmGame rg, Texture2D t )
        {
            game = g;
            rhythmgame = rg;
            texture = t;

            Bounds.X = 0;
            Bounds.Y = 0;
            Bounds.Width = 1920;
            Bounds.Height = g.graphics.PreferredBackBufferHeight;
            Bounds.Width = g.graphics.PreferredBackBufferWidth;
        }

        public void Update(GameTime gt)
        {
            
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Bounds, Color.White);
        }
    }
}
