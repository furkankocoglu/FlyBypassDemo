using System.Collections;
using UnityEngine;

public class BotCreator : MonoBehaviour
{
    [SerializeField]
    GameObject botPrefab;
    [SerializeField]
    byte botCount;
    void Start()
    {
        StartCoroutine(CreateBot());
    }
    IEnumerator CreateBot()
    {
        int createdBotCount = 0;
        float startPosZ = 6;
        while (createdBotCount<botCount)
        {
            GameObject bot = Instantiate(botPrefab, new Vector3(Random.Range(-4, 5), 0, startPosZ),Quaternion.identity);
            bot.transform.GetChild(1).GetComponent<Renderer>().materials[1].color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f),Random.Range(0f, 1f));
            startPosZ -= 2;
            createdBotCount += 1;
            yield return new WaitForSeconds(Random.Range(0.5f,2f));
        }
        EventManager.Fire_onOpponentReady();
    }
}
