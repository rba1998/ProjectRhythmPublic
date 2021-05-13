using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectRhythm.Objects;
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
        Texture2D TextureCharacterIdle;
        Texture2D TextureCharacterTalk1;
        Texture2D TextureCharacterTalk2;
        Texture2D TextureWindow;

        Character character;

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
            Background = content.Load<Texture2D>("TitleScreen");
            TextureCharacterIdle = content.Load<Texture2D>("Characters/Rayo/Rayo");
            TextureCharacterTalk1 = content.Load<Texture2D>("Characters/Rayo/RayoTalk1");
            TextureCharacterTalk2 = content.Load<Texture2D>("Characters/Rayo/RayoTalk2");
            TextureWindow = content.Load<Texture2D>("Textures/UI/Window1Anim");
            SpriteFont fontJetset = content.Load<SpriteFont>("Fonts/JetSet");

            character = new Character( TextureCharacterIdle, TextureCharacterTalk1, TextureCharacterTalk2, TextureWindow, fontJetset, -100, -100 );
            character.Talk("Welcome to Project Rhythm!\nMy name is Rayo, and I'll be your navigator!", 180);
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            if ( KeyboardState.IsKeyDown( Keys.Enter ) && !previousKeyboardState.IsKeyDown( Keys.Enter ) )
            {
                GameStateManager.Instance.ChangeScreen( new ScreenSignIn( graphicsDevice, game ) );
            }

            character.Update();

            previousKeyboardState = KeyboardState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(Background, new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight), Color.White);
            character.Draw( spriteBatch );
            spriteBatch.End();
        }
    }
}
