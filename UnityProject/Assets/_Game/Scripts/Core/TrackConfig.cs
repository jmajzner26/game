using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewTrackConfig", menuName = "Polytrack/Track Config", order = 2)]
public class TrackConfig : ScriptableObject
{
    public enum TimeOfDay { Day, Dusk, Night }
    public enum Weather { Clear, Rain, Snow }
    
    [Flags]
    public enum SurfaceType
    {
        None = 0,
        Asphalt = 1 << 0,
        Dirt = 1 << 1,
        Snow = 1 << 2,
        NeonWet = 1 << 3
    }
    
    [Header("Identity")]
    public string id;
    public string displayName;
    public string sceneName;
    
    [Header("Race Settings")]
    [Range(1, 10)]
    public int laps = 3;
    public int checkpointCount;
    
    [Header("Environment")]
    public TimeOfDay timeOfDay = TimeOfDay.Day;
    public Weather weather = Weather.Clear;
    public SurfaceType surfaceTypes = SurfaceType.Asphalt;
    
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            id = name.ToLower().Replace(" ", "_");
    }
}
