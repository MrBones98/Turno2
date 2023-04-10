using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static List<GameObject> TileGameObjects = new();

    [OnValueChanged("AssignPlayer")]
    private GameObject _bot;

    public delegate void OnGameStart();
    public static event OnGameStart onGameStarted;
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadLevel;
    }
    public void GiveChosenBotDirection(Vector3 direction)
    {
        _bot.GetComponent<Bot>().Move(direction);
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.GetComponent<Bot>())
                {
                    print("Hit a bot");
                    _bot = hitInfo.collider.gameObject;
                    //Click on him better to simulate the card grabbing for now,
                    //raise event from Bot Component, activate UI
                    //that caches bot
                }
            }
        }
    }

    private void AssignPlayer()
    {
        if (_bot == null)
        {
            Debug.LogWarning("Object Player not found, cache it from event");
        }
    }
    private void OnDisable()
    {
        ScriptableObjectLoader.onLevelLoaded -= LoadLevel;
    }

    private void LoadLevel()
    {
        onGameStarted();
    }
}
