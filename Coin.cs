using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace projeto_jogo
{
    public class Coin
    {
        public Vector2 Position { get; set; }
        private Texture2D[] Frames { get; set; }
        
        private int currentFrame;
    
        private float timeSinceLastFrame;
        private const float frameDuration = 0.1f; 

        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Frames[0].Width, Frames[0].Height);

        public Coin(Texture2D[] frames, Vector2 position)
        {
            Frames = frames;
            Position = position;
            currentFrame = 0;
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

        public static List<Coin> CreateCoins(ContentManager content)
        {
            var frames = new Texture2D[16];
            for (int i = 0; i < 16; i++)
            {
                frames[i] = content.Load<Texture2D>($"Coin/coin-{i}");
            }

            var coins = new List<Coin>
            {
                new Coin(frames, new Vector2(2650, 550)),
                new Coin(frames, new Vector2(4200, 430))
                new Coin(frames, new Vector2(3250, 550))
                new Coin(frames, new Vector2(6000, 430))
                new Coin(frames, new Vector2(7000, 430))
            };

            return coins;
        }
    }
}
