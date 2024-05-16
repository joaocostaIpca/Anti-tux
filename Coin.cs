using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace projeto_jogo
{
    public class Coin
    {
        public Vector2 Position { get; set; }
        private Texture2D[] Frames { get; set; }
        private int currentFrame;
        private float frameTime;
        private float timeSinceLastFrame;
        private const float frameDuration = 0.1f; 

        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Frames[0].Width, Frames[0].Height);

        public Coin(Texture2D[] frames, Vector2 position)
        {
            Frames = frames;
            Position = position;
            currentFrame = 0;
            frameTime = 0;
            timeSinceLastFrame = 0;
        }

        public void Update(float deltaTime)
        {
            timeSinceLastFrame += deltaTime;
            if (timeSinceLastFrame >= frameDuration)
            {
                currentFrame = (currentFrame + 1) % Frames.Length;
                timeSinceLastFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Frames[currentFrame], Position, Color.White);
        }
    }
}
