using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.GameStates
{
    public class RhythmGame : GameState
    {
        public RhythmGame( GraphicsDevice graphicsDevice ) : base( graphicsDevice )
        {

        }
        public override void Initialize()
        {

        }

        public override void LoadContent( ContentManager content )
        {

        }

        public override void UnloadContent()
        {

        }

        public override void Update( GameTime gameTime )
        {

        }

        public override void Draw( SpriteBatch spriteBatch )
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            // Draw sprites here
            spriteBatch.End();
        }
    }
}
