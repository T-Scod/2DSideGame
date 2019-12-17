using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Enemy : MonoBehaviour
{
    public enum Type : int
    {
        MELEE   = 1 << 0,
        RANGE   = 1 << 1,
        SMASHER = 1 << 2,
        FLYER   = 1 << 3,

        ANY     = ~0
    }

    public static bool TypeEquals(Type a, Type b)
    {
        int r = (int)a & (int)b;
        return r != 0;
    }

    [SerializeField]
    FiniteStateMachine stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        Type a = Type.MELEE;
        Type b = Type.RANGE;

        Debug.Log(TypeEquals(a, b));
        Debug.Log(TypeEquals(a, Type.ANY));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
