using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : ExperimentBehaviour
{
    [Header("Static")]
    public static readonly Dictionary<string, float> elementSizes = new() {
        {"Si", 1.0f},
        {"O", 0.9f},
        {"X", 0.8f},
        {"C", 0.7f}
    };
    public static readonly Dictionary<string, Color> elementColours = new() {
        {"Si", new Color(1f, 1f, 1f)},
        {"O", new Color(1f, 0f, 0f)},
        {"X", new Color(0f, 0f, 1f)},
        {"C", new Color(0.2f, 0.2f, 0.2f)}
    };

    [Header("Unity Configuration")]
    public Rigidbody2D rigidBody;
    public ParticleExec particleExec;

    [Header("Particles Configuration")]
    public float rootSize;
    public float apexInitVelocity;

    [Header("Attributes")]
    public string element;

    public float size {
        get => transform.localScale.x;
        set {
            float s = rootSize * value;
            transform.localScale = new Vector3(s, s, s);
        }
    }

    public void Init(ParticleExec exec, string elem)
    {
        particleExec = exec;
        element = elem;

        size = elementSizes[element];
        gameObject.GetComponent<SpriteRenderer>().color = elementColours[element];

        rigidBody.velocity = Random.insideUnitCircle * apexInitVelocity;

        Debug.Log($"Spawned {this.element}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Particle(Clone)") {
            Debug.Log("STOPPING");
            return;
        }
        if (!collision.gameObject.GetComponent<Particles>().isActiveAndEnabled) return;

        exp.liveData["collisions"]++;
        exp.TESTING_collisions++;

        var that = collision.gameObject;
        var thatScript = that.GetComponent<Particles>();

        float threshold;
        var reactants = (element, thatScript.element);
        bool canReact = exp.bondingProspects.TryGetValue(reactants, out threshold);
        if (!canReact) return;

        if (Random.value < threshold) {
            exp.liveData[$"particles.{element}"]--;
            exp.liveData[$"particles.{thatScript.element}"]--;

            transform.position = (transform.position + that.transform.position) / 2;

            // particleExec.GetComponent<ParticleExec>()
            //     .SpawnParticle("", transform);

            Destroy(that);
            Destroy(this);

            Debug.Log("ANNIHILATION");
        }
    }
}
