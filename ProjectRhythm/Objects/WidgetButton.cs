using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectRhythm.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    class WidgetButton
    {
        Game game;
        RhythmGame rhythmgame;
        Texture2D textureOff;
        Texture2D textureOn;
        Keys buttonKey;
        public BoundingRectangle Bounds;

        public WidgetButton(Game g, RhythmGame rg, Texture2D tOff, Texture2D tOn, float x, float y, int width, int height, Keys key)
        {
            game = g;
            rhythmgame = rg;
            textureOff = tOff;
            textureOn = tOn;

            Bounds.X = x;
            Bounds.Y = y;
            Bounds.Width = width;
            Bounds.Height = height;

            buttonKey = key;
        }

        public void Update(GameTime gt)
        {
            
        }

        public void Draw(SpriteBatch sb)
        {
            if (Keyboard.GetState().IsKeyDown(buttonKey))
            {
                sb.Draw(textureOn, Bounds, Color.White);
            }
            else
            {
                sb.Draw(textureOff, Bounds, Color.White);
            }
            
        }
    }
}
