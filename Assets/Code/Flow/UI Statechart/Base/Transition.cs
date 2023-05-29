using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace SC
{
    public abstract class StateTransition
    {
        public abstract void Execute(Statechart statechart); 
    }
}
