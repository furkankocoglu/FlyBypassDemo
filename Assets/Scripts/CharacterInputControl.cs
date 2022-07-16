using UnityEngine;

public class CharacterInputControl : MonoBehaviour
{
    float xFingerStartPosition;
    float xFingerCurrentPosition;
    short direction;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                xFingerStartPosition = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                xFingerCurrentPosition = touch.position.x;
                if (xFingerCurrentPosition > xFingerStartPosition)
                {
                    direction = 1;
                }
                else if (xFingerCurrentPosition < xFingerStartPosition)
                {
                    direction = -1;
                }
                xFingerStartPosition = xFingerCurrentPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                direction = 0;
            }
        }
        EventManager.Fire_onTurning(direction);
    }   
}
