using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// attached on the onstacles to handle the shader effect
    /// </summary>
    public class Obstacle : MonoBehaviour
    {

        [SerializeField] private Material dissolveMaterial; // Assign the material with the dissolve shader
        [SerializeField] private float dissolveSpeed = 1.0f; // Speed at which the object dissolves
        private float dissolveAmount = 0.0f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>() != null)
            {
                StartCoroutine(ShowDisolveEffect());
            }
        }

        IEnumerator ShowDisolveEffect()
        {
            while (dissolveAmount < 1)
            {
                dissolveAmount += Time.deltaTime * dissolveSpeed;
                dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);

                yield return null;
            }
        }

        private void OnDisable()
        {
            StopCoroutine(ShowDisolveEffect());
            dissolveAmount = 0;
            dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }
}
