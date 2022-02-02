using System.Collections;
using UnityEngine;

public class GunViewModel : MonoBehaviour
{
    public Gun gun;

    Transform cam;

    int currentAmmo;
    int reserve;

    bool delay;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    public IEnumerator Shoot()
    {
        if (delay)
            yield return null;

        delay = true;

        for (int i = 0; i < gun.BulletCount; i++)
        {
            Raycast();
        }

        yield return new WaitForSeconds(1f / gun.RPS);

        delay = false;
     }

    public void Raycast()
    {
        RaycastHit _hit;
        if (Physics.Raycast(cam.position, cam.forward, out _hit, gun.Range))
        {
            Debug.Log(_hit.transform.gameObject.name);
        }
    }
}
