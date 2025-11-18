using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultyLightBulbNewBulb : MonoBehaviour, IDragHandler
{
    public bool canBeDragged;
    public void OnDrag(PointerEventData eventData)
    {
        if(canBeDragged)
        transform.position = Input.mousePosition;
    }

    public void DraggedToPosition(Transform position)
    {
        canBeDragged = false;
        transform.position = position.position;
    }
}
