using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleExec : ExperimentBehaviour
{
    [Header("Static")]
    public List<GameObject> existingParticles;

    [Header("Unity Configuration")]
    public GameObject particlePrefab;

    public void SpawnParticle(string substance, Transform transform)
    {
        var clone = Instantiate(particlePrefab, transform.position, transform.rotation);
        clone.GetComponent<Particles>().Init(this, substance);
        existingParticles.Add(clone);
    }

    public void SpawnParticles(ExperimentExec exp)
    {
        foreach (KeyValuePair<string, int> particle in exp.particleInitCounts) {
            for (int i = 0; i < particle.Value; i++) {
                var boundLeft = -exp.chamberSize.x + Particles.elementSizes.Values.Max();
                var boundRight = exp.chamberSize.x - Particles.elementSizes.Values.Max();
                var boundLower = -exp.chamberSize.y + Particles.elementSizes.Values.Max();
                var boundUpper = exp.chamberSize.y - Particles.elementSizes.Values.Max();

                transform.position = new Vector3(
                    Random.Range(boundLeft, boundRight),
                    Random.Range(boundLower, boundUpper),
                    0);
                    
                SpawnParticle(particle.Key, transform);
            }
        }
    }

    public void DestroyAll()
    {
        foreach (var particle in existingParticles) {
            Destroy(particle);
        }
    }
}
