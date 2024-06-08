using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Particle : MonoBehaviour
{
    [Header("Unity Configuration")]
    public Rigidbody rigidBody;
    public ParticleExec particleExec;

    [Header("Particles Configuration")]
    public float rootSize;

    [Header("Attributes")]
    public string element;

    public float size {
        get => transform.localScale.x;
        set {
            float s = rootSize * value;
            transform.localScale = new Vector3(s, s, s);
        }
    }

    public void Init(ParticleExec exec, string element, Vector3 velocity)
    {
        particleExec = exec;
        this.element = element;

        size = exec.sizes[element];
        gameObject.GetComponent<SpriteRenderer>().color = exec.colours[element];

        rigidBody.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (
            collision.gameObject.name != "Particle(Clone)" ||
            collision.gameObject.GetComponent<Particle>() == null ||
            !collision.gameObject.GetComponent<Particle>().isActiveAndEnabled
        ) {
            return;
        }

        ExpExec.live.liveData["collisions"]++;

        var that = collision.gameObject;
        var thatScript = that.GetComponent<Particle>();

        float threshold;
        var reactants = (element, thatScript.element);
        bool canReact = ExpExec.live.bondingProspects.TryGetValue(reactants, out threshold);
        if (!canReact) return;

        if (Random.value < threshold) {
            ExpExec.live.liveData["reactions"]++;
            Debug.Log(ExpExec.live.liveData[$"particles.{element}"]);
            Debug.Log(ExpExec.live.liveData[$"particles.{thatScript.element}"]);
            ExpExec.live.liveData[$"particles.{element}"]--;
            ExpExec.live.liveData[$"particles.{thatScript.element}"]--;

            transform.position = (transform.position + that.transform.position) / 2;
            Vector3 velocity = (rigidBody.velocity + thatScript.rigidBody.velocity) / 2;

            particleExec.GetComponent<ParticleExec>()
                .SpawnParticle("Y", transform, velocity);

            Destroy(this);
            Destroy(that);
            particleExec.existingParticles.Remove(gameObject);
            particleExec.existingParticles.Remove(that.gameObject);
        }
    }
}
