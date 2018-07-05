using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType
{
    Simple = 0,
    Intermediate = 1,
    Complex = 2,
    Boss = 3
}

public class GeneralAI : MonoBehaviour {

    public AIType myType = AIType.Simple;   //What AI type am I?
}
