using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectRhythm.GameStates
{
    public abstract class GameState : IGameState
    {
        protected GraphicsDevice _graphicsDevice;
        protected Game _game;

        public GameState(GraphicsDevice graphicsDevice, Game g)
        {
            _graphicsDevice = graphicsDevice;
            _game = g;
        }

        public abstract void Initialize();

        public abstract void LoadContent(ContentManager content);

        public abstract void UnloadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);

    }
}
