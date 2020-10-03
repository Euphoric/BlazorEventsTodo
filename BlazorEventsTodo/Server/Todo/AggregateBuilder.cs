using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazorEventsTodo.Todo
{
    public record AggregateBuilder<TAggregate>
        where TAggregate : class
    {
        private static MethodInfo _initializeMethod;
        private static MethodInfo _updateMethod;

        static AggregateBuilder()
        {
            var aggrType = typeof(TAggregate);
            _initializeMethod = aggrType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            if (_initializeMethod == null)
            {
                throw new Exception("Aggreegate must have static Initialize method.");
            }
            _updateMethod = aggrType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
            if (_updateMethod == null)
            {
                throw new Exception("Aggreegate must have Update method.");
            }
        }

        public TAggregate Aggregate = null;

        public AggregateBuilder<TAggregate> Apply(IDomainEvent<IDomainEventData> evnt)
        {
            TAggregate newAggr;
            if (Aggregate == null)
            {
                newAggr = (TAggregate)_initializeMethod.Invoke(null, new object[] { evnt });
            }
            else
            {
                newAggr = (TAggregate)_updateMethod.Invoke(Aggregate, new object[] { evnt });
            }
            return this with { Aggregate = newAggr };
        }

        public AggregateBuilder<TAggregate> Apply(IEnumerable<IDomainEvent<IDomainEventData>> events)
        {
            return events.Aggregate(this, (builder, evnt) => builder.Apply(evnt));
        }

        public static TAggregate Rehydrate(IEnumerable<IDomainEvent<IDomainEventData>> events)
        {
            return new AggregateBuilder<TAggregate>().Apply(events).Aggregate;
        }
    }
}
