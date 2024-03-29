﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectRhythm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using ProjectRhythm.Objects.UI;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace ProjectRhythm.GameStates
{
    class RhythmGameResults : GameState
    {
        Game game;
        GraphicsDevice graphicsDevice;
        RhythmGame rhythmgame;
        SpriteFont fontJetset;
        KeyboardState previousKeyboardState;
        WindowText windowText;
        Song song;
        AlbumArt albumart;

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

        /*** Firebase ***/
        IFirebaseConfig fcon = new FirebaseConfig()
        {
            AuthSecret = "5xoe8BN7OjEJ1pSTEzyrUHwKgUeKllHGgEIfYyTA",
            BasePath = "https://project-rhythm-3893d-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        public RhythmGameResults(GraphicsDevice graphicsDevice, Game g, RhythmGame rg, AlbumArt art) : base(graphicsDevice, g)
        {
            this.graphicsDevice = graphicsDevice;
            game = g;
            rhythmgame = rg;

            // Bring in Album Artwork to display
            albumart = art;
            albumart.Bounds.Width *= 2;
            albumart.Bounds.Height *= 2;
            albumart.Bounds.X = ( g.graphics.PreferredBackBufferWidth / 2 ) - ( albumart.Bounds.Width / 2 );
            albumart.Bounds.Y = ( g.graphics.PreferredBackBufferHeight / 2 ) - ( albumart.Bounds.Height / 2 );

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

            song = content.Load<Song>("Music/Menu/result_bgm");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            character = new Character(TextureCharacterIdle, TextureCharacterTalk1, TextureCharacterTalk2, TextureWindow, fontJetset, 1000, -100);

            UInt64 globalpoints = 0; // Earned points that will be added to the global total.
            string charactermessage;

            if ( countBreak == 0 && accuracy >= 100.0f ) // X RANK
            {
                charactermessage = "I... I don't know what to say...\nYou cleared the song with a completely\nperfect rating!\nIt really doesn't get better than that!";
                globalpoints += 5;
            }
            else if ( accuracy >= 95.0f ) // S RANK
            {
                charactermessage = "Wow! You're super accurate!\nYou have an amazing sense of rhythm!";
                globalpoints += 3;
            }
            else if ( accuracy >= 90.0f ) // A RANK
            {
                charactermessage = "You played really well, that was great!\nI always enjoy performing with you!";
                globalpoints += 2;
            }
            else if ( accuracy >= 80.0f ) // B RANK
            {
                charactermessage = "Nice one! That was a solid performance.\nI'm excited to see what you play next!";
                globalpoints += 1;
            }
            else if ( accuracy >= 70.0f ) // C RANK
            {
                charactermessage = "Woah, just made it!\nThat one was hard, but you got through!\nI never gave up hope!";
                globalpoints += 1;
            }
            else // D RANMK
            {
                charactermessage = "Oops... that's okay though.\nGive it another shot, you can do it!";
                globalpoints = 0;
            }

            if (countBreak == 0 && accuracy < 100.0f) // MAX CHAIN
            {
                charactermessage = "Amazing! you got a full chain!\nYou're a really skilled performer!\nUm... would you show me again sometime?";
                globalpoints += 1;
            }

            character.Talk(charactermessage, 360);

            // Try connecting to Firebase server
            try
            {
                client = new FireSharp.FirebaseClient(fcon);
                windowText = new WindowText(TextureWindow, fontJetset, "\nCommunicating with server...", 744, 100);

                // Get the current amount of global points
                var getter = client.Get("GlobalData/Total Points");
                UInt64 currenttotal = getter.ResultAs<UInt64>();
                windowText = new WindowText(TextureWindow, fontJetset, "\nData get successful.", 744, 100);

                // Add the amount of points the user scored this game
                UInt64 newtotal = currenttotal + globalpoints;

                // Set the global points to the newly updated value
                var setter = client.Set("GlobalData/Total Points", newtotal);

                if (globalpoints > 0)
                {
                    windowText = new WindowText(TextureWindow, fontJetset, "You earned " + globalpoints + " points this round.\n" +
                                                                           "Global total: " + newtotal + "\n" +
                                                                           "Thank you for your contribution, pilot.\n" +
                                                                           "We look forward to all of your hard work.\n" +
                                                                           "--Mission Control", 744, 100);
                }
                else if ( globalpoints == 0 )
                {
                    windowText = new WindowText(TextureWindow, fontJetset, "You earned " + globalpoints + " points this round.\n" +
                                                                           "Global total: " + newtotal + "\n" +
                                                                           "Keep practicing, pilot. We know you can do it.\n" +
                                                                           "We look forward to all of your hard work.\n" +
                                                                           "--Mission Control", 744, 100);
                }
            }
            catch
            {
                windowText = new WindowText( TextureWindow, fontJetset, "Error connecting to server.", 744, 100 );
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
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Stop();
                GameStateManager.Instance.ChangeScreen(new TitleScreen(graphicsDevice, game));
            }

            character.Update();
            albumart.Update();

            if ( windowText != null )
            {
                windowText.Update();
            }

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
            albumart.Draw( spriteBatch );
            if (windowText != null)
            {
                windowText.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
