using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace projeto_jogo
{
    public class Projectile
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        //Gere animação
        private Texture2D[] Frames { get; set; }
        private int currentFrame;
        private float frameTime;
        private float timeSinceLastFrame;
        private const float frameDuration = 0.1f; // Adjust this for desired frame rate

        private float lifetime;
        private float timeAlive;

        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Frames[0].Width, Frames[0].Height);

        public Projectile(Texture2D[] frames, Vector2 position, Vector2 velocity, float lifetime)
        {
            Frames = frames;
            Position = position;
            Velocity = velocity;
            this.lifetime = lifetime;
            timeAlive = 0f;
            currentFrame = 0;
            frameTime = 0;
            timeSinceLastFrame = 0;
        }

        public void Update(float deltaTime)
        {

            timeAlive += deltaTime;

            // Update projectile position based on velocity
            Position += Velocity * deltaTime;


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


        public bool IsExpired()
        {
            return timeAlive >= lifetime;
        }

    }
}
