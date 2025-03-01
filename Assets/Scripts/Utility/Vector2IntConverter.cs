using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntConverter : JsonConverter<Dictionary<Vector2Int, int>>
{
    // Overrides the WriteJson method to serialize a dictionary to JSON
    public override void WriteJson(JsonWriter writer, Dictionary<Vector2Int, int> value, JsonSerializer serializer)
    {
        var dictionaryObject = new JObject();

        foreach (var pair in value)
        {
            // Convert the Vector2Int key to a string representation
            var key = JToken.FromObject($"({pair.Key.x}, {pair.Key.y})");
            // Add the key-value pair to the JObject
            dictionaryObject.Add(key.ToString(), pair.Value);
        }

        // Write the JObject to the JSON writer
        dictionaryObject.WriteTo(writer);
    }

    // Overrides the ReadJson method to deserialize JSON to a dictionary
    public override Dictionary<Vector2Int, int> ReadJson(JsonReader reader, Type objectType, Dictionary<Vector2Int, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Load the JSON object from the reader into a JObject
        var jObject = JObject.Load(reader);
        var dictionary = new Dictionary<Vector2Int, int>();

        foreach (var pair in jObject)
        {
            var key = pair.Key;
            var value = (int)pair.Value;

            // Try to parse the string key into a Vector2Int
            if (TryParseVector2Int(key, out var vector))
            {
                // Add the key-value pair to the dictionary
                dictionary.Add(vector, value);
            }
        }

        return dictionary;
    }

    // Helper method to parse a string key into a Vector2Int
    private bool TryParseVector2Int(string key, out Vector2Int vector)
    {
        if (key.StartsWith("(") && key.EndsWith(")"))
        {
            var trimmedKey = key.Trim('(', ')');
            var coordinates = trimmedKey.Split(',');

            // Check if the key has the expected format and parse the coordinates
            if (coordinates.Length == 2 && int.TryParse(coordinates[0].Trim(), out var x) && int.TryParse(coordinates[1].Trim(), out var y))
            {
                // Create a new Vector2Int object with the parsed coordinates
                vector = new Vector2Int(x, y);
                return true;
            }
        }

        // Key parsing failed, set the vector to default value and return false
        vector = default;
        return false;
    }
}
