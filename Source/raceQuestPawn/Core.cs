using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace raceQuestPawn;

[StaticConstructorOnStartup]
public static class Core
{
    public static readonly float VanillaRatio;

    static Core()
    {
        new Harmony("raceQuestPawn").PatchAll(Assembly.GetExecutingAssembly());

        var humanlikeModFactionNum = 0;

        foreach (var f in from faction in DefDatabase<FactionDef>.AllDefs
                 where
                     faction.modContentPack is { PackageId: not null }
                     && !faction.modContentPack.PackageId.Contains("ludeon")
                     && !faction.modContentPack.PackageId.Contains("ogliss.alienvspredator")
                     && !faction.modContentPack.PackageId.Contains("Kompadt.Warhammer.Dryad")
                 select faction)
        {
            try
            {
                var isHumanlike = false;
                if (f.pawnGroupMakers == null)
                {
                    continue;
                }

                foreach (var pawnGroupMaker in f.pawnGroupMakers)
                {
                    var options = pawnGroupMaker.options;
                    foreach (var pawnGenOption in options)
                    {
                        if (pawnGenOption.kind.RaceProps == null ||
                            pawnGenOption.kind.RaceProps.intelligence == Intelligence.Humanlike ||
                            pawnGenOption.kind.RaceProps.Humanlike)
                        {
                            isHumanlike = true;
                        }
                    }
                }

                if (!isHumanlike)
                {
                    continue;
                }

                humanlikeModFactionNum++;
            }
            catch
            {
                // ignored
            }
        }

        VanillaRatio = 3f / (humanlikeModFactionNum + 3f);
    }
}