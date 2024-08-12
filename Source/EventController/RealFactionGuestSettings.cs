using Verse;

namespace EventController_rQP
{
    public class RealFactionGuestSettings : ModSettings
    {
        /// <summary>
        /// The three settings our mod has.
        /// </summary>
        public static bool strictQuestGuest;
        public static bool alternativeFaction;
        public static float strictChance;
        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref strictQuestGuest, "strictQuestGuest", true);
            Scribe_Values.Look(ref alternativeFaction, "alternativeFaction", true);
            Scribe_Values.Look(ref strictChance, "guestChance", 1.0f);
            base.ExposeData();
        }
    }


}