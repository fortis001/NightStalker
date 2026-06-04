using System;
using System.Collections.Generic;

namespace NightStalker.Core
{
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, Delegate> _eventTable = new();

        public static void Subscribe<TEvent>(Action<TEvent> handler)
        {
            Type eventType = typeof(TEvent);

            if (_eventTable.TryGetValue(eventType, out Delegate existingDelegate))
            {
                _eventTable[eventType] = Delegate.Combine(existingDelegate, handler);
            }
            else
            {
                _eventTable[eventType] = handler;
            }
        }

        public static void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            Type eventType = typeof(TEvent);

            if (!_eventTable.TryGetValue(eventType, out Delegate existingDelegate))
                return;

            Delegate currentDelegate = Delegate.Remove(existingDelegate, handler);

            if (currentDelegate == null)
            {
                _eventTable.Remove(eventType);
            }
            else
            {
                _eventTable[eventType] = currentDelegate;
            }
        }

        public static void Publish<TEvent>(TEvent eventData)
        {
            Type eventType = typeof(TEvent);

            if (!_eventTable.TryGetValue(eventType, out Delegate existingDelegate))
                return;

            if (existingDelegate is Action<TEvent> action)
            {
                action.Invoke(eventData);
            }
        }

        public static void Clear()
        {
            _eventTable.Clear();
        }
    }
}