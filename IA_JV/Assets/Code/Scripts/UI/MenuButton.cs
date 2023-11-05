using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI theText;
    [SerializeField] private float coefLighten = 1.2f;

    private Color defaultColor;
    private AudioSource hoverSound;

    private void Start()
    {
        defaultColor = theText.color;
        hoverSound = Camera.main.transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = defaultColor * coefLighten;
        hoverSound.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = defaultColor; 
    }
}
