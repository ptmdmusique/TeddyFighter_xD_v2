using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AIType
{
    Simple = 0,
    Intermediate = 1,
    Complex = 2,
}

public abstract class GeneralAI : MonoBehaviour {
    public AIType myType = AIType.Simple;   //What AI type am I?

	//NOTE: CHECK AI SCHEME FOR MORE INFO OF THE STATES
}
