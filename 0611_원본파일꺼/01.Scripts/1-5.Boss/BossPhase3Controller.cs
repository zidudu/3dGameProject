using System.Collections;
using UnityEngine;

public class BossPhase3Controller : MonoBehaviour
{
    public GameObject barrierGizmo;
    public GameObject torchGroup;
    public float moveDownDuration = 1.5f;
    public float stunDuration = 5f;
    public Vector3 moveDownTarget = new Vector3(0f, -2f, 0f);

    private bool torchesLit = false;
    private bool isProtected = false;

    void Start()
    {
        StartProtectingPhase(); // ��ȣ�� + ȶ�� Ȱ��ȭ
    }

    void StartProtectingPhase()
    {
        isProtected = true;
        transform.position = new Vector3(0f, 5f, 0f);

        if (barrierGizmo != null)
            barrierGizmo.SetActive(true);

        if (torchGroup != null)
        {
            torchGroup.SetActive(true);

            foreach (Torch torch in torchGroup.GetComponentsInChildren<Torch>())
                torch.ResetTorch();
        }
    }

    public void OnAllTorchesLit()
    {
        if (torchesLit) return;
        torchesLit = true;

        isProtected = false;

        if (barrierGizmo != null)
            barrierGizmo.SetActive(false);

        if (torchGroup != null)
            torchGroup.SetActive(false);

        StartCoroutine(MoveDownThenStun());
    }

    IEnumerator MoveDownThenStun()
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDownDuration)
        {
            transform.position = Vector3.Lerp(start, moveDownTarget, elapsed / moveDownDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = moveDownTarget;

        StartCoroutine(RecoverFromStun());
    }

    IEnumerator RecoverFromStun()
    {
        Debug.Log("���� ���� ����");
        yield return new WaitForSeconds(stunDuration);
        Debug.Log("���� ���� ���� �� ���� ���� ���� ����");
        // ���� ���� ������ ���� ��ȯ ����
    }
}
