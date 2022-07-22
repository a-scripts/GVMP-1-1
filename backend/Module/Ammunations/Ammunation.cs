﻿using GTANetworkAPI;
using VMP_CNR.Module.Spawners;

namespace VMP_CNR.Module.Ammunations
{
    public class Ammunation
    {
        public int Id { get; }
        public Vector3 Position { get; }
        public ColShape ColShape { get; }

        public Ammunation(int id, Vector3 position)
        {
            Id = id;
            Position = position;
            ColShape = ColShapes.Create(Position, 5f, 0);
            ColShape.SetData("ammunationId", Id);
        }
    }
}