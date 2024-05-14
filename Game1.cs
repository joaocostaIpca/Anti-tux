using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace projeto_jogo
{
    public class Game1 : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        private Texture2D plataformTexture;
        private Texture2D PlayerTexture;
        private Texture2D _backgroundLayer1;
        private Texture2D _backgroundLayer2;
        private Texture2D _backgroundLayer3;
        private Texture2D _backgroundLayer4;


        private Personagem _character;
        private Plataform _platform;


        private Vector2 _cameraPosition;



        private float _gravity =300f;

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
            PlayerTexture= Content.Load<Texture2D>("Character4");
            _character = new Personagem(PlayerTexture, new Vector2(2400, 100));
            plataformTexture = Content.Load<Texture2D>("platform_texture");
            _platform = new Plataform(plataformTexture, new Vector2(2400, 700));


                _backgroundLayer1 = Content.Load<Texture2D>("Background/background1");
            _backgroundLayer2 = Content.Load<Texture2D>("Background/background2");
            _backgroundLayer3 = Content.Load<Texture2D>("Background/background3");
            _backgroundLayer4 = Content.Load<Texture2D>("Background/background4a");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity
            if (!_character.IsOnGround)
            {
                _character.Velocity = new Vector2(_character.Velocity.X, _character.Velocity.Y + _gravity * deltaTime);
            }

            // Update character position
            _character.Position += _character.Velocity * deltaTime;

            if (_character.BoundingBox.Intersects(_platform.BoundingBox))
            {
                // Adjust player position to prevent clipping into the platform
                _character.Position = new Vector2(_character.Position.X, _platform.Position.Y - _character.Texture.Height);
                _character.Velocity = new Vector2(_character.Velocity.X, Math.Max(0, _character.Velocity.Y)); // Prevent downward velocity

                // Update player's on-ground status
                _character.IsOnGround = true;
            }
            else
            {
                _character.IsOnGround = false;
            }

            // Handle input for jumping only if the player is on the ground
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _character.IsOnGround)
            {
                _character.Velocity = new Vector2(_character.Velocity.X, -300); // Adjust jump strength as needed
            }




            // Handle input for jumping and moving
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _character.IsOnGround)
            {
                _character.Velocity = new Vector2(_character.Velocity.X, -300); // Adjust jump strength as needed
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _character.Position = new Vector2(_character.Position.X - 150 * deltaTime, _character.Position.Y);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _character.Position = new Vector2(_character.Position.X + 150 * deltaTime, _character.Position.Y);
            }

            if (_character.Position.Y > GraphicsDevice.Viewport.Height)
            {
                // Reset game state
                ResetGame();
            }

            _cameraPosition.X = Math.Max(_character.Position.X - (_graphics.PreferredBackBufferWidth / 2), 0);

            base.Update(gameTime);


        }

        private void ResetGame()
        {
            // Reset player position
            _character.Position = new Vector2(2400, 100);
            _character.Velocity = Vector2.Zero;
            _character.IsOnGround = false;

            // Reset camera position
            _cameraPosition = Vector2.Zero;

            // Additional reset logic if needed
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Matrix.CreateTranslation(-_cameraPosition.X, 0, 0));

            
            _spriteBatch.Draw(_character.Texture, _character.Position, Color.White);
            _spriteBatch.Draw(_platform.Texture, _platform.Position, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
