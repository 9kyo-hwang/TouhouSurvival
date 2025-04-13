using System;
using UnityEngine;

namespace Unchord
{
    public class SpawnEventArgs : EventArgs
    {
        public SpawnerRuntime spawnerRuntime;
        public GameObject spawnedInstance;
        public Vector2 spawnedPosition;
    }
}