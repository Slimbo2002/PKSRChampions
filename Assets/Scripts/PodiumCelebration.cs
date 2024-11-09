using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PodiumCelebration : MonoBehaviour
{
    Animator danceClip;

    int index;

    float timer;

    [SerializeField] float _minTimer, _maxTimer;
 
    // Start is called before the first frame update
    IEnumerator Start()
    {
        timer = Random.Range(_minTimer, _maxTimer);
        danceClip = GetComponent<Animator>();
        while (true)
        {
            timer = Random.Range(_minTimer, _maxTimer);
            danceClip.SetInteger("Index", Random.Range(0, 3));
            Debug.Log("Random");
            danceClip.SetTrigger("Emote");

            yield return new WaitForSeconds(timer);
        }
    }
}
