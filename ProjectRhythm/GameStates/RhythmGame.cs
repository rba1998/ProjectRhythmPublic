﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        GraphicsDevice graphicsDevice;

        /**** Objects and Object Holders ****/
        Overlay overlay;
        Song song;
        SoundEffect hitsound;
        List<Note> listnote;
        List<Note> listnote_active;
        System.IO.StreamReader file;
        List<WidgetButton> listbutton;
        List<NoteHitEffect> notehits;
        AlbumArt albumart;

        /**** Instance Variables ****/
        UInt64 framecount;
        public const int JudgeLineDistance = 993;
        public string mapname;
        public string songname;
        public string albumname;
        public string artistname;
        public float bpm;
        public float offset;
        public float beatsPerSec;
        public float framesPerBeat;
        private KeyboardState previousKeyState;
        private bool textflash;
        private int textTimer;
        private int textDisappearTime;

        public int Chain;
        public int Score;
        private String chainText;
        private Color chainColor;
        public float hitNoteCount;
        public float totalNoteCount;

        /**** Modifiers ****/
        public float multiplierNotespeed;

        /**** Timing Calibration Variables ****/
        public int calibJudgementLine;
        public int calibBeat;
        public UInt64 calibNoteTiming;
        private float accuPerfect;
        private float accuOkay;

        /**** Game Stats ****/
        public int countPerfect;
        public int countGood;
        public int countBreak;
        public int maxChain;

        /**** Timers ****/
        float timerOffset;
        float timerBeat;
        int timerEndGame;

        /**** Textures ****/
        Texture2D txtNote;
        Texture2D txtOverlay;
        Texture2D txtNoteHit;
        Texture2D txtAlbumArt;

        /****  Fonts   ****/
        SpriteFont fontJetset;

        /**** Debug Variables ****/
        bool enableMetronome;
        string linetest;

        public RhythmGame( GraphicsDevice graphicsDevice, Game g, String s ) : base( graphicsDevice, g )
        {
            this.graphicsDevice = graphicsDevice;
            game = g;
            mapname = s;
            linetest = "Placeholder Debug Text";

            try
            {
                file = new System.IO.StreamReader(@"Maps/" + mapname + ".txt");
                string line;
                string[] splitline;

                // Get song name, album name, artist name, bpm, and offset (first 5 lines of map)
                line = file.ReadLine(); // Song Name
                splitline = line.Split('\n');
                songname = splitline[0];

                line = file.ReadLine(); // Album Name
                albumname = line;

                line = file.ReadLine(); // Artist Name
                artistname = line;

                line = file.ReadLine(); // BPM
                bpm = float.Parse( line );

                line = file.ReadLine(); // Offset
                offset = float.Parse( line );

                framecount = 0;
            }
            catch ( Exception ex )
            {
                game.Exit();
            }

            // Do math that will be needed for keeping beat and note placement timing
            beatsPerSec = bpm / 60.0f;
            framesPerBeat = 60.0f / beatsPerSec;

            enableMetronome = false;
            timerBeat = framesPerBeat * 2;
            timerOffset = 0;

            // Initialize Player-set modifiers and Calibration Variables
            calibJudgementLine = 200;
            calibNoteTiming = 10;
            multiplierNotespeed = 1.0f;
            accuPerfect = 5.0f;
            accuOkay = 10.0f;

            // Initialize game variables
            Chain = 0;
            maxChain = 0;
            Score = 0;
            totalNoteCount = 0;
            textDisappearTime = 60;
            textTimer = 0;
            countBreak = 0;
            countGood = 0;
            countPerfect = 0;
            timerEndGame = 90;
        }

        public override void Initialize()
        {
            listnote = new List<Note>();
            listnote_active = new List<Note>();
            listbutton = new List<WidgetButton>();
            notehits = new List<NoteHitEffect>();
        }

        public override void LoadContent( ContentManager content )
        {
            /**** Load Assets ****/
            // Load Song
            song = content.Load<Song>( "Music/" + songname );

            // Load Album Art
            if (albumname != "n/a")
            {
                try
                {
                    txtAlbumArt = content.Load<Texture2D>("Textures/Album Art/" + albumname);
                }
                catch
                {
                    txtAlbumArt = content.Load<Texture2D>("Textures/Album Art/defaultalbumart");
                }
            }
            else
            {
                txtAlbumArt = content.Load<Texture2D>("Textures/Album Art/defaultalbumart");
            }
            albumart = new AlbumArt(txtAlbumArt, 10, 10);

            // Load SFX
            hitsound = content.Load<SoundEffect>( "sfx/normal-hitclap" );

            // Load Fonts
            fontJetset = content.Load<SpriteFont>( "Fonts/JetSet" );

            // Load Textures
            txtNote = content.Load<Texture2D>( "TestNote" );
            txtOverlay = content.Load<Texture2D>( "TestOverlay2" );
            txtNoteHit = content.Load<Texture2D>( "Textures/NoteHit" );

            
            /**** Further Initialization of game objects ****/
            overlay = new Overlay( game, this, txtOverlay );
            
            listbutton.Add(new WidgetButton(game, this,
                                            content.Load<Texture2D>( "Textures/Overlay/Button1" ),
                                            content.Load<Texture2D>( "Textures/Overlay/Button1On" ),
                                            555, 920, 164, 84, Microsoft.Xna.Framework.Input.Keys.S ) );
            listbutton.Add(new WidgetButton(game, this,
                                            content.Load<Texture2D>("Textures/Overlay/Button2"),
                                            content.Load<Texture2D>("Textures/Overlay/Button2On"),
                                            700, 920, 137, 84, Microsoft.Xna.Framework.Input.Keys.D));
            listbutton.Add(new WidgetButton(game, this,
                                            content.Load<Texture2D>("Textures/Overlay/Button3"),
                                            content.Load<Texture2D>("Textures/Overlay/Button3On"),
                                            835, 920, 117, 84, Microsoft.Xna.Framework.Input.Keys.F));
            listbutton.Add(new WidgetButton(game, this,
                                            content.Load<Texture2D>("Textures/Overlay/Button4"),
                                            content.Load<Texture2D>("Textures/Overlay/Button4On"),
                                            967, 920, 117, 84, Microsoft.Xna.Framework.Input.Keys.J));
            listbutton.Add(new WidgetButton(game, this,
                                            content.Load<Texture2D>("Textures/Overlay/Button5"),
                                            content.Load<Texture2D>("Textures/Overlay/Button5On"),
                                            1082, 920, 137, 84, Microsoft.Xna.Framework.Input.Keys.K));
            listbutton.Add(new WidgetButton(game, this,
                                            content.Load<Texture2D>("Textures/Overlay/Button6"),
                                            content.Load<Texture2D>("Textures/Overlay/Button6On"),
                                            1200, 920, 164, 84, Microsoft.Xna.Framework.Input.Keys.L));


            ReadFile();

            MediaPlayer.Play( song );
        }

        public override void UnloadContent()
        {

        }

        public override void Update( GameTime gameTime )
        {
            int i; //iterative variable

            timerOffset++;

            // Calibrate current frame to be in-time with the position of the song.
            framecount = Convert.ToUInt64(MediaPlayer.PlayPosition.TotalSeconds * 60);

            // Beat
            if (timerOffset >= offset)
            {
                if (timerBeat >= framesPerBeat)
                {
                    if (enableMetronome)
                        hitsound.Play();
                    timerBeat -= framesPerBeat;
                }
                timerBeat++;
            }

            // Update Overlay
            overlay.Update( gameTime );

            // Update Album Art
            albumart.Update();

            // Update Idle Notes
            for ( i = 0; i < listnote.Count; i++ )
            {

                if ( framecount >= listnote[ i ].spawnframe )
                {
                    listnote_active.Add( listnote[ i ] );
                    listnote.RemoveAt( i );
                }
            }
            // Update Active Notes
            for ( i = 0; i < listnote_active.Count; i++ )
            {
                listnote_active[ i ].Update( gameTime );

                // Double Notes
                if ( i + 1 < listnote_active.Count )
                {
                    if ( listnote_active[ i ].spawnframe == listnote_active[ i + 1 ].spawnframe )
                    {
                        listnote_active[ i + 1 ].Bounds.Y = listnote_active[ i ].Bounds.Y;
                        i++;
                    }
                }

                // Miss
                if (framecount - listnote_active[i].hitframe > accuOkay + calibNoteTiming && framecount - listnote_active[ i ].hitframe < 100)
                {
                    if ( Chain > maxChain )
                    {
                        maxChain = Chain;
                    }

                    Chain = 0;
                    chainText = "BREAK";
                    chainColor = Color.Red;
                    textTimer = 0;

                    countBreak++;

                    listnote_active.RemoveAt(i);
                    break;
                }
            }

            // Checks input to see if the player successfully hits the note
            CheckInput();

            // Update Buttons
            for ( i = 0; i < listbutton.Count; i++ )
            {
                listbutton[ i ].Update( gameTime );
            }

            // Update Note Hit Effects
            for ( i = 0; i < notehits.Count; i++ )
            {
                notehits[ i ].Update();
                if ( notehits[ i ].AnimationDone )
                {
                    notehits.RemoveAt( i );
                }
            }

            // Check if Song ended
            if ( listnote.Count <= 0 && listnote_active.Count <= 0 )
            {
                if (timerEndGame <= 0)
                {
                    if (Chain > maxChain)
                    {
                        maxChain = Chain;
                    }

                    MediaPlayer.Stop();
                    GameStateManager.Instance.ChangeScreen(new RhythmGameResults(graphicsDevice, game, this, albumart));
                }
                else
                {
                    timerEndGame--;
                }
            }
        }

        public override void Draw( SpriteBatch spriteBatch )
        {
            int i; //iterative variable

            _graphicsDevice.Clear(Color.Black); // Clear Draw Buffer
            spriteBatch.Begin();

            // Draw Notes
            for ( i = 0; i < listnote_active.Count; i++ )
            {
                listnote_active[ i ].Draw( spriteBatch );
            }

            // Draw Overlay
            overlay.Draw(spriteBatch);

            // Draw Album Art and information
            albumart.Draw(spriteBatch);
            spriteBatch.DrawString(fontJetset, songname, new Vector2(10, 270), Color.White);
            spriteBatch.DrawString(fontJetset, artistname, new Vector2(10, 290), Color.White);

            // Draw Overlay widgets
            for ( i = 0; i < listbutton.Count; i++ )
            {
                listbutton[ i ].Draw( spriteBatch );
            }

            // Draw Note Hit Effects
            for ( i = 0; i < notehits.Count; i++ )
            {
                notehits[ i ].Draw( spriteBatch );
            }

            // Draw Chain Text
            if ( Chain > 0 || chainText == "BREAK" )
            {
                if (textTimer < textDisappearTime)
                {
                    // Uncomment this stuff to make the text flash
                    if (textflash)
                    {
                        Vector2 vector = fontJetset.MeasureString(chainText);
                        float length = vector.X;
                        float posX = (game.graphics.PreferredBackBufferWidth / 2) - length;
                        spriteBatch.DrawString(fontJetset, chainText, new Vector2(posX, 700), chainColor, 0.0f, new Vector2(0, 0), 2.0f, new SpriteEffects(), 1);
                        if ( chainText == "BREAK" )
                            textflash = false;
                    }
                    else
                    {
                        textflash = true;
                    }
                    textTimer++;
                }
            }

            /**** Debug Text ****/
            //spriteBatch.DrawString(fontJetset, MediaPlayer.PlayPosition.ToString(), new Vector2(10, 10), Color.White);
            //spriteBatch.DrawString(fontJetset, framecount.ToString(), new Vector2(10, 30), Color.White);
            //spriteBatch.DrawString(fontJetset, "BPM = " + bpm.ToString(), new Vector2(10, 50), Color.White);
            //spriteBatch.DrawString(fontJetset, "Offset = " + offset.ToString(), new Vector2(10, 70), Color.White);
            //spriteBatch.DrawString( fontJetset, "Beats Per Sec = " + beatsPerSec.ToString(), new Vector2( 10, 90 ), Color.White );
            //spriteBatch.DrawString( fontJetset, "Frames Per Beat = " + framesPerBeat.ToString(), new Vector2( 10, 110 ), Color.White );
            //spriteBatch.DrawString( fontJetset, "Line 4 = " + linetest, new Vector2( 10, 150 ), Color.White );

            // Draw sprites here
            spriteBatch.End();
        }

        private void ReadFile()
        {
            try
            {
                using (file = new System.IO.StreamReader(@"Maps/" + mapname + ".txt"))
                {

                    string line;
                    string[] splitline;

                    // Skip first 5 lines (that's just the metadata we already read in)
                    file.ReadLine();
                    file.ReadLine();
                    file.ReadLine();
                    file.ReadLine();
                    file.ReadLine();

                    // Read in rest of the notes
                    while ((line = file.ReadLine()) != null)
                    {
                        if ( line == "" )
                        {
                            continue;
                        }
                        else if ( line[ 0 ] == '/' )
                        {
                            continue;
                        }

                        splitline = line.Split(';');

                        UInt64 hitframe = Convert.ToUInt64((Convert.ToDouble(splitline[1]) * framesPerBeat) + offset);
                        linetest = hitframe.ToString();

                        // Note type
                        switch (splitline[0])
                        {
                            case "N":
                            default:
                                Note note = new Note(game, this, txtNote, bpm, hitframe, Convert.ToInt32(splitline[2]));
                                totalNoteCount += 1.0f;
                                listnote.Add(note);
                                break;
                        }
                    }

                    //listnote = ListNoteSort( listnote );
                    linetest = listnote[0].spawnframe.ToString();
                    file.Close();
                }
            }
            catch( Exception ex )
            {
                game.Exit();
            }
        }

        /// <summary>
        /// Sorts the list of notes contained within this instance of RhythmGame.
        /// Sorts them by spawn time with the Insertion Sort Algorithm.
        /// </summary>
        private List<Note> ListNoteSort( List<Note> notes )
        {
            List<Note> returnlist = new List<Note>( notes.Count );

            for ( int i = 0; i < notes.Count; i++ )
            {
                Note note = notes[i];
                int currentindex = i;

                while ( currentindex > 0 && returnlist[currentindex - 1].spawnframe > note.spawnframe )
                {
                    currentindex--;
                }

                returnlist.Insert( currentindex, note );
            }

            return returnlist;
        }

        private void CheckInput()
        {
            KeyboardState keyState = Keyboard.GetState();

            CheckLane( keyState, Keys.S );
            CheckLane( keyState, Keys.D );
            CheckLane( keyState, Keys.F );
            CheckLane( keyState, Keys.J );
            CheckLane( keyState, Keys.K );
            CheckLane( keyState, Keys.L );

            previousKeyState = keyState;
        }

        private void CheckLane( KeyboardState keyState, Keys key )
        {
            if (keyState.IsKeyDown( key ) & !previousKeyState.IsKeyDown( key ))
            {
                switch ( key )
                {
                    case Keys.S:
                        for( int i = 0; i < listnote_active.Count; i++ )
                        {
                            if ( listnote_active[ i ].lane == 1 )
                            {
                                // Perfect Judgement
                                if ( framecount - listnote_active[ i ].hitframe - calibNoteTiming < accuPerfect  || listnote_active[ i ].hitframe - framecount + calibNoteTiming < accuPerfect )
                                {
                                    Chain++;
                                    chainText = "PERFECT " + Chain;
                                    chainColor = Color.AliceBlue;
                                    textTimer = 0;

                                    hitNoteCount += 1.0f;
                                    countPerfect++;

                                    hitsound.Play();
                                    notehits.Add( new NoteHitEffect( txtNoteHit, 534, 795 ) );
                                    listnote_active.RemoveAt( i );
                                    break;
                                }
                                // Okay Judgement
                                else if ( framecount - listnote_active[ i ].hitframe - calibNoteTiming < accuOkay  || listnote_active[ i ].hitframe - framecount + calibNoteTiming < accuOkay )
                                {
                                    Chain++;
                                    chainText = "GOOD " + Chain;
                                    chainColor = Color.Aquamarine;
                                    textTimer = 0;

                                    hitNoteCount += 0.5f;
                                    countGood++;

                                    hitsound.Play();
                                    listnote_active.RemoveAt( i );
                                    break;
                                }
                            }
                        }
                        break;
                    case Keys.D:
                        for (int i = 0; i < listnote_active.Count; i++)
                        {
                            if (listnote_active[i].lane == 2)
                            {
                                // Perfect Judgement
                                if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuPerfect || listnote_active[i].hitframe - framecount + calibNoteTiming < accuPerfect)
                                {
                                    Chain++;
                                    chainText = "PERFECT " + Chain;
                                    chainColor = Color.AliceBlue;
                                    textTimer = 0;

                                    hitNoteCount += 1.0f;
                                    countPerfect++;

                                    hitsound.Play();
                                    notehits.Add(new NoteHitEffect(txtNoteHit, 651, 795));
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                                // Okay Judgement
                                else if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuOkay || listnote_active[i].hitframe - framecount + calibNoteTiming < accuOkay)
                                {
                                    Chain++;
                                    chainText = "GOOD " + Chain;
                                    chainColor = Color.Aquamarine;
                                    textTimer = 0;

                                    hitNoteCount += 0.5f;
                                    countGood++;

                                    hitsound.Play();
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        break;
                    case Keys.F:
                        for (int i = 0; i < listnote_active.Count; i++)
                        {
                            if (listnote_active[i].lane == 3)
                            {
                                // Perfect Judgement
                                if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuPerfect || listnote_active[i].hitframe - framecount + calibNoteTiming < accuPerfect)
                                {
                                    Chain++;
                                    chainText = "PERFECT " + Chain;
                                    chainColor = Color.AliceBlue;
                                    textTimer = 0;

                                    hitNoteCount += 1.0f;
                                    countPerfect++;

                                    hitsound.Play();
                                    notehits.Add(new NoteHitEffect(txtNoteHit, 768, 795));
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                                // Okay Judgement
                                else if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuOkay || listnote_active[i].hitframe - framecount + calibNoteTiming < accuOkay)
                                {
                                    Chain++;
                                    chainText = "GOOD " + Chain;
                                    chainColor = Color.Aquamarine;
                                    textTimer = 0;

                                    hitNoteCount += 0.5f;
                                    countGood++;

                                    hitsound.Play();
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        break;
                    case Keys.J:
                        for (int i = 0; i < listnote_active.Count; i++)
                        {
                            if (listnote_active[i].lane == 4)
                            {
                                // Perfect Judgement
                                if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuPerfect || listnote_active[i].hitframe - framecount + calibNoteTiming < accuPerfect)
                                {
                                    Chain++;
                                    chainText = "PERFECT " + Chain;
                                    chainColor = Color.AliceBlue;
                                    textTimer = 0;

                                    hitNoteCount += 1.0f;
                                    countPerfect++;

                                    hitsound.Play();
                                    notehits.Add(new NoteHitEffect(txtNoteHit, 885, 795));
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                                // Okay Judgement
                                else if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuOkay || listnote_active[i].hitframe - framecount + calibNoteTiming < accuOkay)
                                {
                                    Chain++;
                                    chainText = "GOOD " + Chain;
                                    chainColor = Color.Aquamarine;
                                    textTimer = 0;

                                    hitNoteCount += 0.5f;
                                    countGood++;

                                    hitsound.Play();
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        break;
                    case Keys.K:
                        for (int i = 0; i < listnote_active.Count; i++)
                        {
                            if (listnote_active[i].lane == 5)
                            {
                                // Perfect Judgement
                                if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuPerfect || listnote_active[i].hitframe - framecount + calibNoteTiming < accuPerfect)
                                {
                                    Chain++;
                                    chainText = "PERFECT " + Chain;
                                    chainColor = Color.AliceBlue;
                                    textTimer = 0;

                                    hitNoteCount += 1.0f;
                                    countPerfect++;

                                    hitsound.Play();
                                    notehits.Add(new NoteHitEffect(txtNoteHit, 1002, 795));
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                                // Okay Judgement
                                else if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuOkay || listnote_active[i].hitframe - framecount + calibNoteTiming < accuOkay)
                                {
                                    Chain++;
                                    chainText = "GOOD " + Chain;
                                    chainColor = Color.Aquamarine;
                                    textTimer = 0;

                                    hitNoteCount += 0.5f;
                                    countGood++;

                                    hitsound.Play();
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        break;
                    case Keys.L:
                        for (int i = 0; i < listnote_active.Count; i++)
                        {
                            if (listnote_active[i].lane == 6)
                            {
                                // Perfect Judgement
                                if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuPerfect || listnote_active[i].hitframe - framecount + calibNoteTiming < accuPerfect)
                                {
                                    Chain++;
                                    chainText = "PERFECT " + Chain;
                                    chainColor = Color.AliceBlue;
                                    textTimer = 0;

                                    hitNoteCount += 1.0f;
                                    countPerfect++;

                                    hitsound.Play();
                                    notehits.Add(new NoteHitEffect(txtNoteHit, 1119, 795));
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                                // Okay Judgement
                                else if (framecount - listnote_active[i].hitframe - calibNoteTiming < accuOkay || listnote_active[i].hitframe - framecount + calibNoteTiming < accuOkay)
                                {
                                    Chain++;
                                    chainText = "GOOD " + Chain;
                                    chainColor = Color.Aquamarine;
                                    textTimer = 0;

                                    hitNoteCount += 0.5f;
                                    countGood++;

                                    hitsound.Play();
                                    listnote_active.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
