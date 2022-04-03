using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

    private Dictionary <Event, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance {
        get {
            if (!eventManager) {
                eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

                if (!eventManager) {
                    Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
                } else {
                    eventManager.Init(); 
                }
            }

            return eventManager;
        }
    }

    void Init () {
        if (eventDictionary == null) {
            eventDictionary = new Dictionary<Event, UnityEvent>();
        }
    }

    public static void StartListening (Event @event, UnityAction listener) {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (@event, out thisEvent)) {
            thisEvent.AddListener (listener);
        } 
        else {
            thisEvent = new UnityEvent ();
            thisEvent.AddListener (listener);
            instance.eventDictionary.Add (@event, thisEvent);
        }
    }

    public static void StopListening (Event @event, UnityAction listener) {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (@event, out thisEvent)) {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (Event @event) {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (@event, out thisEvent)) {
            thisEvent.Invoke ();
        }
    }

    public enum Event {
        RocketHit,
        PreExplosion,
        Launching,
        RocketExplode,
        Failed,
        Succeeded
    }
}