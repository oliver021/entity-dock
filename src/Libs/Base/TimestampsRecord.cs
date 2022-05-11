using System;

namespace EntityDock.Entities.Base
{
    public class TimestampsRecord<T> : AggregateRoot<T>
    {
        public DateTime AtCreated { get; set; } = DateTime.Now;
        public DateTime AtUpdated { get; set; }
    }
}
