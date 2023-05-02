using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _slots;
    [SerializeField] private List<GameObject> _cardPrefabs = new();
    [SerializeField] private bool _isActive = false;

    private int _slotsCount;
    private Level _level;
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadCards;
        
    }

    private void LoadCards()
    {
        _level = ScriptableObjectLoader.Instance.LevelToLoad;
        if (_isActive)
        {
            for (int i = 0; i < _level.MoveOne; i++)
            {
                GameObject card = null;

                card = Instantiate(_cardPrefabs[0], _slots[0].transform.position, Quaternion.identity);
                card.transform.SetParent(_slots[0].transform, false);
                GameManager.Cards.Add(card);
            }
            for (int i = 0; i < _level.MoveTwo; i++)
            {
                GameObject card = null;

                card = Instantiate(_cardPrefabs[1], _slots[1].transform.position, Quaternion.identity);
                card.transform.SetParent(_slots[1].transform, false);
                GameManager.Cards.Add(card);
            }
            for (int i = 0; i < _level.MoveThree; i++)
            {
                GameObject card = null;

                card = Instantiate(_cardPrefabs[2], _slots[2].transform.position, Quaternion.identity);
                card.transform.SetParent(_slots[2].transform, false);
                GameManager.Cards.Add(card);
            }
            for (int i = 0; i < _level.MoveFour; i++)
            {
                GameObject card = null;

                card = Instantiate(_cardPrefabs[3], _slots[3].transform.position, Quaternion.identity);
                card.transform.SetParent(_slots[3].transform, false);
                GameManager.Cards.Add(card);
            }
        }

    }

    private void OnDisable()
    {
        ScriptableObjectLoader.onLevelLoaded -= LoadCards;
    }
    private void Start()
    {
        
    }
    private void Awake()
    {
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
