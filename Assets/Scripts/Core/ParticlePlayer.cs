using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    private ParticleSystem[] _particles;

    // ----

    private void Awake()
    {
        _particles = new ParticleSystem[transform.childCount];
        _particles = GetComponentsInChildren<ParticleSystem>();
    }

    // ----

    public void Play()
    {
        foreach (var particle in _particles)
        {
            if (particle.isPlaying) particle.Stop();
            particle.Play();
        }
    }
}
