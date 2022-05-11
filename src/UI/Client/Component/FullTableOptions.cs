using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketCrud.UI.Client.Component
{
    public class FullTableOptions
    {

        public ActionLevel[] Level { get; set; } = new[] { ActionLevel.All };

        public bool IsReadOnly => this.Level.Length < 1;
    }

    public enum ActionLevel
    {
        Add,
        Edit,
        Delete,
        All
    }
}
