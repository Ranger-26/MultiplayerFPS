using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Throwable : MonoBehaviour
{
    public ThrowableScriptable throwable;

    float timer;

    private void Awake()
    {
        timer = throwable.Timer;
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

        if (timer <= 0f)
        {
            throwable.Trigger(transform);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        throwable.OnCollide(collision, gameObject);
    }
}
