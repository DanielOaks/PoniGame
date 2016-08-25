﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class TSTelekinesis : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_lineOfSightBlocking;

    [SerializeField]
    [Range(0, 1)]
    private float m_minGrabCenterness = 0.98f;

    [SerializeField]
    [Range(0, 20)]
    private float m_maxGrabRange = 10f;

    [SerializeField]
    [Range(0, 20)]
    private float m_loseRange = 11f;

    [SerializeField]
    [Range(0, 2)]
    private float m_minDistance = 1f;

    [SerializeField]
    [Range(0, 20)]
    private float m_velocityScale = 4f;

    [SerializeField]
    [Range(0, 20)]
    private float m_maxVelocity = 6f;

    [SerializeField]
    [Range(0, 16)]
    private float m_velocitySmoothing = 4f;

    [SerializeField]
    [Range(0, 10)]
    private float m_bobFrequency = 1.0f;

    [SerializeField]
    [Range(0, 5)]
    private float m_bobStrength = 1.0f;

    [SerializeField]
    [Range(0, 2)]
    private float m_gravityOffset = 1f;

    [SerializeField]
    [Range(0, 1)]
    private float m_angVelocityDamp = 0.05f;


    private TSMagic m_magic;

    private TKObject m_tkTarget;
    private float m_distance;

    private void Start()
    {
        m_magic = GetComponent<TSMagic>();
    }

    private void FixedUpdate()
    {
        if (Controls.JustDown(GameButton.Telekinesis))
        {
            TKObject newTarget = (m_tkTarget == null) ? FindTKTarget() : null;
            if (newTarget != null)
            {
                newTarget.Active = true;
                newTarget.SetColor(m_magic.MagicColor);
                m_distance = Mathf.Max(Vector3.Distance(newTarget.transform.position, transform.position), m_minDistance);
            }
            else if (m_tkTarget != null)
            {
                m_tkTarget.Active = false;
            }
            m_tkTarget = newTarget;
        }

        if (m_tkTarget != null && (Vector3.Distance(transform.position, m_tkTarget.transform.position) > m_loseRange || !m_magic.CanUseMagic))
        {
            m_tkTarget.Active = false;
            m_tkTarget = null;
        }

        if (m_tkTarget != null)
        {
            Transform cam = Camera.main.transform;
            float camToPlayerDistance = Vector3.Dot(cam.forward, (transform.position - cam.position));
            Vector3 targetPos = cam.position + (camToPlayerDistance + m_distance) * cam.forward;
            Vector3 spherePos = m_distance * (targetPos - transform.position).normalized + transform.position;
            Vector3 velocity = m_velocityScale * (spherePos - m_tkTarget.transform.position);
            Vector3 bobVelocity = m_bobStrength * Mathf.Sin(Time.time * m_bobFrequency) * Vector3.up;
            Vector3 targetVelocity = Vector3.ClampMagnitude(velocity + bobVelocity, m_maxVelocity) + ((m_gravityOffset / m_velocitySmoothing) * -Physics.gravity);
            m_tkTarget.Rigidbody.velocity = Vector3.Lerp(m_tkTarget.Rigidbody.velocity, targetVelocity, m_velocitySmoothing * Time.deltaTime);
            m_tkTarget.Rigidbody.angularVelocity = (1 - m_angVelocityDamp) * m_tkTarget.Rigidbody.angularVelocity;
        }
    }

    private TKObject FindTKTarget()
    {
        Transform cam = Camera.main.transform;
        TKObject mostSuitable = null;
        float bestSuitability = m_minGrabCenterness;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Telekinesis"))
        {
            TKObject tkObject = go.GetComponent<TKObject>();
            Vector3 dir = (go.transform.position - cam.position).normalized;
            float suitability = Vector3.Dot(cam.forward, dir);
            float distance = Vector3.Distance(go.transform.position, transform.position);
            if (suitability > bestSuitability && distance < m_maxGrabRange && Physics.Linecast(cam.position, go.transform.position, m_lineOfSightBlocking))
            {
                mostSuitable = tkObject;
                bestSuitability = suitability;
            }
        }
        return mostSuitable;
    }
}