using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DivDivEditor.UI
{
    public class UIController
    {
        public SpriteBatch SpriteBatch { get; private set; }
        public GameWindow Window { get; private set; }
        public ContentManager Content { get; private set; }
        public Navigation Nav { get; private set; }

        public UIController(
            SpriteBatch spriteBatch,
            GameWindow window,
            ContentManager content,
            Navigation nav)
        {
            SpriteBatch = spriteBatch;
            Window = window;
            Content = content;
            Nav = nav;
        }
    }
}
