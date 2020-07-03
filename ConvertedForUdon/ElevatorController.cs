using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ElevatorController : UdonSharpBehaviour
{
    public NetworkingController _networkController;
    public ElevatorSoundController _elevatorSoundController;
    [HideInInspector]
    public int _floorLevel = 0;
    //public bool _isReceptionController = false;
    #region ElevatorObjects
    public BoxCollider _elevator1DoorCollider;
    public BoxCollider _elevator2DoorCollider;
    public BoxCollider _elevator3DoorCollider;
    /// <summary>
    /// Trigger area to check if player is inside the elevator door
    /// </summary>
    public BoxCollider _playerInsideDoorElevator1Detector;
    public BoxCollider _playerInsideDoorElevator2Detector;
    public BoxCollider _playerInsideDoorElevator3Detector;
    /// <summary>
    /// Trigger area to check if player is inside the elevator
    /// </summary>
    public BoxCollider _playerInsideElevator1Detector;
    public BoxCollider _playerInsideElevator2Detector;
    public BoxCollider _playerInsideElevator3Detector;
    /// <summary>
    /// The callbutton panel for the elevator
    /// </summary>
    public CallButtonDesktopFloor _elevatorCallPanelDesktop_1;
    public CallButtonDesktopFloor _elevatorCallPanelDesktop_2;
    public ElevatorCallButton _elevatorCallPanelForVR_1;
    public ElevatorCallButton _elevatorCallPanelForVR_2;
    /// <summary>
    /// Elevator Animators
    /// </summary>
    public Animator _animator1;
    public Animator _animator2;
    public Animator _animator3;
    /// <summary>
    /// Gameobjects to disable when elevator is closed
    /// </summary>
    public GameObject _elevator1;
    public GameObject _elevator2;
    public GameObject _elevator3;
    /// <summary>
    /// Gameobjects to get the elevator display renderer from
    /// </summary>
    public GameObject _elevator1numberDisplay_LOD0;
    public GameObject _elevator1numberDisplay_LOD1;
    public GameObject _elevator1numberDisplay_LOD2;
    public GameObject _elevator2numberDisplay_LOD0;
    public GameObject _elevator2numberDisplay_LOD1;
    public GameObject _elevator2numberDisplay_LOD2;
    public GameObject _elevator3numberDisplay_LOD0;
    public GameObject _elevator3numberDisplay_LOD1;
    public GameObject _elevator3numberDisplay_LOD2;
    /// <summary>
    /// Mirrors inside the elevators
    /// </summary>
    public GameObject _mirror1;
    public GameObject _mirror2;
    public GameObject _mirror3;
    /// <summary>
    /// Loli-stairs inside the elevators
    /// </summary>
    public GameObject _loliStairs1;
    public GameObject _loliStairs2;
    public GameObject _loliStairs3;
    /// <summary>
    /// GameObjects to enable when elevator is defect
    /// </summary>
    public GameObject _defectSign1;
    public GameObject _defectSign2;
    public GameObject _defectSign3;
    /// <summary>
    /// Top state panels of elevators
    /// </summary>
    public GameObject _topStatePanel1;
    public GameObject _topStatePanel2;
    public GameObject _topStatePanel3;
    /// <summary>
    /// Renderer from top state panels of elevators
    /// </summary>
    private Renderer _topStatePanelRenderer1;
    private Renderer _topStatePanelRenderer2;
    private Renderer _topStatePanelRenderer3;
    /// <summary>
    /// Renderer from inside the elevator display
    /// </summary>
    private Renderer _elevator1numberDisplayRenderer_LOD0;
    private Renderer _elevator1numberDisplayRenderer_LOD1;
    private Renderer _elevator1numberDisplayRenderer_LOD2;
    private Renderer _elevator2numberDisplayRenderer_LOD0;
    private Renderer _elevator2numberDisplayRenderer_LOD1;
    private Renderer _elevator2numberDisplayRenderer_LOD2;
    private Renderer _elevator3numberDisplayRenderer_LOD0;
    private Renderer _elevator3numberDisplayRenderer_LOD1;
    private Renderer _elevator3numberDisplayRenderer_LOD2;
    /// <summary>
    /// Panel GameObjects that need height adjustements
    /// </summary>
    public Transform _insideControlPanel1;
    public Transform _insideControlPanel2;
    public Transform _insideControlPanel3;
    public Transform _outsideControlPanel1;
    public Transform _outsideControlPanel2;
    public Transform _insideControlPanel1ScriptObjects;
    public Transform _insideControlPanel2ScriptObjects;
    public Transform _insideControlPanel3ScriptObjects;
    public Transform _outsideControlPanel1ScriptObjects;
    public Transform _outsideControlPanel2ScriptObjects;
    #endregion ElevatorObjects
    /// <summary>
    /// Panel height settings
    /// </summary>
    private float MAX_PANEL_HEIGHT = 0.25f;
    private float MIN_PANEL_HEIGHT = -0.646f;
    [HideInInspector]
    public float _avatarHeight = 1.1f;
    private VRCPlayerApi _localPlayer;
    private bool _scriptIsLoaded = false;
    /// <summary>
    /// Is called externally when the avatar height changed
    /// </summary>
    public void OnAvatarChanged()
    {
        if (!_scriptIsLoaded)
            return;
        Debug.Log($"[ElevatorController] Received OnAvatarChanged() for playerHeight {_avatarHeight}");
        //return; //the world is not ready for this (can't take in floor offset right now since we're going up/down)
        float insidePanelHeight = Mathf.Clamp(_avatarHeight - 1.1f, MIN_PANEL_HEIGHT, MAX_PANEL_HEIGHT);
        Debug.Log($"[ElevatorController] Adjusted panels to avatar height inside: " + insidePanelHeight);
        ChangeHeight(_insideControlPanel1, _insideControlPanel1ScriptObjects, insidePanelHeight);
        ChangeHeight(_insideControlPanel2, _insideControlPanel2ScriptObjects, insidePanelHeight);
        ChangeHeight(_insideControlPanel3, _insideControlPanel3ScriptObjects, insidePanelHeight);
        float outsidePanelHeight = Mathf.Clamp(_avatarHeight - 1.1f, MIN_PANEL_HEIGHT, MAX_PANEL_HEIGHT);
        Debug.Log($"[ElevatorController] Adjusted panels to avatar height outside: " + outsidePanelHeight);
        ChangeHeight(_outsideControlPanel1, _outsideControlPanel1ScriptObjects, outsidePanelHeight);
        ChangeHeight(_outsideControlPanel2, _outsideControlPanel2ScriptObjects, outsidePanelHeight);
    }
    /// <summary>
    /// Changes a transform to the passed newHeight
    /// </summary>
    private void ChangeHeight(Transform meshTransform, Transform scriptObjectTransform, float newHeight)
    {
        meshTransform.localPosition = new Vector3(meshTransform.localPosition.x, newHeight, meshTransform.localPosition.z);
        scriptObjectTransform.localPosition = new Vector3(scriptObjectTransform.localPosition.x, newHeight, scriptObjectTransform.localPosition.z);
    }
    /// <summary>
    /// Initializing the scene
    /// </summary>
    public void CustomStart(float buttonVolume)
    {
        Debug.Log("[ElevatorController] is now in Start()");
        _localPlayer = Networking.LocalPlayer;
        //getting material renderer
        _topStatePanelRenderer1 = _topStatePanel1.GetComponent<Renderer>();
        _topStatePanelRenderer2 = _topStatePanel2.GetComponent<Renderer>();
        _topStatePanelRenderer3 = _topStatePanel3.GetComponent<Renderer>();
        _elevator1numberDisplayRenderer_LOD0 = _elevator1numberDisplay_LOD0.GetComponent<Renderer>();
        _elevator1numberDisplayRenderer_LOD1 = _elevator1numberDisplay_LOD1.GetComponent<Renderer>();
        _elevator1numberDisplayRenderer_LOD2 = _elevator1numberDisplay_LOD2.GetComponent<Renderer>();
        _elevator2numberDisplayRenderer_LOD0 = _elevator2numberDisplay_LOD0.GetComponent<Renderer>();
        _elevator2numberDisplayRenderer_LOD1 = _elevator2numberDisplay_LOD1.GetComponent<Renderer>();
        _elevator2numberDisplayRenderer_LOD2 = _elevator2numberDisplay_LOD2.GetComponent<Renderer>();
        _elevator3numberDisplayRenderer_LOD0 = _elevator3numberDisplay_LOD0.GetComponent<Renderer>();
        _elevator3numberDisplayRenderer_LOD1 = _elevator3numberDisplay_LOD1.GetComponent<Renderer>();
        _elevator3numberDisplayRenderer_LOD2 = _elevator3numberDisplay_LOD2.GetComponent<Renderer>();
        //setting elevators into closed state
        _topStatePanelRenderer1.materials[0].DisableKeyword("_EMISSION");
        _topStatePanelRenderer2.materials[0].DisableKeyword("_EMISSION");
        _topStatePanelRenderer3.materials[0].DisableKeyword("_EMISSION");
        _elevator1DoorCollider.enabled = true;
        _elevator2DoorCollider.enabled = true;
        _elevator3DoorCollider.enabled = true;
        _elevator1.SetActive(false);
        _elevator2.SetActive(false);
        _elevator3.SetActive(false);
        CloseElevator(0, false);
        CloseElevator(1, false);
        CloseElevator(2, false);
        //Setting up lower scripts
        _elevatorCallPanelDesktop_1.CustomStart(buttonVolume);
        _elevatorCallPanelDesktop_2.CustomStart(buttonVolume);
        _elevatorCallPanelForVR_1.CustomStart(buttonVolume);
        _elevatorCallPanelForVR_2.CustomStart(buttonVolume);
        if (_avatarHeight != 1.1f)
            OnAvatarChanged();
        _scriptIsLoaded = true;
    }
    //------------------------------------------ external calls -----------------------------------------------------------
    [HideInInspector]
    public bool _elevator1working;
    [HideInInspector]
    public bool _elevator2working;
    [HideInInspector]
    public bool _elevator3working;
    /// <summary>
    /// Setting up the scene at startup or when it isn't setup yet, this is called externally
    /// </summary>
    public void SetupElevatorStates()
    {
        if (_elevator1working)
        {
            _defectSign1.SetActive(false);
        }
        else
        {
            SetElevatorLevelOnDisplay(14, 0);
            // CloseElevator(0); //this function is external where numbers are zero-based
            _elevator1.SetActive(false);
            _defectSign1.SetActive(true);
        }
        //Setting up elevator 2
        if (_elevator2working)
        {
            _defectSign2.SetActive(false);
        }
        else
        {
            SetElevatorLevelOnDisplay(14, 1);
            //CloseElevator(1);
            _elevator2.SetActive(false);
            _defectSign2.SetActive(true);
        }
        //Setting up elevator 3
        if (_elevator3working)
        {
            _defectSign3.SetActive(false);
        }
        else
        {
            SetElevatorLevelOnDisplay(14, 2);
            //CloseElevator(2);
            _elevator3.SetActive(false);
            _defectSign3.SetActive(true);
        }
        Debug.Log($"[ElevatorController] Random elevator states are now set by localPlayer in floor {_floorLevel}");
    }
    /// <summary>
    /// Setting the display over the elevator to the floor number
    /// </summary>
    public void SetElevatorLevelOnDisplay(int floorNumber, int elevatorNumber)
    {
        if (elevatorNumber == 0)
        {
            _topStatePanelRenderer1.materials[0].SetInt("_Index", floorNumber);
            _elevator1numberDisplayRenderer_LOD0.materials[8].SetInt("_Index", floorNumber);
            _elevator1numberDisplayRenderer_LOD1.materials[8].SetInt("_Index", floorNumber);
            _elevator1numberDisplayRenderer_LOD2.materials[8].SetInt("_Index", floorNumber);
        }
        else if (elevatorNumber == 1)
        {
            _topStatePanelRenderer2.materials[0].SetInt("_Index", floorNumber);
            _elevator2numberDisplayRenderer_LOD0.materials[8].SetInt("_Index", floorNumber);
            _elevator2numberDisplayRenderer_LOD1.materials[8].SetInt("_Index", floorNumber);
            _elevator2numberDisplayRenderer_LOD2.materials[8].SetInt("_Index", floorNumber);
        }
        else if (elevatorNumber == 2)
        {
            _topStatePanelRenderer3.materials[0].SetInt("_Index", floorNumber);
            _elevator3numberDisplayRenderer_LOD0.materials[8].SetInt("_Index", floorNumber);
            _elevator3numberDisplayRenderer_LOD1.materials[8].SetInt("_Index", floorNumber);
            _elevator3numberDisplayRenderer_LOD2.materials[8].SetInt("_Index", floorNumber);
        }
    }
    /// <summary>
    /// External toggle of loli stairs inside the elevators
    /// </summary>
    /// <param name="elevatorNumber"></param>
    public void ToggleLoliStairs(int elevatorNumber)
    {
        if (elevatorNumber == 0)
        {
            _loliStairs1.SetActive(!_loliStairs1.activeSelf);
        }
        else if (elevatorNumber == 1)
        {
            _loliStairs2.SetActive(!_loliStairs2.activeSelf);
        }
        else if (elevatorNumber == 2)
        {
            _loliStairs3.SetActive(!_loliStairs3.activeSelf);
        }
    }
    /// <summary>
    /// External toggle of mirrors inside the elevators
    /// </summary>
    /// <param name="elevatorNumber"></param>
    public void ToggleMirror(int elevatorNumber)
    {
        if (elevatorNumber == 0)
        {
            _mirror1.SetActive(!_mirror1.activeSelf);
        }
        else if (elevatorNumber == 1)
        {
            _mirror2.SetActive(!_mirror2.activeSelf);
        }
        else if (elevatorNumber == 2)
        {
            _mirror3.SetActive(!_mirror3.activeSelf);
        }
    }
    /// <summary>
    /// Elevator open
    /// </summary>
    public void OpenElevator(int elevatorNumber, bool directionUp, bool isIdle, bool withSounds)
    {
        Debug.Log($"[ElevatorController] called to open elevator {elevatorNumber} in floor {_floorLevel}");
        if (withSounds)
        {
            _elevatorSoundController.DoorStartsOpening(elevatorNumber);
            _elevatorSoundController.PlayOpeningSound(elevatorNumber);
        }
        if (!isIdle)
        {
            //apply some corrections to avoid weird states that make no sense
            if (!directionUp && _floorLevel == 0)
                directionUp = true;
            if (directionUp && _floorLevel == 13)
                directionUp = false;
        }
        if (elevatorNumber == 0)
        {
            INTERNAL_OpenElevator(_elevator1, _animator1, 0, directionUp, isIdle);
        }
        else if (elevatorNumber == 1)
        {
            INTERNAL_OpenElevator(_elevator2, _animator2, 1, directionUp, isIdle);
        }
        else if (elevatorNumber == 2)
        {
            INTERNAL_OpenElevator(_elevator3, _animator3, 2, directionUp, isIdle);
        }
    }
    /// <summary>
    /// Elevator close
    /// </summary>
    public void CloseElevator(int elevatorNumber, bool withSound)
    {
        Debug.Log($"[ElevatorController] called to close elevator {elevatorNumber} in floor {_floorLevel}");
        if (withSound)
            _elevatorSoundController.PlayClosingSound(elevatorNumber);
        if (elevatorNumber == 0)
        {
            INTERNAL_CloseElevator(_animator1, 0, _topStatePanelRenderer1);
        }
        else if (elevatorNumber == 1)
        {
            INTERNAL_CloseElevator(_animator2, 1, _topStatePanelRenderer2);
        }
        else if (elevatorNumber == 2)
        {
            INTERNAL_CloseElevator(_animator3, 2, _topStatePanelRenderer3);
        }
    }
    //All button-highlight objects
    public GameObject _UP_buttonHighlight_1;
    public GameObject _DOWN_buttonHighlight_1;
    public GameObject _UP_buttonHighlight_2;
    public GameObject _DOWN_buttonHighlight_2;
    /// <summary>
    /// Setting the elevator callbuttons to calle/not called
    /// </summary>
    public void SetCallButtonState(bool buttonUp, bool isCalled)
    {
        if (buttonUp)
        {
            Debug.Log($"[ElevatorController] Setting Elevator UP Buttons to called: {isCalled} in floor {_floorLevel}");
            _UP_buttonHighlight_1.SetActive(isCalled);
            _UP_buttonHighlight_2.SetActive(isCalled);
        }
        else
        {
            Debug.Log($"[ElevatorController] Setting Elevator DOWN Buttons to called: {isCalled} in floor {_floorLevel}");
            _DOWN_buttonHighlight_1.SetActive(isCalled);
            _DOWN_buttonHighlight_2.SetActive(isCalled);
        }
    }
    /// <summary>
    /// Sets the state of which arrow is on, depending on the current elevator travel direction and idle state
    /// </summary>
    public void SetElevatorDirectionDisplay(int elevatorNumber, bool isGoingUp, bool isIdle)
    {
        Debug.Log("[ElevatorController] SetElevatorDirectionDisplay elevator " + elevatorNumber + " with isGoingUp:" + isGoingUp.ToString() + " and isIdle:" + isIdle.ToString());
        bool arrowDown = false;
        bool arrowUp = false;
        if (isIdle)
        {
            arrowDown = true;
            arrowUp = true;
        }
        else if (isGoingUp)
        {
            arrowUp = true;
        }
        else
        {
            arrowDown = true;
        }
        switch (elevatorNumber)
        {
            case 0:
                ToggleArrow(arrowUp, arrowDown, _topStatePanelRenderer1);
                break;
            case 1:
                ToggleArrow(arrowUp, arrowDown, _topStatePanelRenderer2);
                break;
            case 2:
                ToggleArrow(arrowUp, arrowDown, _topStatePanelRenderer3);
                break;
        }
    }
    private void ToggleArrow(bool arrowUp, bool arrowDown, Renderer topStateRenderer)
    {
        if (arrowUp)
        {
            topStateRenderer.materials[2].EnableKeyword("_EMISSION");
        }
        else
        {
            topStateRenderer.materials[2].DisableKeyword("_EMISSION");
        }
        if (arrowDown)
        {
            topStateRenderer.materials[1].EnableKeyword("_EMISSION");
        }
        else
        {
            topStateRenderer.materials[1].DisableKeyword("_EMISSION");
        }
    }
    //------------------------------------------ end of external calls -----------------------------------------------------------
    /// <summary>
    /// Opening an elevator
    /// </summary>
    private void INTERNAL_OpenElevator(GameObject elevatorObj, Animator animator, int elevatorNumber, bool directionUp, bool isIdle)
    {
        Debug.Log($"[ElevatorController] Elevator {elevatorNumber} in Floor {_floorLevel} opened");
        elevatorObj.SetActive(true);
        animator.speed = 1;
        animator.ResetTrigger("close");
        animator.SetTrigger("open");
        SetElevatorDirectionDisplay(elevatorNumber, directionUp, isIdle);
    }
    /// <summary>
    /// Closing an elevator
    /// </summary>
    private void INTERNAL_CloseElevator(Animator animator, int elevatorNumber, Renderer topStateRenderer)
    {
        Debug.Log($"[ElevatorController] Elevator {elevatorNumber} in floor {_floorLevel} closed");
        animator.speed = 1;
        animator.ResetTrigger("open");
        animator.SetTrigger("close");
        topStateRenderer.materials[1].DisableKeyword("_EMISSION"); //up is slot 1, down is slot 2
        topStateRenderer.materials[2].DisableKeyword("_EMISSION");
    }
    //-------------------------------- external animation event calls ---------------------------------
    /// <summary>
    /// When the elevator doors are starting to close while closing, this function is called
    /// </summary>
    public void EventFullyOpenOnClosing(int elevatorNumber)
    {
        Debug.Log("[ElevatorController] Received Event FullyOpenOnClosing");
    }
    /// <summary>
    /// When the elevator doors are already half closed while closing, this function is called
    /// </summary>
    public void EventHalfClosedOnClosing(int elevatorNumber)
    {
        Debug.Log("[ElevatorController] Received Event HalfClosedOnClosing");
        BoxCollider inDoorCollider;
        GameObject elevatorObj;
        BoxCollider doorCollider;
        if (elevatorNumber == 0)
        {
            inDoorCollider = _playerInsideDoorElevator1Detector;
            elevatorObj = _elevator1;
            doorCollider = _elevator1DoorCollider;
        }
        else if (elevatorNumber == 1)
        {
            inDoorCollider = _playerInsideDoorElevator2Detector;
            elevatorObj = _elevator2;
            doorCollider = _elevator2DoorCollider;
        }
        else
        {
            inDoorCollider = _playerInsideDoorElevator3Detector;
            elevatorObj = _elevator3;
            doorCollider = _elevator3DoorCollider;
        }
        //Check if player is inside the door and teleport him/her if that is the case
        if (inDoorCollider.bounds.Contains(_localPlayer.GetBonePosition(HumanBodyBones.Head)))
        {
            _localPlayer.TeleportTo(elevatorObj.transform.position, _localPlayer.GetRotation());
        }
        doorCollider.enabled = true;
    }
    /// <summary>
    /// When the elevator doors are already fully closed while closing, this function is called
    /// </summary>
    public void EventFullyClosedOnClosing(int elevatorNumber)
    {
        Debug.Log("[ElevatorController] Received Event FullyClosedOnClosing");
        BoxCollider inElevatorCollider;
        GameObject elevatorObj;
        if (elevatorNumber == 0)
        {
            inElevatorCollider = _playerInsideElevator1Detector;
            elevatorObj = _elevator1;
        }
        else if (elevatorNumber == 1)
        {
            inElevatorCollider = _playerInsideElevator2Detector;
            elevatorObj = _elevator2;
        }
        else
        {
            inElevatorCollider = _playerInsideElevator3Detector;
            elevatorObj = _elevator3;
        }
        //Check if player is inside elevator, else disable the elevator object
        if (!inElevatorCollider.bounds.Contains(_localPlayer.GetBonePosition(HumanBodyBones.Head)))
        {
            elevatorObj.SetActive(false);
            _elevatorSoundController.ChangeElevatorStaticSoundState(elevatorNumber, turnOn: false);
        }
        else
        {
            if (_floorLevel == 0)
            {
                _networkController._playerIsInReceptionElevator = true;
            }
            else
            {
                _networkController._playerIsInReceptionElevator = false;
            }
            _networkController._playerIsInElevatorNumber = elevatorNumber;
            _elevatorSoundController.DoorIsFullyClosedWithPlayerInside(elevatorNumber);
        }
    }
    /// <summary>
    /// When the elevator doors are already half open while opening, this function is called
    /// </summary>
    public void EventHalfOpenOnOpening(int elevatorNumber)
    {
        Debug.Log("[ElevatorController] Received HalfOpenOnOpening");
        BoxCollider doorCollider;
        if (elevatorNumber == 0)
        {
            doorCollider = _elevator1DoorCollider;
        }
        else if (elevatorNumber == 1)
        {
            doorCollider = _elevator2DoorCollider;
        }
        else
        {
            doorCollider = _elevator3DoorCollider;
        }
        doorCollider.enabled = false;
    }
    /// <summary>
    /// When the elevator doors finished to open, this function is called
    /// </summary>
    public void EventFullyOpenOnOpening(int elevatorNumber)
    {
        Debug.Log("[ElevatorController] Received Event FullyOpenOnOpening");
        Animator animator;
        GameObject elevatorObj;
        if (elevatorNumber == 0)
        {
            animator = _animator1;
            elevatorObj = _elevator1;
        }
        else if (elevatorNumber == 1)
        {
            animator = _animator2;
            elevatorObj = _elevator2;
        }
        else
        {
            animator = _animator3;
            elevatorObj = _elevator3;
        }
        //only if the speed was reversed
        if (animator.speed == -1)
        {
            animator.speed = 0;
        }
    }
    /// <summary>
    /// When the elevator doors starts to open, this function is called to ensure the elevator is enabled
    /// </summary>
    public void EventFullyClosedOnOpening(int elevatorNumber)
    {
        Debug.Log("[ElevatorController] Received FullyClosedOnOpening");
        GameObject elevatorObj;
        if (elevatorNumber == 0)
        {
            elevatorObj = _elevator1;
        }
        else if (elevatorNumber == 1)
        {
            elevatorObj = _elevator2;
        }
        else
        {
            elevatorObj = _elevator3;
        }
        elevatorObj.SetActive(true);
    }
}
