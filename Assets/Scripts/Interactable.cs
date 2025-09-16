using Unity.VisualScripting;
using UnityEngine;
[RequireComponent (typeof(BoxCollider))]
public class Interactable : MonoBehaviour 
{
    bool playerInZone;
    [SerializeField]
    GameObject interactionGUI,interactionText;
    private PlayerController playerController;


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && playerInZone)
        {
            interactionGUI.SetActive(true);
            playerController.canMove = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController = other.GetComponentInParent<PlayerController>();            
            playerInZone = true;
            interactionText.SetActive(true);            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInZone = false;
            interactionText.SetActive(false);
        }
    }
}
