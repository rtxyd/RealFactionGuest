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
        private Vector2 scrollPos = Vector2.zero;
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
            //var Recta = new Rect(0f, 0f, inRect.width - 20f, 780);
            //Widgets.BeginScrollView(inRect, ref this.scrollPos, viewRect);
            listingStandard.Begin(inRect);
            listingStandard.Gap(18);
            listingStandard.CheckboxLabeled("Strict quest guest (Deafault: True)".Translate(), ref RealFactionGuestSettings.strictQuestGuest, "Quest guest will generate the very pawn race of its faction as much as possible.(not xenotype, only main race concerned)".Translate());
            listingStandard.CheckboxLabeled("Alternate factions which has same pawn race (Deafault: False)".Translate(), ref RealFactionGuestSettings.alternativeFaction, "Factions with same pawn race will be alternative. (not xenotype, only main race concerned)".Translate());
            listingStandard.Label("Toggle how often the pawn will trigger the faction-bounded generation (Default: 1.0): ".Translate() + $"{RealFactionGuestSettings.strictChance}", -1, "Main function, if you want sometimes different pawnkinds generate in different factions, you can lower this value, set to 1.0 means it will generate the very pawn race of its current faction as much as possible, the mod cannot really reach 100% for now since if there's an error when generating pawn, basegame may fix it to another pawn. (not xenotype, only main race concerned)".Translate());
            listingStandard.Gap(18);
            RealFactionGuestSettings.strictChance = Mathf.RoundToInt(listingStandard.Slider(RealFactionGuestSettings.strictChance, 0.0f, 1.0f) * 100f) / 100f;
            //Rect rectb = listingStandard.GetRect(Text.LineHeight);
            //Widgets.HorizontalSlider(rectb, RealFactionGuestSettings.strictChance, 0f, 1f);

            if (listingStandard.ButtonText("ResetAllSetting".Translate(), null))
            {
                RealFactionGuestSettings.ResetToDefault();
            }
            listingStandard.End();
            //Widgets.EndScrollView();
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
