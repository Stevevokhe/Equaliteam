using UnityEngine;

public class InteractableTool : MonoBehaviour
{
    bool playerInZone;
    [SerializeField]
    GameObject interactionGUI, interactionText;
    private PlayerController playerController;
    [SerializeField] private PlayerToolSO toolSO;

    private void Start()
    {

        interactionGUI = GameObject.Find("GUI");
        interactionGUI = interactionGUI.transform.Find("InteractableUI").gameObject;

        interactionText = GameObject.Find("GameCanvas");
        interactionText = interactionText.transform.Find("InteractText").gameObject;
    }

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
            playerController.SetInteractedHazard(GetComponent<HazardBase>());
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
            playerController.ResetInteractionHazard();
        }
    }
}
