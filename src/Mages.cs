using System;
using System.IO;
using System.Collections.Generic;
using Pipliz;
using Pipliz.Chatting;
using Pipliz.JSON;
using Pipliz.Threading;
using Pipliz.APIProvider.Recipes;
using Pipliz.APIProvider.Jobs;
using NPC;

namespace ScarabolMods
{
  [ModLoader.ModManager]
  public static class MagesModEntries
  {
    private static string JOB_ITEM_KEY = "mods.scarabol.notenoughblocks.ColonyEmpire.altar";
    private static string AssetsDirectory;

    [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, "scarabol.mages.assemblyload")]
    public static void OnAssemblyLoaded(string path)
    {
      AssetsDirectory = Path.Combine(Path.GetDirectoryName(path), "assets");
      ModLocalizationHelper.localize(Path.Combine(AssetsDirectory, "localization"), "", false);
    }

    [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterStartup, "scarabol.mages.registercallbacks")]
    public static void AfterStartup()
    {
      Pipliz.Log.Write("Loaded Mages Mod 2.0 by Scarabol");
    }

    [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterDefiningNPCTypes, "scarabol.mages.registerjobs")]
    [ModLoader.ModCallbackProvidesFor("pipliz.apiprovider.jobs.resolvetypes")]
    public static void AfterDefiningNPCTypes()
    {
      BlockJobManagerTracker.Register<MageJob>(JOB_ITEM_KEY);
    }
  }

  public class MageJob : CraftingJobBase, IBlockJobBase, INPCTypeDefiner
  {
    public override string NPCTypeKey { get { return "scarabol.mage"; } }

    public override float TimeBetweenJobs { get { return 5f; } }

    public override int MaxRecipeCraftsPerHaul { get { return 7; } }

    public override void OnNPCDoJob (ref NPCBase.NPCState state)
    {
      state.JobIsDone = true;
      usedNPC.LookAt(position.Vector);
      if (!state.Inventory.IsEmpty) {
        usedNPC.Inventory.Dump(blockInventory);
      }
      if (recipesToCraft > 0) {
        ushort manatype = ItemTypes.IndexLookup.GetIndex("mods.scarabol.notenoughblocks.ColonyEmpire.mana");
        blockInventory.Add(new InventoryItem(manatype, 1));
        state.SetIndicator(NPCIndicatorType.Crafted, TimeBetweenJobs, manatype);
        state.JobIsDone = false;
        recipesToCraft--;
        OnRecipeCrafted();
      } else {
        recipesToCraft = 0;
        blockInventory.Dump(usedNPC.Inventory);
        shouldTakeItems = true;
        OverrideCooldown(0.1);
      }
    }

    public override void OnNPCDoStockpile (ref NPCBase.NPCState state)
    {
      state.Inventory.TryDump(usedNPC.Colony.UsedStockpile);
      state.JobIsDone = true;
      if (shouldTakeItems) {
        shouldTakeItems = false;
        recipesToCraft = MaxRecipeCraftsPerHaul;
      }
      OverrideCooldown(0.1);
    }

    NPCTypeSettings INPCTypeDefiner.GetNPCTypeDefinition ()
    {
      NPCTypeSettings def = NPCTypeSettings.Default;
      def.keyName = NPCTypeKey;
      def.printName = "Mage";
      def.maskColor1 = new UnityEngine.Color32(105, 30, 120, 255);
      def.type = NPCTypeID.GetNextID();
      return def;
    }
  }
}
