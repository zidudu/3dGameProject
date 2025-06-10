using System.Collections.Generic;
using UnityEngine;

public class BossBarrierManager : MonoBehaviour
{
    public static BossBarrierManager Instance;

    public GameObject torchGroup;
    public List<Torch> torches = new List<Torch>();
    private int litTorchCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void EnableTorches()
    {
        torchGroup.SetActive(true);
        litTorchCount = 0;

        foreach (var torch in torches)
            torch.ResetTorch();
    }

    public void DisableTorches()
    {
        torchGroup.SetActive(false);
    }

    public void NotifyTorchLit()
    {
        litTorchCount++;
        if (litTorchCount >= torches.Count)
        {
            FindObjectOfType<BossController>()?.OnAllTorchesLit();
        }
    }
}
