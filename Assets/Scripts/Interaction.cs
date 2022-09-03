using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Interaction : MonoBehaviour
{
    public Sprite[] points;
    public Transform canvas;
    
    [Header("Controls")]
    KeyCode inspect = KeyCode.Mouse1;
    KeyCode take = KeyCode.Mouse0;

    [HideInInspector]public bool canTake;
    GameObject inventoryInteract;
    Camera playerCamera;
    Image lookPoint;
    Interactabledescription description;
    Inventory inventory;
    Transform originPoint;
    private RaycastHit interactedObject;
    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        lookPoint = canvas.GetChild(0).GetComponent<Image>();
        description = canvas.GetChild(1).GetComponent<Interactabledescription>();
        inventory = canvas.GetChild(2).GetComponent<Inventory>();
    }
    private void Update()
    {
        if (CanInteract() && !description.isInteracting)
        {
            lookPoint.sprite = points[1];
            if (Input.GetKeyDown(inspect))
            {
                originPoint = interactedObject.transform;
                playerCamera.transform.parent.GetComponent<PlayerController>().canMove = false;
                description.transform.GetChild(0).gameObject.SetActive(true);
                description.InteractionButtonHasPressed(interactedObject.collider.gameObject, canTake, inventory, gameObject.GetComponent<Interaction>() , originPoint);
            }
            else if (Input.GetKeyDown(take) && canTake)
            {
                inventory.NewObject(interactedObject.collider.gameObject, interactedObject.collider.GetComponent<SpriteMask>().sprite, gameObject.GetComponent<Interaction>());
                interactedObject.collider.gameObject.SetActive(false);
            }
        }
        else if (InventoryInteract())
        {
            lookPoint.sprite = points[1];
        }
        else
        {
            lookPoint.sprite = points[0];
            canTake = false;
        }
    }
    public void EndInterract()
    {
        playerCamera.transform.parent.GetComponent<PlayerController>().canMove = true;
        description.transform.GetChild(0).gameObject.SetActive(false);
    }
    public bool CanInteract()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out interactedObject, 1.5f))
        {
            if (interactedObject.collider.gameObject.layer == 6) { canTake = true; return true; }
            else if (interactedObject.collider.gameObject.layer == 7)
            { canTake = false; return true; }
            else
            {
                return false;
            }
        }
        else return false;
    }
    public bool InventoryInteract()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out interactedObject, 1.5f))
        {
            if (interactedObject.collider.gameObject.layer == 8)
            {
                inventoryInteract = interactedObject.collider.gameObject;
                return true;
            }
            else return false;
        }
        else return false;
    }
    public GameObject InventoryInteractObject()
    {
        return inventoryInteract;
    }
}
