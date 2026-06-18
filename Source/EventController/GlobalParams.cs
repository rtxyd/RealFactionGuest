using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventController_rQP
{
    public static class GlobalParams
    {
        [ThreadStatic]
        private static IncidentDef incidentGiveQuestCache = null;
        public static IncidentDef GetCachedIncidentAndReset()
        {
            var temp = incidentGiveQuestCache;
            incidentGiveQuestCache = null;
            return temp;
        }
        internal static void SetCachedIncident(IncidentDef incident)
        {
            incidentGiveQuestCache = incident;
        }
    }
}
