using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]

    public class GameEvent : ScriptableObject
    {
        HashSet<GameEventListener> _listeners = new HashSet<GameEventListener>();

        public void Invoke()
        {
            Debug.Log("[Game Event Invoke] _listeners count: " + _listeners.Count);

            foreach (var globalEventListener in _listeners)
            {
                Debug.Log("Game Event Invoke: " + globalEventListener.name);
                globalEventListener.RaiseEvent();
            }
        }

        public void Register(GameEventListener gameEventListener) => _listeners.Add(gameEventListener);

        public void Deregister(GameEventListener gameEventListener) => _listeners.Remove(gameEventListener);
    }
}
