using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    public static CardHandManager Instance;

    [SerializeField] private GameObject[] _slots;
    [SerializeField] private GameObject _cardContainer;
    [SerializeField] private List<GameObject> _cardPrefabs = new();
    [SerializeField] private bool _isActive = false;

    private int _slotsCount;
    private Level _level;
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadCards;
        
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
            //_slots = GameObject.FindGameObjectsWithTag("CardSlot");
        _cardContainer = GameObject.FindGameObjectWithTag("CardContainer");

        _level = ScriptableObjectLoader.Instance.LevelToLoad;
        if (_isActive)
        {
            for (int i = 0; i < _level.MoveOne; i++)
            {
                GameObject card = Instantiate(_cardPrefabs[0], _cardContainer.transform.position, Quaternion.identity);
                card.transform.SetParent(_cardContainer.transform, true);
                GameManager.Cards.Add(card);
            }
            for (int i = 0; i < _level.MoveTwo; i++)
            {
                GameObject card = Instantiate(_cardPrefabs[1], _cardContainer.transform.position, Quaternion.identity);
                card.transform.SetParent(_cardContainer.transform, true);
                GameManager.Cards.Add(card);
            }
            for (int i = 0; i < _level.MoveThree; i++)
            {
                GameObject card = Instantiate(_cardPrefabs[2], _cardContainer.transform.position, Quaternion.identity);
                card.transform.SetParent(_cardContainer.transform, true);
                GameManager.Cards.Add(card);
            }
            for (int i = 0; i < _level.MoveFour; i++)
            {
                GameObject card = Instantiate(_cardPrefabs[3], _cardContainer.transform.position, Quaternion.identity);
                card.transform.SetParent(_cardContainer.transform, true);
                GameManager.Cards.Add(card);
            }
        }

    }

    private void OnDisable()
    {
        ScriptableObjectLoader.onLevelLoaded -= LoadCards;
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
