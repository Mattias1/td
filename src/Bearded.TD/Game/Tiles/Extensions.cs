﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bearded.Utilities.Math;
using OpenTK;

namespace Bearded.TD.Game.Tiles
{
    static class Extensions
    {
        #region Lookup Tables

        private static readonly Step[] directionDelta =
        {
            new Step(0, 0),
            new Step(1, 0),
            new Step(0, 1),
            new Step(-1, 1),
            new Step(-1, 0),
            new Step(0, -1),
            new Step(1, -1),
        };

        private static readonly Direction[] directionOpposite =
        {
            Direction.Unknown,
            Direction.Right,
            Direction.UpRight,
            Direction.UpLeft,
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
        };

        private static readonly Direction[] directions =
        {
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
            Direction.Right,
            Direction.UpRight,
            Direction.UpLeft,
        };

        private static readonly Vector2[] corners =
            Enumerable.Range(0, 7)
                .Select(i =>
                    Direction2.FromDegrees(i * 60f - 30f).Vector
                )
                .ToArray();

        private static readonly Direction2[] direction2s =
            Enumerable.Range(0, 7)
                .Select(i =>
                    Direction2.FromDegrees(i * 60f - 60f)
                )
                .ToArray();

        private static readonly Vector2[] vectors =
            Enumerable.Range(0, 7)
                .Select(i =>
                    Direction2.FromDegrees(i * 60f - 60f).Vector
                )
                .ToArray();

        #endregion

        #region Tile<TTileInfo>

        public static IEnumerable<Tile<TTileInfo>> PossibleNeighbours<TTileInfo>(this Tile<TTileInfo> tile)
        {
            for (var i = 1; i <= 6; i++)
            {
                yield return tile.Offset(directionDelta[i]);
            }
        }

        #endregion

        #region Direction and Directions

        public static IEnumerable<Direction> Enumerate(this Directions directions)
            => Extensions.directions.Where(direction => directions.Includes(direction));

        public static Vector2 CornerBefore(this Direction direction) => corners[(int) direction - 1];
        public static Vector2 CornerAfter(this Direction direction) => corners[(int) direction];
        public static Direction2 SpaceTimeDirection(this Direction direction) => direction2s[(int) direction];
        public static Vector2 Vector(this Direction direction) => vectors[(int) direction];
        public static Step Step(this Direction direction) => directionDelta[(int) direction];
        public static bool Any(this Directions direction) => direction != Directions.None;
        public static bool Any(this Directions direction, Directions match) => direction.Intersect(match) != Directions.All;
        public static bool All(this Directions direction, Directions match) => direction.Intersect(match) == match;
        public static Direction Hexagonal(this Direction2 direction) => (Direction) ((int) Math.Floor(direction.Degrees * 1 / 60f + 0.5f) % 6 + 1);
        public static Direction Opposite(this Direction direction) => directionOpposite[(int) direction];

        private static Directions toDirections(this Direction direction) => (Directions) (1 << ((int) direction - 1));
        public static bool Includes(this Directions directions, Direction direction) => directions.HasFlag(direction.toDirections());
        public static Directions And(this Directions directions, Direction direction) => directions | direction.toDirections();
        public static Directions Except(this Directions directions, Direction direction) => directions & ~direction.toDirections();
        public static Directions Union(this Directions directions, Directions directions2) => directions | directions2;
        public static Directions Except(this Directions directions, Directions directions2) => directions & ~directions2;
        public static Directions Intersect(this Directions directions, Directions directions2) => directions & directions2;
        public static bool IsSubsetOf(this Directions directions, Directions directions2) => directions2.HasFlag(directions);

        #endregion
    }
}
