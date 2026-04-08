using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowController : MonoBehaviour
{
    [SerializeField] float castDistance;
    DecalProjector projector;
    private void Start()
    {
        projector = GetComponent <DecalProjector>();
    }
    void Update()
    {
        if (Physics.Raycast(transform.position + (Vector3.up * .2f), Vector3.down, out RaycastHit hit, castDistance))
        {
            float distance = hit.distance;

            projector.size = new Vector3(projector.size.x, projector.size.y, distance);

            projector.fadeFactor = 1 - (distance / castDistance);

            projector.pivot = (Vector3.forward * (distance / 2 + -.1f));
        }
    }

}
