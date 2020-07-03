
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class ATM_controller : UdonSharpBehaviour
{
    public NetworkingController _networkController;
    public Text _roomNumberText;
    public GameObject _roomNumberObject;
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
    /// touchscreen-buttons
    /// </summary>
    public BoxCollider _desktopButtonSuiteRoom;
    public BoxCollider _desktopButtonStandardRoom;
    public BoxCollider _desktopButtonBack;
    public BoxCollider _desktopButtonPayment1;
    public BoxCollider _desktopButtonPayment2;
    public BoxCollider _desktopButtonWholeScreen;
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
    /// <summary>
    /// At start we need to read all renderers from the screens
    /// </summary>
    public void Start()
    {
        _localPlayer = Networking.LocalPlayer;
        _isInVR = _localPlayer.IsUserInVR();
        if (_isInVR)
        {
            _desktopOnlyComponents.SetActive(false);
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
    }
    private Transform _cardTransform;
    private bool _moveCard = false;
    public void Update()
    {
        if (_moveCard)
            MoveCard();
        if (_isLoading)
            ProcessLoading();
        if(_isInVR)
        {
            //do VR-only stuff here
        }
    }

    private void ConfirmBooking(int room)
    {
        _assignedRoom = room;
    }
    private void CancelBooking()
    {
        _assignedRoom = -1;
    }
    /// <summary>
    /// When the screen shows a loading picture, this function processes what happens next
    /// </summary>
    private void ProcessLoading()
    {
        _timeSinceLoading += Time.deltaTime;
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
                    if (_assignedRoom == -1) //checking if room was really booked
                    {
                        _timeSinceLoading = 0;
                        //show error screen when master didn't responded and wait x seconds till loading again
                        SetScreenState(_enumStateErrorScreen);
                    }
                    else
                    {
                        _isLoading = false;
                        SetScreenState(_enumStateSuccess);
                        _roomNumberText.text = _assignedRoom.ToString();
                        Debug.Log($"ATM accesses card {_assignedRoom - 1} transform now...");
                        _cardTransform = _keyCards[_assignedRoom - 1].transform;
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
    /// <summary>
    /// Moving the card out
    /// </summary>
    private void MoveCard()
    {
        float distance = Vector3.Distance(_cardTransform.position, _releaseEndPos.position);
        if (distance < 0.001f || distance > 2f)
        {
            _moveCard = false;
        }
        else
        {
            _cardTransform.position = Vector3.Lerp(_cardTransform.position, _releaseEndPos.position, Time.deltaTime);
        }
        //Debug.Log($"Moving card pos x:{_keyCards[_assignedRoom - 1].transform.position.x} y:{_keyCards[_assignedRoom - 1].transform.position.y} z:{_keyCards[_assignedRoom - 1].transform.position.z}");
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
                _boolButtonWholeScreen = true;
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
        SetButtonsActive();
        _currentScreenState = _screenState;
    }
    /// <summary>
    /// Setting all DesktopButtons active where the counterpart-bool is active as well
    /// </summary>
    private void SetButtonsActive()
    {
        if(_boolButtonSuiteRoom)
            _desktopButtonSuiteRoom.enabled = true;
        if(_boolButtonStandardRoom)
            _desktopButtonStandardRoom.enabled = true;
        if(_boolButtonBack)
            _desktopButtonBack.enabled = true;
        if(_boolButtonPayment1)
            _desktopButtonPayment1.enabled = true;
        if(_boolButtonPayment2)
            _desktopButtonPayment2.enabled = true;
        if(_boolButtonWholeScreen)
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
        _desktopButtonSuiteRoom.enabled = false;
        _desktopButtonStandardRoom.enabled = false;
        _desktopButtonBack.enabled = false;
        _desktopButtonPayment1.enabled = false;
        _desktopButtonPayment2.enabled = false;
        _desktopButtonWholeScreen.enabled = false;
    }
    /// <summary>
    /// When a button is pressed, this function is called
    /// </summary>
    public void ButtonPressed(int _button)
    {
        AudioSource.PlayClipAtPoint(_screenPressAudio, _screenMiddleAudioPosition.position, 0.3f);
        switch (_button)
        {
            case 0: //book standard room
                _selectedStandardRoomType = true;
                _roomAvailable = _networkController.CheckForFreeRoom(isStandardRoom: true);
                SetScreenState(_enumStateRoomtypeChoiceLaden);
                break;
            case 1: //book suite room
                _selectedStandardRoomType = false;
                _roomAvailable = _networkController.CheckForFreeRoom(isStandardRoom: false);
                SetScreenState(_enumStateRoomtypeChoiceLaden);
                break;
            case 2: //back-button
                SetScreenState(_enumStateRoomtypeChoice);
                break;
            case 3: //payment-option 1
            case 4: //payment-option 2
                _assignedRoom = _networkController.BookFreeRoom(isStandardRoom: _selectedStandardRoomType);
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
}
