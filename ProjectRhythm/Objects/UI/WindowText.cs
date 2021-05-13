using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectRhythm.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects.UI
{
    class WindowText
    {
        Texture2D texture;
        public BoundingRectangle Bounds;
        private Rectangle sheetlocation;
        private Vector2 textPosition;

        private bool AnimationDone;
        SpriteFont fontJetset;

        KeyboardState previousKeyboardState;

        public string message;
        private string print;

        public WindowText(Texture2D t, SpriteFont font, string text, float x, float y)
        {
            texture = t;
            fontJetset = font;
            message = text;
            print = "";

            Bounds.Height = 208;
            Bounds.Width = 431;
            Bounds.X = x;
            Bounds.Y = y;

            sheetlocation = new Rectangle(0, 0, 431, 208);
            textPosition = new Vector2( x + 25, y + 50 );
            previousKeyboardState = Keyboard.GetState();

            AnimationDone = false;
        }

        public void Update()
        {
            // Animation of the window opening
            if (!AnimationDone)
            {
                int x = sheetlocation.X;
                x += 431;
                if (x >= 3448)
                {
                    if (sheetlocation.Y != 208)
                    {
                        sheetlocation.Y = 208;
                        sheetlocation.X = 0;
                    }
                    else
                    {
                        AnimationDone = true;
                    }
                }
                else
                {
                    sheetlocation.X = x;
                }
            }
            else // Execute this whenever the window is done opening
            {
                print = message;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Bounds, sheetlocation, Color.White);
            
            if ( AnimationDone )
            {
                sb.DrawString(fontJetset, print, textPosition, Color.White);
            }
        }
    }
}
