using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectRhythm.Objects.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.GameStates
{
    class ScreenSignIn : GameState
    {
        Texture2D Background;
        Game game;
        GraphicsDevice graphicsDevice;
        KeyboardState previousKeyboardState;

        Texture2D textureWindow1;

        WindowCreateOrSignIn window1;

        public ScreenSignIn(GraphicsDevice graphicsDevice, Game g) : base(graphicsDevice, g)
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
            textureWindow1 = content.Load<Texture2D>("Textures/UI/Window1Anim");
            window1 = new WindowCreateOrSignIn(game, textureWindow1);

            window1.LoadContent( content );
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                GameStateManager.Instance.ChangeScreen(new RhythmGame(graphicsDevice, game, "Freedom Dive"));
            }

            window1.Update(gameTime);

            previousKeyboardState = KeyboardState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            window1.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
