using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace projeto_jogo
{
    public class Plataform
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); //le a plataforma e devolve os seus limites

        public Plataform(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}