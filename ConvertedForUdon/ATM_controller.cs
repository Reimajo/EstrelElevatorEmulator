
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class ATM_controller : UdonSharpBehaviour
{
    public NetworkingController _networkController;
    //player index bones
    [HideInInspector]
    public HumanBodyBones _leftIndexBone = HumanBodyBones.LeftIndexDistal;
    [HideInInspector]
    public HumanBodyBones _rightIndexBone = HumanBodyBones.RightIndexDistal;
    //payment card components
    public GameObject _paymentCardPickup;
    private Transform _paymentCardPickupTransform;
    public GameObject _paymentCardDummy;
    private Transform _paymentCardDummyTransform;
    public Transform _paymentCardInsertRefPoint;
    public BoxCollider _InsertCardZone;
    //room number components
    public Text _roomNumberText;
    public GameObject _roomNumberObject;
    //button components
    public GameObject _desktopOnlyComponents;
    public GameObject _vrOnlyComponents;
    private VRCPlayerApi _localPlayer;
    /// <summary>
    /// Audio for clicking on the screen
    /// </summary>
    public AudioClip _screenPressAudio;
    public AudioClip _cardOutAudio;
    /// <summary>
    /// different screens
    /// </summary>
    public Material _startScreen;
    public Material _roomChoiceScreen;
    public Material _roomChoiceScreenLaden;
    public Material _paymentChoiceScreen;
    public Material _paymentChoiceScreenLaden;
    public Material _bookedOutScreen;
    public Material _confirmationScreen;
    public Material _errorScreen;
    /// <summary>
    /// touchscreen-buttons for desktop users
    /// </summary>
    public BoxCollider _desktopButtonSuiteRoom;
    public BoxCollider _desktopButtonStandardRoom;
    public BoxCollider _desktopButtonBack;
    public BoxCollider _desktopButtonPayment1;
    public BoxCollider _desktopButtonPayment2;
    public BoxCollider _desktopButtonWholeScreen;
    /// <summary>
    /// touchscreen-buttons for desktop users
    /// </summary>
    public BoxCollider _vrButtonSuiteRoom;
    public BoxCollider _vrButtonStandardRoom;
    public BoxCollider _vrButtonBack;
    public BoxCollider _vrButtonPayment1;
    public BoxCollider _vrButtonPayment2;
    public BoxCollider _vrButtonWholeScreen;
    /// <summary>
    /// distance from finger bones
    /// </summary>
    private float _fingerThickness = 0.03f;
    public GameObject _activateScriptLOD;
    private Renderer _lodLevelRenderer;
    /// <summary>
    /// GameObject that marks the trigger plate behind the buttons
    /// </summary>
    public Transform _planeEndTransform;
    /// <summary>
    /// bools representing the needed button states for both VR and Desktop users
    /// </summary>
    private bool _boolButtonSuiteRoom;
    private bool _boolButtonStandardRoom;
    private bool _boolButtonBack;
    private bool _boolButtonPayment1;
    private bool _boolButtonPayment2;
    private bool _boolButtonWholeScreen;
    /// <summary>
    /// Positions for releasing the keycard
    /// </summary>
    public Transform _releaseStartPos;
    public Transform _releaseEndPos;
    public Transform _unbookedPos;
    /// <summary>
    /// All keycards
    /// </summary>
    public GameObject[] _keyCards;
    /// <summary>
    /// Button that starts the booking process
    /// </summary>
    public BoxCollider _buttonKeypad;
    /// <summary>
    /// position at which audio is spawned
    /// </summary>
    public Transform _screenMiddleAudioPosition;
    /// <summary>
    /// All screen-LODs
    /// </summary>
    public GameObject[] _screenGameObjects = null;
    /// <summary>
    /// All screen-renderers
    /// </summary>
    private Renderer[] _renderers = null;
    /// <summary>
    /// All possible screen-states
    /// </summary>
    private const int _enumStateScreensaver = 0;
    private const int _enumStateRoomtypeChoice = 1;
    private const int _enumStateRoomtypeChoiceLaden = 2;
    private const int _enumStatePaymentChoice = 3;
    private const int _enumStatePaymentChoiceLaden = 4;
    private const int _enumStateBookedOut = 5;
    private const int _enumStateSuccess = 6;
    private const int _enumStateCrash = 7;
    private const int _enumStateErrorScreen = 8;
    /// <summary>
    /// fields for loading events
    /// </summary>
    private bool _isLoading = false;
    private float _timeSinceLoading = 0;
    /// <summary>
    /// current state of the screen
    /// </summary>
    private int _currentScreenState = _enumStateScreensaver;
    /// <summary>
    /// The floor number of the assigned room
    /// </summary>
    private int _assignedRoom = -1;
    /// <summary>
    /// fields for roomtype selection
    /// </summary>
    private bool _roomAvailable = false;
    private bool _selectedStandardRoomType = false;
    private bool _isInVR;
    private bool _isSetup = false;
    private Vector3 _pushUpVector;
    private Vector3 _pushRefPosition;
    private bool[] _i_wasInBound;
    /// <summary>
    /// At start we need to read all renderers from the screens
    /// </summary>
    public void Start()
    {
        _paymentCardDummy.SetActive(false);
        _localPlayer = Networking.LocalPlayer;
        _isInVR = _localPlayer.IsUserInVR();
        _lodLevelRenderer = _activateScriptLOD.GetComponent<Renderer>();
        _paymentCardPickupTransform = _paymentCardPickup.transform;
        _paymentCardDummyTransform = _paymentCardDummy.transform;
        if (_isInVR)
        {
            _desktopOnlyComponents.SetActive(false);
            _pushUpVector = _planeEndTransform.forward;
            _pushRefPosition = _planeEndTransform.position;
            SetupBounds();
        }
        else
        {
            _vrOnlyComponents.SetActive(false);
        }
        _renderers = new Renderer[_screenGameObjects.Length];
        int i = 0; //this will also nicely catch null-exceptions but I was probably retarded when I made it
        foreach (GameObject screen in _screenGameObjects)
        {
            _renderers[i] = _screenGameObjects[i].GetComponent<Renderer>();
            i++;
        }
        SetDefaultState();
        _isSetup = true;
    }
    private Transform _cardTransform;
    private bool _moveCard = false;
    private bool _paymentCardInserted = false;
    public void FixedUpdate()
    {
        if (!_isSetup)
            return;
        if (!_paymentCardInserted)
            InsertCard();
        if (_moveCard)
            MoveCard();
        if (_isLoading)
            ProcessLoading();
        if (_isInVR)
        {
            if (!_lodLevelRenderer.isVisible)
                return;
            //do VR-only stuff here
            ReadBonePositions();
            CheckAllButtons();
        }
    }
    private bool _insertingAnimationStarted = false;
    private float _timeSinceInsertStart = 0;
    private Vector3 _paymentCardReleasePosition;
    private Quaternion _paymentCardReleaseRotation;
    /// <summary>
    /// Inserting the card into the atm
    /// </summary>
    private void InsertCard()
    {
        //checking if card is close to the atm, else return
        if (Vector3.Distance(_releaseEndPos.position, _paymentCardPickupTransform.position) > 2f)
            return;
        //if the inserting animation was already started
        if (_insertingAnimationStarted)
        {
            _timeSinceInsertStart += Time.fixedDeltaTime;
            _paymentCardDummyTransform.position = Vector3.Lerp(_paymentCardReleasePosition, _releaseStartPos.position, _timeSinceInsertStart);
            _paymentCardDummyTransform.rotation = Quaternion.Slerp(_paymentCardReleaseRotation, _releaseStartPos.rotation, _timeSinceInsertStart * 2);
            if (_timeSinceInsertStart >= 1f)
            {
                //now card is fully inserted, so we can disable it
                _paymentCardDummy.SetActive(false);
                _paymentCardInserted = true;
                //show the booking screen now
                SetScreenState(_enumStateRoomtypeChoice);
            }
            return;
        }
        //checking if card is inside the insert-zone
        if (_InsertCardZone.bounds.Contains(_paymentCardInsertRefPoint.position))
        {
            Vector3 cardRotation = _paymentCardPickupTransform.eulerAngles;
            //making sure the card is rotated the right way
            if (cardRotation.x < 330 && cardRotation.x > 30)
                return;
            if (cardRotation.y < 150 && cardRotation.y > 210)
                return;
            if (cardRotation.z < 330 && cardRotation.z > 30)
                return;
            //replacing the pickup with a dummy
            _paymentCardDummy.SetActive(true);
            _paymentCardReleasePosition = _paymentCardPickupTransform.position;
            _paymentCardReleaseRotation = _paymentCardPickupTransform.rotation;
            _paymentCardDummyTransform.position = _paymentCardPickupTransform.position;
            _paymentCardDummyTransform.rotation = _paymentCardPickupTransform.rotation;
            //disabling the pickup furever
            _paymentCardPickup.SetActive(false);
            //start the insert animation
            _insertingAnimationStarted = true;
            //playing insert-audio
            AudioSource.PlayClipAtPoint(_cardOutAudio, _releaseStartPos.position, 0.3f);
        }
    }
    /// <summary>
    /// When the screen shows a loading picture, this function processes what happens next
    /// </summary>
    private void ProcessLoading()
    {
        _timeSinceLoading += Time.fixedDeltaTime;
        if (_timeSinceLoading > 2f)
        {
            //this state is reached when user needs to make a payment choice next
            switch (_currentScreenState)
            {
                case _enumStateRoomtypeChoiceLaden:
                    if (_roomAvailable) //checking if room is available
                    {
                        SetScreenState(_enumStatePaymentChoice); //go to payment choice screen
                    }
                    else
                    {
                        //show that roomtype is booked out
                        SetScreenState(_enumStateBookedOut);
                    }
                    _isLoading = false;
                    break;
                case _enumStatePaymentChoiceLaden:
                    //checking if master confirmed the room booking uwu
                    if (!_networkController.LOCAL_CheckIfRoomWasBooked(_assignedRoom)) //checking if room was really booked
                    {
                        _assignedRoom = -1;
                        _timeSinceLoading = 0;
                        //show error screen when master didn't responded and wait x seconds till loading again
                        SetScreenState(_enumStateErrorScreen);
                    }
                    else
                    {
                        _isLoading = false;
                        Networking.SetOwner(_localPlayer, _keyCards[_assignedRoom].gameObject);
                        SetScreenState(_enumStateSuccess);
                        _roomNumberText.text = (_assignedRoom + 1).ToString();
                        Debug.Log($"ATM accesses card {_assignedRoom} transform now...");
                        _cardTransform = _keyCards[_assignedRoom].transform;
                        _cardTransform.position = _releaseStartPos.position;
                        _cardTransform.rotation = _releaseStartPos.rotation;
                        _moveCard = true;
                        AudioSource.PlayClipAtPoint(_cardOutAudio, _releaseStartPos.position, 0.3f);
                    }
                    break;
                case _enumStateErrorScreen:
                    _isLoading = false;
                    SetScreenState(_enumStateScreensaver);
                    break;
            }
        }
    }
    private float _movingSinceSeconds = 0;
    /// <summary>
    /// Moving the card out
    /// </summary>
    private void MoveCard()
    {
        _movingSinceSeconds += Time.fixedDeltaTime / 2; //travels for 2 seconds
        if (_movingSinceSeconds >= 2.5f)
        {
            Debug.Log("[ATM] stopped moving the card.");
            _moveCard = false;
            return;
        }
        else if (_movingSinceSeconds > 2f)
        {
            //trying to "stop" the card for 500ms
            _cardTransform.position = _releaseEndPos.position;
        }
        else
        {
            _cardTransform.position = Vector3.Lerp(_releaseStartPos.position, _releaseEndPos.position, _movingSinceSeconds);
        }
    }
    /// <summary>
    /// Sets object into "default" state
    /// </summary>
    private void SetDefaultState()
    {
        SetScreenState(_enumStateScreensaver);
    }
    /// <summary>
    /// Sets the material for all renderers
    /// </summary>
    void SetScreenMaterial(Material material)
    {
        foreach (Renderer renderer in _renderers)
        {
            if (renderer.materials[1] != null)
            {
                var mats = renderer.materials;
                mats[1] = material;
                renderer.materials = mats;
            }
        }
    }
    /// <summary>
    /// Changing the current state of the screen
    /// </summary>
    public void SetScreenState(int _screenState)
    {
        DisableAllButtons();
        switch (_screenState)
        {
            case _enumStateScreensaver: //screensaver
                SetScreenMaterial(_startScreen);
                //_boolButtonWholeScreen = true;
                break;
            case _enumStateRoomtypeChoice: //roomtype-choice
                SetScreenMaterial(_roomChoiceScreen);
                _boolButtonStandardRoom = true;
                _boolButtonSuiteRoom = true;
                break;
            case _enumStateRoomtypeChoiceLaden: //after roomtype-choice LADEN-screen
                SetScreenMaterial(_roomChoiceScreenLaden);
                _isLoading = true;
                _timeSinceLoading = 0;
                break;
            case _enumStatePaymentChoice: //payment-choice
                SetScreenMaterial(_paymentChoiceScreen);
                _boolButtonPayment1 = true;
                _boolButtonPayment2 = true;
                _boolButtonBack = true;
                break;
            case _enumStatePaymentChoiceLaden: //after payment-choice LADEN-screen
                SetScreenMaterial(_paymentChoiceScreenLaden);
                _isLoading = true;
                _timeSinceLoading = 0;
                break;
            case _enumStateBookedOut: //booked-out notification
                SetScreenMaterial(_bookedOutScreen);
                _boolButtonBack = true;
                break;
            case _enumStateSuccess: //booking-success
                SetScreenMaterial(_confirmationScreen);
                _roomNumberObject.SetActive(true);
                break;
            case _enumStateCrash: //crash
                SetScreenMaterial(_errorScreen);
                break;
        }
        if (!_isInVR)
            SetButtonsActive();
        _currentScreenState = _screenState;
    }
    /// <summary>
    /// Setting all DesktopButtons active where the counterpart-bool is active as well
    /// </summary>
    private void SetButtonsActive()
    {
        if (_boolButtonSuiteRoom)
            _desktopButtonSuiteRoom.enabled = true;
        if (_boolButtonStandardRoom)
            _desktopButtonStandardRoom.enabled = true;
        if (_boolButtonBack)
            _desktopButtonBack.enabled = true;
        if (_boolButtonPayment1)
            _desktopButtonPayment1.enabled = true;
        if (_boolButtonPayment2)
            _desktopButtonPayment2.enabled = true;
        if (_boolButtonWholeScreen)
            _desktopButtonWholeScreen.enabled = true;
    }
    /// <summary>
    /// Disabling all desktop-buttons
    /// </summary>
    private void DisableAllButtons()
    {
        //disabling the state bools used for VR buttons
        _boolButtonSuiteRoom = false;
        _boolButtonStandardRoom = false;
        _boolButtonBack = false;
        _boolButtonPayment1 = false;
        _boolButtonPayment2 = false;
        _boolButtonWholeScreen = false;
        //disable all desktop buttons
        if (!_isInVR)
        {
            _desktopButtonSuiteRoom.enabled = false;
            _desktopButtonStandardRoom.enabled = false;
            _desktopButtonBack.enabled = false;
            _desktopButtonPayment1.enabled = false;
            _desktopButtonPayment2.enabled = false;
            _desktopButtonWholeScreen.enabled = false;
        }
    }
    /// <summary>
    /// When a button is pressed, this function is called
    /// </summary>
    public void ButtonPressed(int _button)
    {
        AudioSource.PlayClipAtPoint(_screenPressAudio, _screenMiddleAudioPosition.position, 0.3f);
        switch (_button)
        {
            case 0: //select standard room
                _selectedStandardRoomType = true;
                _roomAvailable = _networkController.LOCAL_IsRoomAvailable(isStandardRoom: true);
                SetScreenState(_enumStateRoomtypeChoiceLaden);
                break;
            case 1: //select suite room
                _selectedStandardRoomType = false;
                _roomAvailable = _networkController.LOCAL_IsRoomAvailable(isStandardRoom: false);
                SetScreenState(_enumStateRoomtypeChoiceLaden);
                break;
            case 2: //back-button
                SetScreenState(_enumStateRoomtypeChoice);
                break;
            case 3: //payment-option 1
            case 4: //payment-option 2
                _assignedRoom = _networkController.LOCAL_BookRandomRoom(isStandardRoom: _selectedStandardRoomType);
                if (_assignedRoom == -1)
                {
                    SetScreenState(_enumStateBookedOut);
                }
                else
                {
                    SetScreenState(_enumStatePaymentChoiceLaden);
                }
                break;
            case 5: //whole-screen
                SetScreenState(_enumStateRoomtypeChoice);
                break;
            case 6:
                SetScreenState(_enumStateErrorScreen);
                break;
        }
    }
    /// <summary>
    /// Can be called by master to move a card back to spawn
    /// </summary>
    /// <param name=""></param>
    public void ResetRoomCardPosition(int roomNumber)
    {
        KeycardScript keycardScript = _keyCards[roomNumber].gameObject.GetComponent<KeycardScript>();
        keycardScript.MASTER_RespawnCard();
    }
    /// <summary>
    /// Button-bounds for VR users, set on start via SetupBounds()
    /// </summary>
    private Bounds _0_buttonBounds; //_vrButtonStandardRoom 
    private Bounds _1_buttonBounds; //_vrButtonSuiteRoom
    private Bounds _2_buttonBounds; //_vrButtonBack
    private Bounds _3_buttonBounds; //_vrButtonPayment1
    private Bounds _4_buttonBounds; //_vrButtonPayment2
    private Bounds _5_buttonBounds; //_vrButtonWholeScreen
    /// <summary>
    /// Store Bounds for all PushAreas
    /// </summary>
    private void SetupBounds()
    {
        _0_buttonBounds = new Bounds(_vrButtonStandardRoom.center, _vrButtonStandardRoom.size);
        _1_buttonBounds = new Bounds(_vrButtonSuiteRoom.center, _vrButtonSuiteRoom.size);
        _2_buttonBounds = new Bounds(_vrButtonBack.center, _vrButtonBack.size);
        _3_buttonBounds = new Bounds(_vrButtonPayment1.center, _vrButtonPayment1.size);
        _4_buttonBounds = new Bounds(_vrButtonPayment2.center, _vrButtonPayment2.size);
        _5_buttonBounds = new Bounds(_vrButtonWholeScreen.center, _vrButtonWholeScreen.size);
        _i_wasInBound = new bool[6];
    }
    private Vector3 _indexBoneL;
    private Vector3 _indexBoneR;
    /// <summary>
    /// Reads bone positions of all hand bones
    /// </summary>
    void ReadBonePositions()
    {
        _indexBoneL = _localPlayer.GetBonePosition(_leftIndexBone);
        _indexBoneR = _localPlayer.GetBonePosition(_rightIndexBone);
    }
    //splitting workload into two frames
    private bool _evenFrame = false;
    private bool _oneButtonPressed;
    /// <summary>
    /// Doing a check on all buttons
    /// </summary>
    private void CheckAllButtons()
    {
        _oneButtonPressed = false;
        if (_evenFrame)
        {
            if (_boolButtonSuiteRoom)
                CheckButton(0, _0_buttonBounds, _vrButtonSuiteRoom);
            if (_boolButtonStandardRoom)
                CheckButton(1, _1_buttonBounds, _vrButtonStandardRoom);
            if (_boolButtonBack)
                CheckButton(2, _2_buttonBounds, _vrButtonBack);
        }
        else
        {
            if (_boolButtonPayment1)
                CheckButton(3, _3_buttonBounds, _vrButtonPayment1);
            if (_boolButtonPayment2)
                CheckButton(4, _4_buttonBounds, _vrButtonPayment2);
            if (_boolButtonWholeScreen)
                CheckButton(5, _5_buttonBounds, _vrButtonWholeScreen);
        }
        _evenFrame = !_evenFrame;
    }
    private float _distanceToHand = 0;
    /// <summary>
    /// Check if hand is inside a box collider area, if so it does a lot of magic
    /// </summary>
    void CheckButton(int i, Bounds pushAreaBounds, BoxCollider pushAreaCollider)
    {
        bool _isInBoundRightNow = false;
        if (pushAreaBounds.Contains(pushAreaCollider.transform.InverseTransformPoint(_indexBoneL)))
        {
            //measure distances to both hands
            _distanceToHand = SignedDistancePlanePoint(_pushUpVector, _pushRefPosition, _indexBoneL) - _fingerThickness;
            //the lowest distance is important for us
            if (_distanceToHand < _fingerThickness)
            {
                _isInBoundRightNow = true;
            }
        }
        if (!_isInBoundRightNow && pushAreaBounds.Contains(pushAreaCollider.transform.InverseTransformPoint(_indexBoneR)))
        {
            //measure distances to both hands
            _distanceToHand = SignedDistancePlanePoint(_pushUpVector, _pushRefPosition, _indexBoneR) - _fingerThickness;
            //the lowest distance is important for us
            if (_distanceToHand < _fingerThickness)
            {
                _isInBoundRightNow = true;
            }
        }
        //check if hand is in bounds
        if (_isInBoundRightNow)
        {
            if (!_i_wasInBound[i])
            {
                _i_wasInBound[i] = true;
                if (!_oneButtonPressed)
                {
                    _oneButtonPressed = true;
                    //trigger action
                    ButtonPressed(i);
                }
            }
        }
        else
        {
            _i_wasInBound[i] = false;
        }
    }
    /// <summary>
    /// Get the shortest distance between a point and a plane. The output is signed so it holds information
    /// as to which side of the plane normal the point is.
    /// </summary>
    public float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        return Vector3.Dot(planeNormal, (point - planePoint));
    }
}
