using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5, speedBoost = 0.5f;
    float runSpeed, gravity;
    [SerializeField]
    float turnSpeed = 1, flySpeed = 5, flyHigh = 10,score=0;
    Animator characterAnimator;
    bool isCanRun = false, isFly = false;
    [SerializeField]
    GameObject wingPrefab, wingSlot, wings;
    int wingCount = 0;
    List<GameObject> wingList = new List<GameObject>();
    Vector3 wingCreatePosition = Vector3.zero;
    Vector3 wingCreateScale = Vector3.zero;
    Vector3 characterVelocity = Vector3.zero;
    bool isFinishLevel = false, useGravity = true,levelFailed=false;
    Transform characterTransform;
    CharacterController characterController;
    [SerializeField]
    bool isMe = false;
    float finishZPos,finishRange;
    [SerializeField]
    GameObject wingCountCanvas;
    [SerializeField]
    Text wingCountText;
    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        wingCreatePosition.x = 0.015f;                      //Wing start x position.
        wingCreateScale = wingPrefab.transform.localScale;  //Wing start scale.
        characterTransform = transform;
        runSpeed = moveSpeed;
        gravity = Physics.gravity.y;
        finishZPos = GameObject.FindWithTag("Finish").transform.position.z;
        finishRange = finishZPos - characterTransform.position.z;
    }
    void MoveCharacter()
    {
        if (isCanRun)
        {
            Vector3 forwardVelocity = transform.forward * moveSpeed;
            characterVelocity.x = forwardVelocity.x;
            characterVelocity.z = forwardVelocity.z;
        }
        if (useGravity && !characterController.isGrounded)
        {
            characterVelocity.y += gravity * Time.fixedDeltaTime;
        }
        else if (useGravity && characterController.isGrounded)
        {
            characterVelocity.y = 0;
        }
        characterController.Move(characterVelocity * Time.fixedDeltaTime);
        if (isMe)
        {
            EventManager.Fire_onCharacterMove((finishRange - (finishZPos - characterTransform.position.z)) / finishRange);
        }        
    }
    private void FixedUpdate()
    {
        if (!levelFailed)
        {
            CheckFail();
            MoveCharacter();
            if (!isMe)
            {
                BotControl();
            }
            //Checking ground with ray.
            CheckGround();
        }        
    }
    void CheckFail()
    {
        if (!isFinishLevel && characterTransform.position.y < -5)
        {
            if (isMe)
            {
                levelFailed = true;
                EventManager.Fire_onlevelFailed();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    private void CheckGround()
    {
        if (useGravity)
        {
            Vector3 origin = characterTransform.position;
            origin.y += 1;
            Ray ray = new Ray(origin, Vector3.down);
            if (!Physics.Raycast(ray, 5f) && !isFly)//If not on ground and not flying
            {
                Fly();
                isFly = true;
            }
            else if (Physics.Raycast(ray, 5f) && isFly)//If on ground and flying
            {
                wingSlot.SetActive(false);                
                isFly = false;
                if (isMe)
                {
                    wingCountCanvas.SetActive(false);
                    EventManager.Fire_onChangeCamZoom(true);
                }
                if (isFinishLevel)
                {                    
                    LandAndDance();
                }
                else
                {
                    characterAnimator.SetBool("Fly", false);
                    moveSpeed = runSpeed;
                }

            }
        }

    }

    void Run()
    {
        characterAnimator.SetBool("Run", true);
        isCanRun = true;
    }
    void Fly()
    {
        wingSlot.SetActive(true);
        wings.SetActive(false);
        characterAnimator.SetBool("Fly", true);
        if (wingCount > 0)
        {
            useGravity = false;
            if (isMe)
            {
                EventManager.Fire_onChangeCamZoom(false);
            }
            StartCoroutine(FlyUntilTarget());
            StartCoroutine(CreateWing());
        }

    }
    IEnumerator CreateWing()
    {
        float wingPosUpper = 0.01f;
        for (int i = 0; i < wingCount; i++)
        {
            GameObject wing = Instantiate(wingPrefab, wingSlot.transform);
            wingList.Add(wing);
            if (i % 2 == 0)
            {
                wing.transform.localPosition = wingCreatePosition;
                wing.transform.localScale = wingCreateScale;
            }
            else
            {
                wing.transform.localPosition = new Vector3(-wingCreatePosition.x, wingCreatePosition.y, wingCreatePosition.z);
                wing.transform.localScale = wingCreateScale;
                wingCreatePosition.x += 0.03f;
                wingCreatePosition.y -= wingPosUpper;

                if (wingCount / 2f > i)
                {
                    wingCreatePosition.z -= wingPosUpper;
                }
                else
                {
                    wingCreatePosition.z += wingPosUpper;
                }
                wingCreateScale.z += wingPosUpper * 2;
            }
        }
        moveSpeed *= 2;
        wingCreatePosition = Vector3.zero;
        wingCreatePosition.x = 0.015f;
        wingCreateScale = wingPrefab.transform.localScale;
        wingCount = 0;
        wingList.Reverse();
        StartCoroutine(FallWings());
        yield break;
    }
    IEnumerator FallWings()
    {
        int i = 0;
        while (wingList.Count > 0)
        {
            GameObject wing = wingList[i];
            wingList.RemoveAt(i);
            wing.transform.SetParent(null);
            Rigidbody wingRigidbody = wing.GetComponent<Rigidbody>();
            wingRigidbody.useGravity = true;
            wingRigidbody.isKinematic = false;
            wing.transform.eulerAngles = new Vector3(wing.transform.eulerAngles.x, wing.transform.eulerAngles.y, 0);
            Destroy(wing, 5f);
            if (wingList.Count > 1)
            {
                GameObject wing2 = wingList[i];
                wingList.RemoveAt(i);
                wing2.transform.SetParent(null);
                Rigidbody wingRigidbody2 = wing2.GetComponent<Rigidbody>();
                wingRigidbody2.useGravity = true;
                wingRigidbody2.isKinematic = false;
                wing2.transform.eulerAngles = new Vector3(wing.transform.eulerAngles.x, wing.transform.eulerAngles.y, 0);
                Destroy(wing2, 5f);
            }
            if (isMe)
            {
                wingCountText.text = wingList.Count.ToString();
            }
            yield return new WaitForSeconds(0.1f);
        }
        useGravity = true;
        StopCoroutine(FlyUntilTarget());
        yield break;
    }
    IEnumerator FlyUntilTarget()
    {
        float flyHigh = this.flyHigh + characterTransform.position.y;
        characterVelocity.y = flySpeed;
        yield return new WaitWhile(() => characterTransform.position.y < flyHigh);
        characterVelocity.y = 0;
        yield break;
    }
    void LandAndDance()
    {
        if (isMe)
        {
            score = (characterTransform.position.z - finishZPos)*100;
            EventManager.Fire_onChangeScore(score.ToString());
            EventManager.Fire_onLandAndDance();
        }        
        isCanRun = false;
        characterVelocity = Vector3.zero;
        characterAnimator.SetTrigger("Land");
        characterTransform.Rotate(0, 180, 0);
    }
    void Turn(short direction)
    {
        if (!levelFailed)
        {
            if (isCanRun)
            {
                transform.Rotate(0, direction * turnSpeed * Time.fixedDeltaTime, 0);
                Vector3 clampedAngle = transform.eulerAngles;
                clampedAngle.y = clampedAngle.y > 180 ? Mathf.Clamp(clampedAngle.y, 270, 360) : Mathf.Clamp(clampedAngle.y, 0, 90);
                transform.eulerAngles = clampedAngle;
            }
        }        
    }
    IEnumerator SpeedBoost()
    {
        moveSpeed += speedBoost;
        yield return new WaitForSeconds(1f);
        moveSpeed -= speedBoost;
        yield break;
    }    
    bool isBotTurning = false;
    void BotControl()
    {       
        if (isCanRun)
        {
            if (!isBotTurning)
            {
                isBotTurning = true;
                StartCoroutine(BotTurn());
            }
        }
    }
    IEnumerator BotTurn()
    {
        while (!isFinishLevel)
        {
            float time = Random.Range(0.5f, 1f); ;
            short direction = (short)Random.Range(-1, 2);
            while (time > 0)
            {
                Turn(direction);
                time -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wing"))
        {
            wingCount += 3;
            if (isMe)
            {
                wingCountText.text = wingCount.ToString();
                wingCountCanvas.SetActive(true);
            }            
            wingSlot.SetActive(true);
            wings.SetActive(true);            
            StartCoroutine(SpeedBoost());
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Finish"))
        {
            isFinishLevel = true;
            if (isMe)
            {
                EventManager.Fire_onFinishLevel();
            }
        }
    }
    private void OnEnable()
    {
        EventManager.onStartRun += Run;
        if (isMe)
        {
            EventManager.onTurning += Turn;
        }

    }
    private void OnDisable()
    {
        EventManager.onStartRun -= Run;
        if (isMe)
        {
            EventManager.onTurning -= Turn;
        }
    }
}
