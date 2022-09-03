using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    private Scrollbar staminaBar;
    public PlayerController playerController;
    ColorBlock colorBlock;
    bool isVisible;
    public float duration;
    private void Awake()
    {
        staminaBar = GetComponentInChildren<Scrollbar>();
        colorBlock = staminaBar.colors;
    }

    void Update()
    {
        staminaBar.size = playerController.currentStamina / playerController.maxStamina;
        if (staminaBar.size == 1 || staminaBar.size < .01f)staminaBar.interactable = false;
        else staminaBar.interactable = true;
    }
}
