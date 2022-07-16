using System.Collections;
using UnityEngine;

public class Wing : MonoBehaviour
{
    [SerializeField]
    float hideTime=1f;
    MeshRenderer wingMeshRenderer;
    Collider wingCollider;
    private void Awake()
    {
        wingMeshRenderer = GetComponent<MeshRenderer>();
        wingCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HideWing());
        }
    }
    IEnumerator HideWing()
    {
        wingMeshRenderer.enabled = false;
        wingCollider.enabled = false;
        yield return new WaitForSeconds(hideTime);
        wingMeshRenderer.enabled = true;
        wingCollider.enabled = true;
        yield break;
    }
}
