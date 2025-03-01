using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameData
{
    public string MatchId { get; set; }
    public string MessageType { get; set; }
    public float CityHealth { get; set; }
    public int TroopsUsed { get; set; }

    [JsonConverter(typeof(Vector2IntConverter))]
    public Dictionary<Vector2Int, int> Positions { get; set; }
}