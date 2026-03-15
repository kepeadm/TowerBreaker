using System;
using System.Collections.Generic;

public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> _listeners = new();

    public static void Subscribe<T>(Action<T> listener)
    {
        var type = typeof(T);
        if (!_listeners.ContainsKey(type)) _listeners[type] = new List<Delegate>();
        _listeners[type].Add(listener);
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        if (_listeners.TryGetValue(typeof(T), out var list)) list.Remove(listener);
    }

    public static void Publish<T>(T evt)
    {
        if (_listeners.TryGetValue(typeof(T), out var list))
            foreach (var l in list) ((Action<T>)l)?.Invoke(evt);
    }
}