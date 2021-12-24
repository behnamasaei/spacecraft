using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceCraft
{
    public class Space : DrawableGameComponent
    {
        const int updateInterval = 1;
        int updateChack = 0;
        public bool BulletFire = false;

        public SpaceModle SpaceModel = new SpaceModle();
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D Bullet;
        public List<BulletModle> BulletList = new List<BulletModle>();
        public PlayerModle PlayerModle { get; set; }

        public List<Rectangle> tiles;
        //
        public List<EnemyModle> EnemyList;
        public Song BombSong;
        public Space(
            Game game,
            GraphicsDevice graphics,
            SpriteBatch spriteBatch,
            Texture2D spaceTexture,
            List<EnemyModle> enemyList,
            Song bombSong,
            PlayerModle playerModle
        ) : base(game)
        {
            PlayerModle = playerModle;
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            EnemyList = enemyList;
            BombSong = bombSong;
            SpaceModel.SpaceTexture = spaceTexture;
            SpaceModel.SpacePosX = 0;
            SpaceModel.SpacePosY = (graphics.Viewport.Height / 2) - 30;

            Bullet = new Texture2D(graphics, 1, 1);
            Bullet.SetData(new[] { Color.Red });

            tiles = new List<Rectangle>();
            tiles.Add(new Rectangle(0, 0, 10, 5));
        }

        public override void Update(GameTime gameTime)
        {
            updateChack += gameTime.ElapsedGameTime.Milliseconds;
            if (updateChack >= updateInterval && BulletFire)
            {
                updateChack = 0;
                foreach (var bullet in BulletList)
                {
                    bullet.BulletX += 5;
                    var checkKill = EnemyList.Exists(
                        e =>
                            bullet.BulletX > e.PosX + 50
                            && bullet.BulletY > e.PosY
                            && bullet.BulletY < e.PosY + 100
                    );

                    if (checkKill)
                    {
                        PlayerModle.Score++;
                        new Thread(
                            () =>
                            {
                                MediaPlayer.Play(BombSong);
                            }
                        ).Start();

                        EnemyList.Remove(
                            EnemyList.Find(
                                e =>
                                    bullet.BulletX > e.PosX + 50
                                    && bullet.BulletY >= e.PosY
                                    && e.PosY <= bullet.BulletY + 100
                            )
                        );
                        BulletList.RemoveAt(BulletList.IndexOf(bullet));
                        break;
                    }
                }
            }
            foreach (var bullet in BulletList)
            {
                if (bullet.BulletX > _graphics.Viewport.Width)
                {
                    BulletList.RemoveAt(BulletList.IndexOf(bullet));
                    break;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (PlayerModle.Health > 0)
            {
                _spriteBatch.Draw(
                    SpaceModel.SpaceTexture,
                    new Rectangle(SpaceModel.SpacePosX, SpaceModel.SpacePosY, 100, 100),
                    Color.White
                );

                foreach (var bullet in BulletList)
                {
                    _spriteBatch.Draw(
                        Bullet,
                        new Rectangle(bullet.BulletX, bullet.BulletY, 5, 5),
                        Color.White
                    );
                }
            }
            if (PlayerModle.Health <= 0)
            {
                BulletList.Clear();
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
