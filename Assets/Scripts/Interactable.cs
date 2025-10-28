using Unity.VisualScripting;
using UnityEngine;
[RequireComponent (typeof(BoxCollider))]
public class Interactable : MonoBehaviour 
{
    bool playerInZone;
    [SerializeField]
    GameObject interactionGUI,interactionText;
    private PlayerController playerController;
    private HazardBase thisHazard;

    private void Start()
    {
        thisHazard = GetComponent<HazardBase>();


        interactionText = GameObject.Find("GameCanvas");
        interactionText = interactionText.transform.Find("InteractText").gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && playerInZone && thisHazard.CheckToolRequirement(playerController.GetCurrentTool()))
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
            playerController.SetInteractedHazard(GetComponent<HazardBase>());
            playerController.SetHazardRangeBool(true);
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
            playerController.SetHazardRangeBool(false);
            playerController.ResetInteractionHazard();
        }
    }
}
