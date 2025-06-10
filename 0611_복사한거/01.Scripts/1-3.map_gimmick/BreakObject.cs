using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public string objectID = "Block_Default";

    private void Start()
    {
        if (GameManager.instance.IsObjectDestroyed(objectID))
        {
            Destroy(gameObject);
        }
    }

    public void Break()
    {
        GameManager.instance.RegisterDestroyedObject(objectID);
        Destroy(gameObject);
    }
}
