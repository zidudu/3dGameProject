using UnityEngine;

public class KeyObject : MonoBehaviour
{
    public string keyID = "Key_Default";

    private void Start()
    {
        if (GameManager.instance.IsObjectDestroyed(keyID))
        {
            Destroy(gameObject); // ¿ÃπÃ ∏‘¿∫ ø≠ºË∏È æ» ∫∏¿Ã∞‘
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.RegisterDestroyedObject(keyID);
            Destroy(gameObject); // ø≠ºË ∏‘¿Ω
        }
    }
}
