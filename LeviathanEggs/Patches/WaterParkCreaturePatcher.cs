using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using LeviathanEggs.Spawnables;
using Nautilus.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UWE;

namespace LeviathanEggs.Patches
{
    [HarmonyPatch]
    internal static class WaterParkCreaturePatcher
    {
        /// <summary>
        /// Intercept the birth of a new creature to set up some custom data and behaviours.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.BornAsync))]
        private static IEnumerator PassthroughPatchBorn(IEnumerator enumerator)
        {
            // BornAsync yields a break if the prefab reference was invalid, do nothing if so.
            if (!enumerator.MoveNext())
                yield break;
            // Through the MoveNext() above the method has progressed to yielding and waiting for the prefab to load.
            // This condition should never fail, but just in case, try our best to keep the vanilla flow going.
            if (!(enumerator.Current is CoroutineTask<GameObject> task))
            {
                LeviathanEggsInit._Log.Error("WaterParkCreature birth passthrough task was null!");
                yield return enumerator;
                yield break;
            }
            
            // By yielding only the task we gain execution directly after its completion, *before* the original method.
            yield return task;
            var gameObject = task.GetResult();
            var creature = gameObject.GetComponent<Creature>();
            
            switch (creature)
            {
                case ReaperLeviathan _:
                    PatchReaper(gameObject);
                    break;
                case GhostLeviathan _:
                    break;
            }
            
            // Let the rest of the method continue as normal.
            yield return enumerator;
        }

        private static void PatchReaper(GameObject gameObject)
        {
            var wpc = gameObject.EnsureComponent<WaterParkCreature>();
            wpc.data = ReaperEgg.CreatureData;
            // Constrain the max velocity so that tiny reapers don't go zooming everywhere.
            wpc.swimMaxVelocity = 0.75f;
        }
        
        /// <summary>
        /// Laying an egg takes a reference to an Addressable asset, which is an incredible hassle for mods to build
        /// and does not work well with Nautilus' CustomPrefab system. Add a custom function to spawn our own thing
        /// without Addressables.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.ManagedUpdate))]
        private static IEnumerable<CodeInstruction> Transpiler_ManagedUpdate(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions);
            // Match to the very end of the function, where successful breeding spawns a new egg.
            matcher.MatchForward(false,
                new CodeMatch(CodeInstruction.Call(typeof(WaterParkCreature), nameof(WaterParkCreature.Born))),
                new CodeMatch(OpCodes.Ret)
            );
            if (matcher.IsInvalid)
            {
                LeviathanEggsInit._Log.Error("Failed to transpile WaterParkCreature.Born(), eggs will not spawn!");
                return matcher.InstructionEnumeration();
            }
            
            // Advance to the very end of the if statement, after the Born() call.
            // This is safe because the label that all the branches jump to is on the ret instruction.
            matcher.Advance(1);
            matcher.Insert(
                // Load 'this', the WaterParkCreature.
                new CodeInstruction(OpCodes.Ldarg_0),
                // Pass it to our own creature birthing function.
                CodeInstruction.Call(typeof(WaterParkCreaturePatcher), nameof(LayEgg))
            );

            return matcher.InstructionEnumeration();
        }

        private static void LayEgg(WaterParkCreature parent)
        {
            if (parent.GetTechType() != TechType.ReaperLeviathan)
                return;
            
            CoroutineHost.StartCoroutine(LayEggAsync(parent));
        }

        private static IEnumerator LayEggAsync(WaterParkCreature parent)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(ReaperEgg.TechType, false);
            yield return task;
            var result = task.GetResult();

            // Same as the original method.
            Vector3 position = parent.transform.position + Vector3.down;
            var egg = Object.Instantiate(result, position, Quaternion.identity);
            egg.SetActive(true);
            parent.currentWaterPark.AddItem(egg.GetComponent<Pickupable>());
        }
    }
}