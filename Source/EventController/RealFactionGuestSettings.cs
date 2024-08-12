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
        public static bool hasFaction;
        public static bool mixedquestGuest;

        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref strictRace, "strictRace", true);
            Scribe_Values.Look(ref hasFaction, "strictRace", false);
            Scribe_Values.Look(ref mixedquestGuest, "strictRace", false);
            base.ExposeData();
        }
    }


}