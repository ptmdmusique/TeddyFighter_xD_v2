using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class StaticGlobal {
    public static Transform GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }
}
