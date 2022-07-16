using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform characterTransform;
    Vector3 offset;
    Transform camTrasnform;
    [SerializeField]
    float followSpeed = 1f, zoomRange, lookXDegree, zoomSpeed, zoomZPos;
    short followDirection = 1;
    void Start()
    {
        camTrasnform = transform;
        offset = characterTransform.position - camTrasnform.position;
        lookXDegree = camTrasnform.eulerAngles.x;
    }
    private void FixedUpdate()
    {

        //Calculating camera position behind character.
        camTrasnform.position = characterTransform.position - new Vector3(followDirection*characterTransform.forward.x * (offset.z + zoomZPos) + offset.x, followDirection*characterTransform.forward.y * (offset.z + zoomZPos) + offset.y, followDirection*characterTransform.forward.z * (offset.z + zoomZPos));

        //Calculating look position.
        camTrasnform.LookAt(new Vector3(characterTransform.position.x, characterTransform.position.y + (-1 * offset.y) - ((offset.z / Mathf.Cos((lookXDegree * Mathf.PI) / 180)) * Mathf.Sin((lookXDegree * Mathf.PI) / 180)), characterTransform.position.z));

    }
    void ChangeOffset(bool zoom)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomLerp(zoom));
    }
    IEnumerator ZoomLerp(bool zoom)
    {
        if (zoom)
        {            
            while (zoomZPos > 0.1f)
            {
                zoomZPos = Mathf.Lerp(zoomZPos, 0, zoomSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {            
            while (zoomZPos < zoomRange - 0.1f)
            {
                zoomZPos = Mathf.Lerp(zoomZPos, zoomRange, zoomSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
        yield break;
    }

    void ChangeFollowDirection()
    {
        followDirection = -1;
    }
    private void OnEnable()
    {
        EventManager.onChangeCamZoom += ChangeOffset;
        EventManager.onLandAndDance += ChangeFollowDirection;
    }
    private void OnDisable()
    {
        EventManager.onChangeCamZoom -= ChangeOffset;
        EventManager.onLandAndDance -= ChangeFollowDirection;
    }
}
