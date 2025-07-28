using Verse;

namespace EventController_rQP
{
    public class RealFactionGuestSettings : ModSettings
    {
        /// <summary>
        /// The three settings our mod has.
        /// </summary>
        public static bool strictQuestGuest = true;
        public static bool alternativeFaction = false;
        public static float strictChance = 1.0f;
        public static bool factionLeaderValidator = true;
        public static bool debugOption = false;
        public static bool damageUntilDownedBypassShield = true;
        public static bool dontAdjustXenotypeForRabbie = true;
        public static bool creepJoinerValidator = true;
        public static bool creepJoinerGenerateNoLimit = true;
        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref strictQuestGuest, "strictQuestGuest", true);
            Scribe_Values.Look(ref factionLeaderValidator, "factionLeaderValidator", true);
            Scribe_Values.Look(ref damageUntilDownedBypassShield, "damageUntilDownedBypassShield", true);
            Scribe_Values.Look(ref creepJoinerValidator, "creepJoinerValidator", true);
            Scribe_Values.Look(ref creepJoinerGenerateNoLimit, "creepJoinerGenerateNoLimit", true);
            if (!creepJoinerValidator)
            {
                creepJoinerGenerateNoLimit = false;
            }
            if (ModsConfig.BiotechActive && ModsConfig.IsActive("RunneLatki.RabbieRaceMod"))
            {
                Scribe_Values.Look(ref dontAdjustXenotypeForRabbie, "dontAdjustXenotypeForRabbie", true);
            }
            else
            {
                Scribe_Values.Look(ref dontAdjustXenotypeForRabbie, "dontAdjustXenotypeForRabbie", false);
            }
            Scribe_Values.Look(ref alternativeFaction, "alternativeFaction", false);
            Scribe_Values.Look(ref strictChance, "guestChance", 1.0f);
        }
        public static void ResetToDefault()
        {
            creepJoinerGenerateNoLimit = true;
            creepJoinerValidator = true;
            strictQuestGuest = true;
            damageUntilDownedBypassShield = true;
            if (ModsConfig.BiotechActive && ModsConfig.IsActive("RunneLatki.RabbieRaceMod"))
            {
                dontAdjustXenotypeForRabbie = true;
            }
            factionLeaderValidator = true;
            debugOption = false;
            alternativeFaction = false;
            strictChance = 1f;
        }
    }
}