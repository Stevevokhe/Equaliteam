using System.Collections;
using UnityEngine;

public class GrillDropTop : MonoBehaviour
{
    private bool canBeReseted=true;
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

        yield return new WaitForSeconds(3);
        canBeReseted = true;
        GameObject.FindAnyObjectByType<GrilDropController>().ResetGrillTop();
    }
}
