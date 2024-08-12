using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace EventController_rQP
{
    public class RealFactionGuestSettings : ModSettings
    {
        /// <summary>
        /// The three settings our mod has.
        /// </summary>
        public static bool strictRace;
        public static bool strictQuestGuest;
        public static bool hasFaction;
        public static float strictChance;
        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref strictRace, "strictRace", true);
            Scribe_Values.Look(ref strictQuestGuest, "strictQuestGuest", true);
            Scribe_Values.Look(ref hasFaction, "hasFaction", false);
            Scribe_Values.Look(ref strictChance, "guestChance", 0.8f);
            base.ExposeData();
        }
    }


}