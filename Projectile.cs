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
        public Texture2D Texture { get; set; }

        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        private float lifetime; 
        private float timeAlive; 



        public Projectile(Texture2D texture, Vector2 position, Vector2 velocity, float lifetime)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            this.lifetime = lifetime; 
            timeAlive = 0f;
        }

        public void Update(float deltaTime)
        {

            timeAlive += deltaTime;

            // Update projectile position based on velocity
            Position += Velocity * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }


        public bool IsExpired()
        {
            return timeAlive >= lifetime;
        }

    }
}
