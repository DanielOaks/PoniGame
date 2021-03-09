using UnityEngine;
using System.Collections;
using System.Linq;

public class TSTeleport : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_lineOfSightBlocking;

    [SerializeField]
    [Range(0, 2)]
    private float m_minDistance = 1f;

    [SerializeField]
    private float m_maxDistance = 100f;

    private TSMagic m_magic;
    private GameObject m_twi;

    private void Start()
    {
        m_magic = GetComponent<TSMagic>();
        m_twi = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (m_magic.CurrentSpell == "teleport" && Controls.JustDown(GameButton.TK) && !m_magic.IsUsingMagic)
        {
            // try to teleport
            Debug.Log("Trying to teleport");
            Vector3? hit = FindTeleportHit();
            if (hit != null)
            {
                m_twi.transform.position = (Vector3)hit;
            }
        }
    }

    private Vector3? FindTeleportHit()
    {
        Transform cam = Camera.main.transform;
        RaycastHit[] hits = Physics.RaycastAll(cam.position, cam.forward, m_maxDistance, m_lineOfSightBlocking);
        foreach (var hit in hits)
        {
            // only return the first hit lmao
            return hit.point;
        }
        return null;
    }
}