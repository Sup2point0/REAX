using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class ParticleExec : MonoBehaviour
{
    public Vector3[] velScaled;
    [Header("Unity Configuration")]
    public GameObject particlePrefab;

    [Header("Static Constants")]
    public float sizeSi;
    public float sizeO;
    public float sizeX;
    public float sizeC;
    public Dictionary<string, float> sizes;

    public Color colourSi;
    public Color colourO;
    public Color colourX;
    public Color colourC;
    public Dictionary<string, Color> colours;
    
    public float apexInitVelocity;
    public float apexKineticEnergy;
    
    [Header("Static Dynamic")]
    public List<GameObject> existingParticles;

    void Awake()
    {
        sizes = new() {
            { "Si", sizeSi },
            { "O", sizeO },
            { "X", sizeX },
            { "C", sizeC },
        };
        colours = new() {
            { "Si", colourSi },
            { "O", colourO },
            { "X", colourX },
            { "C", colourC },
        };
    }

    public void SpawnParticle(string substance, Transform transform, Vector3 velocity)
    {
        var clone = Instantiate(particlePrefab, transform.position, transform.rotation);
        clone.GetComponent<Particle>().Init(this, substance, velocity);
        existingParticles.Add(clone);
    }

    public void SpawnParticles(ExpExec exp)
    {
        // Assign kinetic energies for each particle
        var vels = (
            from each in Enumerable.Range(0, exp.particleInitCounts.Values.Sum())
            select Random.insideUnitSphere * apexInitVelocity);

        // E[k] = mv^2
        var totalKineticEnergy = vels.Sum(each => each.sqrMagnitude);
        // Debug.Log($"div = {apexKineticEnergy / totalKineticEnergy}");

        float scale = (float) Math.Sqrt(apexKineticEnergy / totalKineticEnergy);
        // Debug.Log($"scale = {scale}");
        velScaled = vels.Select(each => each / scale).ToArray();

        // Spawn each particle
        foreach (KeyValuePair<string, int> particle in exp.particleInitCounts) {
            for (int i = 0; i < particle.Value; i++) {
                var boundLeft = -exp.chamberSize.x + sizes.Values.Max();
                var boundRight = exp.chamberSize.x - sizes.Values.Max();
                var boundLower = -exp.chamberSize.y + sizes.Values.Max();
                var boundUpper = exp.chamberSize.y - sizes.Values.Max();

                transform.position = new Vector3(
                    Random.Range(boundLeft, boundRight),
                    Random.Range(boundLower, boundUpper),
                    0);
                    
                SpawnParticle(particle.Key, transform, velScaled[i]);
            }
        }

        Debug.Log($"total kinetic energy = {(from each in existingParticles select each.GetComponent<Particle>().rigidBody.velocity.sqrMagnitude).Sum()}");
    }

    public void DestroyAll()
    {
        foreach (var particle in existingParticles) {
            Destroy(particle);
        }
    }
}
