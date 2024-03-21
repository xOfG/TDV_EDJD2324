using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Sokoban2024
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D player, dot, box, wall;
        private int tileSize = 64;
        private Player sokoban;

        public char[,] level;
        public List<Point> boxes;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            LoadLevel("level1.txt");
            _graphics.PreferredBackBufferHeight = tileSize * level.GetLength(1);
            _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0);
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            box = Content.Load<Texture2D>("Crate_Purple");
            wall = Content.Load<Texture2D>("Wall_Brown");
            dot = Content.Load<Texture2D>("EndPoint_Blue");
            player = Content.Load<Texture2D>("Character4");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            sokoban.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Pink);

            _spriteBatch.Begin();
            Rectangle position = new Rectangle(0, 0, tileSize, tileSize);
            for (int x = 0; x < level.GetLength(0); x++)
            {
                for (int y = 0; y < level.GetLength(1); y++)
                {
                    position.X = x * tileSize;
                    position.Y = y * tileSize;
                    switch (level[x, y])
                    {
                        case '.':
                            _spriteBatch.Draw(dot, position, Color.White);
                            break;
                        case 'X':
                            _spriteBatch.Draw(wall, position, Color.White);
                            break;
                    }
                }
            }

            foreach (Point b in boxes)
            {
                position.X = b.X * tileSize;
                position.Y = b.Y * tileSize;
                _spriteBatch.Draw(box, position, Color.White);
            }

            position.X = sokoban.Position.X * tileSize;
            position.Y = sokoban.Position.Y * tileSize;
            _spriteBatch.Draw(player, position, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool HasBox(int x, int y)
        {
            foreach (Point b in boxes)
            {
                if (b.X == x && b.Y == y) return true;
            }
            return false;
        }
        public bool FreeTile(int x, int y)
        {
            if (level[x, y] == 'X') return false;  // Wall means its taken
            if (HasBox(x, y)) return false; // Has a box
            return true;

            /* The same as:    return level[x,y] != 'X' && !HasBox(x,y);   */
        }

        void LoadLevel(string levelFile)
        {
            boxes = new List<Point>();
            string[] linhas = File.ReadAllLines($"Content/{levelFile}");  // "Content/" + level
            int nrLinhas = linhas.Length;
            int nrColunas = linhas[0].Length;

            level = new char[nrColunas, nrLinhas];
            for (int x = 0; x < nrColunas; x++)
            {
                for (int y = 0; y < nrLinhas; y++)
                {
                    if (linhas[y][x] == '#')
                    {
                        boxes.Add(new Point(x, y));
                        level[x, y] = ' '; // put a blank instead of the box '#'
                    }
                    else if (linhas[y][x] == 'Y')
                    {
                        sokoban = new Player(this, x, y);
                        level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                    }
                    else
                    {
                        level[x, y] = linhas[y][x];
                    }
                }
            }
        }
    }
}