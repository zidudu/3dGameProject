using System;
using UnityEngine;
[Serializable]
public class WarpPointData
{
    public string warpID; // 고유 워프 ID
    public string warpName; // 워프 지점 이름
    public string sceneName; // 씬 이름
    public Vector3 position; // 씬 내 위치

    public WarpPointData(string id, string name, string scene, Vector3 pos)
    {
        warpID = id;
        warpName = name;
        sceneName = scene;
        position = pos;
    }
}
