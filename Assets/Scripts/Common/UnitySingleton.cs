using UnityEngine;

namespace Common
{
    public class UnitySingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var o = FindObjectOfType(typeof(T)) as T;

                if (o == null)
                {
                    var go = new GameObject
                    {
                        name = typeof(T).Name
                    };
                    _instance = go.AddComponent<T>();
                }
                else _instance = o;

                return _instance;
            }
        }
        


        protected void OnDestroy()
        {
            _instance = null;
        }
    }
}