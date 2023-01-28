using System;
using BepInEx;
using BreedableLeviathans.Spawnables;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace BreedableLeviathans
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string GUID = "com.github.tinyhoot.BreedableLeviathans";
        public const string NAME = "Breedable Leviathans";
        public const string VERSION = "0.1";

        private void Awake()
        {
            BreedableReaper boi = new BreedableReaper();
            boi.Patch();
        }
    }
}