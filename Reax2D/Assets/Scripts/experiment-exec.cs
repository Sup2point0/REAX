using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Manages an experiment with multiple runs.
/// </summary>
public class ExpExec : MonoBehaviour
{
    [Header("Static")]
    public static ExpExec live;

    [Header("Unity Configuration")]
    public GameObject particleExec;
    public int TESTING_collisions;

    [Header("Experiment Configuration")]
    public Dictionary<string, int> particleInitCounts;
    public AdjacencyMatrix<float> bondingProspects;

    public Vector2 chamberSize;
    public int targetRuns = 1;
    public int targetTicks;  // TODO calibrate
    public float apexKineticEnergy;
    public string exportFilepath;

    [Header("Experiment Data")]
    public Dictionary<string, int> liveData;
    public Dictionary<string, int[]> runData;
    public List<Dictionary<string, int[]>> expData;

    [Header("Particle Counts")]
    public int Si;
    public int X;
    public int O;
    public int C;

    [Header("Bonding Probabilities")]
    public float SiSi;
    public float SiO;
    public float SiX;
    public float SiC;

    [Header("Experiment State")]
    public ExpState state;
    public int runIndex = -1; // increments to 0 on first run
    public int runTicks = 0;

    public enum ExpState { Run, Standby }

    void Start()
    {
        Dictionary<string, int> particles = new() {
            {"Si", Si}, {"X", X}, {"O", O}, {"C", C}
        };
        AdjacencyMatrix<float> prospects = new() {
            ["Si", "Si"] = SiSi,
            ["Si", "O"] = SiO,
            ["Si", "X"] = SiX,
            ["Si", "C"] = SiC
        };

        InitExp(particles, prospects);
        RunExp();
    }

    void FixedUpdate()
    {
        if (state != ExpState.Run) return;

        if (live is not null) {
            if (runTicks == targetTicks) {
                EndRun(save: true);
                return;
            }

            foreach (KeyValuePair<string, int> tracking in liveData) {
                runData[tracking.Key][runTicks] = tracking.Value;
            }

            runTicks++;
        }
    }

    void InitExp(
        Dictionary<string, int> particles,
        AdjacencyMatrix<float> prospects
    ) {
        expData = new();
        particleInitCounts = particles;
        bondingProspects = prospects;
    }

    void RunExp()
    {
        live = this;
        NextRun();
    }

    void NextRun()
    {
        state = ExpState.Run;
        runIndex++;
        runTicks = 0;
        Reset();
    }

    void EndRun(bool save = false)
    {
        state = ExpState.Standby;
        Time.timeScale = 0;

        particleExec.GetComponent<ParticleExec>().DestroyAll();

        expData.Add(runData);
        if (save) {
            SaveData();
        }

        if (runIndex >= targetRuns) {
            EndExp();
        } else {
            NextRun();
        }
    }

    void EndExp()
    {
        live = null;
        Time.timeScale = 0;
    }

    void SaveData()
    {
        Debug.Log("SAVING DATA");

        // List<Dictionary<string, int[]>> expData;
        // List<
        string exportData = JsonUtility.ToJson(expData, prettyPrint: true);
        Debug.Log(exportData);

        using (FileStream stream = new(exportFilepath, FileMode.Create)) {
            using (StreamWriter writer = new(stream)) {
                writer.Write(exportData);
            }
        }
    }

    void Reset()
    {
        TESTING_collisions = 0;
        liveData = new() {
            {"collisions", 0},
            {"particles.Si", particleInitCounts["Si"]},
            {"particles.C", particleInitCounts["C"]},
            {"particles.X", particleInitCounts["X"]},
            {"particles.O", particleInitCounts["O"]}
        };
        runData = new() {
            {"collisions", new int[targetTicks]},
            {"particles.Si", new int[targetTicks]},
            {"particles.C", new int[targetTicks]},
            {"particles.X", new int[targetTicks]},
            {"particles.O", new int[targetTicks]}
        };

        var particleExecScript = particleExec.GetComponent<ParticleExec>();
        particleExecScript.DestroyAll();
        particleExecScript.SpawnParticles(this);
    }
}
