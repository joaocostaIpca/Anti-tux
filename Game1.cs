using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace projeto_jogo
{
    public class Game1 : Game
    {
        Personagem block;
        Plataform Plataform;
        private Texture2D plataformTexture;
        private Texture2D blockTexture;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private const float Gravity = 0.2f;
        private const float JumpStrength = -6f;
        private const float MoveSpeed = 2f;
        private Vector2 velocity = Vector2.Zero;
        bool isOnGround = false;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            blockTexture = new Texture2D(GraphicsDevice, 100, 100);
            blockTexture.SetData<Color>(new Color[100 * 100]);
            plataformTexture = Content.Load<Texture2D>("platform_texture");
            block = new Personagem(blockTexture, new Vector2(100, 100));
            Plataform = new Plataform(plataformTexture, new Rectangle(0, 500, 200, 50));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Vector2 position = block.Position;
            KeyboardState keyboardState = Keyboard.GetState();

            if (!isOnGround)
            {
                velocity.Y += Gravity;
            }
            // Check for left/right movement
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                velocity.X = -MoveSpeed;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                velocity.X = MoveSpeed;
            }
            else
            {
                velocity.X = 0f;
            }

            // Jumping
            if (keyboardState.IsKeyDown(Keys.Space) && isOnGround)
            {
                velocity.Y = JumpStrength;
            }

            // Update block position
            block.Move(block.Position  + velocity);

            // Check collision with platform
            if (block.Bounds.Intersects(Plataform.Bounds) && block.Position.Y + block.Texture.Height <= Plataform.Bounds.Top)
            {
                // If the block is above the platform, stop falling
                position.X= Plataform.Bounds.Top - block.Texture.Height;
                block.Position = position;
                velocity.Y = 0;
                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }

            base.Update(gameTime);


        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // Draw block 
            block.Draw(_spriteBatch);

            // Draw plataform
            Plataform.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
