using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class PhaseRuntimeCommons
    {
        public readonly GameManager gm;
        public readonly List<GameObject> spawnedObjects;

        public PhaseRuntimeCommons(GameManager gameManager)
        {
            gm = gameManager;
            spawnedObjects = new List<GameObject>(1024);
        }
    }
}