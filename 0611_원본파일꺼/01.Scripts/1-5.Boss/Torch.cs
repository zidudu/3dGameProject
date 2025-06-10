using UnityEngine;

public class Torch : MonoBehaviour
{
    public bool isLit = false;
    public GameObject torchEffectGizmo;

    private void OnTriggerEnter(Collider other)
    {
        if (isLit) return;
        if (!other.CompareTag("Player")) return;

        isLit = true;
        if (torchEffectGizmo != null)
            torchEffectGizmo.SetActive(true);

        BossBarrierManager.Instance?.NotifyTorchLit();
    }

    public void ResetTorch()
    {
        isLit = false;
        if (torchEffectGizmo != null)
            torchEffectGizmo.SetActive(false);
    }
}
