using EntityDock.Entities.Base;
using EntityDock.Lib.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketDemo.Data
{
    [SetRouteAttibute("assets")]
    public class MarketAsset : AggregateRoot<uint>
    {
        public uint Stock { get; set; }

        public uint Price { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
