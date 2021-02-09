using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellWheelBG : MonoBehaviour
{
    private Image m_image;
    
    void Start ()
    {
        m_image = GetComponent<Image>();
    }
	
	void Update ()
    {
        m_image.enabled = !GameController.IsGameOver && MainUI.IsSpellWheelOpen;
    }
}
