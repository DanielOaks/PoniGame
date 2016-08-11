﻿using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private float m_maxHealth = 100;

    private float m_currentHealth;
    private bool m_resolvedDeath = false;

    public delegate void DeathHandler();
    public event DeathHandler OnDie;

    public delegate void HurtHandler();
    public event HurtHandler OnHurt;

    private void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.K))
        {
            ApplyDamage(m_maxHealth);
        }

        if (!IsAlive && !m_resolvedDeath)
        {
            OnDie();
            m_resolvedDeath = true;
        }
    }

    public bool IsAlive
    {
        get { return m_currentHealth > 0; }
    }

    public float CurrentHealth
    {
        get { return m_currentHealth; }
    }

    public float MaxHealth
    {
        get { return m_maxHealth; }
    }

    public float HealthFraction
    {
        get { return m_currentHealth / m_maxHealth; }
    }

    public void ApplyDamage(float damage)
    {
        if (!IsAlive)
        {
            return;
        }
        m_currentHealth = Mathf.Max(m_currentHealth - damage, 0);
        if (OnHurt != null)
        {
            OnHurt();
        }
    }

    public void Heal(float healthGained)
    {
        if (!IsAlive)
        {
            return;
        }
        m_currentHealth = Mathf.Min(m_currentHealth + healthGained, m_maxHealth);
    }
}
