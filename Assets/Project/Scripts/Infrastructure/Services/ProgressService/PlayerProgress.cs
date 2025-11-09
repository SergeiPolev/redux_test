using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class PlayerProgress
    {
        [System.Serializable]
        private class Element
        {
            public string ID;
            public string JSON;
        }
        private List<Element> _elements;

        public PlayerProgress()
        {
            _elements = new List<Element>();
        }
        
        public bool GetSaveData<T>(string ID, out T data) where T : class
        {
            data = null;
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].ID == ID)
                {
                    data = JsonUtility.FromJson<T>(_elements[i].JSON);
                    return true;
                }
            }
            return false;
        }

        public void SetData<T>(string ID, T data) where T : class
        {
            var json = JsonUtility.ToJson(data);
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].ID == ID)
                {
                    _elements[i].JSON = json;
                    return;
                }
            }

            _elements.Add(new Element { ID = ID, JSON = json });
            return;
        }

        public bool IsEmpty()
        {
            return _elements == null || _elements.Count == 0;
        }
    }
    
}
