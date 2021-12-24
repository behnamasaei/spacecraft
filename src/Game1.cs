using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceCraft
{
    public class Game1 : Game
    {
        // Global
        public List<EnemyModle> EnemyList = new List<EnemyModle>();
        public PlayerModle PlayerModle = new PlayerModle();

        //time wait use agin space bar
        const int updateInterval = 200;
        int updateChack = 0;
        int updateCheckEnemy = 0;

        // Data in panel
        public int Score { get; set; } = 0;
        public int Life = 3;

        //
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public SpriteFont _font;

        //
        Texture2D _background;
        Texture2D _spaceTexture;
        Texture2D _enemyTexture;
        Texture2D _heart;
        Texture2D _gameover;
        Song song;
        Song bombSong;
        //
        public Space space;
        public EnemyCraft enemyCraft;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.ApplyChanges();

            PlayerModle.Score = 0;
            PlayerModle.Health = 3;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _font = Content.Load<SpriteFont>("font");
            _background = Content.Load<Texture2D>("sky");
            _spaceTexture = Content.Load<Texture2D>("space");
            _enemyTexture = Content.Load<Texture2D>("enemy");
            song = Content.Load<Song>("Fire");
            bombSong = Content.Load<Song>("Bomb");
            _heart = Content.Load<Texture2D>("heart");
            _gameover = Content.Load<Texture2D>("gameover");

            space = new Space(
                this,
                GraphicsDevice,
                _spriteBatch,
                _spaceTexture,
                EnemyList,
                bombSong,
                PlayerModle
            );
            enemyCraft = new EnemyCraft(
                this,
                GraphicsDevice,
                _spriteBatch,
                _enemyTexture,
                EnemyList,
                PlayerModle
            );

            this.Components.Add(enemyCraft);
            this.Components.Add(space);
        }

        void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            // 0.0f is silent, 1.0f is full volume
            MediaPlayer.Volume -= 0.1f;
            MediaPlayer.Play(song);
        }
        protected override void Update(GameTime gameTime)
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && space.SpaceModel.SpacePosY > 0)
            {
                space.SpaceModel.SpacePosY -= 10;
            }

            if (
                Keyboard.GetState().IsKeyDown(Keys.Down)
                && space.SpaceModel.SpacePosY < GraphicsDevice.Viewport.Height - 100
            )
            {
                space.SpaceModel.SpacePosY += 10;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) && space.SpaceModel.SpacePosX > 0)
            {
                space.SpaceModel.SpacePosX -= 10;
            }

            if (
                Keyboard.GetState().IsKeyDown(Keys.Right)
                && space.SpaceModel.SpacePosX < GraphicsDevice.Viewport.Width - 100
            )
            {
                space.SpaceModel.SpacePosX += 10;
            }

            updateChack += gameTime.ElapsedGameTime.Milliseconds;
            Random rnd = new Random();
            int time = rnd.Next(2000, 3000);
            if (updateChack >= time)
            {
                updateChack = 0;
                
                if (PlayerModle.Score >= 20)
                {
                    int randomPosition = rnd.Next(1, 4) * 100;
                    for (int i = 0; i < PlayerModle.Score / 20; i++)
                    {
                        this.EnemyList.Add(
                            new EnemyModle()
                            {
                                PosX = GraphicsDevice.Viewport.Width,
                                PosY = (GraphicsDevice.Viewport.Height - randomPosition)
                            }
                        );
                        int timeEnemy = rnd.Next(1,4)*100;
                        while(timeEnemy == randomPosition)
                        {
                            timeEnemy = rnd.Next(1, 4) * 100;
                        }
                        randomPosition = timeEnemy;
                    }
                }

                if (PlayerModle.Score < 20)
                {
                    this.EnemyList.Add(
                        new EnemyModle()
                        {
                            PosX = GraphicsDevice.Viewport.Width,
                            PosY = (GraphicsDevice.Viewport.Height - rnd.Next(1, 4) * 100)
                        }
                    );
                }
            }

            updateChack += gameTime.ElapsedGameTime.Milliseconds;
            if (
                Keyboard.GetState().IsKeyDown(Keys.Space)
                && updateChack >= updateInterval
                && PlayerModle.Health > 0
            )
            {
                new Thread(
                    () =>
                    {
                        MediaPlayer.Play(song);
                    }
                ).Start();

                updateChack = 0;
                space.BulletList.Add(
                    new BulletModle()
                    {
                        BulletX = space.SpaceModel.SpacePosX + 100,
                        BulletY = space.SpaceModel.SpacePosY + 50
                    }
                );
                space.BulletFire = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && PlayerModle.Health <= 0)
            {
                PlayerModle.Health = 3;
                PlayerModle.Score = 0;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            if (PlayerModle.Health > 0)
            {
                _spriteBatch.Draw(_background, new Rectangle(0, 0, 800, 500), Color.White);

                _spriteBatch.DrawString(
                    _font,
                    $"Score: {PlayerModle.Score}",
                    new Vector2(680, 470),
                    Color.White
                );

                for (int i = 0; i < PlayerModle.Health; i++)
                {
                    _spriteBatch.Draw(
                        _heart,
                        new Rectangle((int)(i * 50 + 10), 10, 40, 40),
                        Color.White
                    );
                }
            }

            if (PlayerModle.Health <= 0)
            {
                EnemyList.Clear();
                _spriteBatch.Draw(_gameover, new Rectangle(0, 0, 800, 500), Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
