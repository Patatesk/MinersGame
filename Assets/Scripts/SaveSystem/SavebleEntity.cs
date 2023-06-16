using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Save
{
    public class SavebleEntity : MonoBehaviour
    {
        [SerializeField] string id;


        public string Id => id;


        [ContextMenu("GetID")]
        private void GenerateID()
        {
            id = Guid.NewGuid().ToString();
        }

        public object SaveState()
        {
            var state = new Dictionary<string, object>();
            foreach (var saveble in GetComponents<ISaveble>())
            {
                state[saveble.GetType().ToString()] = saveble.SaveState();
            }
            return state;
        }


        public void LoadState(object state)
        {
            var stateDictionary = (Dictionary<String, object>)state;

            foreach (var saveble in GetComponents<ISaveble>())
            {
                string typeName = saveble.GetType().ToString();
                if (stateDictionary.TryGetValue(typeName, out object savedState))
                {
                    saveble.LoadState(savedState);
                }
            }
        }
    }
}
