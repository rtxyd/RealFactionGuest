using System.Diagnostics;
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
        //private Vector2 scrollPos = Vector2.zero;
        public static Rect lastRect = default;
        public static bool test = false;

        /// <summary>
        /// A mandatory constructor which resolves the reference to our settings.
        /// </summary>
        /// <param name="content"></param>
        public RealFactionGuest(ModContentPack content) : base(content)
        {
            settings = GetSettings<RealFactionGuestSettings>();
        }
        public static Rect RectB(Listing_Standard listingStandard)
        {
            lastRect = new Rect(listingStandard.GetRect(Text.LineHeight));
            return lastRect;
        }

        //public static Rect RectA(float x, float y, float width, float height)
        //{
        //    lastRect = new Rect(x, y, width, height);
        //    return lastRect;
        //}

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            //Widgets.BeginScrollView(inRect, ref this.scrollPos, viewRect);
            listingStandard.Begin(inRect);
            Rect rectc = RectB(listingStandard);
            rectc.width = 150;
            rectc.x = listingStandard.ColumnWidth / 5 * 4;
            rectc.height = Text.LineHeight + 10;
            if (Widgets.ButtonText(rectc, "ResetAllSetting".Translate()))
            {
                RealFactionGuestSettings.ResetToDefault();
            }
            listingStandard.Gap(Text.LineHeight + 10);
            listingStandard.CheckboxLabeled("Strict quest guest".Translate(), ref RealFactionGuestSettings.strictQuestGuest, "Quest guest will generate the very pawn race of its faction as much as possible.(not xenotype, only main race concerned)".Translate());
            listingStandard.CheckboxLabeled("Alternate factions which has same pawn race".Translate(), ref RealFactionGuestSettings.alternativeFaction, "Factions with same pawn race will be alternative when generate pawns.(not xenotype, only main race concerned)".Translate());

            //Rect rectb = new Rect(listingStandard.ColumnWidth - 225, Text.LineHeight + 30, 450, Text.LineHeight + 10);
            listingStandard.Label("Toggle how often the pawn will trigger the faction-bounded generation: ".Translate() + $"{RealFactionGuestSettings.strictChance}", -1, "Main function, if you want sometimes different pawnkinds generate in different factions, you can lower this value, set to 1.0 means it will generate the very pawn race of its current faction as much as possible, the mod cannot really reach 100% for now since if there's an error when generating pawn, basegame may fix it to another pawn. (not xenotype, only main race concerned)".Translate());
            RealFactionGuestSettings.strictChance = Mathf.RoundToInt(listingStandard.Slider(RealFactionGuestSettings.strictChance, 0f, 1f) * 100f) / 100f;
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
