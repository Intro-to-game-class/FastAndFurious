
using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour
{
    [SerializeField] private AudioSource a; // current
    [SerializeField] private AudioSource b; // next
    private bool usingA = true;

    private void Awake()
    {
        if (a == null) a = gameObject.AddComponent<AudioSource>();
        if (b == null) b = gameObject.AddComponent<AudioSource>();
        a.loop = b.loop = true;
        a.spatialBlend = b.spatialBlend = 0f;
    }

    public void Crossfade(AudioClip nextClip, float duration = 1.5f, float targetVolume = 1f)
    {
        StartCoroutine(CrossfadeRoutine(nextClip, duration, targetVolume));
    }

    private IEnumerator CrossfadeRoutine(AudioClip nextClip, float duration, float targetVolume)
    {
        AudioSource from = usingA ? a : b;
        AudioSource to = usingA ? b : a;

        to.clip = nextClip;
        to.volume = 0f;
        to.Play();

        float time = 0f;
        float startVol = from.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            from.volume = Mathf.Lerp(startVol, 0f, t);
            to.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        from.Stop();
        to.volume = targetVolume;
        usingA = !usingA;
    }
}
