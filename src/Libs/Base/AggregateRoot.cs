using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock.Entities.Base
{
    public class AggregateRoot<T>
    {
        public T Id { get; set; }
    }
}
