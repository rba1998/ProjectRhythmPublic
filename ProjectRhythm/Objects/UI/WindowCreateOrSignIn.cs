using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectRhythm.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRhythm.Objects.UI
{
    class WindowCreateOrSignIn
    {
        Game game;
        Texture2D texture;
        public BoundingRectangle Bounds;
        private Rectangle sheetlocation;

        private bool AnimationDone;
        private ButtonCreateOrSignInWindow buttonCreate;
        private ButtonCreateOrSignInWindow buttonSignin;

        KeyboardState previousKeyboardState;

        public WindowCreateOrSignIn(Game g, Texture2D t)
        {
            game = g;
            texture = t;

            Bounds.Height = 208;
            Bounds.Width = 431;
            Bounds.X = (g.graphics.PreferredBackBufferWidth / 2) - (Bounds.Width / 2);
            Bounds.Y = (g.graphics.PreferredBackBufferHeight / 2) - (Bounds.Height / 2);

            sheetlocation = new Rectangle(0, 0, 431, 208);
            previousKeyboardState = Keyboard.GetState();

            AnimationDone = false;
        }

        public void LoadContent( ContentManager content )
        {
            Texture2D textureButtonCreateSel = content.Load<Texture2D>("Textures/UI/ButtonCreateSelectedAnim");
            Texture2D textureButtonCreateDesel = content.Load<Texture2D>("Textures/UI/ButtonCreateDeselected");
            Texture2D textureButtonSigninSel = content.Load<Texture2D>("Textures/UI/ButtonSigninSelectedAnim");
            Texture2D textureButtonSigninDesel = content.Load<Texture2D>("Textures/UI/ButtonSignInDeselected");

            buttonCreate = new ButtonCreateOrSignInWindow(textureButtonCreateDesel, textureButtonCreateSel, 
                                                          (game.graphics.PreferredBackBufferWidth / 2) - 175, 
                                                          (game.graphics.PreferredBackBufferHeight / 2) - 47);
            buttonSignin = new ButtonCreateOrSignInWindow(textureButtonSigninDesel, textureButtonSigninSel,
                                                          (game.graphics.PreferredBackBufferWidth / 2) + 4,
                                                          (game.graphics.PreferredBackBufferHeight / 2) - 47);

            buttonSignin.Selected = true;
        }

        public void Update(GameTime gt)
        {
            // Animation of the window opening
            if (!AnimationDone)
            {
                int x = sheetlocation.X;
                x += 431;
                if (x >= 3448)
                {
                    if (sheetlocation.Y != 208)
                    {
                        sheetlocation.Y = 208;
                        sheetlocation.X = 0;
                    }
                    else
                    {
                        AnimationDone = true;
                    }
                }
                else
                {
                    sheetlocation.X = x;
                }
            }
            else // Execute this whenever the window is done opening
            {
                buttonCreate.Update();
                buttonSignin.Update();

                KeyboardState KeyboardState = Keyboard.GetState();

                if (KeyboardState.IsKeyDown(Keys.Left) && !buttonCreate.Selected)
                {
                    buttonSignin.Selected = false;
                    buttonCreate.Selected = true;
                }
                else if (KeyboardState.IsKeyDown(Keys.Right) && !buttonSignin.Selected)
                {
                    buttonCreate.Selected = false;
                    buttonSignin.Selected = true;
                }
                if (KeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    
                }

                previousKeyboardState = KeyboardState;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Bounds, sheetlocation, Color.White);
            
            if ( AnimationDone )
            {
                buttonCreate.Draw(sb);
                buttonSignin.Draw(sb);
            }
        }
    }
}
