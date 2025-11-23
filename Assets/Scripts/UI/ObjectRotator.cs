using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private Transform objectToRotate;
    [SerializeField] private Vector3 _rotation;

    void Update()
    {
        RotateMap();
    }

    private void RotateMap()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
