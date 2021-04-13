using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectRhythm.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    public class Note
    {
        private Game game;
        private RhythmGame rhythmgame;
        public BoundingRectangle Bounds;
        private Texture2D texture;

        private float bpm;
        public float notespeed;
        private int yCoordStart;
        private int yCoordHit;
        public float yCoordTravel;
        public bool active;

        public UInt64 spawnframe;
        public UInt64 hitframe;

        /// <summary>
        /// A single-tap, basic note. Starts just above the screen, and scrolls down toward the judgement line.
        /// </summary>
        /// <param name="g"> Game that is containing this note. </param>
        /// <param name="rg"> RhythmGame instance in which the note is placed. </param>
        /// <param name="t"> Texture to be used by the note. </param>
        /// <param name="b"> BPM at which this note will hit (used to calculate travel speed). </param>
        public Note( Game g, RhythmGame rg, Texture2D t, float b, UInt64 hf, int pos )
        {
            game = g;
            rhythmgame = rg;
            texture = t;
            hitframe = hf;
            Bounds.Width = 117;
            Bounds.Height = 30;
            Bounds.Y = 0 - Bounds.Height;

            // Place note at correct X-coordinate based on passed-in position flag
            switch( pos )
            {
                case 1:
                default:
                    Bounds.X = 607;
                    break;
                case 2:
                    Bounds.X = 724;
                    break;
                case 3:
                    Bounds.X = 841;
                    break;
                case 4:
                    Bounds.X = 960;
                    break;
                case 5:
                    Bounds.X = 1077;
                    break;
                case 6:
                    Bounds.X = 1194;
                    break;
            }

            bpm = b;
            notespeed = (bpm / 10.0f) * rhythmgame.multiplierNotespeed;
            yCoordStart = -30;
            yCoordHit = 880 - Convert.ToInt32(Bounds.Y) - rhythmgame.calibJudgementLine;
            yCoordTravel = yCoordHit - yCoordStart;

            // Find the frame at which to spawn the note, given travel speed, distance, and hit frame
            float frameSpawnOffset = yCoordTravel / notespeed;
            while ( frameSpawnOffset > hitframe )
            {
                frameSpawnOffset /= 2;
                notespeed *= 2;
            }
            spawnframe = Convert.ToUInt64(hitframe - frameSpawnOffset);

            // Ensure spawn frame is greater than 0
            if ( spawnframe < 0 )
            {
                spawnframe = 0;
            }

            active = false;
        }

        public void Update( GameTime gt )
        {
            if ( active )
            {
                Bounds.Y += notespeed;
            }
        }

        public void Draw( SpriteBatch sb )
        {
            sb.Draw( texture, Bounds, Color.White );
        }
    }
}
