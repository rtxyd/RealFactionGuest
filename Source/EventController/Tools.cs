using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventController_rQP
{
    static class Tools
    {
        public static string StrFormat(this string str, string colorcode, int size = 12)
        {
            str = string.Format("<color=#{0}>{1}</color>",colorcode, str);
            str = string.Format("<size={0}>{1}</size>", size, str);
            return str;
        }
    }
}
