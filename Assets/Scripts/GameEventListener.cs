using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class GameEventListener : MonoBehaviour
    {
        #region Member Variables
        [SerializeField] GameEvent _gameEvent;
        [SerializeField] UnityEvent _unityEvent;
        #endregion

        void Awake()
        {
            if (_gameEvent)
            {
                Debug.Log("[GameEventListener] Registering event: " + _gameEvent.name);
                _gameEvent.Register(this);
            }
        }

        void OnDestroy()
        {
            if (_gameEvent)
            {
                Debug.Log("[GameEventListener] Unregistering event" + _gameEvent.name);
                _gameEvent.Deregister(this);
            }
        }

        public void RaiseEvent() => _unityEvent.Invoke();
    }
}
