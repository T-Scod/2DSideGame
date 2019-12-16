using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Handles the players health state
/// </summary>
[Serializable]
public class PlayerHealth
{
    /// <summary>
    /// Players health state
    /// </summary>
    public enum HealthState
    {
        ALIVE,
        DEAD
    }

    [Tooltip("Players maximum health amount")]
    [SerializeField]
    private int m_maxHealth;

    public HealthState healthState = HealthState.ALIVE;

    private int m_currentHealth;
    private PlayerController m_player;


    // Start is called before the first frame update
    void Start()
    {
        m_currentHealth = m_maxHealth;

    }

    public void Init(PlayerController player)
    {
        m_player = player;
    }

    public void PlayerState()
    {
        switch (healthState)
        {
            case HealthState.ALIVE:
                break;
            case HealthState.DEAD:
                break;
        }
    }
}