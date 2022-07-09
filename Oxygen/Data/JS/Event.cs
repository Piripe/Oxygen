using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxygen.Data.JS
{
    internal class Event
    {
        public object target { get; }
        public DateTime timeStamp { get; }

        internal Event(object target)
        {
            this.target = target;
            this.timeStamp = DateTime.Now;
        }
    }
}
