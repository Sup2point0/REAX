using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
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

        size = exec.sizes[element];
        gameObject.GetComponent<SpriteRenderer>().color = exec.colours[element];

        rigidBody.velocity = Random.insideUnitCircle * apexInitVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (
            collision.gameObject.name != "Particle(Clone)" ||
            collision.gameObject.GetComponent<Particles>() == null ||
            !collision.gameObject.GetComponent<Particles>().isActiveAndEnabled
        ) {
            return;
        }

        ExpExec.live.liveData["collisions"]++;
        ExpExec.live.TESTING_collisions++;

        var that = collision.gameObject;
        var thatScript = that.GetComponent<Particles>();

        float threshold;
        var reactants = (element, thatScript.element);
        bool canReact = ExpExec.live.bondingProspects.TryGetValue(reactants, out threshold);
        if (!canReact) return;

        if (Random.value < threshold) {
            ExpExec.live.liveData[$"particles.{element}"]--;
            ExpExec.live.liveData[$"particles.{thatScript.element}"]--;

            transform.position = (transform.position + that.transform.position) / 2;

            // particleExec.GetComponent<ParticleExec>()
            //     .SpawnParticle("", transform);

            Destroy(this);
            Destroy(that);
            particleExec.existingParticles.Remove(gameObject);
            particleExec.existingParticles.Remove(that.gameObject);

            Debug.Log("ANNIHILATION");
        }
    }
}
