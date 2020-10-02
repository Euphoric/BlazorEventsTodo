using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace BlazorEventsTodo.EventStorage
{
    public class DomainEventListenerWrapper<TTarget> : IDomainEventListener
    {
        private TTarget _target;
        private ImmutableList<(Type, MethodInfo)> _supportedEvents;

        public DomainEventListenerWrapper(TTarget target)
        {
            _target = target;
            _supportedEvents = typeof(TTarget).GetInterfaces()
                .Where(x => x.GetGenericTypeDefinition() == typeof(IDomainEventListener<>))
                .Select(x => x.GetMethod("Handle"))
                .Select(x => (x.GetParameters()[0].ParameterType, x))
                .ToImmutableList();
        }

        public void Handle(IDomainEventContainer<IDomainEvent> evntContainer)
        {
            foreach (var (eventType, methodInfo) in _supportedEvents)
            {
                if (eventType.IsAssignableFrom(evntContainer.GetType()))
                {
                    methodInfo.Invoke(_target, new object[] { evntContainer });
                }
            }
        }
    }
}
