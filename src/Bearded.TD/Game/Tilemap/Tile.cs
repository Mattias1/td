﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Bearded.TD.Game.Tilemap
{
    struct Tile<TTileInfo> : IEquatable<Tile<TTileInfo>>
    {
        private readonly Tilemap<TTileInfo> tilemap;
        public int X { get; }
        public int Y { get; }

        public Tile(Tilemap<TTileInfo> tilemap, int x, int y)
        {
            this.tilemap = tilemap;
            this.X = x;
            this.Y = y;
        }

        public int Radius => X * Y >= 0
            ? Math.Abs(X + Y)
            : Math.Max(Math.Abs(X), Math.Abs(Y));
        public Vector2 Xy => new Vector2(X, Y);
        public TTileInfo Info => tilemap[this];
        public bool IsValid => tilemap?.IsValidTile(X, Y) == true;

        public Tile<TTileInfo> Neighbour(Direction direction) => this.Neighbour(direction.Step());
        public Tile<TTileInfo> Neighbour(Step step) => new Tile<TTileInfo>(tilemap, X + step.X, Y + step.Y);

        public IEnumerable<Tile<TTileInfo>> Neighbours => this.PossibleNeighbours().Where(t => t.IsValid);

        public bool Equals(Tile<TTileInfo> other) =>
            X == other.X &&
            Y == other.Y &&
            tilemap == other.tilemap;

        public static bool operator ==(Tile<TTileInfo> t1, Tile<TTileInfo> t2) => t1.Equals(t2);
        public static bool operator !=(Tile<TTileInfo> t1, Tile<TTileInfo> t2) => !(t1 == t2);
    }
}