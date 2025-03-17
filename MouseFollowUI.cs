using UnityEngine;
using UnityEngine.UI;

public class MouseFollowUI : MonoBehaviour
{
    // 한글로 된 주석: UI를 따라다닐 RectTransform (자신이라면 동일)
    public RectTransform uiRect;
    
    // 한글로 된 주석: 만약 마우스 오른쪽 위로 조금 띄워서 보여주고 싶다면 오프셋을 줄 수 있음
    public Vector2 offset = new Vector2(20f, -20f);

    void Start()
    {
    
        // 한글로 된 주석: 시스템 마우스를 숨기고, UI만 보이게 하고 싶다면 아래 코드 사용
        Cursor.visible = false;
        
        // 만약 이 스크립트를 UI 오브젝트에 붙였다면, uiRect = GetComponent<RectTransform>(); 사용 가능
        if (uiRect == null)
        {
            uiRect = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        // 한글로 된 주석: 마우스 스크린 좌표
        Vector3 mousePos = Input.mousePosition;
        
        // 한글로 된 주석: 오프셋 적용
        mousePos += (Vector3)offset;
        
        // 한글로 된 주석: Screen Space - Overlay 모드에서는 mousePos를 그대로 이용할 수 있음
        // RectTransform의 position에 마우스 좌표를 대입
        uiRect.position = mousePos;
    }
}
