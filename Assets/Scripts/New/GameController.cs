using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {
    public GameObject StartPanel;
    public GameObject LaserPointer;
    public GameObject OneSecondLeft;
    public GameObject TwoSecondLeft;
    public GameObject ThreeSecondLeft;
    public GameObject FlyingScripts;
    public GameObject CountDownPanel;
    void Awake()
    {
        
    }

    void Start() 
    {
        
    }
    public void StartGame() {
        LaserPointer.SetActive(false);
        StartPanel.SetActive(false);
        StartCoroutine(CountDown());
    }
    
    IEnumerator CountDown()
    {
        CountDownPanel.SetActive(true);
        ThreeSecondLeft.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        ThreeSecondLeft.SetActive(false);
        TwoSecondLeft.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        TwoSecondLeft.SetActive(false);
        OneSecondLeft.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        OneSecondLeft.SetActive(false);
        CountDownPanel.SetActive(false);
        FlyingScripts.SetActive(true);
    }
}