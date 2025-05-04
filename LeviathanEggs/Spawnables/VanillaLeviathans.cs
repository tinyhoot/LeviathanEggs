using HootLib;
using Nautilus.Handlers;

namespace LeviathanEggs.Spawnables
{
    internal static class VanillaLeviathans
    {
        /// <summary>
        /// Add extra info to the reaper leviathan so it'll look right while in an inventory.
        /// </summary>
        public static void PatchReaper()
        {
            // This also influences the number you can keep in WaterParks! They use the x dimension as capacity.
            // A 5x4 creature takes up 5 capacity.
            CraftDataHandler.SetItemSize(TechType.ReaperLeviathan, 4, 4);
            
            Atlas.Sprite sprite = Hootils.LoadSprite("reaper_sprite.png");
            SpriteHandler.RegisterSprite(TechType.ReaperLeviathan, sprite);
            LanguageHandler.SetTechTypeName(TechType.ReaperLeviathan, "Reaper Leviathan");
            LanguageHandler.SetTechTypeTooltip(TechType.ReaperLeviathan, "It twitches when you move.");
        }
    }
}