using System.IO;
using HootLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace LeviathanEggs.Spawnables
{
    internal class ReaperEgg
    {
        public const string ClassId = "ReaperEgg";
        public static TechType TechType;
        public static WaterParkCreatureData CreatureData;
        private PrefabInfo _info;
        private CustomPrefab _prefab;
        
        public ReaperEgg()
        {
            _info = CreatePrefabInfo();
            TechType = _info.TechType;
            _prefab = CreatePrefab(_info);
            CreatureData = CreateCreatureData();
        }
        
        private PrefabInfo CreatePrefabInfo()
        {
            Atlas.Sprite sprite = Hootils.LoadSprite("reaperegg_sprite.png");
            return PrefabInfo.WithTechType(ClassId, "Reaper Egg", "A reaper's egg.")
                .WithSizeInInventory(new Vector2int(2, 2))
                .WithIcon(sprite);
        }

        private CustomPrefab CreatePrefab(PrefabInfo info)
        {
            CustomPrefab prefab = new CustomPrefab(info);
            prefab.CreateCreatureEgg();

            var bundle = AssetBundle.LoadFromFile(Path.Combine(Hootils.GetModDirectory(), "Assets", "eggs"));
            var bundleTemplate = new AssetBundleTemplate(bundle, ClassId, info);
            var eggTemplate = new EggTemplate(info, bundleTemplate)
                .WithHatchingCreature(TechType.ReaperLeviathan)
                .WithHatchingTime(1f)
                .WithMass(100f);
            prefab.SetGameObject(eggTemplate);
            // The basics are ready but some additional setup needs to be done on the actual GameObject.
            prefab.SetPrefabPostProcessor(PostProcess);
            return prefab;
        }

        private void PostProcess(GameObject gameObject)
        {
            // These functions *would* be necessary, but the EggTemplate already does it for us.
            // MaterialUtils.ApplySNShaders(gameObject);
            // PrefabUtils.AddBasicComponents(gameObject, "ReaperEgg", TechType, LargeWorldEntity.CellLevel.Near);
            // PrefabUtils.AddWorldForces(gameObject, 10f);
            
            // Make the egg trackable via the scanner room.
            PrefabUtils.AddResourceTracker(gameObject, TechType);
        }

        /// <summary>
        /// Set up the creature data once so it can be accessed for all hatching reaper eggs.
        /// </summary>
        private WaterParkCreatureData CreateCreatureData()
        {
            var data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
            data.initialSize = 0.025f;
            data.maxSize = 0.1f;
            data.outsideSize = 0.1f;
            data.isPickupableOutside = true;
            data.daysToGrow = 10f;
            data.canBreed = true;

            return data;
        }

        public void Register()
        {
            _prefab.Register();
        }
    }
}