using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.GameStates
{
    public class TitleScreen : GameState
    {
        Game game;
        GraphicsDevice graphicsDevice;
        KeyboardState previousKeyboardState;

        Texture2D Background;

        public TitleScreen(GraphicsDevice graphicsDevice, Game g) : base(graphicsDevice, g)
        {
            this.graphicsDevice = graphicsDevice;
            game = g;

            previousKeyboardState = Keyboard.GetState();
        }

        public override void Initialize()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Background = content.Load<Texture2D>("TitleScreenPlaceholder");
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            if ( KeyboardState.IsKeyDown( Keys.Enter ) && !previousKeyboardState.IsKeyDown( Keys.Enter ) )
            {
                GameStateManager.Instance.ChangeScreen( new RhythmGame( graphicsDevice, game, "Freedom Dive" ) );
            }

            previousKeyboardState = KeyboardState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(Background, new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }
    }
}
