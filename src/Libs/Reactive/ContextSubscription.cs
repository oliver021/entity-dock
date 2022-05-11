using System;

namespace EntityDock.Reactive
{
    public class ContextSubscription
    {
        public string EntityName { get; set; }
        public string[] Records { get; set; }

        public string IdentifierConnection { get; set; }
    }
}
