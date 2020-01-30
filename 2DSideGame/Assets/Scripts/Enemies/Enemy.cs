using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

[RequireComponent(typeof(Collider2D))]
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

    public new Collider2D collider { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.Init(this);
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}
