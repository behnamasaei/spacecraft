using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceCraft
{
    public class EnemyCraft : DrawableGameComponent
    {
        const int updateInterval = 40;
        int updateChack = 0;
        public bool BulletFire = false;

        public SpaceModle SpaceModel = new SpaceModle();
        public PlayerModle PlayerModle { get; set; }
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        public List<EnemyModle> EnemyList;

        public Space space;
        public EnemyCraft(
            Game game,
            GraphicsDevice graphics,
            SpriteBatch spriteBatch,
            Texture2D spaceTexture,
            List<EnemyModle> enemyList,
            PlayerModle playerModle
        ) : base(game)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            EnemyList = enemyList;
            PlayerModle = playerModle;
            SpaceModel.SpaceTexture = spaceTexture;
            SpaceModel.SpacePosX = 0;
            SpaceModel.SpacePosY = (graphics.Viewport.Height / 2) - 30;
        }

        public override void Update(GameTime gameTime)
        {
            updateChack += gameTime.ElapsedGameTime.Milliseconds;
            if (updateChack >= updateInterval)
            {
                updateChack = 0;
                new Thread(
                    () =>
                    {
                        foreach (var enemyCraft in EnemyList)
                        {
                            enemyCraft.PosX -= (PlayerModle.Score / 10) + 10;
                        }
                    }
                ).Start();
            }
            foreach (var enemy in EnemyList)
            {
                if (enemy.PosX <= -100)
                {
                    EnemyList.RemoveAt(EnemyList.IndexOf(enemy));
                    PlayerModle.Health--;
                    break;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (EnemyList != null)
                foreach (var enemy in EnemyList)
                {
                    _spriteBatch.Draw(
                        SpaceModel.SpaceTexture,
                        new Rectangle(enemy.PosX, enemy.PosY, 100, 100),
                        Color.White
                    );
                }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
