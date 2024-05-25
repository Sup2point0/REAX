using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleExec : MonoBehaviour
{
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

    public void SpawnParticle(string substance, Transform transform)
    {
        var clone = Instantiate(particlePrefab, transform.position, transform.rotation);
        clone.GetComponent<Particles>().Init(this, substance);
        Debug.Log($"cloned {clone}");
        existingParticles.Add(clone);
    }

    public void SpawnParticles(ExpExec exp)
    {
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
