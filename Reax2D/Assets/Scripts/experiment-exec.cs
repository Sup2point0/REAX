using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;


/// <summary>
/// Manages an experiment with multiple runs.
/// </summary>
public class ExpExec : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Static")]
    public static ExpExec live;

    [Header("Unity Configuration")]
    public GameObject particleExec;
    public GameObject vitalsDisplay;
    public TMP_Text vitalsDisplayText;

    [Header("Experiment Configuration")]
    public Dictionary<string, int> particleInitCounts;
    public AdjacencyMatrix<float> bondingProspects;
    public AdjacencyMatrix<(string, string)> bondingProducts;

    public Vector2 chamberSize;
    public int temperature;
    public int targetRuns = 1;
    public int targetTicks;  // TODO calibrate
    public string exportFilepath;

    [Header("Experiment Data")]
    public Dictionary<string, int> liveData;
    public Dictionary<string, int[]> runData;
    public List<Dictionary<string, int[]>> expData;

    [Header("Particle Concentrations")]
    public int totalParticles;
    public float Si;
    public float O;

    [Header("Bonding Probabilities")]
    public float SiSi;
    public float SiO;
    public float SiOSi;
    public float OBreakdown;

    [Header("Experiment State")]
    public ExpState state;
    public int runIndex = 0;
    public int runTicks = 0;

    public enum ExpState { Run, Standby, Done }
    #endregion

    #region CORE
    void Start()
    {
        vitalsDisplayText = vitalsDisplay.GetComponent<TMP_Text>();

        InitExp();
        RunExp();
    }

    void Update()
    {
        if (state == ExpState.Standby) {
            Debug.Log($"STATUS - starting run {runIndex}");
            NextRun();
        }
    }

    void FixedUpdate()
    {
        if (state != ExpState.Run || live is null) {
            return;
        };
        if (runTicks == targetTicks) {
            EndRun(save: true);
            return;
        }

        foreach (KeyValuePair<string, int> tracking in liveData) {
            runData[tracking.Key][runTicks] = tracking.Value;
        }
        runTicks++;

        var text = Utils.RenderDebugInfo(
            new Dictionary<string, object>() {
                ["state"] = state,
                ["run"] = $"{runIndex + 1}/{targetRuns}",
                ["ticks"] = $"{runTicks}/{targetTicks}",
                ["collisions"] = liveData["collisions"],
                ["reactions"] = liveData["reactions"],
                [""] = "",
                ["particles"] = liveData["particles.Si"] + liveData["particles.O"] + liveData["particles.X"],
                ["Si"] = liveData["particles.Si"],
                ["O"] = liveData["particles.O"],
                ["X"] = liveData["particles.X"],
                ["SiSi"] = liveData["particles.SiSi"],
                ["SiO"] = liveData["particles.SiO"],
            }
        );
        vitalsDisplayText.SetText(text);
    }
    #endregion

    #region EXP
    void InitExp()
    {
        expData = new();
        particleInitCounts = new() {
            {"Si", Mathf.RoundToInt(totalParticles * Si)},
            {"O", Mathf.RoundToInt(totalParticles * O)},
            {"X", Mathf.RoundToInt(totalParticles * (1- Si - O))},
        };
        bondingProspects = new() {
            ["Si", "Si"] = SiSi,
            ["Si", "O"] = SiO,
            ["Si", "SiO"] = SiOSi,
            ["SiSi", "O"] = OBreakdown,
            ["SiSi", "SiO"] = OBreakdown,
        };
        bondingProducts = new() {
            ["Si", "Si"] = ("SiSi", "SiSi"),
            ["Si", "O"] = ("SiO", "SiO"),
            ["Si", "SiO"] = ("SiOSi", "SiO"),
            ["SiSi", "O"] = ("SPECIAL", "SPECIAL"),
            ["SiSi", "SiO"] = ("SPECIAL", "SPECIAL"),
        };
    }

    void RunExp()
    {
        live = this;
        NextRun();
    }

    void NextRun()
    {
        state = ExpState.Run;
        Time.timeScale = 1;
        runTicks = 0;
        Reset();
    }

    void EndRun(bool save = false)
    {
        Debug.Log("CALL - EndRun()");
        particleExec.GetComponent<ParticleExec>().DestroyAll();

        expData.Add(runData);
        if (save) {
            SaveData();
        }

        if (++runIndex >= targetRuns) {
            EndExp(save: true);
        } else {
            state = ExpState.Standby;
            Time.timeScale = 0;
        }
    }

    void EndExp(bool save = true)
    {
        state = ExpState.Done;
        live = null;
        Time.timeScale = 0;
        if (save) {
            SaveData();
        }
    }
    #endregion

    void SaveData()
    {
        Debug.Log("CALL - SaveData()");
        var exportData = new List<object>(expData);
        exportData.Insert(0, new Dictionary<string, object>() {
            ["target-ticks"] = targetTicks,
            ["target-runs"] = targetRuns,
            ["temperature"] = temperature,
            ["total-particles"] = totalParticles,
            ["init-Si"] = particleInitCounts["Si"],
            ["init-O"] = particleInitCounts["O"],
            ["init-X"] = particleInitCounts["X"],
            ["prob-Si+Si"] = bondingProspects["Si", "Si"],
            ["prob-Si+O"] = bondingProspects["Si", "O"],
        });
        string exportText = JsonConvert.SerializeObject(exportData, Formatting.Indented);

        using (FileStream stream = new(exportFilepath, FileMode.Create)) {
            using (StreamWriter writer = new(stream)) {
                writer.Write(exportText);
            }
        }
    }

    void Reset()
    {
        Debug.Log("CALL - Reset()");
        particleExec.GetComponent<ParticleExec>().SpawnParticles(exp: this);
        
        liveData = new() {
            {"collisions", 0},
            {"reactions", 0},
            {"particles.Si", particleInitCounts["Si"]},
            {"particles.O", particleInitCounts["O"]},
            {"particles.X", particleInitCounts["X"]},
            {"particles.SiSi", 0},
            {"particles.SiO", 0}
        };
        runData = new() {
            {"collisions", new int[targetTicks]},
            {"reactions", new int[targetTicks]},
            {"particles.Si", new int[targetTicks]},
            {"particles.O", new int[targetTicks]},
            {"particles.X", new int[targetTicks]},
            {"particles.SiSi", new int[targetTicks]},
            {"particles.SiO", new int[targetTicks]}
        };
    }
}
