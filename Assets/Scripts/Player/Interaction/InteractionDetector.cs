using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;

    void Start()
    {
        interactionIcon.SetActive(false);    
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (PlayerSpeechController.Instance != null && PlayerSpeechController.Instance.IsSpeaking)
        {
            PlayerSpeechController.Instance.AdvanceOrClose();
            return;
        }

        if (NPC.currentNPC != null && !NPC.currentNPC.CanInteract())
        {
            NPC.currentNPC.Interact();
            return;
        }

        if (interactableInRange == null) return;

        if (interactableInRange.CanInteract())
        {
            interactableInRange.Interact();
        }
        else
        {
            interactionIcon.SetActive(false);
            interactableInRange = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
