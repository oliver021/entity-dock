using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock.Reactive
{
    public interface IDataSubscriber<T>
    {
        public Task OnRefresh(string entity);
        public Task OnAdded(string entity, T payload);
        public Task OnUpdate(string entity, object id, T payload);
        public Task OnUpdate(string entity, object id);
    }
}
