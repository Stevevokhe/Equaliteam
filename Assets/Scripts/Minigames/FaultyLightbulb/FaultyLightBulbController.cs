using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FaultyLightBulbController : MonoBehaviour, IDragHandler
{
    Vector3 originalPosition;
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}
