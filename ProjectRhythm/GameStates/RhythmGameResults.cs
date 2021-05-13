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
    class RhythmGameResults : GameState
    {
        Game game;
        GraphicsDevice graphicsDevice;
        RhythmGame rhythmgame;
        SpriteFont fontJetset;
        KeyboardState previousKeyboardState;

        /*** Display Stats ***/
        int countPerfect;
        int countGood;
        int countBreak;
        int maxChain;
        float accuracy;
        float accuracyFormatted;

        /*** Stat positions ***/
        Vector2 posPerfect, posGood, posBreak, posMax, posAccu;

        Texture2D Background;
        Texture2D TextureCharacterIdle;
        Texture2D TextureCharacterTalk1;
        Texture2D TextureCharacterTalk2;
        Texture2D TextureWindow;

        Character character;

        public RhythmGameResults(GraphicsDevice graphicsDevice, Game g, RhythmGame rg) : base(graphicsDevice, g)
        {
            this.graphicsDevice = graphicsDevice;
            game = g;
            rhythmgame = rg;

            countPerfect = rhythmgame.countPerfect;
            countGood = rhythmgame.countGood;
            countBreak = rhythmgame.countBreak;
            maxChain = rhythmgame.maxChain;
            accuracy = (rhythmgame.hitNoteCount / rhythmgame.totalNoteCount) * 100.0f;
            accuracyFormatted = Convert.ToSingle(decimal.Round((decimal)accuracy, 2, MidpointRounding.AwayFromZero));

            posPerfect = new Vector2(450, 185);
            posGood = new Vector2(450, 330);
            posBreak = new Vector2(450, 475);
            posMax = new Vector2(450, 630);
            posAccu = new Vector2(450, 770);

            previousKeyboardState = Keyboard.GetState();
        }

        public override void Initialize()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Background = content.Load<Texture2D>("Results Screen 2");
            TextureCharacterIdle = content.Load<Texture2D>("Characters/Rayo/Rayo");
            TextureCharacterTalk1 = content.Load<Texture2D>("Characters/Rayo/RayoTalk1");
            TextureCharacterTalk2 = content.Load<Texture2D>("Characters/Rayo/RayoTalk2");
            TextureWindow = content.Load<Texture2D>("Textures/UI/Window1Anim");
            fontJetset = content.Load<SpriteFont>("Fonts/JetSet");

            character = new Character(TextureCharacterIdle, TextureCharacterTalk1, TextureCharacterTalk2, TextureWindow, fontJetset, 1000, -100);

            if ( countBreak == 0 && accuracy >= 100.0f )
            {
                character.Talk("I... I don't know what to say...\nYou cleared the song with a completely\nperfect rating!\nIt really doesn't get better than that!", 360);
            }
            else if ( countBreak == 0 )
            {
                character.Talk("Amazing! you got a full chain!\nYou're a really skilled performer!\nUm... would you show me again sometime?", 360);
            }
            else if ( accuracy >= 95.0f )
            {
                character.Talk("Wow! You're super accurate!\nYou have an amazing sense of rhythm!", 360);
            }
            else if ( accuracy >= 90.0f )
            {
                character.Talk("You played really well, that was great!\nI always enjoy performing with you!", 360);
            }
            else if ( accuracy >= 80.0f )
            {
                character.Talk("Nice one! That was a solid performance.\nI'm excited to see what you play next!", 360);
            }
            else if ( accuracy >= 70.0f )
            {
                character.Talk("Woah, just made it!\nThat one was hard, but you got through!\nI never gave up hope!", 360);
            }
            else
            {
                character.Talk("Oops... that's okay though.\nGive it another shot, you can do it!", 360);
            }
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                GameStateManager.Instance.ChangeScreen(new TitleScreen(graphicsDevice, game));
            }

            character.Update();

            previousKeyboardState = KeyboardState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(Background, new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.DrawString(fontJetset, countPerfect.ToString(), posPerfect, Color.DarkBlue, 0.0f, new Vector2(0, 0), 2.0f, new SpriteEffects(), 1);
            spriteBatch.DrawString(fontJetset, countGood.ToString(), posGood, Color.Green, 0.0f, new Vector2(0, 0), 2.0f, new SpriteEffects(), 1);
            spriteBatch.DrawString(fontJetset, countBreak.ToString(), posBreak, Color.Red, 0.0f, new Vector2(0, 0), 2.0f, new SpriteEffects(), 1);
            spriteBatch.DrawString(fontJetset, maxChain.ToString(), posMax, Color.Black, 0.0f, new Vector2(0, 0), 2.0f, new SpriteEffects(), 1);
            spriteBatch.DrawString(fontJetset, accuracyFormatted.ToString(), posAccu, Color.Black, 0.0f, new Vector2(0, 0), 2.0f, new SpriteEffects(), 1);
            character.Draw( spriteBatch );
            spriteBatch.End();
        }
    }
}
