using System;
using UnityEngine;
[Serializable]
public class WarpPointData
{
    public string warpID; // ���� ���� ID
    public string warpName; // ���� ���� �̸�
    public string sceneName; // �� �̸�
    public Vector3 position; // �� �� ��ġ

    public WarpPointData(string id, string name, string scene, Vector3 pos)
    {
        warpID = id;
        warpName = name;
        sceneName = scene;
        position = pos;
    }
}
