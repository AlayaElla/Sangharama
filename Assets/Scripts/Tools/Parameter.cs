using UnityEngine;
using System.Collections;

public class Parameter{

    public struct Box
    {
        public int ID;
        public object obj;
        public object subobj;
        public OnClickInScorll.ParameterDelegate callback;
        public EventTriggerListener.ParameterDelegate callbackByEvent;
    }
}
