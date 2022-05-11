using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock.Entities.Base
{
    public class AuditableRecord<T> : AggregateRoot<T>
    {
        public T CreatedBy { get; set; }
        public T ModifiedBy { get; set; }

        public DateTime AtCreated { get; set; } = DateTime.Now;
        public DateTime AtUpdated { get; set; }
    }
}
