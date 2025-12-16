using System.Collections;
using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float delaySeconds = 3f;

    private void Start()
    {
        StartCoroutine(PlayMusicAfterDelay());
    }

    private IEnumerator PlayMusicAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music source or clip not assigned!");
        }
    }
}