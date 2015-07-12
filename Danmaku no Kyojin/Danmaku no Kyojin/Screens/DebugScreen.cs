﻿using System;
using System.Collections.Generic;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Danmaku_no_Kyojin.Entities.Boss;
using Danmaku_no_Kyojin.Particles;
using Danmaku_no_Kyojin.Shaders;
using Danmaku_no_Kyojin.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class DebugScreen : BaseGameState
    {
        // Camera
        Viewport _defaultView;

        private Texture2D _pixel;

        private List<Player> Players { get; set; }
        private Boss _boss;

        // Random
        public static readonly Random Rand = new Random();

        // Background
        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;

        // Bloom
        private BloomComponent _bloom;
        private bool _useBloom;

        public DebugScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            Players = new List<Player>();

            // Bloom
            _bloom = new BloomComponent(GameRef);
            Components.Add(_bloom);
            _bloom.Settings = new BloomSettings(null, 0.25f, 4, 2, 1, 1.5f, 1);
            _useBloom = true;
        }

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.GameArea.X, Config.GameArea.Y);
            
            base.Initialize();

            _bloom.Initialize();

            _defaultView = GraphicsDevice.Viewport;

            Players.Clear();

            // First player
            var player1 = new Player(GameRef, _defaultView, 1, Config.PlayersController[0], new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y - Config.GameArea.Y / 4f));
            player1.Initialize();
            Players.Add(player1);

            _boss = new Boss(GameRef, Players, 20);
            _boss.Initialize();

        }

        protected override void LoadContent()
        {
            _backgroundImage = Game.Content.Load<Texture2D>("Graphics/Pictures/background");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();

            if (InputHandler.PressedCancel())
            {
                UnloadContent();
                StateManager.ChangeState(GameRef.TitleScreen);
            }

            base.Update(gameTime);

            foreach (var p in Players)
            {
                if (p.IsAlive)
                {
                    if (p.BulletTime)
                    {
                        var newGameTime = new GameTime(
                            gameTime.TotalGameTime,
                            new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks / Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex].Key))
                        );

                        gameTime = newGameTime;
                    }

                    int i;
                    for (i = 0; i < p.GetBullets().Count; i++)
                    {
                        var currentPlayerBullet = p.GetBullets()[i];
                        currentPlayerBullet.Update(gameTime);

                        if (_boss.Intersects(currentPlayerBullet))
                        {
                            //_boss.TakeDamage(currentPlayerBullet.Power);

                            for (var j = 0; j < 30; j++)
                            {
                                GameRef.ParticleManager.CreateParticle(GameRef.LineParticle,
                                    currentPlayerBullet.Position, Color.LightBlue, 50, 1,
                                    new ParticleState()
                                    {
                                        Velocity = GameRef.Rand.NextVector2(0, 9),
                                        Type = ParticleType.Bullet,
                                        LengthMultiplier = 1
                                    });
                            }

                            p.GetBullets().Remove(currentPlayerBullet);
                            continue;
                        }

                        if (currentPlayerBullet.X < 0 || currentPlayerBullet.X > Config.GameArea.X ||
                            currentPlayerBullet.Y < 0 || currentPlayerBullet.Y > Config.GameArea.Y)
                        {
                            for (var j = 0; j < 30; j++)
                            {
                                GameRef.ParticleManager.CreateParticle(GameRef.LineParticle,
                                    currentPlayerBullet.Position, Color.LightBlue, 50, 1,
                                    new ParticleState()
                                    {
                                        Velocity = GameRef.Rand.NextVector2(0, 9),
                                        Type = ParticleType.Bullet,
                                        LengthMultiplier = 1
                                    });
                            }

                            p.GetBullets().Remove(currentPlayerBullet);
                        }

                        /*
                        // Collision with turrets
                        for (int j = 0; j < _boss.Turrets.Count; j++)
                        {
                            if (_boss.Turrets[j].Intersects(p.GetBullets()[i]))
                            {
                                _boss.Turrets[j].Color = Color.Blue;
                                _boss.DestroyTurret(_boss.Turrets[j], p.GetBullets()[i]);
                                p.GetBullets().Remove(p.GetBullets()[i]);
                                break;
                            }
                        }
                        */
                    }

                    p.Update(gameTime);
                }
            }

            _boss.Update(gameTime);
            
            if (Config.Debug && InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;
        }

        public override void Draw(GameTime gameTime)
        {
            _bloom.BeginDraw();
            if (!_useBloom)
                base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            var backgroundColor = new Color(50, 50, 50);
            GraphicsDevice.Clear(backgroundColor);

            // Draw game arena background
            GameRef.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, Players[0].Camera.GetTransformation());
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            GameRef.SpriteBatch.End();

            GameRef.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Players[0].Camera.GetTransformation());

            Players[0].Draw(gameTime);

            _boss.Draw(gameTime, Players[0].Camera.GetTransformation());

            foreach (var bullet in Players[0].GetBullets())
                bullet.Draw(gameTime);

            GameRef.ParticleManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();

            if (_useBloom)
                base.Draw(gameTime);

            // Draw UI
            GameRef.SpriteBatch.Begin();

            // Text
            /*
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
            "Position: " + Players.First().GetPosition(),
            new Vector2(0, 20), Color.White);
            */
            GameRef.SpriteBatch.End();
        }

        /// <summary>
        /// Handles input for quitting or changing the bloom settings.
        /// </summary>
        private void HandleInput()
        {
            if (InputHandler.KeyPressed(Keys.B))
            {
                _useBloom = !_useBloom;
            }
        }
    }
}
