using EventController_rQP;
using HarmonyLib;
using System.Reflection;
using Verse;

namespace raceQuestPawn;

[StaticConstructorOnStartup]
public static class Core
{
    public static readonly float vanillaRatio;
    public static readonly bool strictRace = new RealFactionGuestSettings().strictRace;
    static Core()
    {
        //Log.Message("# Real Faction Guest - Init");
        new Harmony("raceQuestPawn").PatchAll(Assembly.GetExecutingAssembly());
        vanillaRatio = 3f / (EventController_Work.GetHumanlikeModFactionNum() + 3f);
        Log.Message("# Real Faction Guest - Initiation Complete");
    }
}