using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellWheelSpellIcon : MonoBehaviour
{
    private Image m_image;

    public string SpellName;
    public Sprite ActiveIcon;
    public Sprite DeactiveIcon;
    
    void Start ()
    {
        m_image = GetComponent<Image>();
    }
	
	void Update ()
    {
        // m_image.enabled = !GameController.IsGameOver && MainUI.IsSpellWheelOpen;
    }
}
