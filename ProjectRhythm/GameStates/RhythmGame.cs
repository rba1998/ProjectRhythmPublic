using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectRhythm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.GameStates
{
    public class RhythmGame : GameState
    {
        Game game;
        List<Note> listnote;

        /**** Textures ****/
        Texture2D txtNote;

        public RhythmGame( GraphicsDevice graphicsDevice, Game g ) : base( graphicsDevice, g )
        {
            game = g;
        }

        public override void Initialize()
        {
            listnote = new List<Note>();
        }

        public override void LoadContent( ContentManager content )
        {
            txtNote = content.Load<Texture2D>( "futaba" );

            listnote.Add( new Note( game, txtNote ) );
        }

        public override void UnloadContent()
        {

        }

        public override void Update( GameTime gameTime )
        {

        }

        public override void Draw( SpriteBatch spriteBatch )
        {
            int i; //iterative variable

            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            for ( i = 0; i < listnote.Count; i++ )
            {
                listnote[ i ].Draw( spriteBatch );
            }
            // Draw sprites here
            spriteBatch.End();
        }
    }
}
