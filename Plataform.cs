using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace projeto_jogo
{
    internal class Plataform
    {
        public Rectangle Bounds { get; }
        public Texture2D Texture { get; }

        public Plataform(Texture2D texture, Rectangle bounds)
        {
            Texture = texture;
            Bounds = bounds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
