using System;
using System.Collections.Generic;
using System.Text;

namespace EntityDock.Reactive
{
    public class EventMessage
    {
        public string EventName { get; set; }
        public bool Passed { get; set; }
        public string PayloadSource { get; set; }
    }
}
