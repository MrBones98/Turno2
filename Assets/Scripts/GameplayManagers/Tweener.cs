using Sirenix.OdinInspector;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    //Bot
    [FoldoutGroup("Bot Variables")] [SerializeField] [Range(0.5f,2f)] private float _botStepDelay;
    [FoldoutGroup("Bot Variables")] [SerializeField] private float _botStepSpeed; // will have to change movement async function call on GameManager (could also just call from bot tbh(
    [FoldoutGroup("Bot Variables")] [SerializeField] private float _collisionCheckDelay;
    [FoldoutGroup("Bot Variables")] [SerializeField] [Range(0.1f,0.5f)] private float _rotationSpeed;
    [FoldoutGroup("Bot Variables")] [SerializeField] private float _rotaionDuration;
    [FoldoutGroup("Bot Variables")] [SerializeField] private float _botFallSpeed;
    [FoldoutGroup("Bot Variables")] [SerializeField] [Range(0.3f,0.8f)] private float _botHighlightHeight;


    [FoldoutGroup("Card Variables")] [SerializeField] private float _cardInstantiationSpeed;
    [FoldoutGroup("Card Variables")] [SerializeField] private float _cardFanningOutSpeed;
    [FoldoutGroup("Card Variables")] [SerializeField] private float _cardFanningInSpeed;
    [FoldoutGroup("Card Variables")] [SerializeField] private float _cardPopOutHeight;

    [SerializeField] private float _pushableBoxStepSpeed;
    [SerializeField] private float _pushableBoxFallSpeed;
    
    [FoldoutGroup("MovingPlatform")] [SerializeField] private float _movingPlatformSpeed;

    [FoldoutGroup("RainingAnimationVariables")] [SerializeField] private float _rainInAnimation;

    [SerializeField] private float _boxCollisionCheckDelay;
    #region Properties
    public float BotStepDelay { get { return _botStepDelay; } }
    public float BotStepSpeed { get { return _botStepSpeed; } }
    public float CollisionCheckDelay { get { return _collisionCheckDelay; } }
    public float CardInstantiationSpeed { get { return _cardInstantiationSpeed; } }
    public float CardFanningOutSpeed { get { return _cardFanningOutSpeed;} }
    public float CardFanningInSpeed { get { return _cardFanningInSpeed;} }
    public float BotRotationSpeed { get { return _rotationSpeed; } }
    public float BotRotationDuration {  get { return _rotaionDuration; }  }
    public float PushableBoxStepSpeed { get { return _pushableBoxStepSpeed; } }
    public float BotFallSpeed { get { return _botFallSpeed; } }
    public float BotHighlightHeight { get { return _botHighlightHeight; } }
    public float CardOnHoverHeight { get {return _cardPopOutHeight; } }
    #endregion

    public static  Tweener Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
