using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace projeto_jogo
{
    public class Game1 : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Texturas
        private Texture2D plataformTexture;
        private Texture2D enemyTexture;
        private Texture2D _backgroundTexture;
        private Texture2D pixelTexture;
        private Texture2D[] projectileTexture;
        private Texture2D[] lavaFrames;
        



        //Classes do jogo
        private Personagem _character;
        private List<Plataform> _platforms;
        private Menu _menu;
        private List<Enemy> _enemies;
        private List<Projectile> _projectiles = new List<Projectile>();
        private int currentLavaFrame;
        private float lavaFrameTimer;
        private List<Coin> _coins;

        //Variaveis do jogo
        private List<Vector2> _initialEnemyPositions = new List<Vector2>();
        private Vector2 _cameraPosition;
        private float _gravity = 500f;
        private float timeSinceLastFrame = 0f;
        private float enemyFollowRange = 200f; // Adjust the range as needed
        private float enemySpeed = 100f;
        private float projectileCooldown = 2f; // Cooldown duration in seconds
        private float timeSinceLastProjectile = 0f;
        private Vector2 playerDirection = Vector2.UnitX;
        private SpriteFont _font;
        private int _collectedCoins;



      


        //Estado do jogo
        private enum GameState
        {
            Menu,
            Playing,
            GameOver
        }

        private GameState _gameState = GameState.Menu;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = false;
        }

        protected override void Initialize()

        {
            base.Initialize();
        }

        //===============================CONTENT===========================
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //===============================BACKGROUND=====================
            _backgroundTexture = Content.Load<Texture2D>("Background/background5");


            // Load lava animation frames
            lavaFrames = new Texture2D[14]; // numberOfFrames is the total number of frames in the lava animation
            for (int i = 1; i < 14; i++)
            {
                lavaFrames[i] = Content.Load<Texture2D>("Background/Lava/lava-" + i); // Adjust the naming convention as per your files
            }

            // Initialize variables for lava animation
            currentLavaFrame = 0;
            lavaFrameTimer = 0f;



            //===============================MENU===========================
            _font = Content.Load<SpriteFont>("Menu/Font_menu"); //fonte do debug !

            _menu = new Menu(Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);


            // ===============================COINS========================

            _coins = Coin.CreateCoins(Content);

            // ==============================PROJETEIS=====================

            projectileTexture = Projectile.LoadFrames(Content);


            //===============================ENIMIGOS=====================


            pixelTexture = Content.Load<Texture2D>("pixel");

            (enemyTexture, pixelTexture) = (Content.Load<Texture2D>("Enemy/stand-0"), Content.Load<Texture2D>("pixel"));

            (_enemies, _initialEnemyPositions) = Enemy.CreateEnemies(Content, enemySpeed);





            //===============================PLATAFORMA=====================

            plataformTexture = Content.Load<Texture2D>("platform_texture");

            //Criar plataformas e adicionar a lista
            _platforms = new List<Plataform>();
            _platforms.Add(new Plataform(plataformTexture, new Vector2(2400, 600)));
            _platforms.Add(new Plataform(plataformTexture, new Vector2(3200, 600)));
            _platforms.Add(new Plataform(plataformTexture, new Vector2(4000, 470)));
            _platforms.Add(new Plataform(plataformTexture, new Vector2(4950, 550)));



            //===============================PERSONAGEM=====================
            var idleAnimation = new Animation(LoadAnimationFrames("Player/Idle/stand-", 9), 0.2f);
            var runAnimation = new Animation(LoadAnimationFrames("Player/Move/run-", 9), 0.1f);
            var jumpAnimation = new Animation(LoadAnimationFrames("Player/Jump/jump-", 8), 0.4f);

            var animations = new Dictionary<string, Animation>
            {
                { "idle", idleAnimation },
                { "run", runAnimation },
                { "jump", jumpAnimation }
            };
            _character = new Personagem(animations, new Vector2(2400, 500));


            

        }
        //===============================UPDATE=====================

        private void UpdateLavaAnimation(float deltaTime)
        {
            float frameDuration = 6f; // Adjust the duration as needed for the animation speed

            // Update time since last frame
            timeSinceLastFrame += deltaTime;

                timeSinceLastFrame += deltaTime;
                if (timeSinceLastFrame >= frameDuration)
                {
                    currentLavaFrame = (currentLavaFrame + 1) % lavaFrames.Length;
                    timeSinceLastFrame = 0;
                }
            
        }




        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (_gameState)
            {
                case GameState.Menu:
                    UpdateMenu();
                    break;
                case GameState.Playing:
                    UpdateGame(deltaTime, gameTime);
                    break;
            }

            base.Update(gameTime);
        }
        private void UpdateMenu()
        {
            // Atualiza o menu
            _menu.Update();

            // Verifica se o botão de começar jogo foi pressionado
            if (_menu.StartButtonClicked())
            {
                // Começa o jogo
                _gameState = GameState.Playing;
            }
            else if (_menu.ExitButtonClicked())
            {
                // Sai do jogo
                Exit();
            }
        }
        private void UpdateGame(float deltaTime, GameTime gameTime)

        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Se a tecla Esc for pressionada, volta para o menu ( este reset tem de ficar porque se nao o menu fica na posição onde o personagem estava)
                ResetGame();
                _gameState = GameState.Menu;
                return; 
            }
            bool isOnPlatform = false;
            // Apply gravity
            if (!_character.IsOnGround)
            {
                _character.Velocity = new Vector2(_character.Velocity.X, _character.Velocity.Y + _gravity * deltaTime);
            }
            // Update character position
            _character.Position += _character.Velocity * deltaTime;

            

            foreach (var projectile in _projectiles.ToList())
            {

                //Da update a posição do projetil
                projectile.Update(deltaTime);

                // Remove o projetil se ele estiver passar um determinado tempo
                if (projectile.IsExpired())
                {
                    _projectiles.Remove(projectile);
                }

                foreach (var enemy in _enemies.ToList())
                {
                    if (projectile.BoundingBox.Intersects(enemy.BoundingBox))
                    {
                        // Remove the projectile and the enemy
                        _projectiles.Remove(projectile);
                        _enemies.Remove(enemy);
                    }
                }
            }


            // Calculate the total elapsed time since the game started
            float totalElapsedSeconds = (float)gameTime.TotalGameTime.TotalSeconds;

            // Calculate the current frame index based on the elapsed time and frame duration
            float frameDuration = 0.1f; // Adjust the duration as needed for the animation speed
            float totalAnimationDuration = frameDuration * lavaFrames.Length;
            float normalizedTime = totalElapsedSeconds % totalAnimationDuration;
            currentLavaFrame = (int)(normalizedTime / frameDuration);

            // Ensure the current frame index stays within the range of available frames
            currentLavaFrame %= lavaFrames.Length;



            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];

                // Update the enemy position
                enemy.Update(_character.Position, enemyFollowRange, _platforms, _gravity, deltaTime);

                // Check for collision with the player
                if (_character.BoundingBox.Intersects(enemy.BoundingBox))
                {
                    
                    
                        // Reset the game and switch to menu state
                        ResetGame();
                        _gameState = GameState.Menu;
                        return;
                    
                }
            }


            UpdateLavaAnimation(deltaTime);


            //Gere as moedas

            foreach (var coin in _coins)
            {
                coin.Update(deltaTime);
            }

            // Check for coin collection
            for (int i = _coins.Count - 1; i >= 0; i--)
            {
                if (_character.BoundingBox.Intersects(_coins[i].BoundingBox))
                {
                    _coins.RemoveAt(i);
                    _collectedCoins++;
                }
            }







            //Gere a interação com as plataformas
            foreach (var platform in _platforms)
            {
                    if (_character.BoundingBox.Intersects(platform.BoundingBox)&& _character.BoundingBox.Bottom >= platform.BoundingBox.Top)
                   
                    {

                    _character.Position = new Vector2(_character.Position.X, platform.Position.Y - _character.GetCurrentFrameHeight());
                    _character.Velocity = new Vector2(_character.Velocity.X, Math.Max(0, _character.Velocity.Y));

                        
                        isOnPlatform = true;
                        break; 
                    }
            }
             _character.IsOnGround = isOnPlatform;




            //tempo do ultimo projetil lancado
            timeSinceLastProjectile += deltaTime;
            //Habilidade de projetil
            if (Keyboard.GetState().IsKeyDown(Keys.E) && timeSinceLastProjectile >= projectileCooldown)
            {
                LaunchProjectile();
                timeSinceLastProjectile = 0f; // reniciar o tempo
            }


            // Gere o input do jogador 


            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _character.IsOnGround )
            {
                _character.Velocity = new Vector2(_character.Velocity.X, -300);
                _character.SetAnimation("jump");
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _character.IsOnGround )
            {
                _character.Velocity = new Vector2(_character.Velocity.X, -300);
                _character.SetAnimation("jump");
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _character.Position = new Vector2(_character.Position.X - 250 * deltaTime, _character.Position.Y);
                _character.SetAnimation("run");
                _character.isFacingRight = false;
                playerDirection = -Vector2.UnitX;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _character.Position = new Vector2(_character.Position.X + 250 * deltaTime, _character.Position.Y);
                _character.SetAnimation("run");
                _character.isFacingRight = true;
                playerDirection = Vector2.UnitX;
            }
            else if (_character.IsOnGround && _character.Velocity.X==0 )
            {
                _character.SetAnimation("idle");
            }
            _character.Update(deltaTime);

            if (_character.Position.Y > GraphicsDevice.Viewport.Height)
            {
                // Reset game state
                ResetGame();
            }

            _cameraPosition.X = _character.Position.X - (_graphics.PreferredBackBufferWidth / 2);

            // Calculate the camera position so that it follows the player upwards but not downwards
            // Clamp the camera position to prevent it from moving below y = 0
            _cameraPosition.Y = Math.Min(_character.Position.Y - (_graphics.PreferredBackBufferHeight / 2), 0);

            


        }





        // ======================================DRAW=============================
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Matrix.CreateTranslation(-_cameraPosition.X, -_cameraPosition.Y, 0));

            if (_gameState == GameState.Menu)
            {
                _menu.Draw(_spriteBatch);
            }
            else if (_gameState == GameState.Playing)
            {

                int bgWidth = _backgroundTexture.Width;
                int bgHeight = _backgroundTexture.Height;

                int screenWidth = GraphicsDevice.Viewport.Width;
                int screenHeight = GraphicsDevice.Viewport.Height;

                // Calculate the number of repetitions needed to cover the entire screen horizontally
                int numRepetitionsX = (int)Math.Ceiling((float)screenWidth / bgWidth);

                // Calculate the scale factor for stretching the background texture vertically
                float scaleY = (float)screenHeight / bgHeight;

                // Draw the background texture repeatedly to cover the entire screen horizontally
                for (int i = 0; i < numRepetitionsX; i++)
                {
                    // Calculate the position to draw each repetition of the background texture
                    Vector2 position = new Vector2(i * bgWidth, 0) + _cameraPosition;

                    // Draw the background texture with the appropriate scale and position
                    _spriteBatch.Draw(_backgroundTexture, position, null, Color.White, 0f, Vector2.Zero, new Vector2(1f, scaleY), SpriteEffects.None, 0f);
                }


                //DRAW LAVA

                

                if (lavaFrames[currentLavaFrame] != null)
                {
                    int lavaWidth = lavaFrames[currentLavaFrame].Width;
                    int lavaHeight = lavaFrames[currentLavaFrame].Height;

                    int _numRepetitionsX = (int)Math.Ceiling((float)screenWidth / lavaWidth);

                    for (int i = 0; i < _numRepetitionsX; i++)
                    {
                        Vector2 position = new Vector2(i * lavaWidth, screenHeight - lavaHeight - 25) + _cameraPosition;
                        _spriteBatch.Draw(lavaFrames[currentLavaFrame], position, Color.White);
                    }
                }






                //Desenha as plataformas
                foreach (var platform in _platforms)
                {
                    platform.Draw(_spriteBatch);
                }




                //desenha os inimigos
                foreach (var enemy in _enemies)
                {
                    if (_enemies != null)
                    {
                        //Draw hitbox do inimigo   _spriteBatch.Draw(pixelTexture, enemy.BoundingBox, Color.Red * 0.5f);
                        enemy.Draw(_spriteBatch);
                    }
                }

                
                

                //Desenha os projetis
                foreach (var projectile in _projectiles)
                {
                    projectile.Draw(_spriteBatch);
                }

                //Desenha as moedas
                foreach (var coin in _coins)
                {
                    coin.Draw(_spriteBatch);
                }


                //Contador coins
                _spriteBatch.DrawString(_font, "Coins: " + _collectedCoins, _cameraPosition+ new Vector2(0, 20), Color.White);

                _character.Draw(_spriteBatch);


                // ======================================DEBUGS================================================

                //debug player position
                string positionText = $" Player Position: {_character.Position.X}, {_character.Position.Y}";
                _spriteBatch.DrawString(_font, positionText, _cameraPosition+new Vector2(0,70), Color.White);
                //debug camera position
                string positionCameraText = $" Camera Position: {_cameraPosition.X}, {_cameraPosition.Y}";
                _spriteBatch.DrawString(_font, positionCameraText, _cameraPosition+new Vector2(0,45), Color.White);
                //debug checka colision com o ground
                string isonground = $" Estado da colision: {_character.IsOnGround}";
                _spriteBatch.DrawString(_font, isonground, _cameraPosition + new Vector2(0, 95), Color.White);
                //Hitbox do player
                //_spriteBatch.Draw(pixelTexture, _character.BoundingBox, Color.Red * 0.5f);

                // =============================================================================================

            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }



        private void LaunchProjectile()
        {
            // Create and add a new projectile to the list
            _projectiles.Add(new Projectile(projectileTexture, _character.Position+new Vector2(20,30), new Vector2(500, 0) * playerDirection, 5f));
        }



        // ======================================RESET DO JOGO=============================
        private void RespawnEnemies()
        {
            _enemies.Clear(); // Clear existing enemies

            // Create new enemies at initial positions
            foreach (var initialEnemyPosition in _initialEnemyPositions)
            {
                _enemies.Add(new Enemy(enemyTexture, initialEnemyPosition, enemySpeed));
            }
        }

        private void ResetGame()
        {

            // Reniciar o personagem
            _character.Position = new Vector2(2400, 550);
            _character.Velocity = Vector2.Zero;
            _character.IsOnGround = false;

            // Dá respawn aos inimigos
            RespawnEnemies();
           
            // Reset camera position
            _cameraPosition = Vector2.Zero;

        }

        private Texture2D[] LoadAnimationFrames(string basePath, int frameCount)
        {
            Texture2D[] frames = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = Content.Load<Texture2D>($"{basePath}{i }");
            }
            return frames;
        }

    }
}