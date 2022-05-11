using System;

namespace EntityDock.Persistence
{
    public class InsertionResult<TResult>
    {
        public int WritesRecords { get; internal set; }

        public bool Stored => WritesRecords > 0;

        public TResult Result { get; set; }
        public Exception Error { get; internal set; } = null;

        public bool HasError => Error != null;
    }
}