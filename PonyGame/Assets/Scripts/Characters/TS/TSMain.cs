﻿using UnityEngine;
using System.Collections;

public class TSMain : MonoBehaviour
{
    private TSAI m_AI;
    private TSMovement m_movement;
    private TSAnimation m_animation;
    private Health m_health;


    public bool IsPlayer
    {
        get { return gameObject.tag == "Player"; }
    }

	void Start()
    {
        m_AI = GetComponent<TSAI>();
        m_movement = GetComponent<TSMovement>();
        m_animation = GetComponent<TSAnimation>();
        m_health = GetComponent<Health>();
        
        m_health.OnDie += OnDie;

        GameController.AddCharacter(transform);
    }

    void OnDestroy()
    {
        m_health.OnDie -= OnDie;
    }

    private void OnDie()
    {
        if (IsPlayer)
        {
            GameController.PlayerDied();
        }
    }

    private void FixedUpdate()
    {
        if (Controls.JustDown(GameButton.ConsiderSuicide))
        {
            m_health.ApplyDamage(m_health.MaxHealth / 10);
        }

        if (m_health.IsAlive)
        {
            MoveInputs inputs;
            if (IsPlayer)
            {
                inputs = MoveInputs.GetPlayerInputs(transform);
            }
            else
            {
                m_AI.UpdateAI();
                inputs = m_AI.GetInputs();
            }
            m_movement.ExecuteMovement(inputs);
            m_animation.FixedUpdate();
        }
    }

    private void Update()
    {
        if (m_health.IsAlive)
        {
            m_animation.PreAnimationUpdate(IsPlayer);
        }
    }

    private void LateUpdate()
    {
        m_animation.PostAnimationUpdate(m_health.IsAlive);
    }
}