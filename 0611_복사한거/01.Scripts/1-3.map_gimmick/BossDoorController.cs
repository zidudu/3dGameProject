using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BossDoorController : MonoBehaviour
{
    [Header("문 요소")]
    [Tooltip("문 애니메이터 (Open 트리거)")]
    public Animator doorAnimator;
    [Tooltip("문 재질 (Emission 색상 변조용)")]
    public Renderer doorRenderer;

    [Header("연출")]
    [Tooltip("열쇠 한 개 삽입 시 바뀔 색상")]
    public Color insertColor = Color.yellow;
    [Tooltip("LetterboxController 참조")]
    public LetterboxController letterbox;
    [Tooltip("시네마틱 카메라 (CinemachineVirtualCamera)")]
    public Cinemachine.CinemachineVirtualCamera vCam;
    [Tooltip("시네마틱 지속 시간")]
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
        // 인벤토리에 열쇠가 있는지 확인
        if (!KeyInventory.instance.HasKey()) return;

        // 한 개 소비
        KeyInventory.instance.ConsumeKey();
        insertedCount++;

        // 문 색상 살짝 변경 (누적)
        float t = insertedCount / 4f;
        doorRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.gray, insertColor, t));

        // 4개 모두 넣었으면 문 열기
        if (insertedCount >= 4)
            StartCoroutine(CutSceneOpen());
    }

    private IEnumerator CutSceneOpen()
    {
        // 플레이어 이동/입력 잠금
        PlayerInput input = FindObjectOfType<PlayerInput>();
        if (input) input.enabled = false;

        // 레터박스 & 가상 카메라 켜기
        letterbox.ShowLetterbox(0.5f);
        vCam.Priority = 20;        // 메인 카메라보다 높게

        // 문 열기 트리거
        doorAnimator.SetTrigger(OpenHash);

        // 5초간 시네마틱 유지
        yield return new WaitForSecondsRealtime(cutSceneTime);

        // 레터박스 해제 & 카메라 복귀
        letterbox.HideLetterbox(0.5f);
        vCam.Priority = 0;

        // 플레이어 컨트롤 다시 활성화
        if (input) input.enabled = true;
    }
}
