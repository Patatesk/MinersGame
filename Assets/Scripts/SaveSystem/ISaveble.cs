using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    public interface ISaveble
    {
        object SaveState();
        void LoadState(object state);
    }
}
