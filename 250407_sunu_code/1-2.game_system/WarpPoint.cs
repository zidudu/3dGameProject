using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WarpPoint : MonoBehaviour
{
    public string warpID = "default"; // 워프 고유 ID
    public string warpName = "Unnamed"; // 워프 이름
    public string sceneName = ""; // 워프가 속한 씬 이름

    private bool playerInRange = false; // 플레이어가 범위 내에 있는지 여부
    private PlayerControls controls; // 입력 시스템 참조

    private void Awake()
    {
        if (string.IsNullOrEmpty(sceneName)) // 씬 이름이 비어있으면
        {
            sceneName = SceneManager.GetActiveScene().name; // 현재 씬 이름 설정
        }

        controls = new PlayerControls(); // 입력 컨트롤 초기화
        controls.Gameplay.Interact.performed += ctx => TryOpenWarpUI(); // Interact 입력에 UI 열기 연결
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable(); // 입력 활성화
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable(); // 입력 비활성화
    }

    private void Start()
    {
        WarpPointData data = new WarpPointData(warpID, warpName, sceneName, transform.position); // 워프 데이터 생성
        GameManager.instance.RegisterWarpPointData(data); // 세이브용 데이터 등록
        if (GameManager.instance.IsWarpUnlocked(warpID)) // 워프가 해금된 경우
        {
            WarpManager.instance.RegisterWarpPoint(data); // UI용 워프 등록
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // 플레이어가 아니면 무시

        playerInRange = true; // 범위 내로 들어옴
        WarpPointData data = new WarpPointData(warpID, warpName, sceneName, transform.position); // 워프 데이터 생성

        if (!GameManager.instance.IsWarpUnlocked(warpID)) // 처음 해금되는 경우
        {
            GameManager.instance.UnlockWarp(warpID); // 워프 해금
        }

        WarpManager.instance.RegisterWarpPoint(data); // UI 갱신용 워프 등록
        GameManager.instance.SaveCheckpoint(transform.position); // 체크포인트 저장
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 나가면
        {
            playerInRange = false; // 범위 플래그 해제
        }
    }

    private void TryOpenWarpUI()
    {
        if (playerInRange && WarpManager.instance != null) // 범위 안이고 워프 매니저 존재하면
        {
            WarpManager.instance.OpenWarpUI(); // 워프 UI 열기
        }
    }
}
