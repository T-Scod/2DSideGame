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

    public void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void Update()
    {
        //PlayerState();
    }

    public void Init(PlayerController player)
    {
        m_player = player;
    }

    /// <summary>
    /// Handles what happens in each health state
    /// </summary>
    private void PlayerState()
    {
        switch (healthState)
        {
            case HealthState.ALIVE:
                break;
            case HealthState.DEAD:
                break;
        }
    }

    /// <summary>
    /// Player taking damage
    /// </summary>
    /// <param name="damage"></param>
    private void TakeDamage(int damage)
    {
        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
        {
            IsDead();
        }
    }

    /// <summary>
    /// Player dead state
    /// </summary>
    private void IsDead()
    {
        healthState = HealthState.DEAD;
    }
}