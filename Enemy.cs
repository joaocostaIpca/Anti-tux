using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace projeto_jogo
{
    public class Enemy
    {

        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public float Speed { get; set; }
        public Vector2 Velocity { get; set; }
        private int hitboxWidth = 20;
        private int hitboxHeight = 65;
        public Rectangle BoundingBox => new Rectangle((int)(Position.X + (Texture.Width - hitboxWidth) / 2), (int)(Position.Y + (Texture.Height - hitboxHeight)), hitboxWidth, hitboxHeight);


        public Vector2 InitialPosition { get; private set; }

        public Enemy(Texture2D texture, Vector2 position, float speed)
        {
            Texture = texture;
            Position = position;
            Speed = speed;
            Velocity = Vector2.Zero;
            InitialPosition = position;
        }

        public void Update(Vector2 playerPosition, float followRange, List<Plataform> platforms, float gravity, float deltatime)
        {
            float distanceToPlayer = Math.Abs(playerPosition.X - Position.X);

            // Apply gravity
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity* deltatime);

            // Move horizontally towards the player if within follow range
            if (distanceToPlayer <= followRange)
            {
                if (playerPosition.X > Position.X)
                {
                    Velocity = new Vector2(Speed, Velocity.Y);
                }
                else if (playerPosition.X < Position.X)
                {
                    Velocity = new Vector2(-Speed, Velocity.Y );
                }
            }
            else
            {
                Velocity = new Vector2(0, Velocity.Y ); // Stop horizontal movement if out of range
            }

            // Update position based on velocity
            Position += Velocity*deltatime;

            // Check for collisions with platforms
            bool isOnPlatform = false;
            foreach (var platform in platforms)
            {
                if (BoundingBox.Intersects(platform.BoundingBox) && BoundingBox.Bottom >= platform.BoundingBox.Top)

                {
                    // Adjust player position to prevent clipping into the platform
                    Position = new Vector2(Position.X, platform.Position.Y - Texture.Height);
                   Velocity = new Vector2(Velocity.X, Math.Max(0, Velocity.Y)); // Prevent downward velocity

                    // Update player's on-ground status
                    isOnPlatform = true;
                    break; // Stop checking further platforms as the character is already on one
                }
            }

            // If not on any platform, continue falling
            if (!isOnPlatform)
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + gravity*deltatime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        public static (List<Enemy>, List<Vector2>) CreateEnemies(ContentManager content, float enemySpeed)
        {
            Texture2D enemyTexture = content.Load<Texture2D>("Enemy/stand-0");

            var enemies = new List<Enemy>
            {
                new Enemy(enemyTexture, new Vector2(2600, 500), enemySpeed),
                new Enemy(enemyTexture, new Vector2(3300, 500), enemySpeed)
            };

            var initialPositions = new List<Vector2>();
            foreach (var enemy in enemies)
            {
                initialPositions.Add(enemy.Position);
            }

            return (enemies, initialPositions);
        }
    }

}
