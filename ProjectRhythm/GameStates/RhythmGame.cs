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
        Dictionary<float, Note> dictnote;
        System.IO.StreamReader file;

        /**** Instance Variables ****/
        UInt64 framecount;
        public const int JudgeLineDistance = 993;
        public string mapname;
        public string songname;
        public float bpm;
        public float offset;
        float beatsPerSec;
        float framesPerBeat;

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
        }

        public override void Initialize()
        {
            listnote = new List<Note>();
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
            txtOverlay = content.Load<Texture2D>( "TestOverlay" );




            /**** Further Initialization of game objects ****/
            overlay = new Overlay( game, this, txtOverlay );

            //listnote.Add( new Note( game, this, txtNote, bpm ) );
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

            overlay.Update( gameTime );

            for ( i = 0; i < listnote.Count; i++ )
            {
                listnote[ i ].Update( gameTime );

                if ( framecount > listnote[ i ].spawnframe )
                {
                    listnote[ i ].active = true;
                }


                if (listnote[ i ].Bounds.Y >= game.graphics.PreferredBackBufferHeight)
                {
                    listnote[ i ].active = false;
                }
            }
        }

        public override void Draw( SpriteBatch spriteBatch )
        {
            int i; //iterative variable

            _graphicsDevice.Clear(Color.Black); // Clear Draw Buffer
            spriteBatch.Begin();

            overlay.Draw( spriteBatch );

            for ( i = 0; i < listnote.Count; i++ )
            {
                listnote[ i ].Draw( spriteBatch );
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
                file = new System.IO.StreamReader(@"Maps/" + mapname + ".txt");
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

                    UInt64 hitframe = Convert.ToUInt64(Math.Round(Convert.ToDouble(splitline[1]) + offset));
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
