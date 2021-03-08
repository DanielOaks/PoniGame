using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpellWheelManager : MonoBehaviour
{
    private bool m_isOpen = false;
    private string m_lastSpellSelected;
    private Vector2 m_centreVector;
    private Dictionary<string,Vector2> m_spellVectorsNorm = new Dictionary<string,Vector2>();
    private Dictionary<string,GameObject> m_spellGameObjects = new Dictionary<string,GameObject>();
    private TSMagic m_magic;
    private Vector2 m_lastVectorNorm;
    [SerializeField]
    [Tooltip("Where does the spell wheel stop counting input? This helps us throw out garbage mouse input at low mouse speeds. Generally falls within 0.1 to 0.9")]
    [Min(0)]
    private float directionDeadzone = 0.1F;

    void Start ()
    {
        m_magic = FindObjectOfType(typeof(TSMagic)) as TSMagic;
        m_centreVector = new Vector2(transform.position.x, transform.position.y);
        foreach (GameObject entityGO in GameObject.FindGameObjectsWithTag("SpellIcon")) {
            SpellWheelSpellIcon entity = entityGO.GetComponent(typeof(SpellWheelSpellIcon)) as SpellWheelSpellIcon;
            m_spellVectorsNorm[entity.SpellName] = (new Vector2(entityGO.transform.position.x, entityGO.transform.position.y) - m_centreVector).normalized;
            m_spellGameObjects[entity.SpellName] = entityGO;
        }
    }
	
	void Update ()
    {
        // m_image.enabled = !GameController.IsGameOver && MainUI.IsSpellWheelOpen;
        bool newIsOpen = !GameController.IsGameOver && MainUI.IsSpellWheelOpen;
        if (!m_isOpen && newIsOpen) {
            // just opened
            foreach (GameObject entityGO in GameObject.FindGameObjectsWithTag("SpellIcon")) {
                SpellWheelSpellIcon entity = entityGO.GetComponent(typeof(SpellWheelSpellIcon)) as SpellWheelSpellIcon;
                Image entityImage = entityGO.GetComponent(typeof(Image)) as Image;
                entityImage.sprite = entity.DeactiveIcon;
                if (m_magic.CurrentSpell == entity.SpellName) {
                    m_lastVectorNorm = m_spellVectorsNorm[entity.SpellName];
                }
            }
        } else if (m_isOpen && !newIsOpen) {
            // just closed
            m_magic.CurrentSpell = m_lastSpellSelected;
        }
        m_isOpen = newIsOpen;

        // set new current spell based on latest input from mouse or controller
        if (m_isOpen) {
            // update lastVectorNorm
            Vector2 padVector = new Vector2(Input.GetAxis("R_XAxis"), -Input.GetAxis("R_YAxis"));
            Vector2 mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Vector2 newVector = padVector.magnitude > 0 ? padVector : mouseVector;
            if (newVector.magnitude > directionDeadzone) {
                m_lastVectorNorm = newVector.normalized;
            }

            // set new active spell icon
            float lowestDistance = 10; // norm. vector won't be bigger than... 2, maybe? 2.5? idk. but def not 10.
            string? lowestName = null;
            foreach (var item in m_spellVectorsNorm) {
                if (lowestName == null) {
                    lowestName = item.Key;
                }
                if (Vector2.Distance(m_lastVectorNorm, item.Value) < lowestDistance) {
                    lowestDistance = Vector2.Distance(m_lastVectorNorm, item.Value);
                    lowestName = item.Key;
                }
                // reset old active image
                Image entityImageE = m_spellGameObjects[item.Key].GetComponent(typeof(Image)) as Image;
                SpellWheelSpellIcon entityE = m_spellGameObjects[item.Key].GetComponent(typeof(SpellWheelSpellIcon)) as SpellWheelSpellIcon;
                entityImageE.sprite = entityE.DeactiveIcon;
            }
            Image entityImage = m_spellGameObjects[lowestName].GetComponent(typeof(Image)) as Image;
            SpellWheelSpellIcon entity = m_spellGameObjects[lowestName].GetComponent(typeof(SpellWheelSpellIcon)) as SpellWheelSpellIcon;
            entityImage.sprite = entity.ActiveIcon;
            m_lastSpellSelected = lowestName;
        }
    }
}
