﻿using System.Collections.Generic;
using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Danmaku_no_Kyojin.Entities
{
    class Bullet : BaseBullet
    {
        public bool WaveMode { get; set; }
        private float _distance;

        public Bullet(DnK game, Texture2D sprite, Vector2 position, Vector2 direction, float velocity)
            : base(game, sprite, position, direction, velocity)
        {
            Rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
            _distance = 0;

            WaveMode = false;

            var vertices = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(sprite.Width, 0),
                    new Vector2(sprite.Width, sprite.Height),
                    new Vector2(0, sprite.Height),
                };

            CollisionBoxes.Add(new CollisionCircle(this, Vector2.Zero, sprite.Width / 2f));

            Power = Improvements.ShootPowerData[PlayerData.ShootPowerIndex].Key;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (WaveMode)
            {
                _distance += 0.75f;
                Direction.X = (float)Math.Cos(_distance);
            }

            //Rotation = (Rotation + 0.25f) % 360;

            Position += Direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}
