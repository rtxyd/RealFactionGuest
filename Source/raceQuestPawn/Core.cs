using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace raceQuestPawn;

[StaticConstructorOnStartup]
public static class Core
{
    public static readonly float vanillaRatio;

    static Core()
    {
        //Log.Message("# Real Faction Guest - Init");
        new Harmony("raceQuestPawn").PatchAll(Assembly.GetExecutingAssembly());

        int humanlikeModFactionNum = 0;
        var factions =
            from faction in DefDatabase<FactionDef>.AllDefs
            where
                faction.modContentPack is { PackageId: not null }
                && !faction.modContentPack.PackageId.Contains("ludeon")
                && !faction.modContentPack.PackageId.Contains("ogliss.alienvspredator")
                && !faction.modContentPack.PackageId.Contains("Kompadt.Warhammer.Dryad")
            select faction;
        try
        {
            //this may boost the loading time on game starting
            humanlikeModFactionNum = factions.
                Where(f => f.pawnGroupMakers != null).
                Count(t => t.pawnGroupMakers.Where(t => t.options.Select(t => t.kind.RaceProps).Where(t => t == null || t.intelligence == Intelligence.Humanlike || t.Humanlike).Any()).Any());
        }
        catch
        { }
        vanillaRatio = 3f / (humanlikeModFactionNum + 3f);

        //Log.Message($"humanlikeModFactionNum : {humanlikeModFactionNum}");
        //Log.Message($"vanillaRatio : {vanillaRatio}");

        Log.Message("# Real Faction Guest - Initiation Complete");
    }
}