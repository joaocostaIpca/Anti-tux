using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace projeto_jogo
{
    public class Personagem
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Texture2D Texture { get; set; }

        private Vector2 _position = new(100, 100);
        private readonly float _speed = 200f;
        private readonly AnimationManager _anims = new();

        //movimento
        private static Vector2 _direction;
        public static Vector2 Direction => _direction;
        public static bool Moving => _direction != Vector2.Zero;




        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); //le o personagem e devolve os seus limites
        public bool IsOnGround { get; set; }

        

        

        public Personagem()
        {
            var playeridleTexture = Globals.Content.Load<Texture2D>("player-idle");
            var playerrunleftTexture = Globals.Content.Load<Texture2D>("player-run-left");
            var playerrunrightTexture = Globals.Content.Load<Texture2D>("player-run-right");
            var playerjump = Globals.Content.Load<Texture2D>("player-jump");
           
            _anims.AddAnimation(new Vector2(0, 0), new(playeridleTexture, 9, 0.1f)); //idle
            _anims.AddAnimation(new Vector2(1, 0), new(playerrunleftTexture, 9, 0.1f)); //direita
            _anims.AddAnimation(new Vector2(-1, 0), new(playerrunrightTexture, 9, 0.1f)); //esquerda
            _anims.AddAnimation(new Vector2(0, -1), new(playerjump, 8, 0.1f)); //salto
        }

        public void Update()
        {
            _direction = Vector2.Zero;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.GetPressedKeyCount() > 0)
            {
                if (keyboardState.IsKeyDown(Keys.Left)) _direction.X--; //mlhr verificar assim só para as animações do player assim não confunde muito
                if (keyboardState.IsKeyDown(Keys.Right)) _direction.X++;
                if (keyboardState.IsKeyDown(Keys.Space)) _direction.Y++;
            }


            if (Moving)
            {
                _position += Vector2.Normalize(_direction) * _speed * Globals.TotalSeconds;
            }

            _anims.Update(Direction);
        }

        public void Draw()
        {
            _anims.Draw(_position); 
        }

    }
}
