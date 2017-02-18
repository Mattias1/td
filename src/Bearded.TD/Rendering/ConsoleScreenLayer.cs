﻿using System;
using System.Collections.Generic;
using amulware.Graphics;
using Bearded.Utilities;
using Bearded.Utilities.Math;
using OpenTK;

namespace Bearded.TD.Rendering
{
    class ConsoleScreenLayer : UIScreenLayer
    {
        private const float consoleHeight = 320;
        private const float fontSize = 14;
        private const float lineHeight = 16;
        private const float padding = 6;

        private static readonly Dictionary<Logger.Severity, Color> colors = new Dictionary<Logger.Severity, Color>
        {
            { Logger.Severity.Fatal, Color.DeepPink },
            { Logger.Severity.Error, Color.Red },
            { Logger.Severity.Warning, Color.Yellow },
            { Logger.Severity.Info, Color.White },
            { Logger.Severity.Debug, Color.SpringGreen },
            { Logger.Severity.Trace, Color.SkyBlue },
        };

        private readonly Logger logger;

        public ConsoleScreenLayer(Logger logger, GeometryManager geometries) : base(geometries)
        {
            this.logger = logger;
        }

        public override void Draw()
        {
            Geometries.ConsoleBackground.Color = Color.Black.WithAlpha(.7f).Premultiplied;
            Geometries.ConsoleBackground.DrawRectangle(-640, 0, 1280, consoleHeight);

            var logEntries = logger.GetSafeRecentEntries();

            Geometries.ConsoleFont.Height = fontSize;

            var maxVisible = Mathf.CeilToInt(consoleHeight / lineHeight);
            var start = Math.Max(0, logEntries.Count - maxVisible - 1);

            for (int i = start; i < logEntries.Count; i++)
            {
                var y = consoleHeight - padding - (logEntries.Count - i) * lineHeight;
                Geometries.ConsoleFont.Color = colors[logEntries[i].Severity];
                Geometries.ConsoleFont.DrawString(new Vector2(padding - 640, y), logEntries[i].Text);
            }
        }
    }
}