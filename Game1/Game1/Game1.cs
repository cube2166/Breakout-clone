using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D blank;
        Texture2D bb1;
        Texture2D bb2;
        Texture2D ball;
        Texture2D[] brickArray;
        SpriteFont font;
        MyBoard player;
        MyBall mb;
        MyObjList myObjectCollect;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;

            base.Initialize();
        }

        void prepareShow(object obj1, object obj2)
        {
            Texture2D temp = obj1 as Texture2D;
            MyObject temp2 = obj2 as MyObject;
//            Vector2 spritePosition = new Vector2(temp2.X, temp2.Y);
            spriteBatch.Draw(temp, new Rectangle((int)temp2.X, (int)temp2.Y, temp2.Width, temp2.Height), temp2.cc);
 //           spriteBatch.Draw(temp, spritePosition, new Rectangle((int)temp2.X, (int)temp2.Y, temp2.Width, temp2.Height), null, temp2.cc);
        }

        void prepareShowFont(object obj1, object obj2)
        {
            SpriteFont temp = obj1 as SpriteFont;
            MyFont temp2 = obj2 as MyFont;
            spriteBatch.DrawString(temp, temp2.ss, new Vector2(temp2.sX, temp2.sY), temp2.sC);
            spriteBatch.DrawString(temp, temp2.ss, new Vector2(temp2.X, temp2.Y), temp2.cc);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blank = Content.Load<Texture2D>("blank");
            font = Content.Load<SpriteFont>("font");
            ball = Content.Load<Texture2D>("ball");
            brickArray = new Texture2D[4];
            for (int ii = 0; ii < brickArray.Length; ii++)
            {
                string ss = string.Format("b{0}", ii + 1);
                brickArray[ii] = Content.Load<Texture2D>(ss);
            }
            // TODO: use this.Content to load your game content here


            MyWall[] wallCollect;
            MyFont gameTitle;

            int offset = 600 + (800 - 600) / 2 - (int)font.MeasureString("Brick - Breaking").X / 2;
            gameTitle = new MyFont(offset, 20, "Brick - Breaking", Color.Yellow, -1,font);
            gameTitle.showHandler += prepareShowFont;

            player = new MyBoard(600 / 2 - 100 / 2, 600 - 20 - 10, 100, 20, Color.White, 5, 0 + 20, 600 - 20, blank);
            player.showHandler += prepareShow;
            wallCollect = new MyWall[3];
            wallCollect[0] = new MyWall(0, 0, 600, 20, Color.Gray, blank);
            wallCollect[0].showHandler += prepareShow;
            wallCollect[1] = new MyWall(0, 0, 20, 600, Color.Gray, blank);
            wallCollect[1].showHandler += prepareShow;
            wallCollect[2] = new MyWall(600 - 20, 0, 20, 600, Color.Gray, blank);
            wallCollect[2].showHandler += prepareShow;

            myObjectCollect = new MyObjList();
            myObjectCollect.Add(player);
            myObjectCollect.Add(wallCollect[0]);
            myObjectCollect.Add(wallCollect[1]);
            myObjectCollect.Add(wallCollect[2]);
            myObjectCollect.Add(gameTitle);

            int maxCol = 10;
            int maxRow = 10;
            Random rr = new Random();

            for (int row = 0; row < maxRow; row++)
            {
                int width = (600 - (20 * 2)) / maxCol;
                int height = 30;
                for (int col = 0; col < maxCol; col++)
                {
                    int px = ((width * col) + 20) + 1;
                    int py = ((height * row) + 20) + 1;
                    int ww = width - 2;
                    int hh = height - 2;
                    Texture2D text2D;
                    text2D = brickArray[rr.Next(0, 3)];

                    MyBrick temp;
                    temp = new MyBrick(px, py, ww, hh, Color.White, text2D);
                    temp.showHandler += prepareShow;
                    myObjectCollect.Add(temp);
                }
            }
            int size = 20;

 //           mb = new MyBall((int)player.X + player.Width / 2 - size/2, (int)player.Y - size, size, 5, Color.Red, ball);
            mb = new MyBall((int)player.X + 50, (int)player.Y - 50, size, 200, Color.Red, blank);
            mb.showHandler += prepareShow;
            myObjectCollect.Add(mb);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            mb.Update(elapsedTime);

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            {
                player.MoveLeft();
            }
            else if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            {
                player.MoveRight();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (var item in myObjectCollect)
            {
                item.Show();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
