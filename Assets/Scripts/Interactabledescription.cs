using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Interactabledescription : MonoBehaviour
{
    string[] description;
    [HideInInspector]public bool isInteracting;
    bool canTake;
    Interaction interaction;
    Inventory inventory;
    GameObject interactedObject;
    Transform originpoint;
    public float inspectRatio;
    public float duration;
    public Transform interactionPoint;
    public GameObject takeButtonUI;
    public TextMeshProUGUI objectName;
    public TextMeshProUGUI objectDescription;
    public TextAsset descriptiontxt;
    public void Update()
    {
        if (canTake)
        {
            takeButtonUI.SetActive(true);
        }
        else
        {
            takeButtonUI.SetActive(false);
        }
        if (isInteracting)
        {
            if (Vector3.Distance(interactedObject.transform.position, interactionPoint.position) > 0.01f)
            {
                interactedObject.transform.position = Vector3.Lerp(interactedObject.transform.position, interactionPoint.position, Time.deltaTime / duration);
                interactedObject.transform.rotation *= Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * inspectRatio);
            }
            else
            {
                interactedObject.transform.rotation *= Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * inspectRatio);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isInteracting = false;
                interaction.EndInterract();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && canTake)
            {
                isInteracting = false;
                interaction.EndInterract();
                inventory.NewObject(interactedObject,interactedObject.GetComponent<SpriteMask>().sprite, interaction);
                Destroy(interactedObject);
            }
        }
    }
    public void InteractionButtonHasPressed(GameObject interactedObject, bool canTake, Inventory inventory, Interaction interaction , Transform originpoint)
    {
        StartCoroutine(Interact(interactedObject, canTake, inventory, interaction , originpoint));
    }
    public IEnumerator Interact(GameObject _interactedObject, bool _canTake, Inventory _inventory, Interaction _interaction , Transform _originpoint)
    {
        interactedObject = _interactedObject;
        canTake = _canTake;
        interaction = _interaction;
        inventory = _inventory;
        originpoint = _originpoint;
        objectDescription.text = description[Array.IndexOf(description, Array.Find(description, element => element == interactedObject.name)) + 1];
        yield return new WaitForSeconds(0.1f);
        isInteracting = true;
    }
    private void OnValidate()
    {
        description = descriptiontxt ? descriptiontxt.text.Split(separator: new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries) : null;
    }
}
