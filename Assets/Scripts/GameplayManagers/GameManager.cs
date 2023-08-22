using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Threading.Tasks;
using DG.Tweening;
using System;
using Editor;
using Assets.Scripts.Interactables;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static List<GameObject> TileGameObjects = new();
    public static List<GameObject> Cards = new();
    public static List<GameObject> SpawnedObjects = new();
    public static Dictionary<Vector3, Tile> TilesDictionary = new();
    public static Dictionary<Vector3, InteractableObject> Interactables = new();

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private float _highlightHeight;
    [SerializeField] private GameObject _deathPlatformVisual;
    [SerializeField] [Range(0.5f, 3.0f)] private float _rainInDuration;
    [SerializeField] private LayerMask _highlightPathLayer;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private GameObject _botParentGameObject;

    public Draggable CurrentDraggable { get { return _currentDraggable; } set { _currentDraggable = value; } }

    //[OnValueChanged("AssignPlayer")]
    private Draggable _currentDraggable = null;
    private GameObject _currentCard = null;

    private ActionCardData _currentCardData = null;

    private GameObject _bot;
    private GameObject _voidHighlightPlatformReference = null;
    private Camera _camera;
    private DirectionalInputBot _directionalInputBot;
    private List<Transform> _highlightedPath = new();
    private List<Transform> _higlightedInteractables = new();
    private List<GameObject> _deathHighlightObjects = new();
    private RaycastHit _hit;
    private Ray _interactableRay;
    private DirectionIs _raisingPathDirection;
    private int _currentBotStepCount;
    private int _voidDistance;
    private int _idReference;
    private bool _selectCheck = false;
    private bool _pathHighlighted = false;
    private bool _raycastCheck = false;
    //public WinTile WinTile;

    //ON THE LEVEL SO ADD COUNT OF BUTTONS FOR WINNING FOR DIFFERENT NEEED AMOUNTS

    public delegate void OnObjectsInstantiated();
    public static event OnObjectsInstantiated onObjectsInstantiated;
    public delegate void OnGameStart();
    public static event OnGameStart onGameStarted;
    public delegate void OnBotMove();
    public static event OnBotMove onBotMove;
    public delegate void OnUndoButtonPressed();
    public static event OnUndoButtonPressed onUndoButtonPressed;
    public delegate void OnBotDirectionSelected();
    public static event OnBotDirectionSelected onBotDirectionSelected;
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadLevel;
        ScriptableObjectLoader.onLevelQeued += async () => await ClearLevel();
        WinTile.onButtonPressed += FinishLevel;
        SwitchTile.onSwitchPressed += Activate;
        SwitchTile.onSwitchReleased += DeActivate;
        Bot.onFinishedMove += UpdateTurn;
        SwitchTile.onSwitchHighlighted += HighlightInteractable;
        WallTile.onWallHighlighted += HighlightInteractable;
        Bot.onStartedMove += CleanVisualOnBotMove;
        Draggable.onCardSelected += CacheCard;
        Draggable.onCardGiven += ResolveCardInteraction;
        GameSpaceUIController.onCardButtonClicked += CacheCardUpdated;
    }
    
    private void CacheCard(GameObject cardObject, Draggable draggable)
    {
        _currentDraggable = draggable;
        _currentCard = cardObject;
    }

    private void CacheCardUpdated(ActionCardData data)
    {
        _currentCardData = data;
    }
    private void ResolveCardInteraction()
    {
        print("Remember to delete this event on the card ty, also the onCardDropped (for the sound) move it");
    }
    private void BotCaching()
    {
        //print("Caching card with new gm function");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 40f, _playerLayer))
        {
            print(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.GetComponent<Bot>())
            {
                AssignPlayer(hitInfo.collider.gameObject);
                //print("card cached");
            }
            else
            {
                _bot = null;
            }
        }

        if (_bot != null)
        {
            //_currentDraggable.DropScaling();
            if (_currentCardData.isJump == false)
            {
                _bot.GetComponent<Bot>().SetDistance(_currentCardData.distance);
            }
            else
            {
                _bot.GetComponent<Bot>().SetJumpDistance(_currentCardData.distance);
            }
            //could save it in qeue jic
            //Destroy(_currentCard, 0.1f);
        }
        else
        {
            //_currentDraggable.ResetCard();
            //_currentDraggable = null;
            _currentCardData = null;
            _currentCard = null;
        }
    }
    private async void CleanVisualOnBotMove()
    {
        await Task.Yield();
        //await ClearPath();
    }

    private void HighlightInteractable(int id)
    {
        foreach (GameObject tiles in TileGameObjects)
        {
            if (tiles.GetComponent<Tile>().InteractableID == id && tiles.GetComponent<Tile>().IsHighlighted == false)
            {
                //print(tiles.name);

                //tiles.GetComponent<ISwitchActivatable>().HighlightInteractable(_highlightHeight);
                _higlightedInteractables.Add(tiles.transform);
                tiles.GetComponent<Tile>().IsHighlighted = true;

            }
        }
        foreach (Transform interactable in _higlightedInteractables)
        {

            //await PrettyHighlightAsync(interactable,true,_highlightHeight);
            interactable.transform.DOMoveY(_highlightHeight, 0.3f, false);

        }
        //_selectCheck = !_selectCheck;
    }
    private async Task PrettyHighlightAsync(Transform interactableTransform, bool up, float offset)
    {
        float height = offset;
        if (up == false)
        {
            height = 0;
        }

        interactableTransform.DOMoveY(height, 0.3f, false);
        await Task.Delay(100);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public async Task ClearLevel()
    {
        if (SceneLoader.Instance.GetCurrentSceneIndex() == 1)
        {
            await UnloadLevel();
        }
        if (TileGameObjects.Count != 0)
        {
            for (int i = 0; i < TilesDictionary.Count - 1; i++)
            {
                TilesDictionary.Remove(TileGameObjects[i].transform.position);
            }
            for (int i = TileGameObjects.Count - 1; i >= 0; i--)
            {
                Destroy(TileGameObjects[i]);
            }
            for (int i = Cards.Count - 1; i >= 0; i--)
            {
                Destroy(Cards[i]);
            }
            for (int i = SpawnedObjects.Count - 1; i >= 0; i--)
            {
                Destroy(SpawnedObjects[i]);
            }
            TileGameObjects.Clear();
            TilesDictionary.Clear();
            Cards.Clear();
            SpawnedObjects.Clear();
            Interactables.Clear();
            Destroy(_voidHighlightPlatformReference);
        }
        await Task.Yield();
    }

    private async Task UnloadLevel()
    {
        if (TileGameObjects.Count > 0)
        {
            await RainOutAnimation();
        }
    }

    private void UpdateTurn()
    {
        _highlightedPath.Clear();

        _bot = null;
        Invoke(nameof(BotMoved), 0.3f);

        //print(Interactables);
    }
    private void BotMoved()
    {
        onBotMove?.Invoke();
    }

    private void DeActivate(int id)
    {
        foreach (GameObject tiles in TileGameObjects)
        {
            if (tiles.GetComponent<Tile>().InteractableID == id)
            {
                tiles.GetComponent<ISwitchActivatable>().Deactivate();
            }
        }
    }

    private void Activate(int id)
    {
        foreach (GameObject tiles in TileGameObjects)
        {
            if (tiles.GetComponent<Tile>().InteractableID == id)
            {
                tiles.GetComponent<ISwitchActivatable>().Activate();
            }
        }
    }
    public async Task Resetlevel()
    {
        await ClearLevel();
        await Task.Yield();
    }
    public async Task UndoPressed()
    {
        onUndoButtonPressed?.Invoke();
        await Task.Yield();
        int countReference = _bot.GetComponent<Bot>().StepCount;
        if (_bot != null && countReference > 0)
        {
            //remove focused
            _bot.GetComponent<Bot>().IsFocused = false;
            //Check based ond StepCount or reference to given Card (Set !active when given Distance to player, it destroys itself after the movement?)
            CardHandManager.Instance.DebugGiveMoveCard(countReference);
            //set step count to 0
            _bot.GetComponent<Bot>().StepCount = 0;

        }
        if (_voidHighlightPlatformReference != null)
        {
            Destroy(_voidHighlightPlatformReference);
        }
    }
    public Tile FindTile(Vector3 keyPos)
    {
        //print($"Finding Tile at: {keyPos}");
        if (TilesDictionary.TryGetValue(keyPos, out Tile tile))
        {
            return tile;
        }
        else
        {
            return null;
        }
    }
    public InteractableObject FindInteractable(Vector3 keyPos)
    {
        if (Interactables.TryGetValue(keyPos, out InteractableObject interactable))
        {
            return interactable;
        }
        else
        {
            return null;
        }
    }
    private void FinishLevel()
    {
        //event for the gamespaceuicontroller to load WinScreen
        //_winScreen.SetActive(true);
        //_gameplayUI.SetActive(false);
    }
    private void UpdateInteractableRayCast()
    {
        _interactableRay = _camera.ScreenPointToRay(Input.mousePosition);
    }

    public async void GiveChosenBotDirection(DirectionIs directionIs)
    {
        Vector3 moveVector;

        if (directionIs == DirectionIs.PosX)
        {
            moveVector = Vector3.right;
        }
        else if (directionIs == DirectionIs.NegZ)
        {
            moveVector = -Vector3.forward;
        }
        else if (directionIs == DirectionIs.NegX)
        {
            moveVector = -Vector3.right;
        }
        else if (directionIs == DirectionIs.PosZ)
        {
            moveVector = Vector3.forward;
        }
        else
        {
            return;
        }

        var clearPathTask = ClearPath();
        await clearPathTask;
        _bot.GetComponent<Bot>().CheckMove(moveVector);
    }
    public async void GiveChosenBotJumpDirection(DirectionIs directionIs)
    {
        Vector3 moveVector;

        if (directionIs == DirectionIs.PosX)
        {
            moveVector = Vector3.right;
        }
        else if (directionIs == DirectionIs.NegZ)
        {
            moveVector = -Vector3.forward;
        }
        else if (directionIs == DirectionIs.NegX)
        {
            moveVector = -Vector3.right;
        }
        else if (directionIs == DirectionIs.PosZ)
        {
            moveVector = Vector3.forward;
        }
        else
        {
            return;
        }
        onBotDirectionSelected?.Invoke();
        var clearPathTask = ClearPath();
        await clearPathTask;
        _bot.GetComponent<Bot>().CheckMove(moveVector);
    }

    void Update()
    {

        if (_bot != null)
        {
            if (_bot.GetComponent<Bot>().IsFocused)
            {
                if (!_bot.GetComponent<Bot>().IsMoving)
                {
                    if (_pathHighlighted== false && _highlightedPath.Count<=0)
                    {
                        //ShowDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                        ShowDirectionDictionary(_botParentGameObject.transform.position, _bot.GetComponent<Bot>().StepCount);
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        GiveChosenBotDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                        ClearDeathTileVisuals();
                    }
                }
            }
        }
        if (_currentCardData != null && Input.GetMouseButtonDown(0)&& _bot==null)
        {
            BotCaching();
        }
        //TODO Check for Scene
        else if (_bot == null || (_bot != null && !_bot.GetComponent<Bot>().IsMoving))
        {
            HighlightInteractables();
        }
        //if (_bot == null ||(_bot!= null && ! _bot.GetComponent<Bot>().IsMoving))
        //{
        //    HighlightInteractables();
        //}
    }

    private void ClearDeathTileVisuals()
    {
        for (int i = 0; i <= _deathHighlightObjects.Count-1; i++)
        {
            Destroy(_deathHighlightObjects[i]);
        }
        _deathHighlightObjects.Clear();
    }

    private void HighlightInteractables()
    {
        if(SceneLoader.Instance.GetCurrentSceneIndex() == 1) //Gameplay Scene 
        { 
            if (Input.GetMouseButtonDown(0))
            {
                if (!_raycastCheck)
                {
                    _raycastCheck = true;
                    UpdateInteractableRayCast();
                    if (Physics.Raycast(_interactableRay, out _hit, 100, _highlightPathLayer))
                    {
                        _idReference = _hit.collider.transform.parent.GetComponent<Tile>().InteractableID;
                        if (_idReference != 0)
                        {
                            HighlightInteractable(_idReference);
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _raycastCheck = false;
                if (_idReference != 0)
                {
                    foreach (Transform interactable in _higlightedInteractables)
                    {
                        interactable.transform.DOMoveY(0, 0.3f, false);
                        interactable.GetComponent<Tile>().IsHighlighted = false;
                    }
                    _higlightedInteractables.Clear();
                }
            }

        } 
    }

    public async void ShowDirectionDictionary(Vector3 origin, int botStepCount)
    {
        Tile tile;
        InteractableObject interactable;
        int a = 0;int b = 0; int c = 0; int d = 0;
//        print($"a:{a},b:{b} c:{c}d:{d}");
        //print("Showing direction for bot");
        for (int i = 1; i < botStepCount+1; i++)
        {
            tile = FindTile(new Vector3(origin.x + i, 0, origin.z));
            interactable = FindInteractable(new Vector3(origin.x + i, 0, origin.z));
            if (tile != null && a == 0)
            {
                _highlightedPath.Add(tile.gameObject.transform);
            }
            else
            {
                if (a == 1)
                {
                    _voidHighlightPlatformReference = Instantiate(_deathPlatformVisual, new Vector3(origin.x+i,0, origin.z), Quaternion.identity);
                    _deathHighlightObjects.Add(_voidHighlightPlatformReference);
                }
                a = 1;
                
            }
            if(interactable != null)
            {
                _highlightedPath.Add(interactable.transform);
            }
            tile = FindTile(new Vector3(origin.x, 0, origin.z + i));
            interactable = FindInteractable(new Vector3(origin.x, 0, origin.z+i));
            if (tile != null && b== 0)
            {
                _highlightedPath.Add(tile.gameObject.transform);
            }
            else
            {
                if (b == 0)
                {
                    _voidHighlightPlatformReference = Instantiate(_deathPlatformVisual, new Vector3(origin.x, 0, origin.z + i), Quaternion.identity);
                    _deathHighlightObjects.Add(_voidHighlightPlatformReference);
                }
                b=1;
            }
            if(interactable != null)
            {
                _highlightedPath.Add(interactable.transform);
            }
            tile = FindTile(new Vector3(origin.x - i, 0, origin.z));
            interactable = FindInteractable(new Vector3(origin.x - i, 0, origin.z));
            if (tile != null && c==0)
            {
                _highlightedPath.Add(tile.gameObject.transform);
                tile = null;
            }
            else
            {
                if (c == 0)
                {
                    _voidHighlightPlatformReference = Instantiate(_deathPlatformVisual, new Vector3(origin.x - i, 0, origin.z), Quaternion.identity);
                    _deathHighlightObjects.Add(_voidHighlightPlatformReference);
                }
                c=1;
            }
            if (interactable != null)
            {
                _highlightedPath.Add(interactable.transform);
            }
            tile = FindTile(new Vector3(origin.x,0,origin.z - i));
            interactable = FindInteractable(new Vector3(origin.x, 0, origin.z-i));
            if (tile != null && d==0)
            {
                _highlightedPath.Add(tile.gameObject.transform);
                tile = null;
            }
            else
            {
                if (d == 0)
                {
                    _voidHighlightPlatformReference = Instantiate(_deathPlatformVisual, new Vector3(origin.x, 0, origin.z - i), Quaternion.identity);
                    _deathHighlightObjects.Add(_voidHighlightPlatformReference);
                }
                d=1;
            }
            if (interactable != null)
            {
                _highlightedPath.Add(interactable.transform);
            }
            await Task.Yield();
        }
        
        foreach  (Transform transform in _highlightedPath)
        {
            transform.DOMoveY(transform.position.y + _highlightHeight, 0.3f, false);
        }

        _pathHighlighted = true;
    }
    //async
    public async void ShowDirection(DirectionIs direction)
    {
        Vector3 rayOrientation;

        rayOrientation = MoveUtils.SetDirection(direction);
        
        RaycastHit[] platformsToRaise = _bot.GetComponent<Bot>().PlatformsToRaise(rayOrientation);
        if (_bot.GetComponent<Bot>().StepCount > platformsToRaise.Length)
        {
            if(platformsToRaise.Length > 0)
            {
                if (platformsToRaise.Length > 1)
                {
                    int distance = (int)Vector3.Distance(platformsToRaise[0].collider.gameObject.transform.position,_bot.transform.position);
                    if(distance > 1)
                    {
                        //_voidDistance = _bot.GetComponent<Bot>().StepCount - distance;
                        _voidDistance = 1;
                        print(distance);
                    }
                    else if (distance == 1)
                    {
                        
                        distance = (int)Vector3.Distance(platformsToRaise[1].collider.gameObject.transform.position, _bot.transform.position);
                        if(distance > 2)
                        {
                            _voidDistance =distance-1;
                        }
                        else
                        {
                            _voidDistance = 0;
                            Destroy(_voidHighlightPlatformReference);

                        }
                        print(_voidDistance);
                        
                    }
                    else
                    {
                        _voidDistance = _bot.GetComponent<Bot>().StepCount;
                        Destroy(_voidHighlightPlatformReference);
                    }
                }
            }
            else
            {
                _voidDistance = 1;
            }
           
        }
        else if(platformsToRaise.Length==0)
        {
            _voidDistance = 1;
        }
        else
        {
            _voidDistance = 0;
            Destroy(_voidHighlightPlatformReference);
        }
        
        if (MoveUtils.SetDirection(_raisingPathDirection) == Vector3.zero || _raisingPathDirection != direction)
        {
            await ClearPath();
            foreach (var item in platformsToRaise)
            {
                Transform platformToShow = item.collider.transform.parent.transform;
                float yValue = platformToShow.position.y;
                
                _highlightedPath.Add(platformToShow);
            }
            foreach (Transform transform in _highlightedPath)
            {
                transform.DOMoveY(transform.position.y + _highlightHeight, 0.3f, false);
            }

        }
        if (_voidDistance > 0)
        {
            if (_voidHighlightPlatformReference == null)
            {
            }
            else
            {
                Destroy(_voidHighlightPlatformReference);
            }
            _voidHighlightPlatformReference = Instantiate(_deathPlatformVisual,new Vector3(_bot.transform.position.x, 0, _bot.transform.position.z) 
                                                                                + (MoveUtils.SetDirection(direction)*_voidDistance), Quaternion.identity);
        }
        

        _raisingPathDirection = direction;
    }
    //async

    private async Task ClearPath()
    {
        _pathHighlighted = false;
        if (_highlightedPath.Count > 0)
        {
            //foreach (Transform transform in _highlightedPath)
            //{
            //    if((transform.position.y > _highlightHeight && transform.position.y < 0.4f)|| ((transform.position.y - _highlightHeight) > 0.44 ))
            //    {
            //        transform.DOMoveY(transform.position.y - _highlightHeight, 0.3f, false);
            //    }
            //    else
            //    {
            //        transform.DOMoveY(0, 0.3f, false);
            //    }
            //}
            foreach (Transform transform in _highlightedPath)
            {
                transform.DOMoveY(transform.position.y - _highlightHeight, 0.3f, false);

            }
        }
        await Task.Yield();
    }

    public void AssignPlayer(GameObject selectedBot)
    {
        _bot = selectedBot;
        _directionalInputBot = _bot.GetComponent<DirectionalInputBot>();
        _botParentGameObject = _bot.transform.parent.transform.parent.gameObject;
    }
    private void OnDisable()
    {
        ScriptableObjectLoader.onLevelLoaded -= LoadLevel;
        ScriptableObjectLoader.onLevelQeued -= async () => await ClearLevel();
        WinTile.onButtonPressed -= FinishLevel;
        SwitchTile.onSwitchPressed -= Activate;
        SwitchTile.onSwitchReleased += DeActivate;
        Bot.onFinishedMove -= UpdateTurn;
        SwitchTile.onSwitchHighlighted -= HighlightInteractable;
        WallTile.onWallHighlighted -= HighlightInteractable;
        Bot.onStartedMove -= CleanVisualOnBotMove;
    }
    public void AddToTileToDictionary(Vector3 pos, Tile tile)
    {
        if (TilesDictionary.ContainsKey(pos))
        {
            Debug.LogWarning($"Tile{ gameObject.name} is already added.");
        }
        else
        {
            TilesDictionary.Add(pos, tile);
        }
    }
    public void ClearTileDictionary()
    {
        TilesDictionary.Clear();
    }
    public void AddInteractableToDictionary(Vector3 pos, InteractableObject interactable)
    {
        Interactables.Add(pos, interactable);
    }
    private async void LoadLevel()
    {
        onObjectsInstantiated?.Invoke();
        Level levelToLoad = ScriptableObjectLoader.Instance.LevelToLoad;
        if (levelToLoad != null)
        {
            GameSpaceUIController uIController = FindObjectOfType<GameSpaceUIController>();
            uIController.SetLevelNameText(levelToLoad.Name);
        }
        await RainInAnimation();
        //if(TilesDictionary.Count <= 0)
        //{
            //for (int i = 0; i < TileGameObjects.Count-1; i++)
            //{
            //    TilesDictionary.Add(new Vector3(TileGameObjects[i].transform.position.x, 0, TileGameObjects[i].transform.position.z), TileGameObjects[i].GetComponent<Tile>());
            //    print("Went into the dictionary saving system");
            //    //print($"Tile Objects: {TileGameObjects.Count}");
            //    print($"Tile Dictionary: {TilesDictionary.Count}");
            //}
        //}
        //for (int i = 0; i < TileGameObjects.Count - 1; i++)
        //{
        //    print($"Tile at :{TileGameObjects[i].transform.position}");
        //}
        //This after the raining in animation
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        await Task.Yield();
    }
    private async Task RainInAnimation()
    {
        foreach (GameObject tile in TileGameObjects)
        {
            RainInTween(tile.transform);
            tile.GetComponent<Tile>().ReferenceToDictionary();  
        }
        foreach (GameObject interactable in SpawnedObjects)
        {
            float randomSpeed = 1f;
            interactable.transform.DOMoveY(0.45f, randomSpeed, false).SetEase(Ease.InQuint);
            if (interactable.GetComponentInChildren<Bot>())
            {
                await interactable.GetComponentInChildren<Bot>().CheckForLanding();
            }

        }
        foreach (var interact in Interactables)
        {
            //print($"{interact.Key} + {interact.Value.Type}");
        }
        await Task.Yield();
        onGameStarted?.Invoke();


    }
    private async Task RainOutAnimation()
    {
        foreach (GameObject tile in TileGameObjects)
        {
              RainOutTween(tile.transform);
        }
        foreach (GameObject interactable in SpawnedObjects)
        {
             RainOutTween(interactable.transform);

        }
        await Task.Delay(300);
    }
    private async void RainOutTween(Transform transform)
    {
        transform.DOMoveY(-6f,_rainInDuration*0.25f,false).SetEase(Ease.InQuart);
        await Task.Delay(100);
    }
    private async void RainInTween(Transform transform) 
    {
        transform.DOMoveY(0, _rainInDuration, false).SetEase(Ease.OutBack);
        await Task.Delay(100);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
    }
}
