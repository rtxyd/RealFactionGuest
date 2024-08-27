using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EventController_rQP
{
    public class TmpLabel
    {
        public int index { get; set; }

        public TmpLabel()
        {
            index = default;
        }
        public TmpLabel(int index)
        {
            this.index = index;
        }
        public TmpLabel(int index, out List<Label> labels, int extraLabelNum = 0)
        {
            labels = new List<Label>();
            if (index + extraLabelNum < 0)
            {
                return;
            }
            else
            {
                DynamicMethod d = new DynamicMethod("temp", typeof(void), null);
                var il = d.GetILGenerator();
                for (int i = 0; i < index + extraLabelNum + 1; i++)
                {
                    labels.Add(il.DefineLabel());
                }
            }
        }
    }
}
