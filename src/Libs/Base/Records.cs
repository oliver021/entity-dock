using System;

namespace EntityDock.Entities.Base
{
    /// <summary>
    /// This is a base class record
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class BaseRecord<TId> : AggregateRoot<TId>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SmallRecord : BaseRecord<ushort> { }
    public class Record : BaseRecord<uint> { }
    public class SimpleRecord : AggregateRoot<uint> { }
    public class BiggestRecord : BaseRecord<ulong> { }
    public class UniqueRecord : BaseRecord<Guid> { }
    public class SemanticRecord : BaseRecord<string> { }
}
