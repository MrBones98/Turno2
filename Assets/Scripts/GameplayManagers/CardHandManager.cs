using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    public static CardHandManager Instance;

    [SerializeField] private GameObject[] _slots;
    [SerializeField] private GameObject _cardContainer;
    [SerializeField] private List<GameObject> _cardPrefabs = new();
    [SerializeField] private bool _isActive = false;

    private Vector3 _cardContainerPos;

    private int _slotsCount;
    private Level _level;
    private void OnEnable()
    {
        //ScriptableObjectLoader.onLevelLoaded += LoadCards;
        GameManager.onGameStarted += LoadCards;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        if (_slots == null)
        {
            _slotsCount = 0;
            Debug.LogWarning($"No slots referenced inside of Inspector: {gameObject.name}");
        }
        else
        {
            _slotsCount = _slots.Length;
        }
        
    }
    public void DebugGiveMoveCard(int amount)
    {
        GameObject card = Instantiate(_cardPrefabs[amount - 1], _cardContainer.transform.position, Quaternion.identity);
        card.transform.SetParent(_cardContainer.transform, true);
        GameManager.Cards.Add(card);
    }
    private void LoadCards()
    {
        _cardContainer = GameObject.FindGameObjectWithTag("CardContainer");
        _cardContainerPos = _cardContainer.transform.position;
        _cardContainer.transform.position += new Vector3(0, -1000, 0);
            //_slots = GameObject.FindGameObjectsWithTag("CardSlot");

        _level = ScriptableObjectLoader.Instance.LevelToLoad;
        if (_isActive)
        {
            for (int i = 0; i < _level.MoveOne; i++)
            {
                SpawnCard(_cardPrefabs[0], _cardContainer.transform);
            }
            for (int i = 0; i < _level.MoveTwo; i++)
            {
                SpawnCard(_cardPrefabs[1], _cardContainer.transform);
            }
            for (int i = 0; i < _level.MoveThree; i++)
            {
                SpawnCard(_cardPrefabs[2], _cardContainer.transform);
            }
            for (int i = 0; i < _level.MoveFour; i++)
            {
                SpawnCard(_cardPrefabs[3], _cardContainer.transform);
            }
            for (int i = 0; i < _level.JumpCardTwo; i++)
            {
                SpawnCard(_cardPrefabs[4], _cardContainer.transform);
            }
        }
        
        _cardContainer.transform.DOMove(_cardContainerPos, 2).SetEase(Ease.OutQuint);
    }


    private void SpawnCard(GameObject prefab, Transform containerTransform)
    {
        GameObject card = Instantiate(prefab, containerTransform.position, Quaternion.identity);
        card.transform.SetParent(containerTransform.transform, true);
        GameManager.Cards.Add(card);
    }

    private void OnDisable()
    {
        //ScriptableObjectLoader.onLevelLoaded -= LoadCards;
        GameManager.onGameStarted -= LoadCards;
    }
    private void OrganizeSlots()
    {

        for (int i = 0; i < _slots.Length; i++)
        {
            foreach (Transform cardTransform in _slots[i].transform)
            {
                //if()
            }

        }
    }
}
