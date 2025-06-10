using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BossDoorController : MonoBehaviour
{
    [Header("�� ���")]
    [Tooltip("�� �ִϸ����� (Open Ʈ����)")]
    public Animator doorAnimator;
    [Tooltip("�� ���� (Emission ���� ������)")]
    public Renderer doorRenderer;

    [Header("����")]
    [Tooltip("���� �� �� ���� �� �ٲ� ����")]
    public Color insertColor = Color.yellow;
    [Tooltip("LetterboxController ����")]
    public LetterboxController letterbox;
    [Tooltip("�ó׸�ƽ ī�޶� (CinemachineVirtualCamera)")]
    public Cinemachine.CinemachineVirtualCamera vCam;
    [Tooltip("�ó׸�ƽ ���� �ð�")]
    public float cutSceneTime = 5f;

    private int insertedCount = 0;
    private static readonly int OpenHash = Animator.StringToHash("Open");

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TryInsertKey();
        }
    }

    private void TryInsertKey()
    {
        // �κ��丮�� ���谡 �ִ��� Ȯ��
        if (!KeyInventory.instance.HasKey()) return;

        // �� �� �Һ�
        KeyInventory.instance.ConsumeKey();
        insertedCount++;

        // �� ���� ��¦ ���� (����)
        float t = insertedCount / 4f;
        doorRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.gray, insertColor, t));

        // 4�� ��� �־����� �� ����
        if (insertedCount >= 4)
            StartCoroutine(CutSceneOpen());
    }

    private IEnumerator CutSceneOpen()
    {
        // �÷��̾� �̵�/�Է� ���
        PlayerInput input = FindObjectOfType<PlayerInput>();
        if (input) input.enabled = false;

        // ���͹ڽ� & ���� ī�޶� �ѱ�
        letterbox.ShowLetterbox(0.5f);
        vCam.Priority = 20;        // ���� ī�޶󺸴� ����

        // �� ���� Ʈ����
        doorAnimator.SetTrigger(OpenHash);

        // 5�ʰ� �ó׸�ƽ ����
        yield return new WaitForSecondsRealtime(cutSceneTime);

        // ���͹ڽ� ���� & ī�޶� ����
        letterbox.HideLetterbox(0.5f);
        vCam.Priority = 0;

        // �÷��̾� ��Ʈ�� �ٽ� Ȱ��ȭ
        if (input) input.enabled = true;
    }
}
