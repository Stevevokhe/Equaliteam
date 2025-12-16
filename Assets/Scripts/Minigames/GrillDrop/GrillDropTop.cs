using System.Collections;
using UnityEngine;

public class GrillDropTop : MonoBehaviour
{
    private bool canBeReseted=true;
    [SerializeField] private float resetTime=2;
    void OnCollisionEnter2D(Collision2D col)
    {
        if(canBeReseted)
        {
            canBeReseted = false;
            StartCoroutine(ResetTop());
        }
            
    }

    IEnumerator ResetTop()
    {

        yield return new WaitForSeconds(resetTime);
        canBeReseted = true;
        GameObject.FindAnyObjectByType<GrilDropController>().ResetGrillTop();
    }
}
