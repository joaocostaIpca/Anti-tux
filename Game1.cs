﻿using Microsoft.Xna.Framework;
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
        private Texture2D PlayerTexture;
        private Texture2D enemyTexture;
        private Texture2D _backgroundLayer1;
        private Texture2D _backgroundLayer2;
        private Texture2D _backgroundLayer3;
        private Texture2D _backgroundLayer4;
        private Texture2D pixelTexture;
        private Texture2D projectileTexture;
        
        
       

        //Classes do jogo
        private Personagem _character;
        private List<Plataform> _platforms;
        private Menu _menu;
        private List<Enemy> _enemies;
        private List<Projectile> _projectiles = new List<Projectile>();


        //Variaveis do jogo
        private List<Vector2> _initialEnemyPositions = new List<Vector2>();
        private Vector2 _cameraPosition;
        private float _gravity = 400f;
        private float enemyFollowRange = 200f; // Adjust the range as needed
        private float enemySpeed = 100f;
        private float projectileCooldown = 2f; // Cooldown duration in seconds
        private float timeSinceLastProjectile = 0f;
        private Vector2 playerDirection = Vector2.UnitX;
        private SpriteFont _font;
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

        protected override void LoadContent()
        {
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //debug

            _font = Content.Load<SpriteFont>("Menu/Font_menu"); // Make sure the name matches your .spritefont file

            //Carregar texturas
            PlayerTexture = Content.Load<Texture2D>("Character4");
            plataformTexture = Content.Load<Texture2D>("platform_texture");
            enemyTexture = Content.Load<Texture2D>("Enemy/stand-0");
            pixelTexture = Content.Load<Texture2D>("pixel");
            projectileTexture = Content.Load<Texture2D>("projectile");
            //Adicionar inimigos a lista
            _enemies = new List<Enemy>
            {
                new Enemy(enemyTexture, new Vector2(2600, 500), enemySpeed),
                new Enemy(enemyTexture, new Vector2(3300, 500), enemySpeed)
            };

            //Guardar as posições iniciais dos inimigos
            foreach (var enemy in _enemies)
            {
                _initialEnemyPositions.Add(enemy.Position);
            }

            _character = new Personagem(PlayerTexture, new Vector2(2400, 550));

            //Criar plataformas e adicionar a lista
            _platforms = new List<Plataform>();
            _platforms.Add(new Plataform(plataformTexture, new Vector2(2400, 600)));
            _platforms.Add(new Plataform(plataformTexture, new Vector2(3100, 600)));



            _backgroundLayer1 = Content.Load<Texture2D>("Background/background1");
            _backgroundLayer2 = Content.Load<Texture2D>("Background/background2");
            _backgroundLayer3 = Content.Load<Texture2D>("Background/background3");
            _backgroundLayer4 = Content.Load<Texture2D>("Background/background4a");

            //inciar o menu
            _menu = new Menu(Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
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


        private void ResetGame()
        {

            // Reniciar o personagem
            _character.Position = new Vector2(2400, 550);
            _character.Velocity = Vector2.Zero;
            _character.IsOnGround = false;

            // Dá respawn aos inimigos
            RespawnEnemies();
            foreach (var enemy in _enemies)
            {
                enemy.Position = enemy.InitialPosition; // Reset to initial position
                enemy.Velocity = Vector2.Zero;
            }
            // Reset camera position
            _cameraPosition = Vector2.Zero;

            // Additional reset logic if needed
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


        private void RespawnEnemies()
        {
            _enemies.Clear(); // Clear existing enemies

            // Create new enemies at initial positions
            foreach (var initialEnemyPosition in _initialEnemyPositions)
            {
                _enemies.Add(new Enemy(enemyTexture, initialEnemyPosition, enemySpeed));
            }
        }


        private void LaunchProjectile()
        {
            // Create and add a new projectile to the list
            _projectiles.Add(new Projectile(projectileTexture, _character.Position, new Vector2(500, 0)* playerDirection, 5f)); // Adjust velocity as needed
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



            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];

                // Update the enemy position
                enemy.Update(_character.Position, enemyFollowRange, _platforms, _gravity, deltaTime);

                // Check for collision with the player
                if (_character.BoundingBox.Intersects(enemy.BoundingBox))
                {
                    // Check if the player is landing on top of the enemy
                    if (_character.BoundingBox.Bottom <= enemy.BoundingBox.Top + _character.Velocity.Y * deltaTime)
                    {
                        // Remove the enemy from the list
                        _enemies.RemoveAt(i);
                    }
                    else
                    {
                        // Reset the game and switch to menu state
                        ResetGame();
                        _gameState = GameState.Menu;
                        return;
                    }
                }
            }




           
            //Gere a interação com as plataformas
            foreach (var platform in _platforms)
            {
                    if (_character.BoundingBox.Intersects(platform.BoundingBox)&& _character.BoundingBox.Bottom >= platform.BoundingBox.Top)
                   
                    {
                        // Adjust player position to prevent clipping into the platform
                        _character.Position = new Vector2(_character.Position.X, platform.Position.Y - _character.Texture.Height);
                        _character.Velocity = new Vector2(_character.Velocity.X, Math.Max(0, _character.Velocity.Y)); // Prevent downward velocity

                        // Update player's on-ground status
                        isOnPlatform = true;
                        break; // Stop checking further platforms as the character is already on one
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


            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _character.IsOnGround)
            {
                _character.Velocity = new Vector2(_character.Velocity.X, -300);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _character.Position = new Vector2(_character.Position.X - 150 * deltaTime, _character.Position.Y);
                playerDirection = -Vector2.UnitX;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _character.Position = new Vector2(_character.Position.X + 150 * deltaTime, _character.Position.Y);
                playerDirection = Vector2.UnitX;
            }

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


                foreach (var platform in _platforms)
                {
                    platform.Draw(_spriteBatch);
                }

                foreach (var enemy in _enemies)
                {
                    if (_enemies != null)
                    {
                        _spriteBatch.Draw(pixelTexture, enemy.BoundingBox, Color.Red * 0.5f);
                        enemy.Draw(_spriteBatch);
                    }
                }
                _spriteBatch.Draw(pixelTexture, _character.BoundingBox, Color.Red * 0.5f);
                _spriteBatch.Draw(_character.Texture, _character.Position, Color.White);


                foreach (var projectile in _projectiles)
                {
                    projectile.Draw(_spriteBatch);
                }

                string positionText = $" Player Position: {_character.Position.X}, {_character.Position.Y}";
                _spriteBatch.DrawString(_font, positionText, _cameraPosition+new Vector2(0,45), Color.White);


                string positionCameraText = $" Camera Position: {_cameraPosition.X}, {_cameraPosition.Y}";
                _spriteBatch.DrawString(_font, positionCameraText, _cameraPosition+new Vector2(0,20), Color.White);


            }



          

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}