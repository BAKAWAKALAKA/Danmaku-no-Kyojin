﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework.Input;
using Danmaku_no_Kyojin.Collisions;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.Entities
{
    public class Ship : Entity
    {
        #region Fields

        DnK _gameRef;

        // Specific to the sprite
        private Texture2D _sprite;
        private Vector2 _center;

        private float _velocity;
        private bool _slowMode;
        private float _velocitySlowMode;
        private float _rotation;
        private Vector2 _distance;

        #endregion

        public Ship(DnK game, Vector2 position)
            : base(game)
        {
            _gameRef = game;

            _position = position;
            _velocity = 400f;
            _velocitySlowMode = 125f;
            _rotation = 0f;
            _center = Vector2.Zero;
            _distance = Vector2.Zero;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _sprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/ship2");
            _position.X = _position.X - _sprite.Width;
            _center = new Vector2(_sprite.Width / 2, _sprite.Height / 2);
            _boundingElement = new CollisionCircle((DnK)this.Game, this, 20);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Point motion = Point.Zero;

            Debug.Print(gameTime.ElapsedGameTime.TotalSeconds.ToString());

            // Keyboard
            if (InputHandler.KeyDown(Keys.D))
                motion.X = 1;
            if (InputHandler.KeyDown(Keys.Q))
                motion.X = -1;
            if (InputHandler.KeyDown(Keys.Z))
                motion.Y = -1;
            if (InputHandler.KeyDown(Keys.S))
                motion.Y = 1;

            if (InputHandler.KeyPressed(Keys.PageUp))
                _rotation += MathHelper.ToRadians(45);
            if (InputHandler.KeyPressed(Keys.PageDown))
                _rotation -= MathHelper.ToRadians(45);

            _slowMode = InputHandler.KeyDown(Keys.LeftShift) ? true : false;

            // Mouse
            _distance.X = _position.X - InputHandler.MouseState.X;
            _distance.Y = _position.Y - InputHandler.MouseState.Y;

            _rotation = (float)Math.Atan2(_distance.Y, _distance.X) - MathHelper.PiOver2;

            UpdatePosition(motion, dt);
        }

        private void UpdatePosition(Point motion, float dt)
        {
            if (_slowMode)
            {
                _position.X += motion.X * _velocitySlowMode * dt;
                _position.Y += motion.Y * _velocitySlowMode * dt;
            }
            else
            {
                _position.X += motion.X * _velocity * dt;
                _position.Y += motion.Y * _velocity * dt;
            }

            _center.X = _sprite.Width / 2;
            _center.Y = _sprite.Height / 2;
        }

        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin();

            _gameRef.SpriteBatch.Draw(_sprite, _position, null, Color.White, _rotation, _center, 1f, SpriteEffects.None, 0f);
            //_boundingElement.DrawDebug(_position, _rotation, new Vector2(_sprite.Width, _sprite.Height));
            //_gameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _rotation.ToString(), Vector2.Zero, Color.Black);
            //_gameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _distance.ToString(), new Vector2(0, 20), Color.Black);

            _gameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}