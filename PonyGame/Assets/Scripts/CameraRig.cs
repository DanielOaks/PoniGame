﻿using UnityEngine;
using System.Collections;

public class CameraRig : MonoBehaviour
{
    public Transform posTarget;
    public Transform rotTarget;

    [Tooltip("How fast the character may look around horizontally (No Units)")]
    [Range(1.0f, 10.0f)]
    public float lookXSensitivity = 4.0f;

    [Tooltip("How fast the character may look around vertically (No Units)")]
    [Range(1.0f, 10.0f)]
    public float lookYSensitivity = 2.0f;

    [Tooltip("Max amount the character may look around horizontally (Degrees / Second)")]
    [Range(1.0f, 30.0f)]
    public float lookXRateCap = 10.0f;

    [Tooltip("Max amount the character may look around vertically in (Degrees / Second)")]
    [Range(1.0f, 30.0f)]
    public float lookYRateCap = 10.0f;

    [Tooltip("Max angle towards the upper pole the camera may be elevated (Degrees)")]
    [Range(0, 90.0f)]
    public float maxElevation = 45.0f;

    [Tooltip("Max angle towards the lower pole the camera may be depressed (Degrees)")]
    [Range(-90, 0)]
    public float minElevation = -30;

    private Transform m_player;
    private float m_elevation = 0;

    void Update()
    {
        if (!m_player)
        {
            m_player = GameObject.FindGameObjectWithTag("Player").transform;

            if (m_player)
            {
                transform.position = m_player.position;
                transform.rotation = m_player.rotation;
                Camera.main.transform.position = posTarget.position;
                Camera.main.transform.LookAt(rotTarget);
            }
        }
    }
	
	// Update is called once per frame
	void LateUpdate () 
	{
        if (m_player)
        {
            transform.position = m_player.position;

            float rotateX = Mathf.Clamp(Input.GetAxis("Mouse X") * lookXSensitivity, -lookXRateCap, lookXRateCap);
            transform.Rotate(0, rotateX, 0, Space.Self);

            m_elevation += Mathf.Clamp(-Input.GetAxis("Mouse Y") * lookYSensitivity, -lookYRateCap, lookYRateCap);
            m_elevation = Mathf.Clamp(m_elevation, minElevation, maxElevation);
            rotTarget.rotation = transform.rotation * Quaternion.Euler(m_elevation, 0, 0);

            Vector3 camPos = posTarget.position;

            RaycastHit hit;
            if (Physics.Linecast(rotTarget.position, posTarget.position, out hit))
            {
                camPos = hit.point + (rotTarget.position - hit.point).normalized * 0.1f;
            }

            Camera.main.transform.position = camPos;
            Camera.main.transform.LookAt(rotTarget);
        }
    }
}
