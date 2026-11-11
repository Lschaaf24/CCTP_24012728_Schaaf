using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    public Vector3 offset;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;
    private bool isShaking = false;
    private Vector3 originalOffset;


    // Start is called before the first frame update
    void Start()
    {
        originalOffset = offset;
        player = GameObject.FindWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
        if(!isShaking){
        Vector3 targetPosition = player.transform.position + offset;
        transform.position = targetPosition;
        }
    }
    public void TriggerShake(){
        if(!isShaking){
            StartCoroutine(Shake());
        }
    }
    private IEnumerator Shake(){
        isShaking = true;
         float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomShake = Random.insideUnitSphere * shakeMagnitude;
            transform.position = player.transform.position + originalOffset + new Vector3(randomShake.x, randomShake.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
    }
}
