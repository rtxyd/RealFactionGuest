using HarmonyLib;
using RimWorld;
using System.Linq;
using System.Reflection;
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
        //TimeSpan a = new TimeSpan(DateTime.Now.Ticks);
        try
        {
            humanlikeModFactionNum = DefDatabase<FactionDef>.AllDefs.Count(f =>
            f.modContentPack is { PackageId: not null }
            && !f.modContentPack.PackageId.Contains("ludeon")
            && !f.modContentPack.PackageId.Contains("ogliss.alienvspredator")
            && !f.modContentPack.PackageId.Contains("Kompadt.Warhammer.Dryad")
            && f.pawnGroupMakers != null
            && f.pawnGroupMakers.Any(t => t.options.Any(t => t.kind.RaceProps == null || t.kind.RaceProps.intelligence == Intelligence.Humanlike || t.kind.RaceProps.Humanlike)));

            //TimeSpan b = new TimeSpan(DateTime.Now.Ticks);
            //Log.Message($"# Real Faction Guest - {a.Subtract(b).Duration()}");
        }
        catch
        { }
        vanillaRatio = 3f / (humanlikeModFactionNum + 3f);

        //Log.Message($"humanlikeModFactionNum : {humanlikeModFactionNum}");
        //Log.Message($"vanillaRatio : {vanillaRatio}");

        Log.Message("# Real Faction Guest - Initiation Complete");
    }
}