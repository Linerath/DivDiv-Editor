using DivDivEditor.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DivDivEditor.UI
{
    public class MapController
    {
        private readonly UIController ui;
        private readonly MapService map;

        private Texture2D[,] TilesBottom = new Texture2D[60, 34];
        private Texture2D[,] TilesTop = new Texture2D[60, 34];

        public MapController(UIController ui, MapService map)
        {
            this.ui = ui;
            this.map = map;
        }

        public void Update()
        {
            for (int i = 0; i < ui.Window.ClientBounds.Width / Vars.TileSize; i++)
            {
                for (int j = 0; j < ui.Window.ClientBounds.Height / Vars.TileSize + 1; j++)
                {
                    var x = i + ui.Nav.X;
                    var y = j + ui.Nav.Y;

                    var bottomTexture = map.GetTileTextureName(x, y, true);
                    var topTexture = map.GetTileTextureName(x, y, false);

                    TilesBottom[i, j] = ui.Content.Load<Texture2D>(bottomTexture);
                    TilesTop[i, j] = ui.Content.Load<Texture2D>(topTexture);
                }
            }
        }

        public void Render()
        {
            for (int i = 0; i < ui.Window.ClientBounds.Width / Vars.TileSize; i++)
            {
                for (int j = 0; j < ui.Window.ClientBounds.Height / Vars.TileSize + 1; j++)
                {
                    Color color;
                    int effect = map.GetTileEffect(i + ui.Nav.X, j + ui.Nav.Y);

                    if (Settings.ShowTileEffect && effect != 0)
                    {
                        var effectType = (TileEffect)effect;
                        color = effectType switch
                        {
                            TileEffect.Water => Color.Gold,
                            TileEffect.Indoors => Color.Maroon,
                            TileEffect.Fog => Color.Aqua,
                            TileEffect.WaterFog => Color.Red,
                            _ => Color.Silver
                        };
                    }
                    else
                    {
                        color = Color.White;
                    }

                    ui.SpriteBatch.Draw(TilesBottom[i, j], new Vector2(i * Vars.TileSize, j * Vars.TileSize), color);
                    ui.SpriteBatch.Draw(TilesTop[i, j], new Vector2(i * Vars.TileSize, j * Vars.TileSize), color);
                }
            }
        }
    }
}
