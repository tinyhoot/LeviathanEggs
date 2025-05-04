using System;
using BepInEx;
using HarmonyLib;
using HootLib;
using LeviathanEggs.Spawnables;
using UnityEngine;
using ILogHandler = HootLib.Interfaces.ILogHandler;

namespace LeviathanEggs
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("com.snmodding.nautilus", "1.0.0.35")]
    internal class LeviathanEggsInit : BaseUnityPlugin
    {
        public const string GUID = "com.github.tinyhoot.LeviathanEggs";
        public const string NAME = "Breedable Leviathans";
        public const string VERSION = "0.1";

        internal static ILogHandler _Log;

        private void Awake()
        {
            _Log = new HootLogger(NAME);
            _Log.Info($"{NAME} v{VERSION} starting up!");

            ReaperEgg egg = new ReaperEgg();
            egg.Register();
            VanillaLeviathans.PatchReaper();

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll(Hootils.GetAssembly());
            
            _Log.Info($"Finished loading.");
        }
    }
}