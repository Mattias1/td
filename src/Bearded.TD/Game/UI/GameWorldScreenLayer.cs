﻿using amulware.Graphics;
using Bearded.TD.Rendering;
using Bearded.TD.Screens;
using Bearded.TD.UI;
using OpenTK;

namespace Bearded.TD.Game.UI
{
    class GameWorldScreenLayer : ScreenLayer
    {
        private readonly GameInstance game;
        private readonly GameRunner runner;
        private readonly GeometryManager geometries;

        public override Matrix4 ViewMatrix => game.Camera.ViewMatrix;

        public GameWorldScreenLayer(ScreenLayerCollection parent, GameInstance game, GameRunner runner, GeometryManager geometries) : base(parent)
        {
            this.game = game;
            this.runner = runner;
            this.geometries = geometries;
        }

        public override bool HandleInput(UpdateEventArgs args, InputState inputState)
        {
            runner.HandleInput(args);
            return false;
        }

        public override void Update(UpdateEventArgs args)
        {
            runner.Update(args);
        }

        public override void Draw()
        {
            geometries.ConsoleFont.SizeCoefficient = new Vector2(1, -1);

            game.State.Level.Draw(geometries);
            //game.State.Navigator.DrawDebug(geometries, game.State.Level);

            foreach (var obj in game.State.GameObjects)
            {
                obj.Draw(geometries);
            }
        }

        protected override void OnViewportSizeChanged()
        {
            base.OnViewportSizeChanged();
            game.Camera.OnViewportSizeChanged(ViewportSize);
        }
    }
}
