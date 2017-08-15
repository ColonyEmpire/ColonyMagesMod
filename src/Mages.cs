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
    }

    [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterStartup, "scarabol.mages.registercallbacks")]
    public static void AfterStartup()
    {
      Pipliz.Log.Write("Loaded Mages Mod 1.1 by Scarabol");
    }

    [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterDefiningNPCTypes, "scarabol.mages.registerjobs")]
    [ModLoader.ModCallbackProvidesFor("pipliz.apiprovider.jobs.resolvetypes")]
    public static void AfterDefiningNPCTypes()
    {
      BlockJobManagerTracker.Register<MageJob>(JOB_ITEM_KEY);
    }

    [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, "scarabol.mages.loadrecipes")]
    [ModLoader.ModCallbackProvidesFor("pipliz.apiprovider.registerrecipes")]
    public static void AfterItemTypesDefined()
    {
      RecipeManager.LoadRecipes("scarabol.mage", Path.Combine(AssetsDirectory, "craftingmagic.json"));
    }
  }

  public class MageJob : CraftingJobBase, IBlockJobBase, INPCTypeDefiner
  {
    public override string NPCTypeKey { get { return "scarabol.mage"; } }

    public override float TimeBetweenJobs { get { return 5f; } }

    public override int MaxRecipeCraftsPerHaul { get { return 7; } }

    // TOOD add job tool?
//    public override InventoryItem RecruitementItem { get { return new InventoryItem(ItemTypes.IndexLookup.GetIndex("mods.scarabol.construction.buildtool"), 1); } }

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
