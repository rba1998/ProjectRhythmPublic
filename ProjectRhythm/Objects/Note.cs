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
        Game game;
        RhythmGame rhythmgame;
        public BoundingRectangle Bounds;
        Texture2D texture;

        float bpm;
        float notespeed;

        public UInt64 hitframe;

        /// <summary>
        /// A single-tap, basic note. Starts just above the screen, and scrolls down toward the judgement line.
        /// </summary>
        /// <param name="g"> Game that is containing this note. </param>
        /// <param name="rg"> RhythmGame instance in which the note is placed. </param>
        /// <param name="t"> Texture to be used by the note. </param>
        /// <param name="b"> BPM at which this note will hit (used to calculate travel speed). </param>
        public Note( Game g, RhythmGame rg, Texture2D t, float b, UInt64 hf )
        {
            game = g;
            rhythmgame = rg;
            texture = t;
            hitframe = hf;
            Bounds.Width = 117;
            Bounds.Height = 30;
            Bounds.X = 960;
            Bounds.Y = 0 - Bounds.Height;

            bpm = b;
            notespeed = bpm / 10;
        }

        public void Update( GameTime gt )
        {
            Bounds.Y += notespeed;
            if ( Bounds.Y >= game.graphics.PreferredBackBufferHeight )
            {
                Bounds.Y = 0 - Bounds.Height;
            }
        }

        public void Draw( SpriteBatch sb )
        {
            sb.Draw( texture, Bounds, Color.White );
        }
    }
}
