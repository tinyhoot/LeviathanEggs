using System.Collections;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using UnityEngine;
using UWE;

namespace BreedableLeviathans.Spawnables
{
    public class BreedableReaper : Spawnable
    {
        public override Vector2int SizeInInventory { get; }
        public override WorldEntityInfo EntityInfo { get; }
        private const float _scale = 0.1f;

        public BreedableReaper() : base("fren", "Reaper Leviathan", "A big boi!")
        {
            //SpriteHandler.RegisterSprite(TechType, SpriteManager.Get(TechType.HoleFish));
            SizeInInventory = new Vector2int(4, 4);
            //Description = "Hi! :3";
            //FriendlyName = "Richard";
            EntityInfo = GetEntityInfo();

        }
        
        private WorldEntityInfo GetEntityInfo()
        {
            WorldEntityInfo info = new WorldEntityInfo();
            info.classId = ClassID;
            info.cellLevel = LargeWorldEntity.CellLevel.Medium;
            info.slotType = EntitySlot.Type.Creature;
            info.techType = TechType;
            info.localScale = new Vector3(_scale, _scale, _scale);

            return info;
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            IPrefabRequest task = PrefabDatabase.GetPrefabAsync(CraftData.GetClassIdForTechType(TechType.ReaperLeviathan));
            yield return task;
            _ = task.TryGetPrefab(out GameObject prefab);
            GameObject reaper = Object.Instantiate(prefab);
            Pickupable pickup = reaper.EnsureComponent<Pickupable>();
            //Creature creature = reapyBoi.GetComponentInParent<Creature>();
            ReaperLeviathan levi = reaper.GetComponent<ReaperLeviathan>();
            levi.SetScale(_scale);  // This doesn't work? Too early?
            levi.babyScaleSize = 0.3f;
            levi.friendlyToPlayer = true;

            // Insert the prepared GameObject into the final result of this method.
            gameObject.Set(reaper);
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            // Leviathans don't have sprites. Will need a custom solution.
            return SpriteManager.Get(TechType.Aquarium);
        }
    }
}