using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventController_rQP
{
    public class Dialogue_Changelog : Window
    {
        private ChangelogDef def;

        private int line;

        private static Vector2 scrollPosition;

        public override Vector2 InitialSize => new Vector2(900f, 700f);

        public Dialogue_Changelog(ChangelogDef loaddef)
        {
            forcePause = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnClickedOutside = false;
            def = loaddef;
            line = CountEnter(def.description);
        }
        public override void DoWindowContents(Rect inRect)
        {
            Rect rect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + (float)line * 20f);
            Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 80f);
            Listing_Standard listing_Standard = new Listing_Standard();
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
            listing_Standard.maxOneColumn = true;
            listing_Standard.ColumnWidth = rect.width / 1.1f;
            listing_Standard.Begin(rect);
            listing_Standard.Gap(10f);
            Text.Font = GameFont.Medium;
            listing_Standard.Label(def.label);
            Text.Font = GameFont.Small;
            listing_Standard.GapLine();
            listing_Standard.Gap(10f);
            listing_Standard.Label(def.description);
            listing_Standard.End();
            Widgets.EndScrollView();
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.End();
        }
        private static int CountEnter(string desc)
        {
            return desc.Count((char c) => c == '\n') + 1;
        }
    }
}
