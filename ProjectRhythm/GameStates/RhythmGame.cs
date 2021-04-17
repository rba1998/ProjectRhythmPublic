using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        SoundEffect metronome;
        List<Note> listnote;
        List<Note> listnote_active;
        System.IO.StreamReader file;
        List<WidgetButton> listbutton;

        /**** Instance Variables ****/
        UInt64 framecount;
        public const int JudgeLineDistance = 993;
        public string mapname;
        public string songname;
        public float bpm;
        public float offset;
        float beatsPerSec;
        float framesPerBeat;

        /**** Modifiers ****/
        public float multiplierNotespeed;

        /**** Timing Calibration Variables ****/
        public int calibJudgementLine;
        public int calibBeat;

        /**** Timers ****/
        float timerOffset;
        float timerBeat;

        /**** Textures ****/
        Texture2D txtNote;
        Texture2D txtOverlay;

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

                // Get song name, bpm, and offset (first 3 lines of map)
                line = file.ReadLine(); // Song Name
                splitline = line.Split('\n');
                songname = splitline[0];

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

            enableMetronome = true;
            timerBeat = framesPerBeat * 2;
            timerOffset = 0;

            // Initialize Player-set modifiers and Calibration Variables
            calibJudgementLine = 100;
            multiplierNotespeed = 1.0f;
        }

        public override void Initialize()
        {
            listnote = new List<Note>();
            listnote_active = new List<Note>();
            listbutton = new List<WidgetButton>();
        }

        public override void LoadContent( ContentManager content )
        {
            /**** Load Assets ****/
            // Load Song
            song = content.Load<Song>( "Music/" + songname );

            // Load SFX
            metronome = content.Load<SoundEffect>( "Metronome" );

            // Load Fonts
            fontJetset = content.Load<SpriteFont>( "Fonts/JetSet" );

            // Load Textures
            txtNote = content.Load<Texture2D>( "TestNote" );
            txtOverlay = content.Load<Texture2D>( "TestOverlay2" );

            
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
            framecount++;

            // Beat
            if (timerOffset >= offset)
            {
                if (timerBeat >= framesPerBeat)
                {
                    if (enableMetronome)
                        metronome.Play();
                    timerBeat -= framesPerBeat;
                }
                timerBeat++;
            }

            // Update Overlay
            overlay.Update( gameTime );

            // Update Idle Notes
            for ( i = 0; i < listnote.Count; i++ )
            {

                if ( framecount > listnote[ i ].spawnframe )
                {
                    listnote_active.Add( listnote[ i ] );
                    listnote.RemoveAt( i );
                }
            }
            // Update Active Notes
            for ( i = 0; i < listnote_active.Count; i++ )
            {
                listnote_active[ i ].Update( gameTime );
            }

            // Update Buttons
            for ( i = 0; i < listbutton.Count; i++ )
            {
                listbutton[ i ].Update( gameTime );
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

            // Draw Overlay widgets
            for ( i = 0; i < listbutton.Count; i++ )
            {
                listbutton[ i ].Draw( spriteBatch );
            }

            /**** Debug Text ****/
            spriteBatch.DrawString( fontJetset, MediaPlayer.PlayPosition.ToString(), new Vector2( 10, 10 ), Color.White );
            spriteBatch.DrawString( fontJetset, framecount.ToString(), new Vector2( 10, 30 ), Color.White );
            spriteBatch.DrawString( fontJetset, "BPM = " + bpm.ToString(), new Vector2( 10, 50 ), Color.White );
            spriteBatch.DrawString( fontJetset, "Offset = " + offset.ToString(), new Vector2( 10, 70 ), Color.White );
            spriteBatch.DrawString( fontJetset, "Beats Per Sec = " + beatsPerSec.ToString(), new Vector2( 10, 90 ), Color.White );
            spriteBatch.DrawString( fontJetset, "Frames Per Beat = " + framesPerBeat.ToString(), new Vector2( 10, 110 ), Color.White );
            spriteBatch.DrawString( fontJetset, "Song name = " + songname, new Vector2( 10, 130 ), Color.White );
            spriteBatch.DrawString( fontJetset, "Line 4 = " + linetest, new Vector2( 10, 150 ), Color.White );

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

                    // Skip first 3 lines
                    file.ReadLine();
                    file.ReadLine();
                    file.ReadLine();

                    // Read in rest of the notes
                    while ((line = file.ReadLine()) != null)
                    {
                        linetest = line;
                        splitline = line.Split(';');

                        UInt64 hitframe = Convert.ToUInt64((Convert.ToDouble(splitline[1]) * framesPerBeat) + offset);
                        linetest = hitframe.ToString();

                        // Note type
                        switch (splitline[0])
                        {
                            case "N":
                            default:
                                Note note = new Note(game, this, txtNote, bpm, hitframe, Convert.ToInt32(splitline[2]));
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
    }
}
