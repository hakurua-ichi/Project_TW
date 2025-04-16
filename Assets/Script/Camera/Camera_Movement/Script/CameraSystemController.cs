using UnityEngine;

[RequireComponent(typeof(RotationInput))]
[RequireComponent(typeof(RotationPositionCalculator))]
[RequireComponent(typeof(RotationInterpolator))]
[RequireComponent(typeof(TrackingModeController))]
[RequireComponent(typeof(PlayerPositionTracker))]
[RequireComponent(typeof(CameraValueInitializer))]
public class CameraSystemController : MonoBehaviour, ICameraSystem
{
    [SerializeField] private Transform playerTransform;
    
    // 컴포넌트 참조
    private RotationInput rotationInput;
    private RotationPositionCalculator positionCalculator;
    private RotationInterpolator rotationInterpolator;
    private TrackingModeController trackingController;
    private PlayerPositionTracker positionTracker;
    private CameraValueInitializer valueInitializer;
    
    // 이벤트
    public event System.Action<float> RotationCompleted;
    
    // 프로퍼티
    public bool IsRotating => rotationInterpolator != null && rotationInterpolator.IsRotating;
    
    private void Awake()
    {
        InitializeComponents();
        SetupEventSubscriptions();
    }
    
    private void Start()
    {
        SetupPlayerReference();
        DisableLegacyScripts();
        valueInitializer.Initialize(transform, playerTransform);
    }
    
    private void InitializeComponents()
    {
        rotationInput = GetComponent<RotationInput>();
        positionCalculator = GetComponent<RotationPositionCalculator>();
        rotationInterpolator = GetComponent<RotationInterpolator>();
        trackingController = GetComponent<TrackingModeController>();
        positionTracker = GetComponent<PlayerPositionTracker>();
        valueInitializer = GetComponent<CameraValueInitializer>();
    }
    
    private void SetupEventSubscriptions()
    {
        // 이벤트 연결
        rotationInput.RotationRequested += OnRotationRequested;
        positionCalculator.PositionCalculated += rotationInterpolator.StartRotation;
        rotationInterpolator.RotationFinished += OnRotationFinished;
        trackingController.TrackingModeChanged += positionTracker.SetTrackingMode;
        valueInitializer.InitializationCompleted += OnInitializationCompleted;
    }
    
    private void OnInitializationCompleted(float distance, float height, float angle)
    {
        positionCalculator.Initialize(playerTransform, height, angle);
        trackingController.UpdateTrackingMode(transform.eulerAngles.y);
    }
    
    private void SetupPlayerReference()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogError("플레이어를 찾을 수 없습니다!");
        }
        
        positionTracker.SetPlayerTransform(playerTransform);
    }
    
    private void DisableLegacyScripts()
    {
        CameraFollow legacyScript = GetComponent<CameraFollow>();
        if (legacyScript != null)
            legacyScript.enabled = false;
    }
    
    private void OnRotationRequested(float angle)
    {
        if (IsRotating) return;
        positionCalculator.CalculateNewPosition(angle);
    }
    
    private void OnRotationFinished(float finalAngle)
    {
        positionCalculator.SetCurrentAngle(finalAngle);
        trackingController.UpdateTrackingMode(finalAngle);
        RotationCompleted?.Invoke(finalAngle);
    }
    
    private void Update()
    {
        if (IsRotating)
        {
            rotationInterpolator.UpdateRotation();
        }
        else
        {
            positionTracker.UpdatePosition();
        }
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (rotationInput != null)
            rotationInput.RotationRequested -= OnRotationRequested;
        
        if (positionCalculator != null && rotationInterpolator != null)
            positionCalculator.PositionCalculated -= rotationInterpolator.StartRotation;
        
        if (rotationInterpolator != null)
            rotationInterpolator.RotationFinished -= OnRotationFinished;
        
        if (trackingController != null && positionTracker != null)
            trackingController.TrackingModeChanged -= positionTracker.SetTrackingMode;
        
        if (valueInitializer != null)
            valueInitializer.InitializationCompleted -= OnInitializationCompleted;
    }
}