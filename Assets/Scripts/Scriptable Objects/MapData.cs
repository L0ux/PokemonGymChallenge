using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public Bounds bounds;
    public bool allowRun;
    public MapData underMap;
    public AudioClip levelMusic;
    public Vector2 spawnPosition;
    public Vector2 teleporterPosition;
}