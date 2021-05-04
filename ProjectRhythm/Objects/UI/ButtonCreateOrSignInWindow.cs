using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects.UI
{
    class ButtonCreateOrSignInWindow
    {
        Texture2D textureDesel;
        Texture2D textureSel;
        public BoundingRectangle Bounds;
        private Rectangle sheetLocation;

        public bool Selected;

        public ButtonCreateOrSignInWindow( Texture2D tDesel, Texture2D tSel, float x, float y )
        {
            textureDesel = tDesel;
            textureSel = tSel;

            Bounds.X = x;
            Bounds.Y = y;
            Bounds.Width = 171;
            Bounds.Height = 94;

            Selected = false;

            sheetLocation = new Rectangle(0, 0, 171, 94);
        }

        public void Update()
        {
            int x = sheetLocation.X;
            x += 171;
            if (x >= 1539)
            {
                if (sheetLocation.Y < 564)
                {
                    sheetLocation.Y += 94;
                    sheetLocation.X = 0;
                }
                else
                {
                    sheetLocation.X = 0;
                    sheetLocation.Y = 0;
                }
            }
            else
            {
                sheetLocation.X = x;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if ( Selected )
            {
                sb.Draw(textureSel, Bounds, sheetLocation, Color.White);
            }
            else
            {
                sb.Draw(textureDesel, Bounds, Color.White);
            }
        }
    }
}
