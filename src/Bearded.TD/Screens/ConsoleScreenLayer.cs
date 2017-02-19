﻿using System;
using System.Collections.Generic;
using amulware.Graphics;
using Bearded.TD.Rendering;
using Bearded.Utilities;
using Bearded.Utilities.Input;
using Bearded.Utilities.Math;
using OpenTK;
using OpenTK.Input;

namespace Bearded.TD.Screens
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

        #if DEBUG
        private static readonly HashSet<Logger.Severity> visibleSeverities = new HashSet<Logger.Severity>
        {
            Logger.Severity.Fatal, Logger.Severity.Error, Logger.Severity.Warning,
            Logger.Severity.Info, Logger.Severity.Debug, Logger.Severity.Trace
        };
        #else
        private static readonly HashSet<Logger.Severity> visibleSeverities = new HashSet<Logger.Severity>
        {
            Logger.Severity.Fatal, Logger.Severity.Error, Logger.Severity.Warning, Logger.Severity.Info
        };
        #endif

        private readonly Logger logger;
        private bool isConsoleEnabled;

        public ConsoleScreenLayer(Logger logger, GeometryManager geometries) : base(geometries, 0, 1, true)
        {
            this.logger = logger;
        }

        public override bool HandleInput(UpdateEventArgs args)
        {
            if (InputManager.IsKeyHit(Key.Tilde))
                isConsoleEnabled = !isConsoleEnabled;

            return true;
        }

        public override void Update(UpdateEventArgs args) { }

        public override void Draw()
        {
            if (!isConsoleEnabled) return;

            Geometries.ConsoleBackground.Color = Color.Black.WithAlpha(.7f).Premultiplied;
            Geometries.ConsoleBackground.DrawRectangle(0, 0, ViewportSize.Width, consoleHeight);

            var logEntries = logger.GetSafeRecentEntries();

            Geometries.ConsoleFont.SizeCoefficient = new Vector2(1, 1);
            Geometries.ConsoleFont.Height = fontSize;

            var maxVisible = Mathf.CeilToInt(consoleHeight / lineHeight);
            var start = Math.Max(0, logEntries.Count - maxVisible - 1);

            var y = consoleHeight - padding - lineHeight;
            var i = logEntries.Count;

            while (y >= -lineHeight && i > 0)
            {
                var entry = logEntries[--i];
                if (!visibleSeverities.Contains(entry.Severity)) continue;
                Geometries.ConsoleFont.Color = colors[logEntries[i].Severity];
                Geometries.ConsoleFont.DrawString(new Vector2(padding, y), logEntries[i].Text);
                y -= lineHeight;
            }
        }
    }
}