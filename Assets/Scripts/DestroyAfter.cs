using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float Timer = 1f;

    private void Start()
    {
        Destroy(gameObject, Timer);
    }
}
