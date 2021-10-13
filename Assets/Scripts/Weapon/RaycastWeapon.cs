using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastOrigin;
    public Transform raycastDestination;

    Ray ray;
    RaycastHit hitInfo;

    public void StartFiring()
    {
        isFiring = true;
        foreach(var particle in muzzleFlash)
        {
            particle.Emit(1);
        }
        ray.origin = raycastOrigin.position;
        ray.direction = raycastOrigin.forward;
        if(Physics.Raycast(ray, out hitInfo)){
            Debug.Log(hitInfo.collider.gameObject.name);
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
