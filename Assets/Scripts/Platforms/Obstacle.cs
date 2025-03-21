using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private Material dissolveMaterial; // Assign the material with the dissolve shader
    [SerializeField] private float dissolveSpeed = 1.0f; // Speed at which the object dissolves

    private bool isHit = false;
    private float dissolveAmount = 0.0f;

    void Update()
    {
        if (isHit)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }

    // Call this method when the obstacle is hit
    public void Hit()
    {
        isHit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            Hit();
        }
    }

    private void OnDisable()
    {
        dissolveAmount = 0;
        dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
    }
}
