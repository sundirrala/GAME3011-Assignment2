using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TimerBehavior : MonoBehaviour
{
    float currentTime = 0.0f;
    float startingTime = 30.0f;

    [SerializeField]
    Text timerText;

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        //Debug.Log(currentTime);
        timerText.text = currentTime.ToString("0");

        if(currentTime <= 0)
        {
            currentTime = 0;
            SceneManager.LoadScene("GameOver");
        }
    }
}
