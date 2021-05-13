using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectRhythm.Objects.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects
{
    enum State { idle, talk }
    enum Talking { open, closed }
    

    class Character
    {
        Texture2D textureIdle, textureTalk1, textureTalk2;
        Texture2D textureCurrent;
        Texture2D textureWindow;
        SpriteFont fontJetset;
        State state;
        Talking talkingState;
        public BoundingRectangle Bounds;
        private String message;
        StringBuilder currentmessage;
        WindowText textwindow;

        // Timers
        int idleBlinkTimer;
        int idleBlinkDoneTimer;
        int talkTimer;
        int talkLipFlapTimer;

        // Timer starting numbers
        int idleBlinkTimerStart;
        int idleBlinkDoneTimerStart;
        int talkTimerStart;
        int talkLipFlapTimerStart;


        bool blinking;
        int currentmessagePosition;
        int currentmessageLength;

        public Character( Texture2D tIdle, Texture2D tTalk1, Texture2D tTalk2, Texture2D tWindow, SpriteFont font, float x, float y )
        {
            textureIdle = tIdle;
            textureTalk1 = tTalk1;
            textureTalk2 = tTalk2;
            textureWindow = tWindow;
            textureCurrent = textureIdle;
            state = State.idle;
            talkingState = Talking.closed;
            fontJetset = font;

            idleBlinkTimerStart = 120;
            idleBlinkTimer = idleBlinkTimerStart;
            idleBlinkDoneTimerStart = 4;
            idleBlinkDoneTimer = idleBlinkDoneTimerStart;
            talkTimerStart = 360;
            talkLipFlapTimerStart = 5;
            talkLipFlapTimer = talkLipFlapTimerStart;
            blinking = false;

            Bounds.Width = 1080;
            Bounds.Height = 1920;
            Bounds.X = x;
            Bounds.Y = y;

            message = "";
        }

        public void Update()
        {
            switch (state)
            {
                default:
                case State.idle:
                    if ( idleBlinkTimer <= 0 )
                    {
                        Blink();
                    }
                    else
                    {
                        idleBlinkTimer--;
                    }
                    break;
                case State.talk:
                    if ( talkTimer <= 0 )
                    {
                        state = State.idle;
                        textureCurrent = textureIdle;
                    }
                    else
                    {
                        talkTimer--;
                        talkLipFlapTimer--;
                        if ( talkLipFlapTimer <= 0 )
                        {
                            if ( talkingState == Talking.closed )
                            {
                                textureCurrent = textureTalk1;
                                talkingState = Talking.open;
                            }
                            else
                            {
                                textureCurrent = textureTalk2;
                                talkingState = Talking.closed;
                            }
                            talkLipFlapTimer = talkLipFlapTimerStart;
                        }
                    }
                    if ( currentmessagePosition < currentmessageLength )
                    {
                        currentmessage.Append(message[currentmessagePosition]);
                        currentmessagePosition++;
                        textwindow.message = currentmessage.ToString();
                    }
                    break;
            }

            if ( textwindow != null )
            {
                textwindow.Update();
            }
        }

        public void Draw( SpriteBatch sb )
        {
            sb.Draw( textureCurrent, Bounds, Color.White );

            if (textwindow != null)
            {
                textwindow.Draw(sb);
            }
        }

        /// <summary>
        /// Makes the character talk.
        /// </summary>
        /// <param name="text"> Message you would like them to say. </param>
        /// <param name="TimeToTalk"> Time you would like the character to stay in the talking animation. </param>
        public void Talk(String text, int TimeToTalk)
        {
            message = text;
            state = State.talk;
            currentmessage = new StringBuilder();
            currentmessagePosition = 0;
            currentmessageLength = text.Length;
            textwindow = new WindowText( textureWindow, fontJetset, currentmessage.ToString(), Bounds.X + (Bounds.Width / 4), Bounds.Y + (Bounds.Height / 2) );
            talkTimerStart = TimeToTalk;
            talkTimer = talkTimerStart;
        }

        private void Blink()
        {
            if ( !blinking )
            {
                blinking = true;
                textureCurrent = textureTalk1;
            }
            else if ( idleBlinkDoneTimer <= 0 )
            {
                idleBlinkDoneTimer = idleBlinkDoneTimerStart;
                idleBlinkTimer = idleBlinkTimerStart;
                blinking = false;
                textureCurrent = textureIdle;
            }
            else
            {
                idleBlinkDoneTimer--;
            }
        }
    }
}
