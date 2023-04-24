using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Editor;

public class ScriptableObjectLoader : MonoBehaviour
{
    [SerializeField] private Level _levelToLoad;
    [SerializeField] private GameObject _levelContainer;
    [SerializeField] private List<SaveLevelPrefab> _prefabList = new();
    [SerializeField] private List<GameObject> _cardPrefabs = new();
    [SerializeField] private GameObject _playerHand; //TODO move to game manager when head's clear


    public delegate void LevelLoaded();
    public static event LevelLoaded onLevelLoaded;
    void Start()
    {
        LoadLevel();
        for (int i = 0; i < _levelToLoad.MoveOne; i++)
        {
            GameObject cardPrefab = null;

            cardPrefab = Instantiate(_cardPrefabs[0],_playerHand.transform.position, Quaternion.identity);
            cardPrefab.transform.SetParent(_playerHand.transform,false);
            GameManager.Cards.Add(cardPrefab);
        }
        for (int i = 0; i < _levelToLoad.MoveTwo; i++)
        {
            GameObject cardPrefab = null;

            cardPrefab = Instantiate(_cardPrefabs[1], _playerHand.transform.position, Quaternion.identity);
            cardPrefab.transform.SetParent(_playerHand.transform, false);
            GameManager.Cards.Add(cardPrefab);
        }
        for (int i = 0; i < _levelToLoad.MoveThree; i++)
        {
            GameObject cardPrefab = null;

            cardPrefab = Instantiate(_cardPrefabs[2], _playerHand.transform.position, Quaternion.identity);
            cardPrefab.transform.SetParent(_playerHand.transform, false);
            GameManager.Cards.Add(cardPrefab);
        }
        for (int i = 0; i < _levelToLoad.MoveFour; i++)
        {
            GameObject cardPrefab = null;

            cardPrefab = Instantiate(_cardPrefabs[3], _playerHand.transform.position, Quaternion.identity);
            cardPrefab.transform.SetParent(_playerHand.transform, false);
            GameManager.Cards.Add(cardPrefab);
        }
        //TODO
        //A SOMEWHAT LESS DUMB INSTANTIATION PLEASE JAHSDJHA AH
        //ON THE GAME MANAGER (then it can keep track of already instantiated objects and not just values aka
        //easier to access
        //Add later a list to keep track of which cards were use for undo button
    }

    private void LoadLevel()
    {
        if(_levelToLoad == null)
        {
            Debug.LogError("No Level Object");
            return;
        }

        foreach (TileObject tileObject in _levelToLoad.tileObjects)
        {
            GameObject prefab = null;
            foreach(SaveLevelPrefab levelPrefab in _prefabList)
            {
                if(tileObject.Type == levelPrefab.Type)
                {
                    prefab = levelPrefab.Prefab;
                    break;
                }
            }
            if (prefab == null)
            {
                Debug.LogError("Couldn't find prefab of type: " + tileObject.Type.ToString());
                continue;
            }

            GameObject newTileInstance = Instantiate(prefab, _levelContainer.transform);
            newTileInstance.name = $"X: {newTileInstance.transform.position.x} | Z: {newTileInstance.transform.position.z}";

            //Jesus dude
            newTileInstance.transform.position = new Vector3(tileObject.Position[0], tileObject.Position[1], tileObject.Position[2]);
            newTileInstance.GetComponent<Tile>().InteractableID = tileObject.InteractableID;
            newTileInstance.GetComponent<Tile>().Direction = new Vector2(tileObject.Direction[0], tileObject.Direction[1]);
            newTileInstance.GetComponent<Tile>().Distance = tileObject.Distance;

            foreach (Transform child in _levelContainer.transform)
            {
                GameManager.TileGameObjects.Add(child.gameObject);
            }
        }
        onLevelLoaded();
    }
}
