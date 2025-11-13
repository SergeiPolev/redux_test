using System.Collections;
using UnityEngine;

namespace Tools
{
    public sealed class Coroutines : MonoBehaviour
    {
        private static Coroutines _instanse;

        private static Coroutines instanse
        {
            get
            {
                if (_instanse == null)
                {
                    var go = new GameObject("[COROUTINE MANAGER]");
                    _instanse = go.AddComponent<Coroutines>();
                    DontDestroyOnLoad(go);
                }

                return _instanse;
            }
        }

        public static Coroutine StartRoutine(IEnumerator enumerator)
        {
            return instanse.StartCoroutine(enumerator);
        }

        public static void StopRoutine(Coroutine routine)
        {
            if (routine != null)
            {
                instanse.StopCoroutine(routine);
            }
        }
    }
}