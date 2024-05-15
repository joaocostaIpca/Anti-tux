using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace projeto_jogo
{
    public class Menu
    {
        private Texture2D _menuBackground;
        private Texture2D _startButtonTexture;
        private Texture2D _exitButtonTexture;

        private Rectangle _startButtonRectangle;
        private Rectangle _exitButtonRectangle;

        private SpriteFont _font; // Adicionando uma variável para a fonte
        private string _texto;


        public Menu(ContentManager content, int screenWidth, int screenHeight)
        {
            _menuBackground = content.Load<Texture2D>("Menu/menu_background");
            _startButtonTexture = content.Load<Texture2D>("Menu/start_button");
            _exitButtonTexture = content.Load<Texture2D>("Menu/exit_button");
            _font = content.Load<SpriteFont>("Menu/Font_menu"); // carrega a fonte para o texto
            _texto = "Trabalho Realizado por : Afonso Paiva / Joao Costa / Rafael Santos";
       


            // Posicionamento dos botões
            int buttonWidth = 600;
            int buttonHeight = 200;
            int buttonSpacing = 680;           

            int startX = (screenWidth - buttonWidth  )/ 4;
            int startY = (screenHeight - (2 *  buttonHeight) );

            _startButtonRectangle = new Rectangle(startX, startY, buttonWidth, buttonHeight);
            _exitButtonRectangle = new Rectangle(startX + buttonSpacing, startY, buttonWidth, buttonHeight);

            
        }

        public void Update()
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Desenha o fundo do menu
            spriteBatch.Draw(_menuBackground, new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.White);

            // Desenha os botões
            spriteBatch.Draw(_startButtonTexture, _startButtonRectangle, Color.White);
            spriteBatch.Draw(_exitButtonTexture, _exitButtonRectangle, Color.White);

            // Desenha o texto
            spriteBatch.DrawString(_font, _texto, new Vector2(10, _exitButtonRectangle.Bottom + 100), Color.Black);
        }

        public bool StartButtonClicked()
        {
            // Verifica se o botão de começar jogo foi clicado
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed &&
                _startButtonRectangle.Contains(mouseState.Position))
            {
                return true;
            }
            return false;
        }

        public bool ExitButtonClicked()
        {
            // Verifica se o botão de sair foi clicado
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed &&
                _exitButtonRectangle.Contains(mouseState.Position))
            {
                return true;
            }
            return false;
        }
    }
}
