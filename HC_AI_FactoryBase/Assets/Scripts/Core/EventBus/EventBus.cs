using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EventBus : IInitializable, IDisposable
{
    private readonly Dictionary<Type, List<object>> _eventHandlers = new Dictionary<Type, List<object>>();
    private readonly Queue<IEvent> _eventQueue = new Queue<IEvent>();
    private bool _isProcessingEvents = false;

    public void Initialize()
    {
        Debug.Log("EventBus initialized");
    }

    public void Dispose()
    {
        _eventHandlers.Clear();
        _eventQueue.Clear();
        Debug.Log("EventBus disposed");
    }

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (!_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = new List<object>();
        }
        
        _eventHandlers[eventType].Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType].Remove(handler);
            
            if (_eventHandlers[eventType].Count == 0)
            {
                _eventHandlers.Remove(eventType);
            }
        }
    }

    public void Publish<T>(T eventData) where T : IEvent
    {
        if (_isProcessingEvents)
        {
            _eventQueue.Enqueue(eventData);
            return;
        }

        PublishImmediate(eventData);
        ProcessQueuedEvents();
    }

    private void PublishImmediate<T>(T eventData) where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            _isProcessingEvents = true;
            
            var handlers = new List<object>(_eventHandlers[eventType]);
            
            foreach (var handler in handlers)
            {
                try
                {
                    ((Action<T>)handler)?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error handling event {eventType.Name}: {e.Message}");
                }
            }
            
            _isProcessingEvents = false;
        }
    }

    private void ProcessQueuedEvents()
    {
        while (_eventQueue.Count > 0)
        {
            var queuedEvent = _eventQueue.Dequeue();
            var eventType = queuedEvent.GetType();
            
            if (_eventHandlers.ContainsKey(eventType))
            {
                var method = typeof(EventBus).GetMethod(nameof(PublishImmediate), 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var genericMethod = method.MakeGenericMethod(eventType);
                genericMethod.Invoke(this, new object[] { queuedEvent });
            }
        }
    }

    public int GetSubscriberCount<T>() where T : IEvent
    {
        Type eventType = typeof(T);
        return _eventHandlers.ContainsKey(eventType) ? _eventHandlers[eventType].Count : 0;
    }

    public void ClearAllSubscriptions()
    {
        _eventHandlers.Clear();
        Debug.Log("All event subscriptions cleared");
    }
}