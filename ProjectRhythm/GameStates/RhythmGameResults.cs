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

        /*** Stat positions ***/
        Vector2 posPerfect, posGood, posBreak, posMax, posAccu;

        Texture2D Background;

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

            posPerfect = new Vector2(400, 325);
            posGood = new Vector2(400, 490);
            posBreak = new Vector2(400, 655);
            posMax = new Vector2(400, 820);
            posAccu = new Vector2(400, 985);
        }

        public override void Initialize()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Background = content.Load<Texture2D>("ResultsScreenPlaceholder");
            fontJetset = content.Load<SpriteFont>("Fonts/JetSet");
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

            previousKeyboardState = KeyboardState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(Background, new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.DrawString(fontJetset, countPerfect.ToString(), posPerfect, Color.AliceBlue);
            spriteBatch.DrawString(fontJetset, countGood.ToString(), posGood, Color.Aquamarine);
            spriteBatch.DrawString(fontJetset, countBreak.ToString(), posBreak, Color.Red);
            spriteBatch.DrawString(fontJetset, maxChain.ToString(), posMax, Color.Aquamarine);
            spriteBatch.DrawString(fontJetset, accuracy.ToString(), posAccu, Color.Aquamarine);
            spriteBatch.End();
        }
    }
}
