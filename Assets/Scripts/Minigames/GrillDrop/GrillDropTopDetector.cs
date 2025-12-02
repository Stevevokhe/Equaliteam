using UnityEngine;

public class GrillDropTopDetector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Cloth"))
        {
            GameObject.FindAnyObjectByType<GrilDropManager>().TopConnected();
        }

    }
}
