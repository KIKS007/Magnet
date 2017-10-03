using UnityEngine;
using System.Collections;

public class ParticlesAutoDestroy : MonoBehaviour
{
    public bool replayParticles = false;
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitWhile(() => ps && ps.IsAlive());

        if (!replayParticles)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
