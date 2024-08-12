﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventController_rQP
{
    public class RealFactionGuest : Mod
    {
        /// <summary>
        /// A reference to our settings.
        /// </summary>
        public static RealFactionGuestSettings settings;

        /// <summary>
        /// A mandatory constructor which resolves the reference to our settings.
        /// </summary>
        /// <param name="content"></param>
        public RealFactionGuest(ModContentPack content) : base(content)
        {
            settings = GetSettings<RealFactionGuestSettings>();
        }

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("[Beta]Real faction quest guest", ref RealFactionGuestSettings.strictQuestGuest, "Quest guest will generate the very pawn race of the faction as much as possible.(not xenotype, only main race concerned)");
            listingStandard.CheckboxLabeled("[Beta]Strict faction-bounded pawn race", ref RealFactionGuestSettings.strictRace, "Main function, must generate the very pawn race of the faction.(not xenotype, only main race concerned)");
            listingStandard.CheckboxLabeled("[Beta]Refugee pod crash need have a faction", ref RealFactionGuestSettings.hasFaction, "Refugee pod crash will generate pawn with a faction as much as possible");
            listingStandard.Label("[Beta]Toggle the chance of how often the pawn will trigger the 'Strict faction-bounded pawn race'");
            RealFactionGuestSettings.strictChance = listingStandard.Slider(RealFactionGuestSettings.strictChance, 0f, 1f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "RealFactionGuest".Translate();
        }
    }
}
