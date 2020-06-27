using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;

public class NetworkingController : UdonSharpBehaviour
{
    #region variables
    public PlayerModCheck _playerModCheck;
    public Transform _currentSpawn;
    public Transform _readonlyReceptionSpawn;
    public Transform _readonlyFloorSpawn;
    public ElevatorRequester _elevatorRequester;
    public ElevatorController _elevatorControllerReception;
    public ElevatorController _elevatorControllerArrivalArea;
    public InsidePanelScriptForDesktop _insidePanelScriptElevatorDesktop_0;
    public InsidePanelScriptForVR _InsidePanelScriptElevatorForVR_0;
    public InsidePanelScriptForDesktop _insidePanelScriptElevatorDesktop_1;
    public InsidePanelScriptForVR _InsidePanelScriptElevatorForVR_1;
    public InsidePanelScriptForDesktop _insidePanelScriptElevatorDesktop_2;
    public InsidePanelScriptForVR _InsidePanelScriptElevatorForVR_2;
    public InsidePanelScriptForDesktop _insidePanelFloorScriptElevatorDesktop_0;
    public InsidePanelScriptForVR _InsidePanelFloorScriptElevatorForVR_0;
    public InsidePanelScriptForDesktop _insidePanelFloorScriptElevatorDesktop_1;
    public InsidePanelScriptForVR _InsidePanelFloorScriptElevatorForVR_1;
    public InsidePanelScriptForDesktop _insidePanelFloorScriptElevatorDesktop_2;
    public InsidePanelScriptForVR _InsidePanelFloorScriptElevatorForVR_2;
    /// <summary>
    /// The callbutton panel for the elevator
    /// </summary>
    public CallButtonDesktopFloor _floorElevatorCallPanelDesktop_1;
    public CallButtonDesktopFloor _floorElevatorCallPanelDesktop_2;
    public ElevatorCallButton _floorElevatorCallPanelForVR_1;
    public ElevatorCallButton _floorElevatorCallPanelForVR_2;
    /// <summary>
    /// Floor-dependent number sign where _Index needs to be set to floor number-1
    /// </summary>
    public GameObject _floorNumberSign;
    public GameObject _floorRoomNumberSign;
    private Renderer _floorNumberSignRenderer;
    private Renderer _floorRoomNumberSignRenderer;
    public Transform _moveArrivalFloorHere;
    ///<summary> animation-timing-parameters </summary>
    private const float TIME_TO_STAY_CLOSED_AFTER_GOING_OUT_OF_IDLE = 3f;
    private const float TIME_TO_STAY_OPEN = 10f; // 10f is normal
    private const float TIME_TO_STAY_OPEN_RECEPTION = 15f; //was 10f
    private const float TIME_TO_STAY_CLOSED = 4f; //MUST BE 4f IN UNITY! Because of the closing animation
    private const float TIME_TO_DRIVE_ONE_FLOOR = 3f; //was 2f
    /// <summary>
    /// elevator states, synced by master
    /// </summary>
    [HideInInspector, UdonSynced(UdonSyncMode.None)]    
    private long _syncData1 = 0;
    /// <summary>
    /// elevator request states, synced by master
    /// </summary>
    [HideInInspector, UdonSynced(UdonSyncMode.None)]
    public long _syncData2 = 0;
    /// <summary>
    /// Current floor level that localPlayer is on
    /// </summary>
    private int _localPlayerCurrentFloor = 0;
    /// <summary>
    /// Current elevator that the player is in
    /// </summary>
    [HideInInspector]
    public int _playerIsInElevatorNumber = -1;
    [HideInInspector]
    public bool _playerIsInReceptionElevator;
    /// <summary>
    /// other public variables
    /// </summary>
    VRCPlayerApi _localPlayer;
    private bool _finishedLocalSetup = false;
    private bool _isMaster = false;
    private bool _worldIsLoaded = false;
    private bool _userIsInVR;
    /// <summary>
    /// Locally storing which elevator is currently working and which isn't, since we only need to read this 
    /// once from the SYNC state and it won't change, so it would be a waste to read it every time again
    /// This is read by LOCAL but also used by MASTER
    /// </summary>
    private bool _elevator0Working = false;
    private bool _elevator1Working = false;
    private bool _elevator2Working = false;
    #endregion variables
    //------------------------------------------------------------------------------------------------------------
    //------------------------------------SYNCBOOL ENUM-----------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region ENUM_SYNCBOOL
    /// <summary>
    /// "ENUM" of different bools that are synced in _syncData
    /// (ENUM isn't possible in Udon, so we use this here)
    ///  - 55-52 variable_3 (4bits)
    ///  - 0-51 binary bools [0-51]
    ///  - 0-31 binary bools [52-83(?)]
    /// </summary>
    /// 
    private const int SyncBoolReq_BellOn = 0;
    private const int SyncBool_Elevator0open = 1;
    private const int SyncBool_Elevator1open = 2;
    private const int SyncBool_Elevator2open = 3;
    private const int SyncBool_Elevator0idle = 4;
    private const int SyncBool_Elevator1idle = 5;
    private const int SyncBool_Elevator2idle = 6;
    private const int SyncBool_Elevator0goingUp = 7;
    private const int SyncBool_Elevator1goingUp = 8;
    private const int SyncBool_Elevator2goingUp = 9;
    /// <summary>
    /// Sync-data positions for elevator call up buttons
    /// </summary>
    private const int SyncBoolReq_ElevatorCalledUp_0 = 10;
    private const int SyncBoolReq_ElevatorCalledUp_1 = 11;
    private const int SyncBoolReq_ElevatorCalledUp_2 = 12;
    private const int SyncBoolReq_ElevatorCalledUp_3 = 13;
    private const int SyncBoolReq_ElevatorCalledUp_4 = 14;
    private const int SyncBoolReq_ElevatorCalledUp_5 = 15;
    private const int SyncBoolReq_ElevatorCalledUp_6 = 16;
    private const int SyncBoolReq_ElevatorCalledUp_7 = 17;
    private const int SyncBoolReq_ElevatorCalledUp_8 = 18;
    private const int SyncBoolReq_ElevatorCalledUp_9 = 19;
    private const int SyncBoolReq_ElevatorCalledUp_10 = 20;
    private const int SyncBoolReq_ElevatorCalledUp_11 = 21;
    private const int SyncBoolReq_ElevatorCalledUp_12 = 22;
    private const int SyncBoolReq_ElevatorCalledUp_13 = 23;
    /// <summary>
    /// Sync-data positions for elevator call down buttons
    /// </summary>
    private const int SyncBoolReq_ElevatorCalledDown_0 = 24;
    private const int SyncBoolReq_ElevatorCalledDown_1 = 25;
    private const int SyncBoolReq_ElevatorCalledDown_2 = 26;
    private const int SyncBoolReq_ElevatorCalledDown_3 = 27;
    private const int SyncBoolReq_ElevatorCalledDown_4 = 28;
    private const int SyncBoolReq_ElevatorCalledDown_5 = 29;
    private const int SyncBoolReq_ElevatorCalledDown_6 = 30;
    private const int SyncBoolReq_ElevatorCalledDown_7 = 31;
    private const int SyncBoolReq_ElevatorCalledDown_8 = 32;
    private const int SyncBoolReq_ElevatorCalledDown_9 = 33;
    private const int SyncBoolReq_ElevatorCalledDown_10 = 34;
    private const int SyncBoolReq_ElevatorCalledDown_11 = 35;
    private const int SyncBoolReq_ElevatorCalledDown_12 = 36;
    private const int SyncBoolReq_ElevatorCalledDown_13 = 37;
    /// <summary>
    /// Sync-data positions for internal elevator 0 buttons
    /// </summary>
    private const int SyncBoolReq_Elevator0CalledToFloor_0 = 38;
    private const int SyncBoolReq_Elevator0CalledToFloor_1 = 39;
    private const int SyncBoolReq_Elevator0CalledToFloor_2 = 40;
    private const int SyncBoolReq_Elevator0CalledToFloor_3 = 41;
    private const int SyncBoolReq_Elevator0CalledToFloor_4 = 42;
    private const int SyncBoolReq_Elevator0CalledToFloor_5 = 43;
    private const int SyncBoolReq_Elevator0CalledToFloor_6 = 44;
    private const int SyncBoolReq_Elevator0CalledToFloor_7 = 45;
    private const int SyncBoolReq_Elevator0CalledToFloor_8 = 46;
    private const int SyncBoolReq_Elevator0CalledToFloor_9 = 47;
    private const int SyncBoolReq_Elevator0CalledToFloor_10 = 48;
    private const int SyncBoolReq_Elevator0CalledToFloor_11 = 49;
    private const int SyncBoolReq_Elevator0CalledToFloor_12 = 50;
    private const int SyncBoolReq_Elevator0CalledToFloor_13 = 51;
    /// <summary>
    /// Sync-data positions for internal elevator 1 buttons
    /// </summary>
    private const int SyncBoolReq_Elevator1CalledToFloor_0 = 52;
    private const int SyncBoolReq_Elevator1CalledToFloor_1 = 53;
    private const int SyncBoolReq_Elevator1CalledToFloor_2 = 54;
    private const int SyncBoolReq_Elevator1CalledToFloor_3 = 55;
    private const int SyncBoolReq_Elevator1CalledToFloor_4 = 56;
    private const int SyncBoolReq_Elevator1CalledToFloor_5 = 57;
    private const int SyncBoolReq_Elevator1CalledToFloor_6 = 58;
    private const int SyncBoolReq_Elevator1CalledToFloor_7 = 59;
    private const int SyncBoolReq_Elevator1CalledToFloor_8 = 60;
    private const int SyncBoolReq_Elevator1CalledToFloor_9 = 61;
    private const int SyncBoolReq_Elevator1CalledToFloor_10 = 62;
    private const int SyncBoolReq_Elevator1CalledToFloor_11 = 63;
    private const int SyncBoolReq_Elevator1CalledToFloor_12 = 64;
    private const int SyncBoolReq_Elevator1CalledToFloor_13 = 65;
    /// <summary>
    /// Sync-data positions for internal elevator 2 buttons
    /// </summary>
    private const int SyncBoolReq_Elevator2CalledToFloor_0 = 66;
    private const int SyncBoolReq_Elevator2CalledToFloor_1 = 67;
    private const int SyncBoolReq_Elevator2CalledToFloor_2 = 68;
    private const int SyncBoolReq_Elevator2CalledToFloor_3 = 69;
    private const int SyncBoolReq_Elevator2CalledToFloor_4 = 70;
    private const int SyncBoolReq_Elevator2CalledToFloor_5 = 71;
    private const int SyncBoolReq_Elevator2CalledToFloor_6 = 72;
    private const int SyncBoolReq_Elevator2CalledToFloor_7 = 73;
    private const int SyncBoolReq_Elevator2CalledToFloor_8 = 74;
    private const int SyncBoolReq_Elevator2CalledToFloor_9 = 75;
    private const int SyncBoolReq_Elevator2CalledToFloor_10 = 76;
    private const int SyncBoolReq_Elevator2CalledToFloor_11 = 77;
    private const int SyncBoolReq_Elevator2CalledToFloor_12 = 78;
    private const int SyncBoolReq_Elevator2CalledToFloor_13 = 79;
    /// <summary>
    /// Last few state bools
    /// </summary>
    private const int SyncBool_Initialized = 80;
    private const int SyncBool_Elevator0working = 81;
    private const int SyncBool_Elevator1working = 82;
    private const int SyncBool_Elevator2working = 83;
    private const int SyncBool_Elevator0IsDriving = 84;
    private const int SyncBool_Elevator1IsDriving = 85;
    private const int SyncBool_Elevator2IsDriving = 86;
    #endregion ENUM_SYNCBOOL
    #region ENUM_DIRECTSYNCBOOL
    /// <summary>
    /// Direct bit masks and addresses for the "Enum_Syncbool" bools - Remember to update both!!!
    /// 
    /// The GetSyncValue(*) function has been swapped to speed things up a bit.                        
    ///                 
    /// Accessing using mask (if true)
    ///   -"0L != (_syncData1 & (SyncBool_MaskLong1"
    ///   - "0L != (_syncData2 & (SyncBool_MaskLong"
    /// Or for "not"ed functions (if false)
    ///   -"0L == (_syncData1 & (SyncBool_MaskLong1"
    ///   - "0L == (_syncData2 & (SyncBool_MaskLong"
    /// 
    /// Accessing using it like an array (aka address based [slower])
    ///  Checks if true:  
    ///  - (0L != (_syncData1 & (1L << (SyncBool_AddressLong1
    ///  - (0L != (_syncData2 & (1L << (SyncBool_AddressUint
    ///  Checks if false:
    ///  - (0L == (_syncData1 & (1L << (SyncBool_AddressLong1
    ///  - (0L == (_syncData2 & (1L << (SyncBool_AddressUint
    /// </summary>
    ///         
    private const long SyncBoolReq_MaskLong1_BellOn = (1L);
    private const int SyncBool_AddressLong1_ElevatorXopen = 1;
    private const long SyncBool_MaskLong1_Elevator0open = (1L << 1);
    private const long SyncBool_MaskLong1_Elevator1open = (1L << 2);
    private const long SyncBool_MaskLong1_Elevator2open = (1L << 3);
    private const int SyncBool_AddressLong1_ElevatorXidle = 4;
    private const long SyncBool_MaskLong1_Elevator0idle = (1L << 4);
    private const long SyncBool_MaskLong1_Elevator1idle = (1L << 5);
    private const long SyncBool_MaskLong1_Elevator2idle = (1L << 6);
    private const int SyncBool_AddressLong1_ElevatorXgoingUp = 7;
    private const long SyncBool_MaskLong1_Elevator0goingUp = (1L << 7);
    private const long SyncBool_MaskLong1_Elevator1goingUp = (1L << 8);
    private const long SyncBool_MaskLong1_Elevator2goingUp = (1L << 9);
    /// <summary>     
    /// Sync-data positions for elevator call up
    /// </summary>            
    private const int SyncBoolReq_AddressLong1_ElevatorCalledUp = 10;
    /*private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_0 = (1L << 10);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_1 = (1L << 11);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_2 = (1L << 12);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_3 = (1L << 13);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_4 = (1L << 14);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_5 = (1L << 15);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_6 = (1L << 16);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_7 = (1L 17);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_8 = (1L << 18);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_9 = (1L << 19);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_10 = (1L << 20);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_11 = (1L << 21);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_12 = (1L << 22);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledUp_13 = (1L << 23);*/

    /// <summary>     
    /// Sync-data positions for elevator call down
    /// </summary>     
    private const int SyncBoolReq_AddressLong1_ElevatorCalledDown = 24;
    /*private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_0 = (1L << 24);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_1 = (1L << 25);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_2 = (1L << 26);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_3 = (1L << 27);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_4 = (1L << 28);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_5 = (1L << 29);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_6 = (1L << 30);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_7 = (1L << 31);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_8 = (1L << 32);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_9 = (1L << 33);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_10 = (1L << 34);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_11 = (1L << 35);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_12 = (1L << 36);
    private const long SyncBoolReq_MaskLong1_ElevatorCalledDown_13 = (1L << 37);*/

    /// <summary>     
    /// Sync-data positions for internal elevator 0
    /// ******THIS CANNOT BE USED****** It spans both the Long and Uint, use getSyncValues instead
    /// </summary>    
    private const int SyncBoolReq_AddressLong1_Elevator0CalledToFloor = 38;
    /*private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_0 = (1L << 38);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_1 = (1L << 39);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_2 = (1L << 40);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_3 = (1L << 41);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_4 = (1L << 42);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_5 = (1L << 43);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_6 = (1L << 44);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_7 = (1L << 45);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_8 = (1L << 46);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_9 = (1L << 47);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_10 = (1L << 48);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_11 = (1L << 49);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_12 = (1L << 50);
    private const long SyncBoolReq_MaskLong1_Elevator0CalledToFloor_13 = (1L << 51);*/
    /// <summary>     
    /// Sync-data positions for internal elevator 1
    /// </summary>     
    private const int SyncBoolReq_AddressLong2_Elevator1CalledToFloor = 0;
    /*private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_0 = (1L << 0);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_1 = (1L << 1);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_2 = (1L << 2);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_3 = (1L << 3);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_4 = (1L << 4);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_5 = (1L << 5);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_6 = (1L << 6);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_7 = (1L << 7);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_8 = (1L << 8);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_9 = (1L << 9);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_10 = (1L << 10);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_11 = (1L << 11);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_12 = (1L << 12);
    private const long SyncBoolReq_MaskLong2_Elevator1CalledToFloor_13 = (1L << 13);*/
    /// <summary>     
    /// Sync-data positions for internal elevator 2
    /// </summary>     
    private const int SyncBoolReq_AddressLong2_Elevator2CalledToFloor = 14;
    /*private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_0 = (1L << 14);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_1 = (1L << 15);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_2 = (1L << 16);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_3 = (1L << 17);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_4 = (1L << 18);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_5 = (1L << 19);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_6 = (1L << 20);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_7 = (1L << 21);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_8 = (1L << 22);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_9 = (1L << 23);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_10 = (1L << 24);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_11 = (1L << 25);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_12 = (1L << 26);
    private const long SyncBoolReq_MaskLong2_Elevator2CalledToFloor_13 = (1L << 27);*/

    private const long SyncBool_MaskLong2_Initialized = (1L << 28);
    private const int SyncBool_AddressLong2_ElevatorXworking = 29;
    private const long SyncBool_MaskLong2_Elevator0working = (1L << 29);
    private const long SyncBool_MaskLong2_Elevator1working = (1L << 30);
    private const long SyncBool_MaskLong2_Elevator2working = (1L << 31);
    private const int SyncBool_AddressLong2_ElevatorXIsDriving = 32;
    private const long SyncBool_MaskLong2_Elevator0IsDriving = (1L << 32);
    private const long SyncBool_MaskLong2_Elevator1IsDriving = (1L << 33);
    private const long SyncBool_MaskLong2_Elevator2IsDriving = (1L << 34);

    #endregion ENUM_DIRECTSYNCBOOL
    //------------------------------------------------------------------------------------------------------------
    //----------------------------------- START/UPDATE functions -------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region START_UPDATE_FUNCTIONS
    /// <summary>
    /// Initializing the scene
    /// </summary>
    public void Start()
    {
        Debug.Log("[NetworkController] NetworkingController is now in Start()");
        _floorNumberSignRenderer = _floorNumberSign.GetComponent<Renderer>();
        _floorRoomNumberSignRenderer = _floorRoomNumberSign.GetComponent<Renderer>();
        _localPlayer = Networking.LocalPlayer;
        _userIsInVR = _localPlayer.IsUserInVR();
        //the first master has to set the constant scene settings
        if (_localPlayer.isMaster && 0L == (_syncData2 & (SyncBool_MaskLong2_Initialized)))
        {
            _isMaster = true;
            MASTER_SetConstSceneElevatorStates();
            MASTER_FirstMasterSetupElevatorControl();
        }
        _elevatorControllerReception.CustomStart();
        _elevatorControllerArrivalArea.CustomStart();
        //for reception level
        if (_userIsInVR)
        {
            _InsidePanelScriptElevatorForVR_0.CustomStart();
            _InsidePanelScriptElevatorForVR_1.CustomStart();
            _InsidePanelScriptElevatorForVR_2.CustomStart();
            _InsidePanelFloorScriptElevatorForVR_0.CustomStart();
            _InsidePanelFloorScriptElevatorForVR_1.CustomStart();
            _InsidePanelFloorScriptElevatorForVR_2.CustomStart();
            Debug.Log("[NetworkController] Fired CustomStart for Inside Button Panels");
        }
        else
        {
            _insidePanelScriptElevatorDesktop_0.CustomStart();
            _insidePanelScriptElevatorDesktop_1.CustomStart();
            _insidePanelScriptElevatorDesktop_2.CustomStart();
            _insidePanelFloorScriptElevatorDesktop_0.CustomStart();
            _insidePanelFloorScriptElevatorDesktop_1.CustomStart();
            _insidePanelFloorScriptElevatorDesktop_2.CustomStart();
        }
        Debug.Log("[NetworkController] Elevator NetworkingController is now loaded");
        _worldIsLoaded = true;
    }
    /// <summary>
    /// This update is run every frame
    /// </summary>
    public void Update()
    {
        //first checking if there is a pending teleport request
        CheckTeleportCounter();
        //master needs to run a different routine
        if (_localPlayer.isMaster)
        {
            if (!_isMaster)
            {
                Debug.Log("[NetworkController] Master has changed!");
                MASTER_OnMasterChanged();
                _isMaster = true;
            }
            //first process network events because Master can't do that else
            LOCAL_OnDeserialization();
            //only the current master does this
            MASTER_RunElevatorControl();
        }
        //Checking if local external call was handled or dropped
        LOCAL_CheckIfElevatorExternalCallWasReceived();
        //Checking if local internal call was handled or dropped
        LOCAL_CheckIfElevatorInternalCallWasReceived();
    }
    #endregion START_UPDATE_FUNCTIONS
    //------------------------------------------------------------------------------------------------------------
    //----------------------------------- MASTER functions -------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region MASTER_FUNCTIONS
    /// <summary>
    /// locally storing where each elevator is and has to go, these need to be checked against SyncBool states 
    /// </summary>    
    private bool[] _calledToFloorToGoUp_MASTER = new bool[14];
    private int _calledToFloorToGoUp_MASTER_COUNT = 0;
    private bool[] _calledToFloorToGoDown_MASTER = new bool[14];
    private int _calledToFloorToGoDown_MASTER_COUNT = 0;
    private bool[] _elevator0FloorTargets_MASTER = new bool[14];
    private int _elevator0FloorTargets_MASTER_COUNT = 0;
    private bool[] _elevator1FloorTargets_MASTER = new bool[14];
    private int _elevator1FloorTargets_MASTER_COUNT = 0;
    private bool[] _elevator2FloorTargets_MASTER = new bool[14];
    private int _elevator2FloorTargets_MASTER_COUNT = 0;
    private float[] _timeAtCurrentFloorElevatorOpened_MASTER = new float[3];
    private float[] _timeAtCurrentFloorElevatorClosed_MASTER = new float[3];
    private bool[] _floorHasFakeEREQ_MASTER = new bool[14];
    private int _elevatorCheckTick_MASTER = 1;
    /// <summary>
    /// The first Master (on instance start) needs to run this once to set the initial elevator states and positions
    /// </summary>
    private void MASTER_FirstMasterSetupElevatorControl()
    {
        Debug.Log("[NetworkController] FirstMasterSetupElevatorControl started");
        MASTER_SetSyncValue(SyncBool_Elevator0goingUp, false);
        MASTER_SetSyncValue(SyncBool_Elevator1goingUp, false);
        MASTER_SetSyncValue(SyncBool_Elevator2goingUp, false);
        MASTER_SetSyncValue(SyncBool_Elevator0idle, true);
        MASTER_SetSyncValue(SyncBool_Elevator1idle, true);
        MASTER_SetSyncValue(SyncBool_Elevator2idle, true);
        MASTER_SetSyncElevatorFloor(0, 13);
        MASTER_SetSyncElevatorFloor(1, 8);
        MASTER_SetSyncElevatorFloor(2, 2);
        Debug.Log("[NetworkController] FirstMasterSetupElevatorControl finished");
    }
    /// <summary>
    /// When the master changes, we need to load the SyncBool states into local copies to run the elevator controller correct
    /// before we actually run the elevator controller for the first time
    /// </summary>
    private void MASTER_OnMasterChanged()
    {
        //resetting arrays and counters        
        _elevator0FloorTargets_MASTER = new bool[14];
        _elevator0FloorTargets_MASTER_COUNT = 0;
        _elevator1FloorTargets_MASTER = new bool[14];
        _elevator1FloorTargets_MASTER_COUNT = 0;
        _elevator2FloorTargets_MASTER = new bool[14];
        _elevator2FloorTargets_MASTER_COUNT = 0;
        _calledToFloorToGoUp_MASTER = new bool[14];
        _calledToFloorToGoUp_MASTER_COUNT = 0;
        _calledToFloorToGoDown_MASTER = new bool[14];
        _calledToFloorToGoDown_MASTER_COUNT = 0;
        _floorHasFakeEREQ_MASTER = new bool[14];
        //taking all content from SyncedData into local arrays
        for (int i = 0; i <= 13; i++)
        {
            //If Elevator0 called to floor i
            if (0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_Elevator0CalledToFloor + i))))
            {
                _elevator0FloorTargets_MASTER[i] = true;
                _elevator0FloorTargets_MASTER_COUNT++;
            }
            //If Elevator1 called to floor i
            if (0L != (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator1CalledToFloor + i))))
            {
                _elevator1FloorTargets_MASTER[i] = true;
                _elevator1FloorTargets_MASTER_COUNT++;
            }
            //If Elevator2 called to floor i
            if (0L != (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator2CalledToFloor + i))))
            {
                _elevator2FloorTargets_MASTER[i] = true;
                _elevator2FloorTargets_MASTER_COUNT++;
            }
            //If floor has "Called Up" pressed
            if (0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + i))))
            {
                _calledToFloorToGoUp_MASTER[i] = true;
                _calledToFloorToGoUp_MASTER_COUNT++;
            }
            //If floor has "Called Down" pressed
            if (0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + i))))
            {
                _calledToFloorToGoDown_MASTER[i] = true;
                _calledToFloorToGoDown_MASTER_COUNT++;
            }
        }
        for (int i = 0; i <= 2; i++)
        {
            _timeAtCurrentFloorElevatorOpened_MASTER[i] = Time.time;
            _timeAtCurrentFloorElevatorClosed_MASTER[i] = Time.time;
        }
        _elevatorCheckTick_MASTER = 1;
    }
    /// <summary>
    /// The master runs this elevator controller in every Update()
    /// The load is splitted accross 3 frames to have a better performance
    /// </summary>
    private void MASTER_RunElevatorControl()
    {
        if (_elevator0Working && _elevatorCheckTick_MASTER == 1)
        {
            MASTER_RunElevator(0, _elevator0FloorTargets_MASTER);
        }
        if (_elevator1Working && _elevatorCheckTick_MASTER == 2)
        {
            MASTER_RunElevator(1, _elevator1FloorTargets_MASTER);
        }
        if (_elevator2Working && _elevatorCheckTick_MASTER == 3)
        {
            MASTER_RunElevator(2, _elevator2FloorTargets_MASTER);
        }
        _elevatorCheckTick_MASTER++;
        //TODO: Remove before pushing live
        if (false && _elevatorCheckTick_MASTER == 4)
        {
            if (_elevator0FloorTargets_MASTER_COUNT != 0)
                Debug.Log("_elevator0FloorTargets_MASTER_COUNT:" + _elevator0FloorTargets_MASTER_COUNT);
            if (_elevator1FloorTargets_MASTER_COUNT != 0)
                Debug.Log("_elevator1FloorTargets_MASTER_COUNT:" + _elevator1FloorTargets_MASTER_COUNT);
            if (_elevator2FloorTargets_MASTER_COUNT != 0)
                Debug.Log("_elevator2FloorTargets_MASTER_COUNT:" + _elevator2FloorTargets_MASTER_COUNT);
            if (_calledToFloorToGoUp_MASTER_COUNT != 0)
                Debug.Log("_calledToFloorToGoUp_MASTER_COUNT:" + _calledToFloorToGoUp_MASTER_COUNT);
            if (_calledToFloorToGoDown_MASTER_COUNT != 0)
                Debug.Log("_calledToFloorToGoDown_MASTER_COUNT:" + _calledToFloorToGoDown_MASTER_COUNT);
        }
        if (_elevatorCheckTick_MASTER >= 4)
            _elevatorCheckTick_MASTER = 1;
    }
    /// <summary>
    /// Running a single elevator, is only called by master in every Update
    /// 
    /// TODO: Moving down without internal target! (currently, setting an EREQ is a workaround for cheaper calculations)
    /// TODO: Ignore targets that generated fake-EREQs for other elevators
    /// 
    /// </summary>
    private void MASTER_RunElevator(int elevatorNumber, bool[] elevatorFloorTargets)
    {
        //Debug.Log("Elevator " + elevatorNumber + " has " + MASTER_GetInternalTargetCount(elevatorNumber) + " targets.");
        int currentFloor = GetSyncElevatorFloor(elevatorNumber);
        bool elevatorIdle;
        bool elevatorGoingUp;
        bool elevatorOpen;

        if (elevatorNumber == 0)
        {
            elevatorIdle = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0idle)));
            elevatorGoingUp = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0goingUp)));
            elevatorOpen = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0open)));
        }
        else if (elevatorNumber == 1)
        {
            elevatorIdle = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1idle)));
            elevatorGoingUp = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1goingUp)));
            elevatorOpen = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1open)));
        }
        else
        {
            elevatorIdle = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2idle)));
            elevatorGoingUp = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2goingUp)));
            elevatorOpen = (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2open)));
        }

        bool targetFound = false;
        //we can't handle people blocking the elevator, so we will ignore ongoing requests and save them for later
        if (elevatorOpen)
        {
            //an elevator must stay open for n seconds
            if (!(currentFloor == 0) && Time.time - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > TIME_TO_STAY_OPEN || currentFloor == 0 && Time.time - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > TIME_TO_STAY_OPEN_RECEPTION)
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " closing on floor " + currentFloor);
                //time to close this elevator
                MASTER_SetSyncValue(SyncBool_Elevator0open + elevatorNumber, false);
                _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = Time.time;
                //closing it was enough for now
                return;
            }
            else
            {
                //while open, check if there is another internal target, else set the elevator idle
                if (!elevatorIdle && MASTER_GetInternalTargetCount(elevatorNumber) == 0)
                {
                    MASTER_SetElevatorIdle(elevatorNumber);
                    elevatorIdle = true;
                    elevatorGoingUp = false;
                }
                //handle all targets that are pointing in the current direction on the current floor
                MASTER_HandleFloorTarget(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                //we can't move an open elevator, so the code ends here
                return;
            }
        }
        else if (0L == (_syncData2 & (1L << (SyncBool_AddressLong2_ElevatorXIsDriving + elevatorNumber))))
        {
            if (Time.time - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < TIME_TO_STAY_CLOSED)
            {
                //an elevator must stay closed for the duration of the closing animation
                //however, we could still process user-requests to open it again here
                //we can't move an elevator that isn't fully closed yet
                return;
            }
            else
            {
                //Doors closed and timeout exceeded. Set elevator to drive and block door requests
                MASTER_SetSyncValue(SyncBool_Elevator0IsDriving + elevatorNumber, true);
            }
        }
        else if (Time.time - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < TIME_TO_DRIVE_ONE_FLOOR)
        {
            //driving a floor must take a certain amount of time
            return;
        }
        //an elevator that is going up will only handle internal targets and up requests
        if (!elevatorIdle && elevatorGoingUp)
        {
            //when the current floor was requested and we've arrived
            if (elevatorFloorTargets[currentFloor] || _calledToFloorToGoUp_MASTER[currentFloor])
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " opening on floor " + currentFloor);
                MASTER_HandleFloorDoorOpening(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                //the code must end here since we just stopped the elevator
                return;
            }
            else if (MASTER_GetInternalTargetCount(elevatorNumber) != 0) //checking for next target
            {
                for (int i = currentFloor + 1; i <= 13; i++)
                {
                    if (elevatorFloorTargets[i]) //those are internal targets called from passengers which have priority
                    {
                        targetFound = true;
                        break;
                    }
                }
                if (targetFound) //this means that there is a target on the way up, so we drive one level up
                {
                    Debug.Log("[NetworkController] Elevator " + elevatorNumber + " driving up to floor " + (int)(currentFloor + 1));
                    MASTER_SetSyncElevatorFloor(elevatorNumber, currentFloor + 1);
                    _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = Time.time; //resetting the timer for next floor
                                                                                          //the code must end here since we are now travelling further
                    return;
                }
                //since there is no internal target on the way up, we now check if there is one on the way down
                //first checking if there is a target on the way down (haha lol rip people on higher floors)
                for (int i = currentFloor - 1; i >= 0; i--)
                {
                    if (elevatorFloorTargets[i]) //those are internal targets called from passengers
                    {
                        targetFound = true;
                        break;
                    }
                }
                if (targetFound)
                {
                    // this means we are now reversing the elevator direction
                    MASTER_SetElevatorDirection(elevatorNumber, goingUp: false);
                    elevatorGoingUp = false;
                    // since the following code will handle this direction, we don't need to do anything else.
                }
            }
        }
        if (!elevatorIdle && !elevatorGoingUp) //this elevator can handle internal targets and external down-requests
        {
            //when the current floor was internally or externally requested and we've arrived
            if (elevatorFloorTargets[currentFloor] || _calledToFloorToGoDown_MASTER[currentFloor])
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " opening on floor " + currentFloor);
                MASTER_HandleFloorDoorOpening(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                //the code must end here since we just opened the elevator
                return;
            }
            else if (MASTER_GetInternalTargetCount(elevatorNumber) != 0) //checking for next target
            {
                if (!targetFound)
                {
                    //first checking if there is a target on the way down
                    for (int i = currentFloor - 1; i >= 0; i--)
                    {
                        if (elevatorFloorTargets[i]) //those are internal targets called from passengers
                        {
                            targetFound = true;
                            break;
                        }
                    }
                }
                if (targetFound) //this means that there is a target on the way down, so we drive one level down
                {
                    Debug.Log("[NetworkController] Elevator " + elevatorNumber + " driving down to floor " + (int)(currentFloor - 1));
                    MASTER_SetSyncElevatorFloor(elevatorNumber, currentFloor - 1);
                    _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = Time.time; //resetting the timer for next floor
                                                                                          //the code must end here since we are now travelling further
                    return;
                }
                //since there is no internal target on the way down, we now check if there is one on the way up
                for (int i = currentFloor + 1; i <= 13; i++)
                {
                    if (elevatorFloorTargets[i]) //those are internal targets called from passengers
                    {
                        targetFound = true;
                        break;
                    }
                }
                if (targetFound)
                {
                    // this means we are now reversing the elevator direction
                    MASTER_SetSyncValue(SyncBool_Elevator0goingUp + elevatorNumber, true);
                    // since the next loop code will handle this direction, we need to stop execution now
                    return;
                }
            }
        }
        //when we reach this line of code, the elevator found no internal target and is closed
        //when the current floor was requested by anyone and we are already there, we open the elevator
        if (elevatorFloorTargets[currentFloor] || _calledToFloorToGoUp_MASTER[currentFloor] || _calledToFloorToGoDown_MASTER[currentFloor])
        {
            Debug.Log("[NetworkController] Elevator " + elevatorNumber + " opening again on floor " + currentFloor);
            MASTER_HandleFloorDoorOpening(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
            //the code must end here since we just stopped the elevator
            return;
        }
        //if we reach this line, we need to find the next target first
        int nextTarget = 0;
        if (MASTER_GetInternalTargetCount(elevatorNumber) != 0) //checking for next target
        {
            //Now we need to check if there is an internal target on the way up
            for (int i = currentFloor + 1; i <= 13; i++)
            {
                if (elevatorFloorTargets[i]) //those are all internal targets
                {
                    targetFound = true;
                    nextTarget = i;
                    break;
                }
            }
            //if we found an internal target, we go out of idle mode and set the new direction up
            if (targetFound)
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an internal target and is going up");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp: true);
                return;
            }
            //Now we need to check if there is an internal target on the way down
            for (int i = currentFloor - 1; i >= 0; i--)
            {
                if (elevatorFloorTargets[i]) //those are internal targets called from passengers
                {
                    targetFound = true;
                    nextTarget = i;
                    break;
                }
            }
            //if we found an internal target, we go out of idle mode and set the new direction down
            if (targetFound)
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an internal target and is going down");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp: false);
                return;
            }
        }
        //--------------------------------------------------------------------------------------------------
        if (_calledToFloorToGoUp_MASTER_COUNT != 0 || _calledToFloorToGoDown_MASTER_COUNT != 0) //checking for next target
        {
            //if we reach this code line, there is no internal target and we need to check external targets next
            for (int i = currentFloor + 1; i <= 13; i++)
            {
                if ((_calledToFloorToGoUp_MASTER[i] || _calledToFloorToGoDown_MASTER[i]) && !_floorHasFakeEREQ_MASTER[i])  //those are external targets
                {
                    targetFound = true;
                    nextTarget = i;
                    break;
                }
            }
            //if we found an internal target, we go out of idle mode and set the new direction up
            if (targetFound)
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an external target and is going up");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp: true);
                //this elevator basicly belongs to that floor then, so both targets are handled, but this isn't perfect
                Debug.Log("[NetworkController] We're faking an EREQ next to set an internal target");
                ELREQ_SetInternalTarget(elevatorNumber, nextTarget);
                _floorHasFakeEREQ_MASTER[nextTarget] = true;
                return;
            }
            //Now we need to check if there is an external target on the way down
            for (int i = currentFloor - 1; i >= 0; i--)
            {
                if ((_calledToFloorToGoUp_MASTER[i] || _calledToFloorToGoDown_MASTER[i]) && !_floorHasFakeEREQ_MASTER[i]) //those are external targets
                {
                    targetFound = true;
                    nextTarget = i;
                    break;
                }
            }
            //if we found an internal target, we go out of idle mode and set the new direction down
            if (targetFound)
            {
                Debug.Log("[NetworkController] Elevator " + elevatorNumber + " was idle but now as an external target and is going down");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp: false);
                //this elevator basicly belongs to that floor then, so both targets are handled, but this isn't perfect
                Debug.Log("[NetworkController] We're faking an EREQ next to set an internal target");
                ELREQ_SetInternalTarget(elevatorNumber, nextTarget);
                _floorHasFakeEREQ_MASTER[nextTarget] = true;
                return;
            }
        }
        //------------------------------------
        //reaching this code line means there is no next target and the elevator must go into idle mode
        if (!elevatorIdle)
        {
            Debug.Log("[NetworkController] Elevator " + elevatorNumber + " is now idle since there are no targets.");
            MASTER_SetElevatorIdle(elevatorNumber);
        }
    }
    /// <summary>
    /// Resets the elevator call button network when the elevator opens
    /// </summary>
    /// <param name="floor"></param>
    private void MASTER_HandleFloorDoorOpening(int elevatorNumber, int currentFloor, bool directionUp, bool isIdle)
    {
        //TODO: I tried to solve this issue here, please check/test
        //When the other directional button is pressed on that floor level and we should consider to handle it
        //and set this elevator to idle if the other button on that floor wasn't pressed and there is no internal target
        //on this way, so the elevator would reverse next. We also need to reverse the elevator in that case.
        //now we can actually open the elevator
        if (!isIdle && MASTER_GetInternalTargetCount(elevatorNumber) == 0)
        {
            //time to set it idle
            MASTER_SetElevatorIdle(elevatorNumber);
            isIdle = true; directionUp = false;
        }
        //preparing to check internal targets if there are any
        else if (!isIdle)
        {
            bool internalTargetAboveFound = false;
            bool internalTargetBelowFound = false;
            bool internalTargetOnThisLevel;
            bool[] elevatorFloorTargets;
            switch (elevatorNumber)
            {
                case 0:
                    elevatorFloorTargets = _elevator0FloorTargets_MASTER;
                    break;
                case 1:
                    elevatorFloorTargets = _elevator1FloorTargets_MASTER;
                    break;
                default:
                    elevatorFloorTargets = _elevator2FloorTargets_MASTER;
                    break;
            }
            internalTargetOnThisLevel = elevatorFloorTargets[currentFloor];
            if (internalTargetOnThisLevel && MASTER_GetInternalTargetCount(elevatorNumber) == 1)
            {
                //time to set it idle
                MASTER_SetElevatorIdle(elevatorNumber);
                isIdle = true; directionUp = false;
            }
            else //this means we have at least one internal target below or above
            {
                //we need to check if there is an internal target below
                for (int i = currentFloor - 1; i >= 0; i--)
                {
                    if (elevatorFloorTargets[i]) //those are internal targets called from passengers
                    {
                        internalTargetBelowFound = true;
                        break;
                    }
                }
                //we need to check if there is an internal target above
                for (int i = currentFloor + 1; i <= 13; i++)
                {
                    if (elevatorFloorTargets[i]) //those are internal targets called from passengers
                    {
                        internalTargetAboveFound = true;
                        break;
                    }
                }
                //now check if we need to reverse
                if ((directionUp && !internalTargetAboveFound) || (!directionUp && !internalTargetBelowFound))
                {
                    //this means we need to reverse
                    directionUp = !directionUp;
                    MASTER_SetElevatorDirection(elevatorNumber, directionUp);
                    MASTER_HandleFloorTarget(elevatorNumber, currentFloor, directionUp, isIdle);
                }
            }
        }
        //then handle the floor targets
        MASTER_HandleFloorTarget(elevatorNumber, currentFloor, directionUp, isIdle);
        MASTER_SetSyncValue(SyncBool_Elevator0open + elevatorNumber, true); //opening the elevator
        MASTER_SetSyncValue(SyncBool_Elevator0IsDriving + elevatorNumber, false);
        _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] = Time.time;
    }
    /// <summary>
    /// Handles all floor targets on the current floor and direction
    /// </summary>
    /// <param name="floor"></param>
    private void MASTER_HandleFloorTarget(int elevatorNumber, int currentFloor, bool directionUp, bool isIdle)
    {
        if ((directionUp || isIdle) && _calledToFloorToGoUp_MASTER[currentFloor])
        {
            MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + currentFloor, false);
            _calledToFloorToGoUp_MASTER[currentFloor] = false; //this target was now handled
            _calledToFloorToGoUp_MASTER_COUNT--;
        }
        if ((!directionUp || isIdle) && _calledToFloorToGoDown_MASTER[currentFloor])
        {
            MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + currentFloor, false);
            _calledToFloorToGoDown_MASTER[currentFloor] = false; //this target was now handled
            _calledToFloorToGoDown_MASTER_COUNT--;
        }
        if (elevatorNumber == 0 && _elevator0FloorTargets_MASTER[currentFloor])
        {
            MASTER_SetSyncValue(SyncBoolReq_Elevator0CalledToFloor_0 + currentFloor, false);
            _elevator0FloorTargets_MASTER[currentFloor] = false;
            _elevator0FloorTargets_MASTER_COUNT--;
        }
        else if (elevatorNumber == 1 && _elevator1FloorTargets_MASTER[currentFloor])
        {
            MASTER_SetSyncValue(SyncBoolReq_Elevator1CalledToFloor_0 + currentFloor, false);
            _elevator1FloorTargets_MASTER[currentFloor] = false;
            _elevator1FloorTargets_MASTER_COUNT--;
        }
        else if (elevatorNumber == 2 && _elevator2FloorTargets_MASTER[currentFloor])
        {
            MASTER_SetSyncValue(SyncBoolReq_Elevator2CalledToFloor_0 + currentFloor, false);
            _elevator2FloorTargets_MASTER[currentFloor] = false;
            _elevator2FloorTargets_MASTER_COUNT--;
        }
        _floorHasFakeEREQ_MASTER[currentFloor] = false;
    }
    /// <summary>
    /// Sets the elevator travel direction
    /// </summary>
    private void MASTER_SetElevatorDirection(int elevatorNumber, bool goingUp)
    {
        MASTER_SetSyncValue(SyncBool_Elevator0goingUp + elevatorNumber, goingUp);
        //If elevator x is Idle
        if (0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))))
        {
            MASTER_SetSyncValue(SyncBool_Elevator0idle + elevatorNumber, false);
            _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = Time.time + TIME_TO_STAY_CLOSED - TIME_TO_STAY_CLOSED_AFTER_GOING_OUT_OF_IDLE;
        }
    }
    /// <summary>
    /// Setting the elevator in idle mode
    /// </summary>
    private void MASTER_SetElevatorIdle(int elevatorNumber)
    {
        MASTER_SetSyncValue(SyncBool_Elevator0goingUp + elevatorNumber, false);
        MASTER_SetSyncValue(SyncBool_Elevator0idle + elevatorNumber, true);
    }
    /// <summary>
    /// returns the number of internal floor targets for an elevator
    /// </summary>
    private int MASTER_GetInternalTargetCount(int elevatorNumber)
    {
        if (elevatorNumber == 0)
            return _elevator0FloorTargets_MASTER_COUNT;
        if (elevatorNumber == 1)
            return _elevator1FloorTargets_MASTER_COUNT;
        if (elevatorNumber == 2)
            return _elevator2FloorTargets_MASTER_COUNT;
        Debug.Log("ERROR: Unknown elevator number in MASTER_GetInternalTargetCount!");
        return 0; // to make the compiler happy
    }
    //-----------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///  sets the elevators into the random state which is determined by master user uwu
    ///  this can't be "random", but we have a pool of 7 allowed states
    /// </summary>
    private void MASTER_SetConstSceneElevatorStates()
    {
        //to make testing easier, we only allow one state right now
        int random = 0; // UnityEngine.Random.Range(0, 7);
        switch (random)
        {
            case 0:
                MASTER_SetSyncValue(SyncBool_Elevator0working, true);
                MASTER_SetSyncValue(SyncBool_Elevator1working, true);
                MASTER_SetSyncValue(SyncBool_Elevator2working, true);
                break;
            case 1:
                MASTER_SetSyncValue(SyncBool_Elevator0working, false);
                MASTER_SetSyncValue(SyncBool_Elevator1working, true);
                MASTER_SetSyncValue(SyncBool_Elevator2working, true);
                break;
            case 2:
                MASTER_SetSyncValue(SyncBool_Elevator0working, true);
                MASTER_SetSyncValue(SyncBool_Elevator1working, false);
                MASTER_SetSyncValue(SyncBool_Elevator2working, true);
                break;
            case 3:
                MASTER_SetSyncValue(SyncBool_Elevator0working, true);
                MASTER_SetSyncValue(SyncBool_Elevator1working, true);
                MASTER_SetSyncValue(SyncBool_Elevator2working, false);
                break;
            case 4:
                MASTER_SetSyncValue(SyncBool_Elevator0working, true);
                MASTER_SetSyncValue(SyncBool_Elevator1working, false);
                MASTER_SetSyncValue(SyncBool_Elevator2working, false);
                break;
            case 5:
                MASTER_SetSyncValue(SyncBool_Elevator0working, false);
                MASTER_SetSyncValue(SyncBool_Elevator1working, true);
                MASTER_SetSyncValue(SyncBool_Elevator2working, false);
                break;
            default:
                MASTER_SetSyncValue(SyncBool_Elevator0working, false);
                MASTER_SetSyncValue(SyncBool_Elevator1working, false);
                MASTER_SetSyncValue(SyncBool_Elevator2working, true);
                break;
        }
        MASTER_SetSyncValue(SyncBool_Initialized, true);
        Debug.Log("[NetworkController] Random elevator states are now set by master");
    }
    #endregion MASTER_FUNCTIONS
    //------------------------------------------------------------------------------------------------------------
    //----------------------------------- LOCAL functions --------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region LOCAL_FUNCTIONS
    /// <summary>
    /// elevator request states, synced by master
    /// </summary>
    private long _localSyncData1 = 0;
    private long _localSyncData2 = 0;
    private bool[] _localSyncDataBools = new bool[116];
    /// <summary>
    /// The long maps as follows:-
    ///  - 63-60 variable_1 (4bits)
    ///  - 59-56 variable_2 (4bits)
    ///  - 55-52 variable_3 (4bits)
    ///  - 0-51 binary bools [0-51]
    ///
    /// The uint maps as follows:-
    ///  - 0-31 binary bools [52-83(?)]
    ///  
    /// Is run every time a network packet is received by localPlayer
    /// </summary>
    private void LOCAL_CheckSyncData()
    {
        //check if something from this synced var has changed
        if (_syncData1 != _localSyncData1)
        {
            //position 52 to position 63 are floor levels that might have changed
            LOCAL_CheckElevatorLevels();
            bool[] cachedSync1Bools = GetBoolArrayLong1ONLY();
            //the positions 0-51 are binary bools that might have changed
            for (int i = 0; i < 52; i++) //no need to check bool 0
            {
                if (cachedSync1Bools[i] != _localSyncDataBools[i])
                {
                    LOCAL_HandleSyncBoolChanged(i);
                }
            }
            //store new local sync data 1
            _localSyncData1 = _syncData1;
        }
        //check if something from this synced var has changed
        if (_syncData2 != _localSyncData2)
        {
            bool[] cachedSync2Bools = GetBoolArrayLong2ONLY();
            //the positions 0-31 are binary bools that might have changed (position 52-83)
            for (int i = 52; i < 116; i++)
            {
                if (cachedSync2Bools[i] != _localSyncDataBools[i])
                {
                    LOCAL_HandleSyncBoolChanged(i);
                }
            }
            //store new local sync data 2
            _localSyncData2 = _syncData2;
        }
    }
    /// <summary>
    /// Is called when the syncbool on position <see cref="syncBoolPosition"/> has changed
    /// </summary>
    private void LOCAL_HandleSyncBoolChanged(int syncBoolPosition)
    {
        //read the new state
        bool newState = !_localSyncDataBools[syncBoolPosition];
        //change the locally known state to it
        _localSyncDataBools[syncBoolPosition] = newState;
        //adjust the scene elements to the new state
        switch (syncBoolPosition)
        {
            case SyncBool_Elevator0open:
                LOCAL_OpenCloseElevator(0, setOpen: newState);
                break;
            case SyncBool_Elevator1open:
                LOCAL_OpenCloseElevator(1, setOpen: newState);
                break;
            case SyncBool_Elevator2open:
                LOCAL_OpenCloseElevator(2, setOpen: newState);
                break;
            case SyncBool_Elevator0idle:
                LOCAL_SetElevatorIdle(0, isIdle: newState);
                break;
            case SyncBool_Elevator1idle:
                LOCAL_SetElevatorIdle(1, isIdle: newState);
                break;
            case SyncBool_Elevator2idle:
                LOCAL_SetElevatorIdle(2, isIdle: newState);
                break;
            case SyncBool_Elevator0goingUp:
                LOCAL_SetElevatorDirection(0, goingUp: newState);
                break;
            case SyncBool_Elevator1goingUp:
                LOCAL_SetElevatorDirection(1, goingUp: newState);
                break;
            case SyncBool_Elevator2goingUp:
                LOCAL_SetElevatorDirection(2, goingUp: newState);
                break;
            //case SyncBoolReq_ElevatorCalled#J2#_#1#:
            //LOCAL_SetElevatorCallButtonState(#1#, buttonUp: #J1#, called: newState);
            //break;
            case SyncBoolReq_ElevatorCalledUp_0:
                LOCAL_SetElevatorCallButtonState(0, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_1:
                LOCAL_SetElevatorCallButtonState(1, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_2:
                LOCAL_SetElevatorCallButtonState(2, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_3:
                LOCAL_SetElevatorCallButtonState(3, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_4:
                LOCAL_SetElevatorCallButtonState(4, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_5:
                LOCAL_SetElevatorCallButtonState(5, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_6:
                LOCAL_SetElevatorCallButtonState(6, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_7:
                LOCAL_SetElevatorCallButtonState(7, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_8:
                LOCAL_SetElevatorCallButtonState(8, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_9:
                LOCAL_SetElevatorCallButtonState(9, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_10:
                LOCAL_SetElevatorCallButtonState(10, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_11:
                LOCAL_SetElevatorCallButtonState(11, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_12:
                LOCAL_SetElevatorCallButtonState(12, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledUp_13:
                LOCAL_SetElevatorCallButtonState(13, buttonUp: true, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_0:
                LOCAL_SetElevatorCallButtonState(0, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_1:
                LOCAL_SetElevatorCallButtonState(1, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_2:
                LOCAL_SetElevatorCallButtonState(2, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_3:
                LOCAL_SetElevatorCallButtonState(3, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_4:
                LOCAL_SetElevatorCallButtonState(4, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_5:
                LOCAL_SetElevatorCallButtonState(5, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_6:
                LOCAL_SetElevatorCallButtonState(6, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_7:
                LOCAL_SetElevatorCallButtonState(7, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_8:
                LOCAL_SetElevatorCallButtonState(8, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_9:
                LOCAL_SetElevatorCallButtonState(9, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_10:
                LOCAL_SetElevatorCallButtonState(10, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_11:
                LOCAL_SetElevatorCallButtonState(11, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_12:
                LOCAL_SetElevatorCallButtonState(12, buttonUp: false, isCalled: newState);
                break;
            case SyncBoolReq_ElevatorCalledDown_13:
                LOCAL_SetElevatorCallButtonState(13, buttonUp: false, isCalled: newState);
                break;
            //case SyncBoolReq_Elevator#J1#CalledToFloor_#1#:
            //    LOCAL_SetElevatorInternalButtonState(#J1#,#1#, called: newState);
            //    break;
            case SyncBoolReq_Elevator0CalledToFloor_0:
                LOCAL_SetElevatorInternalButtonState(0, 4, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_1:
                LOCAL_SetElevatorInternalButtonState(0, 5, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_2:
                LOCAL_SetElevatorInternalButtonState(0, 6, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_3:
                LOCAL_SetElevatorInternalButtonState(0, 7, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_4:
                LOCAL_SetElevatorInternalButtonState(0, 8, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_5:
                LOCAL_SetElevatorInternalButtonState(0, 9, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_6:
                LOCAL_SetElevatorInternalButtonState(0, 10, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_7:
                LOCAL_SetElevatorInternalButtonState(0, 11, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_8:
                LOCAL_SetElevatorInternalButtonState(0, 12, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_9:
                LOCAL_SetElevatorInternalButtonState(0, 13, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_10:
                LOCAL_SetElevatorInternalButtonState(0, 14, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_11:
                LOCAL_SetElevatorInternalButtonState(0, 15, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_12:
                LOCAL_SetElevatorInternalButtonState(0, 16, called: newState);
                break;
            case SyncBoolReq_Elevator0CalledToFloor_13:
                LOCAL_SetElevatorInternalButtonState(0, 17, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_0:
                LOCAL_SetElevatorInternalButtonState(1, 4, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_1:
                LOCAL_SetElevatorInternalButtonState(1, 5, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_2:
                LOCAL_SetElevatorInternalButtonState(1, 6, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_3:
                LOCAL_SetElevatorInternalButtonState(1, 7, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_4:
                LOCAL_SetElevatorInternalButtonState(1, 8, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_5:
                LOCAL_SetElevatorInternalButtonState(1, 9, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_6:
                LOCAL_SetElevatorInternalButtonState(1, 10, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_7:
                LOCAL_SetElevatorInternalButtonState(1, 11, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_8:
                LOCAL_SetElevatorInternalButtonState(1, 12, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_9:
                LOCAL_SetElevatorInternalButtonState(1, 13, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_10:
                LOCAL_SetElevatorInternalButtonState(1, 14, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_11:
                LOCAL_SetElevatorInternalButtonState(1, 15, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_12:
                LOCAL_SetElevatorInternalButtonState(1, 16, called: newState);
                break;
            case SyncBoolReq_Elevator1CalledToFloor_13:
                LOCAL_SetElevatorInternalButtonState(1, 17, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_0:
                LOCAL_SetElevatorInternalButtonState(2, 4, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_1:
                LOCAL_SetElevatorInternalButtonState(2, 5, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_2:
                LOCAL_SetElevatorInternalButtonState(2, 6, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_3:
                LOCAL_SetElevatorInternalButtonState(2, 7, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_4:
                LOCAL_SetElevatorInternalButtonState(2, 8, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_5:
                LOCAL_SetElevatorInternalButtonState(2, 9, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_6:
                LOCAL_SetElevatorInternalButtonState(2, 10, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_7:
                LOCAL_SetElevatorInternalButtonState(2, 11, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_8:
                LOCAL_SetElevatorInternalButtonState(2, 12, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_9:
                LOCAL_SetElevatorInternalButtonState(2, 13, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_10:
                LOCAL_SetElevatorInternalButtonState(2, 14, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_11:
                LOCAL_SetElevatorInternalButtonState(2, 15, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_12:
                LOCAL_SetElevatorInternalButtonState(2, 16, called: newState);
                break;
            case SyncBoolReq_Elevator2CalledToFloor_13:
                LOCAL_SetElevatorInternalButtonState(2, 17, called: newState);
                break;
            case 80: //initialized-bool
            case 81: //elevator1-working
            case 82: //elevator2-working
            case 83: //elevator3-working
                break;
            default:
                Debug.Log("ERROR: UNKNOWN BOOL HAS CHANGED IN SYNCBOOL, position: " + syncBoolPosition);
                break;
        }
    }
    /// <summary>
    /// Storing locally known elevator levels
    /// </summary>
    private int _localElevator0Level = 15;
    private int _localElevator1Level = 15;
    private int _localElevator2Level = 15;
    /// <summary>
    /// Checking all elevatorlevels if they have changed from master
    /// </summary>
    private void LOCAL_CheckElevatorLevels()
    {
        if (_elevator0Working && _localElevator0Level != GetSyncElevatorFloor(0))
        {
            int floorNumber = GetSyncElevatorFloor(0);
            _elevatorControllerReception.SetElevatorLevelOnDisplay(floorNumber, 0);
            _elevatorControllerArrivalArea.SetElevatorLevelOnDisplay(floorNumber, 0);
            _localElevator0Level = floorNumber;
        }
        if (_elevator1Working && _localElevator1Level != GetSyncElevatorFloor(1))
        {
            int floorNumber = GetSyncElevatorFloor(1);
            _elevatorControllerReception.SetElevatorLevelOnDisplay(floorNumber, 1);
            _elevatorControllerArrivalArea.SetElevatorLevelOnDisplay(floorNumber, 1);
            _localElevator1Level = floorNumber;
        }
        if (_elevator2Working && _localElevator2Level != GetSyncElevatorFloor(2))
        {
            int floorNumber = GetSyncElevatorFloor(2);
            _elevatorControllerReception.SetElevatorLevelOnDisplay(floorNumber, 2);
            _elevatorControllerArrivalArea.SetElevatorLevelOnDisplay(floorNumber, 2);
            _localElevator2Level = floorNumber;
        }
    }
    /// <summary>
    /// Setting a button inside an elevator to a different state
    /// </summary>
    private void LOCAL_SetElevatorInternalButtonState(int elevatorNumber, int buttonNumber, bool called)
    {
        if (_userIsInVR)
        {
            switch (elevatorNumber)
            {
                case 0:
                    _InsidePanelScriptElevatorForVR_0.SetElevatorInternalButtonState(buttonNumber, called);
                    _InsidePanelFloorScriptElevatorForVR_0.SetElevatorInternalButtonState(buttonNumber, called);
                    break;
                case 1:
                    _InsidePanelScriptElevatorForVR_1.SetElevatorInternalButtonState(buttonNumber, called);
                    _InsidePanelFloorScriptElevatorForVR_1.SetElevatorInternalButtonState(buttonNumber, called);
                    break;
                case 2:
                    _InsidePanelScriptElevatorForVR_2.SetElevatorInternalButtonState(buttonNumber, called);
                    _InsidePanelFloorScriptElevatorForVR_2.SetElevatorInternalButtonState(buttonNumber, called);
                    break;
            }
        }
        else
        {
            switch (elevatorNumber)
            {
                case 0:
                    _insidePanelScriptElevatorDesktop_0.SetElevatorInternalButtonState(buttonNumber, called);
                    _insidePanelFloorScriptElevatorDesktop_0.SetElevatorInternalButtonState(buttonNumber, called);
                    break;
                case 1:
                    _insidePanelScriptElevatorDesktop_1.SetElevatorInternalButtonState(buttonNumber, called);
                    _insidePanelFloorScriptElevatorDesktop_1.SetElevatorInternalButtonState(buttonNumber, called);
                    break;
                case 2:
                    _insidePanelScriptElevatorDesktop_2.SetElevatorInternalButtonState(buttonNumber, called);
                    _insidePanelFloorScriptElevatorDesktop_2.SetElevatorInternalButtonState(buttonNumber, called);
                    break;
            }
        }
    }
    /// <summary>
    /// When a state of a floor callbutton changed, we need to update that button to on or off
    /// </summary>
    private void LOCAL_SetElevatorCallButtonState(int floor, bool buttonUp, bool isCalled)
    {
        if (floor == 0)
        {
            _elevatorControllerReception.SetCallButtonState(buttonUp, isCalled);
        }
        else if (floor == _localPlayerCurrentFloor)
        {
            _elevatorControllerArrivalArea.SetCallButtonState(buttonUp, isCalled);
        }
    }
    /// <summary>
    /// Is run ONCE by localPlayer on scene load.
    /// Setting up the scene at startup or when it isn't setup yet
    /// </summary>
    private void LOCAL_ReadConstSceneElevatorStates()
    {
        Debug.Log("[NetworkController] Setting random elevator states for reception by localPlayer");
        _elevator0Working = 0L != (_syncData2 & (SyncBool_MaskLong2_Elevator0working));
        _elevator1Working = 0L != (_syncData2 & (SyncBool_MaskLong2_Elevator1working));
        _elevator2Working = 0L != (_syncData2 & (SyncBool_MaskLong2_Elevator2working));
        _elevatorControllerReception._elevator1working = _elevator0Working;
        _elevatorControllerReception._elevator2working = _elevator1Working;
        _elevatorControllerReception._elevator3working = _elevator2Working;
        _elevatorControllerReception.SetupElevatorStates();
        _elevatorControllerArrivalArea._elevator1working = _elevator0Working;
        _elevatorControllerArrivalArea._elevator2working = _elevator1Working;
        _elevatorControllerArrivalArea._elevator3working = _elevator2Working;
        _elevatorControllerArrivalArea.SetupElevatorStates();
        //TODO: Take those just-set bits into the local copy (not needed to work but would be nice to track errors)
        Debug.Log("[NetworkController] Random elevator states for reception are now set by localPlayer");
    }
    /// <summary>
    /// is called when network packets are received (only happens when there are more players except Master in the scene
    /// </summary>
    public override void OnDeserialization()
    {
        if (!_worldIsLoaded)
            return;
        LOCAL_OnDeserialization(); //do nothing else in here or shit will break!
    }
    /// <summary>
    /// Can be called by master (locally in Update) or by everyone else OnDeserialization (when SyncBool states change)
    /// </summary>
    private void LOCAL_OnDeserialization()
    {
        if (!_finishedLocalSetup)
        {
            if (Time.time < 1f) //no scene setup before at least 1 second has passed to ensure the update loop has already started
                return;
            Debug.Log("[NetworkController] Local setup was started");
            if (0L != (_syncData2 & (SyncBool_MaskLong2_Initialized)))
            {
                LOCAL_ReadConstSceneElevatorStates();
                _finishedLocalSetup = true;
                Debug.Log("[NetworkController] Local setup was finished");
            }
            else
            {
                return;
            }
        }
        else
        {
            LOCAL_CheckSyncData();
        }
    }
    /// <summary>
    /// Sending an elevator open/close event to the elevator controller of the right floor
    /// </summary>
    /// <param name="elevatorNumber"></param>
    private void LOCAL_OpenCloseElevator(int elevatorNumber, bool setOpen)
    {
        int floorNumber = GetSyncElevatorFloor(elevatorNumber);
        if (setOpen)
        {
            Debug.Log($"[NetworkController] LocalPlayer received to open elevator {elevatorNumber} on floor {floorNumber} while localPlayer is on floor {_localPlayerCurrentFloor}");
            bool thisElevatorWasAlreadyOpened = false;
            if (_playerIsInElevatorNumber == elevatorNumber)
            {
                thisElevatorWasAlreadyOpened = PrepareFloorForArrival(floorNumber, elevatorNumber);
            }
            if (!thisElevatorWasAlreadyOpened)
            {
                if (floorNumber == 0)
                {
                    //Passes elevatorNumber, (if going up), (if idle)
                    _elevatorControllerReception.OpenElevator(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
                }
                else if (floorNumber == _localPlayerCurrentFloor)
                {
                    //Passes elevatorNumber, (if going up), (if idle)
                    _elevatorControllerArrivalArea.OpenElevator(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
                }
            }
        }
        else
        {
            Debug.Log("[NetworkController] LocalPlayer received to close elevator " + elevatorNumber + " on floor " + floorNumber);
            if (floorNumber == 0)
            {
                _elevatorControllerReception.CloseElevator(elevatorNumber);
            }
            else if (floorNumber == _localPlayerCurrentFloor)
            {
                _elevatorControllerArrivalArea.CloseElevator(elevatorNumber);
            }
        }
    }
    /// <summary>
    /// Setting an elevator idle will set all calls handled on that floor and set both arrows active, if the elevator is open, else nothing happens
    /// </summary>
    /// <param name="elevatorNumber"></param>
    private void LOCAL_SetElevatorIdle(int elevatorNumber, bool isIdle)
    {
        //If elevator NOT open
        if (0L == (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))))
        {
            Debug.Log("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " IDLE=" + isIdle.ToString() + ", but it isn't open");
            return;
        }
        int floor = GetSyncElevatorFloor(elevatorNumber);
        Debug.Log("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " IDLE=" + isIdle.ToString() + " on floor " + floor);
        if (floor == 0)
        {
            //Passes elevatorNumber, isGoingUp, isIdle
            _elevatorControllerReception.SetElevatorDirectionDisplay(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), isIdle);
        }
        else if (floor == _localPlayerCurrentFloor)
        {
            //Passes elevatorNumber, isGoingUp, isIdle
            _elevatorControllerArrivalArea.SetElevatorDirectionDisplay(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), isIdle);
        }
    }
    /// <summary>
    /// Setting the new elevator direction will affect button calls and arrows if the elevator is open
    /// </summary>
    private void LOCAL_SetElevatorDirection(int elevatorNumber, bool goingUp)
    {
        //If elevator NOT open
        if (0L == (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))))
        {
            Debug.Log("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " GoingUp=" + goingUp.ToString() + ", but it isn't open");
            return;
        }
        int floor = GetSyncElevatorFloor(elevatorNumber);
        Debug.Log("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " GoingUp=" + goingUp.ToString() + " on floor " + floor);
        if (floor == 0)
        {
            //Passes elevatorNumber, goingUp, (if idle)
            _elevatorControllerReception.SetElevatorDirectionDisplay(elevatorNumber, goingUp, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
        }
        else if (floor == _localPlayerCurrentFloor)
        {
            //Passes elevatorNumber, goingUp, (if idle)
            _elevatorControllerArrivalArea.SetElevatorDirectionDisplay(elevatorNumber, goingUp, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
        }
    }
    //------------------------------------- external elevator calls from floor buttons ------------------------------------------------
    //Local copies of elevator state to check them against SyncBool states
    private bool[] _pendingCallDown_LOCAL_EXT = new bool[14];
    private int _pendingCallDown_COUNT_LOCAL_EXT = 0;
    private bool[] _pendingCallUp_LOCAL_EXT = new bool[14];
    private int _pendingCallUp_COUNT_LOCAL_EXT = 0;
    private float[] _pendingCallTimeUp_LOCAL_EXT = new float[14];
    private float[] _pendingCallTimeDown_LOCAL_EXT = new float[14];
    /// <summary>
    /// In every Update(): Checking if the external elevator was successfully called by master after we've called it, else we drop the request
    /// </summary>
    private void LOCAL_CheckIfElevatorExternalCallWasReceived()
    {
        if (_pendingCallUp_COUNT_LOCAL_EXT != 0)
        {
            //Debug.Log("There is " + _pendingCallUp_COUNT + " pending call up.");
            for (int floor = 0; floor <= 13; floor++)
            {
                //Check if there is a pending request
                if (_pendingCallUp_LOCAL_EXT[floor] && Time.time - _pendingCallTimeUp_LOCAL_EXT[floor] > 1.5f)
                {
                    _pendingCallUp_LOCAL_EXT[floor] = false;
                    _pendingCallUp_COUNT_LOCAL_EXT--;
                    //if NOT called up to floor X
                    if (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + floor))))
                    {
                        //TODO: link all elevator controllers here in Unity later
                        if (floor == 0)
                        {
                            Debug.Log("Dropped request, SetElevatorNotCalledUp() floor " + floor + " after " + (Time.time - _pendingCallTimeUp_LOCAL_EXT[floor]).ToString() + " seconds.");
                            _elevatorControllerReception.SetCallButtonState(buttonUp: true, isCalled: false);
                        }
                        else if (floor == _localPlayerCurrentFloor)
                        {
                            Debug.Log("Dropped request, SetElevatorNotCalledUp() floor " + floor + " after " + (Time.time - _pendingCallTimeUp_LOCAL_EXT[floor]).ToString() + " seconds.");
                            _elevatorControllerArrivalArea.SetCallButtonState(buttonUp: true, isCalled: false);
                        }
                    }
                }
            }
        }
        if (_pendingCallDown_COUNT_LOCAL_EXT != 0)
        {
            //Debug.Log("There is " + _pendingCallUp_COUNT + " pending call down.");
            for (int floor = 0; floor <= 13; floor++)
            {
                if (_pendingCallDown_LOCAL_EXT[floor] && Time.time - _pendingCallTimeDown_LOCAL_EXT[floor] > 1.5f)
                {
                    _pendingCallDown_LOCAL_EXT[floor] = false;
                    _pendingCallDown_COUNT_LOCAL_EXT--;
                    //if NOT called down to floor X
                    if (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + floor))))
                    {
                        //TODO: link all elevator controllers here in Unity later
                        if (floor == 0)
                        {
                            Debug.Log("Dropped request, SetElevatorNotCalledDown() floor " + floor + " after " + (Time.time - _pendingCallTimeDown_LOCAL_EXT[floor]).ToString() + " seconds.");
                            _elevatorControllerReception.SetCallButtonState(buttonUp: false, isCalled: false);
                        }
                        else if (floor == _localPlayerCurrentFloor)
                        {
                            Debug.Log("Dropped request, SetElevatorNotCalledDown() floor " + floor + " after " + (Time.time - _pendingCallTimeDown_LOCAL_EXT[floor]).ToString() + " seconds.");
                            _elevatorControllerArrivalArea.SetCallButtonState(buttonUp: false, isCalled: false);
                        }
                    }
                }
            }
        }
    }
    //------------------------------------- internal elevator calls from elevator buttons ------------------------------------------------
    //Local copies of elevator state to check them against SyncBool states
    private bool[] _pendingCallElevator0_LOCAL_INT = new bool[14];
    private int _pendingCallElevator0_COUNT_LOCAL_INT = 0;
    private float[] _pendingCallElevator0Time_LOCAL_INT = new float[14];
    //Local copies of elevator state to check them against SyncBool states
    private bool[] _pendingCallElevator1_LOCAL_INT = new bool[14];
    private int _pendingCallElevator1_COUNT_LOCAL_INT = 0;
    private float[] _pendingCallElevator1Time_LOCAL_INT = new float[14];
    //Local copies of elevator state to check them against SyncBool states
    private bool[] _pendingCallElevator2_LOCAL_INT = new bool[14];
    private int _pendingCallElevator2_COUNT_LOCAL_INT = 0;
    private float[] _pendingCallElevator2Time_LOCAL_INT = new float[14];
    /// <summary>
    /// In every Update(): Checking if the INTernal elevator was successfully called by master after we've called it, else we drop the request
    /// </summary>
    private void LOCAL_CheckIfElevatorInternalCallWasReceived()
    {
        if (_pendingCallElevator0_COUNT_LOCAL_INT != 0)
        {
            //Debug.Log("There is " + _pendingCallElevator0_COUNT + " pending call internally.");
            for (int floor = 0; floor <= 13; floor++)
            {
                if (_pendingCallElevator0_LOCAL_INT[floor] && Time.time - _pendingCallElevator0Time_LOCAL_INT[floor] > 1.5f)
                {
                    _pendingCallElevator0_LOCAL_INT[floor] = false;
                    _pendingCallElevator0_COUNT_LOCAL_INT--;
                    if (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_Elevator0CalledToFloor + floor))))
                    {
                        Debug.Log("Dropped request, SetElevatorInternalButtonState() button " + floor + " after " + (Time.time - _pendingCallElevator0Time_LOCAL_INT[floor]).ToString() + " seconds.");
                        LOCAL_SetElevatorInternalButtonState(0, floor + 4, called: false);
                    }
                }
            }
        }
        if (_pendingCallElevator1_COUNT_LOCAL_INT != 0)
        {
            //Debug.Log("There is " + _pendingCallElevator1_COUNT + " pending call internally.");
            for (int floor = 0; floor <= 13; floor++)
            {
                if (_pendingCallElevator1_LOCAL_INT[floor] && Time.time - _pendingCallElevator1Time_LOCAL_INT[floor] > 1.5f)
                {
                    _pendingCallElevator1_LOCAL_INT[floor] = false;
                    _pendingCallElevator1_COUNT_LOCAL_INT--;

                    //if NOT elevator1 called to floor X
                    if (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator1CalledToFloor + floor))))
                    {
                        Debug.Log("Dropped request, SetElevatorInternalButtonState() button " + floor + " after " + (Time.time - _pendingCallElevator1Time_LOCAL_INT[floor]).ToString() + " seconds.");
                        LOCAL_SetElevatorInternalButtonState(0, floor + 4, called: false);
                    }
                }
            }
        }
        if (_pendingCallElevator2_COUNT_LOCAL_INT != 0)
        {
            //Debug.Log("There is " + _pendingCallElevator2_COUNT + " pending call internally.");
            for (int floor = 0; floor <= 13; floor++)
            {
                if (_pendingCallElevator2_LOCAL_INT[floor] && Time.time - _pendingCallElevator2Time_LOCAL_INT[floor] > 1.5f)
                {
                    _pendingCallElevator2_LOCAL_INT[floor] = false;
                    _pendingCallElevator2_COUNT_LOCAL_INT--;
                    //if NOT elevator0 called to floor X
                    if (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator2CalledToFloor + floor))))
                    {
                        Debug.Log("Dropped request, SetElevatorInternalButtonState() button " + floor + " after " + (Time.time - _pendingCallElevator2Time_LOCAL_INT[floor]).ToString() + " seconds.");
                        LOCAL_SetElevatorInternalButtonState(0, floor + 4, called: false);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Storing the current state of some elevator functions
    /// </summary>
    private bool[] _elevatorLoliStairsAreEnabled = new bool[3];
    private bool[] _elevatorMirrorIsEnabled = new bool[3];
    /// <summary>
    /// To check if a player is inside a certain elevator
    /// </summary>
    public BoxCollider _playerInsideElevator0DetectorReception;
    public BoxCollider _playerInsideElevator1DetectorReception;
    public BoxCollider _playerInsideElevator2DetectorReception;
    public BoxCollider _playerInsideElevator0DetectorFloor;
    public BoxCollider _playerInsideElevator1DetectorFloor;
    public BoxCollider _playerInsideElevator2DetectorFloor;
    public Transform _elevator0PositionReception;
    public Transform _elevator1PositionReception;
    public Transform _elevator2PositionReception;
    public Transform _elevator0PositionFloor;
    public Transform _elevator1PositionFloor;
    public Transform _elevator2PositionFloor;
    /// <summary>
    /// variables for teleporting the player
    /// </summary>
    private int _teleportCounter = 0;
    private Vector3 _teleportTarget;
    /// <summary>
    /// Setting the floor up for the moment of arrival
    /// x:100 Z: 50, 100....
    /// </summary>
    private bool PrepareFloorForArrival(int floorNumber, int elevatorNumberWithPlayerInside)
    {
        Debug.Log($"[Prepare] Preparing floor {floorNumber} for arrival (player in elevator {elevatorNumberWithPlayerInside})");
        //first disable loli stairs when they are still enabled, since they block the door
        if (_elevatorLoliStairsAreEnabled[elevatorNumberWithPlayerInside])
        {
            _elevatorControllerReception.ToggleLoliStairs(elevatorNumberWithPlayerInside);
            _elevatorControllerArrivalArea.ToggleLoliStairs(elevatorNumberWithPlayerInside);
            _elevatorLoliStairsAreEnabled[elevatorNumberWithPlayerInside] = false;
            LOCAL_SetElevatorInternalButtonState(elevatorNumberWithPlayerInside, buttonNumber: 2, called: false);
        }
        //checking if the player is still inside that elevator
        BoxCollider inElevatorCollider;
        Vector3 _currentElevatorPosition;
        if (_playerIsInReceptionElevator)
        {
            switch (elevatorNumberWithPlayerInside)
            {
                case 0:
                    inElevatorCollider = _playerInsideElevator0DetectorReception;
                    _currentElevatorPosition = _elevator0PositionReception.position;
                    break;
                case 1:
                    inElevatorCollider = _playerInsideElevator1DetectorReception;
                    _currentElevatorPosition = _elevator1PositionReception.position;
                    break;
                case 2:
                    inElevatorCollider = _playerInsideElevator2DetectorReception;
                    _currentElevatorPosition = _elevator2PositionReception.position;
                    break;
                default:
                    Debug.Log("[Prepare] ERROR: Unknown elevator number in PrepareFloorForArrival()");
                    return false;
            }
        }
        else
        {
            switch (elevatorNumberWithPlayerInside)
            {
                case 0:
                    inElevatorCollider = _playerInsideElevator0DetectorFloor;
                    _currentElevatorPosition = _elevator0PositionFloor.position;
                    break;
                case 1:
                    inElevatorCollider = _playerInsideElevator1DetectorFloor;
                    _currentElevatorPosition = _elevator1PositionFloor.position;
                    break;
                case 2:
                    inElevatorCollider = _playerInsideElevator2DetectorFloor;
                    _currentElevatorPosition = _elevator2PositionFloor.position;
                    break;
                default:
                    Debug.Log("[Prepare] ERROR: Unknown elevator number in PrepareFloorForArrival()");
                    return false;
            }
        }
        //Check if player is really inside the current elevator, else return
        if (!inElevatorCollider.bounds.Contains(_localPlayer.GetBonePosition(HumanBodyBones.Head)))
        {
            _playerIsInElevatorNumber = -1;
            Debug.Log($"[Prepare] Player wasn't inside elevator {elevatorNumberWithPlayerInside} for real, returning.");
            return false;
        }
        //calculate the offset of player to current elevator
        Vector3 playerPosition = _localPlayer.GetPosition();
        Debug.Log($"[Prepare] playerPosition x:{playerPosition.x},y:{playerPosition.y},z:{playerPosition.z}");
        Vector3 teleportOffset = playerPosition - _currentElevatorPosition;
        Debug.Log($"[Prepare] teleportOffset x:{teleportOffset.x},y:{teleportOffset.y},z:{teleportOffset.z}");
        //get the floor height from where the player is currently standing on
        float arrivalFloorOldHeight = 0;
        if (!_playerIsInReceptionElevator)
            arrivalFloorOldHeight = _moveArrivalFloorHere.position.y;
        //move the whole floor to that new floor position
        float floorLevelHeight = (50 * floorNumber);
        if (floorNumber != 0)
            _moveArrivalFloorHere.position = new Vector3(-32.6f, floorLevelHeight, 10.55f);//new Vector3(150, 0, (50 * floorNumber) - 300);
        //read the new target position
        Vector3 _targetElevatorPosition;
        if (floorNumber == 0)
        {
            switch (elevatorNumberWithPlayerInside)
            {
                case 0:
                    _targetElevatorPosition = _elevator0PositionReception.position;
                    break;
                case 1:
                    _targetElevatorPosition = _elevator1PositionReception.position;
                    break;
                case 2:
                    _targetElevatorPosition = _elevator2PositionReception.position;
                    break;
                default:
                    Debug.Log("[Prepare] ERROR: Unknown elevator number in PrepareFloorForArrival()");
                    return false;
            }
        }
        else
        {
            switch (elevatorNumberWithPlayerInside)
            {
                case 0:
                    _targetElevatorPosition = _elevator0PositionFloor.position;
                    break;
                case 1:
                    _targetElevatorPosition = _elevator1PositionFloor.position;
                    break;
                case 2:
                    _targetElevatorPosition = _elevator2PositionFloor.position;
                    break;
                default:
                    Debug.Log("[Prepare] ERROR: Unknown elevator number in PrepareFloorForArrival()");
                    return false;
            }
        }
        //Vector3 teleportTarget;
        if (floorNumber == 0)
        {
            //floor is zero here
            _teleportTarget = new Vector3(playerPosition.x, playerPosition.y - arrivalFloorOldHeight, playerPosition.z);
        }
        else
        {
            _teleportTarget = new Vector3(playerPosition.x, playerPosition.y + floorLevelHeight - arrivalFloorOldHeight, playerPosition.z);
        }
        ////teleport player to the new target
        //teleportTarget = _targetElevatorPosition + teleportOffset;
        ////setting the spawn a bit higher to avoid falling down
        //teleportTarget.y = _targetElevatorPosition.y + 0.2f; // + 0.04f;
        Debug.Log($"[Prepare] new teleportTarget is x:{_teleportTarget.x},y:{_teleportTarget.y},z:{_teleportTarget.z}");
        //saving the floor the player is now on
        SetPlayerFloorLevel(floorNumber);
        //no need to setup reception currently, since this level is always synced
        if (floorNumber == 0)
        {
            Debug.Log("[Prepare] Floor is 0 so we'll open reception elevator and teleport there.");
            //open just the reception elevator where the player is inside
            _elevatorControllerReception.OpenElevator(elevatorNumberWithPlayerInside, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumberWithPlayerInside))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumberWithPlayerInside))));
            //teleport to reception
            //_localPlayer.TeleportTo(_teleportTarget, _localPlayer.GetRotation());
            _teleportCounter = 3;
            return true;
        }
        Debug.Log("[Prepare] Setting the Callbutton-States on the arrival floor");
        //setting the callbutton-states
        _elevatorControllerArrivalArea.SetCallButtonState(buttonUp: false, isCalled: (0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + floorNumber)))));
        _elevatorControllerArrivalArea.SetCallButtonState(buttonUp: true, isCalled: (0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + floorNumber)))));
        Debug.Log("[Prepare] Setting the Elevatordoor-States on the arrival floor");
        //setting the elevators-open/closed states
        for (int elevatorNumber = 0; elevatorNumber < 3; elevatorNumber++)
        {
            bool setOpen = (0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))));
            if (setOpen && GetSyncElevatorFloor(elevatorNumber) == floorNumber)
            {
                Debug.Log($"[Prepare] Opening elevator {elevatorNumber}");
                //Passes elevatorNumber, (if going up), (if idle)
                _elevatorControllerArrivalArea.OpenElevator(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
            }
            else
            {
                Debug.Log($"[Prepare] Closing elevator {elevatorNumber}");
                _elevatorControllerArrivalArea.CloseElevator(elevatorNumber);
            }
        }
        //finally, teleport the player
        if (floorNumber != 0)
        {
            Debug.Log($"[Prepare] Teleporting player to floor {floorNumber}");
            Debug.Log($"[Prepare] TeleportTarget is x:{_teleportTarget.x},y:{_teleportTarget.y},z:{_teleportTarget.z}");
            //_localPlayer.TeleportTo(_teleportTarget, _localPlayer.GetRotation());
            _teleportCounter = 3;
            //Debug.Log($"[Prepare] Teleported.");
        }
        else
        {
            Debug.Log($"[Prepare] Error: Reached last line in function despite floor being " + floorNumber);
        }
        Debug.Log($"[Prepare] Finished.");
        return true;
    }
    /// <summary>
    /// Checking if there is a pending teleport waiting for us. 
    /// Successful teleport should be followed by 2x teleport for extra safety
    /// </summary>
    private void CheckTeleportCounter()
    {
        if (_teleportCounter == 0)
            return;
        Debug.Log($"[Prepare] TeleportTarget is x:{_teleportTarget.x},y:{_teleportTarget.y},z:{_teleportTarget.z}");
        _localPlayer.TeleportTo(_teleportTarget, _localPlayer.GetRotation());
        Debug.Log($"[Prepare] Teleported.");
        _teleportCounter--;
        if (_teleportCounter == 2 && Vector3.Distance(_localPlayer.GetPosition(), _teleportTarget) > 5f)
        {
            _teleportCounter = 3; // reset counter since teleport failed
        }
    }
    /// <summary>
    /// Setting a new floor level where the player is currently on
    /// </summary>
    /// <param name="floorNumber"></param>
    private void SetPlayerFloorLevel(int floorNumber)
    {
        _localPlayerCurrentFloor = floorNumber;
        _elevatorControllerArrivalArea._floorLevel = floorNumber;
        _playerModCheck._currentFloorLocalPlayer = floorNumber;
        if (_userIsInVR)
        {
            _floorElevatorCallPanelForVR_1._floorNumber = floorNumber;
            _floorElevatorCallPanelForVR_2._floorNumber = floorNumber;
        }
        else
        {
            _floorElevatorCallPanelDesktop_1._floorNumber = floorNumber;
            _floorElevatorCallPanelDesktop_2._floorNumber = floorNumber;
        }
        //move the spawn as well
        if (floorNumber != 0)
        {
            _currentSpawn.position = _readonlyFloorSpawn.position;
            _currentSpawn.rotation = _readonlyFloorSpawn.rotation;
        }
        else
        {
            _currentSpawn.position = _readonlyReceptionSpawn.position;
            _currentSpawn.rotation = _readonlyReceptionSpawn.rotation;
        }
        //when the floor is reception-level, we don't change the number signs
        if (floorNumber == 0)
            return;
        //setup the floor to the networked state of that level
        _floorNumberSignRenderer.materials[2].SetInt("_Index", floorNumber - 1);
        _floorRoomNumberSignRenderer.materials[0].SetInt("_Index", floorNumber - 1);
    }
    #endregion LOCAL_FUNCTIONS
    //------------------------------------------------------------------------------------------------------------
    //------------------------------- API for elevator buttons ---------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region API_FUNCTIONS
    public void API_LocalPlayerPressedCallButton(int floorNumber, bool directionUp)
    {
        if (directionUp)
        {
            Debug.Log("[NetworkController] Elevator called to floor " + floorNumber + " by localPlayer (Up)");
            //if something with an array OR Elevator called up on floor X
            if (_pendingCallUp_LOCAL_EXT[floorNumber] || 0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + floorNumber))))
                return;
            _pendingCallUp_LOCAL_EXT[floorNumber] = true;
            _pendingCallTimeUp_LOCAL_EXT[floorNumber] = Time.time;
            _pendingCallUp_COUNT_LOCAL_EXT++;
            _elevatorRequester.RequestElevatorFloorButton(directionUp, floorNumber);
        }
        else
        {
            Debug.Log("[NetworkController] Elevator called to floor " + floorNumber + " by localPlayer (Down)");
            //if something with an array OR Elevator called down on floor X
            if (_pendingCallDown_LOCAL_EXT[floorNumber] || 0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + floorNumber))))
                return;
            _pendingCallDown_LOCAL_EXT[floorNumber] = true;
            _pendingCallTimeDown_LOCAL_EXT[floorNumber] = Time.time;
            _pendingCallDown_COUNT_LOCAL_EXT++;
            _elevatorRequester.RequestElevatorFloorButton(directionUp, floorNumber);
        }
    }
    /// <summary>
    /// When localPlayer pressed a button INSIDE the elevator
    /// </summary>
    public void API_LocalPlayerPressedElevatorButton(int elevatorNumber, int buttonNumber)
    {
        Debug.Log($"[NetworkController] LocalPlayer pressed button {buttonNumber} in elevator {elevatorNumber}");
        if (buttonNumber == 0) //OPEN
        {
            //If NOT elevator X open
            if (0L == (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))))
            {
                _elevatorRequester.RequestElevatorDoorStateChange(elevatorNumber, true);
            }
            return;
        }
        if (buttonNumber == 1) //CLOSE
        {
            //If elevator X open
            if (0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))))
            {
                _elevatorRequester.RequestElevatorDoorStateChange(elevatorNumber, false);
            }
            return;
        }
        if (buttonNumber == 2) // (m2) mirror-button
        {
            //toggle loli stairs locally in both ElevatorControllers
            _elevatorControllerReception.ToggleLoliStairs(elevatorNumber);
            _elevatorControllerArrivalArea.ToggleLoliStairs(elevatorNumber);
            _elevatorLoliStairsAreEnabled[elevatorNumber] = !_elevatorLoliStairsAreEnabled[elevatorNumber];
            LOCAL_SetElevatorInternalButtonState(elevatorNumber, buttonNumber, called: _elevatorLoliStairsAreEnabled[elevatorNumber]);
            return;
        }
        if (buttonNumber == 3) // (m1) loli-stairs button
        {
            //toggle mirror locally in both ElevatorControllers
            _elevatorControllerReception.ToggleMirror(elevatorNumber);
            _elevatorControllerArrivalArea.ToggleMirror(elevatorNumber);
            _elevatorMirrorIsEnabled[elevatorNumber] = !_elevatorMirrorIsEnabled[elevatorNumber];
            LOCAL_SetElevatorInternalButtonState(elevatorNumber, buttonNumber, called: _elevatorMirrorIsEnabled[elevatorNumber]);
            return;
        }
        if (buttonNumber == 18) // RING-button
        {
            //This button does absolutely nothing atm lol
            return;
        }
        //every other button is an internal floor request, button 4 is floor 0 etc.
        int floorNumber = buttonNumber - 4;
        switch (elevatorNumber)
        {
            case 0:
                _pendingCallElevator0_LOCAL_INT[floorNumber] = true; ;
                _pendingCallElevator0_COUNT_LOCAL_INT++;
                _pendingCallElevator0Time_LOCAL_INT[floorNumber] = Time.time;
                break;
            case 1:
                _pendingCallElevator1_LOCAL_INT[floorNumber] = true; ;
                _pendingCallElevator1_COUNT_LOCAL_INT++;
                _pendingCallElevator1Time_LOCAL_INT[floorNumber] = Time.time;
                break;
            case 2:
                _pendingCallElevator2_LOCAL_INT[floorNumber] = true; ;
                _pendingCallElevator2_COUNT_LOCAL_INT++;
                _pendingCallElevator2Time_LOCAL_INT[floorNumber] = Time.time;
                break;
        }
        _elevatorRequester.RequestElevatorInternalTarget(elevatorNumber, floorNumber);
    }
    #endregion API_FUNCTIONS
    //------------------------------------------------------------------------------------------------------------
    //--------------------------------- Network Call Receivers----------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region ELREQ_FUNCTIONS
    /// <summary>
    /// This function receives a client request (and is run by master-only)
    /// </summary>
    public void ELREQ_CallFromFloor(bool directionUp, int floor)
    {
        Debug.Log("[NetworkingController] Master received Elevator called to floor " + floor + " by localPlayer (DirectionUp: " + directionUp.ToString() + ")");
        //if direction up AND NOT elevator called up to floor x
        if (directionUp && (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + floor)))))
        {
            if (!MASTER_ElevatorAlreadyThereAndOpen(floor, true))
            {
                MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + floor, true);
                _calledToFloorToGoUp_MASTER[floor] = true;
                _calledToFloorToGoUp_MASTER_COUNT++;
            }
        }
        //if NOT direction up AND NOT elevator called down to floor x
        else if (!directionUp && (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + floor)))))
        {
            if (!MASTER_ElevatorAlreadyThereAndOpen(floor, false))
            {
                MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + floor, true);
                _calledToFloorToGoDown_MASTER[floor] = true;
                _calledToFloorToGoDown_MASTER_COUNT++;
            }
        }
    }
    /// <summary>
    /// Only Master receives this, it's called by ElevatorRequester
    /// </summary>
    public void ELREQ_CallToChangeDoorState(int elevatorNumber, bool open)
    {
        float test = Time.time - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber];
        Debug.Log("Master received CallToChangeDoorState for elevator " + elevatorNumber + " (Direction open: " + open.ToString() + ") Elevator driving:" + (0L != (_syncData2 & (1L << (SyncBool_AddressLong2_ElevatorXIsDriving + elevatorNumber)))));

        //if (open AND elevator X idle) OR (some timing stuff AND NOT driving)
        if (open && 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))) || (Time.time - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < 2.5f && (0L == (_syncData2 & (1L << (SyncBool_AddressLong2_ElevatorXIsDriving + elevatorNumber))))))
        {
            MASTER_HandleFloorDoorOpening(elevatorNumber, GetSyncElevatorFloor(elevatorNumber), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
        }
        //if NOT open AND elevator X idle AND some timing stuff
        else if (!open && 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))) && Time.time - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > 6f)
        {
            MASTER_SetSyncValue(SyncBool_Elevator0open + elevatorNumber, false);
            _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = Time.time;
        }
    }
    /// <summary>
    /// Only Master receives this, it's called by ElevatorRequester
    /// </summary>
    public void ELREQ_SetInternalTarget(int elevatorNumber, int floorNumber)
    {

        Debug.Log("[NetworkController] Master received client request to set target for elevator " + elevatorNumber + " to floor " + floorNumber);
        //if elevatorNumber0 AND NOT elevator0 called to floor X
        if (elevatorNumber == 0 && (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_Elevator0CalledToFloor + floorNumber)))))
        {
            Debug.Log("Internal target was now set.");
            MASTER_SetSyncValue(SyncBoolReq_Elevator0CalledToFloor_0 + floorNumber, true);
            _elevator0FloorTargets_MASTER[floorNumber] = true;
            _elevator0FloorTargets_MASTER_COUNT++;
            return;
        }
        //if elevatorNumber1 AND NOT elevator1 called to floor X
        else if (elevatorNumber == 1 && (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator1CalledToFloor + floorNumber)))))
        {
            Debug.Log("Internal target was now set.");
            MASTER_SetSyncValue(SyncBoolReq_Elevator1CalledToFloor_0 + floorNumber, true);
            _elevator1FloorTargets_MASTER[floorNumber] = true;
            _elevator1FloorTargets_MASTER_COUNT++;
            return;
        }
        //if elevatorNumber2 AND NOT elevator2 called to floor X
        else if (elevatorNumber == 2 && (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator2CalledToFloor + floorNumber)))))
        {
            Debug.Log("Internal target was now set.");
            MASTER_SetSyncValue(SyncBoolReq_Elevator2CalledToFloor_0 + floorNumber, true);
            _elevator2FloorTargets_MASTER[floorNumber] = true;
            _elevator2FloorTargets_MASTER_COUNT++;
            return;
        }
        Debug.Log("No target was set since the elevator is already called to that floor");
    }
    /// <summary>
    /// Checks if there is already an open elevator on this floor which is going in the target direction
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="directionUp"></param>
    /// <returns></returns>
    private bool MASTER_ElevatorAlreadyThereAndOpen(int floor, bool directionUp)
    {
        if (_elevator0Working && GetSyncElevatorFloor(0) == floor && 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0open)) && (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0goingUp)) || 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0idle))))
        {
            return true;
        }
        if (_elevator1Working && GetSyncElevatorFloor(1) == floor && 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1open)) && (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1goingUp)) || 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1idle))))
        {
            return true;
        }
        if (_elevator2Working && GetSyncElevatorFloor(2) == floor && 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2open)) && (0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2goingUp)) || 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2idle))))
        {
            return true;
        }
        return false;
    }
    #endregion ELREQ_FUNCTIONS
    //------------------------------------------------------------------------------------------------------------
    //----------------------------------SyncBool Interface -------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    #region SYNCBOOL_FUNCTIONS
    //------------------------------------------------------------------------------------------------------------
    //------------------------------------------ SyncBool lowlevel code ------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// This script sets and reads individual bits within two Longs as well as encoding three numbers (nibbles) within the most significant bytes
    /// 
    /// The first long maps as follows:-
    ///  - 63-60 variable_1 (4bits)
    ///  - 59-56 variable_2 (4bits)
    ///  - 55-52 variable_3 (4bits)
    ///  - 51-0 binary bools [51-0]
    ///
    /// The second long maps as follows:-
    ///  - 0-63 binary bools [52-115]
    /// 
    /// Script by NotFish
    /// </summary>     
    private const byte elevatorOneOffset = 60;
    private const byte elevatorTwoOffset = 56;
    private const byte elevatorThreeOffset = 52;
    private const byte long1BoolEndPosition = 51; //You will need to recalculate the bool array classes if you modify this
    private const long nibbleMask = 15; // ...0000 0000 1111        

    /// <summary>
    /// Modifies _syncData1 & _syncData2 on the bit level.
    /// Sets "value" to bit "position" of "input".
    /// </summary>       
    /// <param name="input">uint to modify</param>
    /// <param name="position">Bit position to modify (0-83)</param>
    /// <param name="value">Value to set the bit</param>        
    /// <returns>Returns the modified uint</returns>
    private void MASTER_SetSyncValue(int position, bool value)
    {
        Debug.Log($"SYNC DATA bool {position} set to {value.ToString()}");
        //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
        long locallong1 = _syncData1;
        long locallong2 = _syncData2;

        //Sanitise position
        if (position < 0 || position > 115)
        {
            //TODO: remove on live build
            Debug.Log("uintConverter - Position out of range");
            return;
        }

        //Fill long then uint            
        if (position <= long1BoolEndPosition)
        {
            //Store in the long
            if (value)
            {
                //We want to set the value to true
                //Set the bit using a bitwise OR. 
                locallong1 |= (1L << position);
            }
            else
            {
                //We want to set the value to false
                //Udon does not currently support bitwise NOT
                //Instead making sure bit is set to true and using a bitwise XOR.
                long mask = (1L << position);
                locallong1 |= mask;
                locallong1 ^= mask;
            }
        }
        else // position > length of long
        {
            //Store in the uint
            //Need to shift to to a valid address first!
            position -= long1BoolEndPosition + 1;

            if (value)
            {
                //We want to set the value to true
                //Set the bit using a bitwise OR. 
                locallong2 |= (1L << position);
            }
            else
            {
                //We want to set the value to false
                //Udon does not currently support bitwise NOT
                //Instead making sure bit is set to true and using a bitwise XOR.
                long mask = (1L << position);
                locallong2 |= mask;
                locallong2 ^= mask;
            }
        }

        //Let's not forget to actually write it back to syncData!
        _syncData1 = locallong1;
        _syncData2 = locallong2;
    }

    /// <summary>
    /// Reads the value of the bit at "position" of the combined syncData (_syncData1 & _syncData2).
    /// </summary>       
    /// <param name="input">uint to inspect</param>
    /// <param name="position">Bit position to read (0-83)</param>
    /// <returns>Boolean of specified bit position. Returns false on error.</returns>
    private bool GetSyncValue(int position)
    {
        //Sanitise position
        if (position < 0 || position > 115)
        {
            //TODO: remove on live build
            Debug.Log("uintConverter - Position out of range");
            return false;
        }

        //Read from long then uint            

        if (position <= long1BoolEndPosition)
        {
            //Read from the long
            //Inspect using a bitwise AND and a mask.
            //Branched in an IF statment for readability.
            if ((_syncData1 & (1L << position)) != 0L)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else // position < long length
        {
            //Read from the uint
            //Need to shift to to a valid address first!
            position -= long1BoolEndPosition + 1;

            //Inspect using a bitwise AND and a mask.
            //Branched in an IF statment for readability.
            if ((_syncData2 & (1L << position)) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Reads out all the booleans at once (preserving mapping compared to direct access)
    /// </summary>               
    /// <returns>Returns all the bools within both longs</returns>
    private bool[] GetBoolArray()
    {
        bool[] output = new bool[116];

        //Look a precomputed masks and no loops :)
        output[0] = (_syncData1 & 1L) != 0L;
        output[1] = (_syncData1 & 2L) != 0L;
        output[2] = (_syncData1 & 4L) != 0L;
        output[3] = (_syncData1 & 8L) != 0L;
        output[4] = (_syncData1 & 16L) != 0L;
        output[5] = (_syncData1 & 32L) != 0L;
        output[6] = (_syncData1 & 64L) != 0L;
        output[7] = (_syncData1 & 128L) != 0L;
        output[8] = (_syncData1 & 256L) != 0L;
        output[9] = (_syncData1 & 512L) != 0L;
        output[10] = (_syncData1 & 1024L) != 0L;
        output[11] = (_syncData1 & 2048L) != 0L;
        output[12] = (_syncData1 & 4096L) != 0L;
        output[13] = (_syncData1 & 8192L) != 0L;
        output[14] = (_syncData1 & 16384L) != 0L;
        output[15] = (_syncData1 & 32768L) != 0L;
        output[16] = (_syncData1 & 65536L) != 0L;
        output[17] = (_syncData1 & 131072L) != 0L;
        output[18] = (_syncData1 & 262144L) != 0L;
        output[19] = (_syncData1 & 524288L) != 0L;
        output[20] = (_syncData1 & 1048576L) != 0L;
        output[21] = (_syncData1 & 2097152L) != 0L;
        output[22] = (_syncData1 & 4194304L) != 0L;
        output[23] = (_syncData1 & 8388608L) != 0L;
        output[24] = (_syncData1 & 16777216L) != 0L;
        output[25] = (_syncData1 & 33554432L) != 0L;
        output[26] = (_syncData1 & 67108864L) != 0L;
        output[27] = (_syncData1 & 134217728L) != 0L;
        output[28] = (_syncData1 & 268435456L) != 0L;
        output[29] = (_syncData1 & 536870912L) != 0L;
        output[30] = (_syncData1 & 1073741824L) != 0L;
        output[31] = (_syncData1 & 2147483648L) != 0L;
        output[32] = (_syncData1 & 4294967296L) != 0L;
        output[33] = (_syncData1 & 8589934592L) != 0L;
        output[34] = (_syncData1 & 17179869184L) != 0L;
        output[35] = (_syncData1 & 34359738368L) != 0L;
        output[36] = (_syncData1 & 68719476736L) != 0L;
        output[37] = (_syncData1 & 137438953472L) != 0L;
        output[38] = (_syncData1 & 274877906944L) != 0L;
        output[39] = (_syncData1 & 549755813888L) != 0L;
        output[40] = (_syncData1 & 1099511627776L) != 0L;
        output[41] = (_syncData1 & 2199023255552L) != 0L;
        output[42] = (_syncData1 & 4398046511104L) != 0L;
        output[43] = (_syncData1 & 8796093022208L) != 0L;
        output[44] = (_syncData1 & 17592186044416L) != 0L;
        output[45] = (_syncData1 & 35184372088832L) != 0L;
        output[46] = (_syncData1 & 70368744177664L) != 0L;
        output[47] = (_syncData1 & 140737488355328L) != 0L;
        output[48] = (_syncData1 & 281474976710656L) != 0L;
        output[49] = (_syncData1 & 562949953421312L) != 0L;
        output[50] = (_syncData1 & 1125899906842624L) != 0L;
        output[51] = (_syncData1 & 2251799813685248L) != 0L;
        output[52] = (_syncData2 & 1L) != 0L;
        output[53] = (_syncData2 & 2L) != 0L;
        output[54] = (_syncData2 & 4L) != 0L;
        output[55] = (_syncData2 & 8L) != 0L;
        output[56] = (_syncData2 & 16L) != 0L;
        output[57] = (_syncData2 & 32L) != 0L;
        output[58] = (_syncData2 & 64L) != 0L;
        output[59] = (_syncData2 & 128L) != 0L;
        output[60] = (_syncData2 & 256L) != 0L;
        output[61] = (_syncData2 & 512L) != 0L;
        output[62] = (_syncData2 & 1024L) != 0L;
        output[63] = (_syncData2 & 2048L) != 0L;
        output[64] = (_syncData2 & 4096L) != 0L;
        output[65] = (_syncData2 & 8192L) != 0L;
        output[66] = (_syncData2 & 16384L) != 0L;
        output[67] = (_syncData2 & 32768L) != 0L;
        output[68] = (_syncData2 & 65536L) != 0L;
        output[69] = (_syncData2 & 131072L) != 0L;
        output[70] = (_syncData2 & 262144L) != 0L;
        output[71] = (_syncData2 & 524288L) != 0L;
        output[72] = (_syncData2 & 1048576L) != 0L;
        output[73] = (_syncData2 & 2097152L) != 0L;
        output[74] = (_syncData2 & 4194304L) != 0L;
        output[75] = (_syncData2 & 8388608L) != 0L;
        output[76] = (_syncData2 & 16777216L) != 0L;
        output[77] = (_syncData2 & 33554432L) != 0L;
        output[78] = (_syncData2 & 67108864L) != 0L;
        output[79] = (_syncData2 & 134217728L) != 0L;
        output[80] = (_syncData2 & 268435456L) != 0L;
        output[81] = (_syncData2 & 536870912L) != 0L;
        output[82] = (_syncData2 & 1073741824L) != 0L;
        output[83] = (_syncData2 & 2147483648L) != 0L;
        output[84] = (_syncData2 & 4294967296L) != 0L;
        output[85] = (_syncData2 & 8589934592L) != 0L;
        output[86] = (_syncData2 & 17179869184L) != 0L;
        output[87] = (_syncData2 & 34359738368L) != 0L;
        output[88] = (_syncData2 & 68719476736L) != 0L;
        output[89] = (_syncData2 & 137438953472L) != 0L;
        output[90] = (_syncData2 & 274877906944L) != 0L;
        output[91] = (_syncData2 & 549755813888L) != 0L;
        output[92] = (_syncData2 & 1099511627776L) != 0L;
        output[93] = (_syncData2 & 2199023255552L) != 0L;
        output[94] = (_syncData2 & 4398046511104L) != 0L;
        output[95] = (_syncData2 & 8796093022208L) != 0L;
        output[96] = (_syncData2 & 17592186044416L) != 0L;
        output[97] = (_syncData2 & 35184372088832L) != 0L;
        output[98] = (_syncData2 & 70368744177664L) != 0L;
        output[99] = (_syncData2 & 140737488355328L) != 0L;
        output[100] = (_syncData2 & 281474976710656L) != 0L;
        output[101] = (_syncData2 & 562949953421312L) != 0L;
        output[102] = (_syncData2 & 1125899906842624L) != 0L;
        output[103] = (_syncData2 & 2251799813685248L) != 0L;
        output[104] = (_syncData2 & 4503599627370496L) != 0L;
        output[105] = (_syncData2 & 9007199254740992L) != 0L;
        output[106] = (_syncData2 & 18014398509481984L) != 0L;
        output[107] = (_syncData2 & 36028797018963968L) != 0L;
        output[108] = (_syncData2 & 72057594037927936L) != 0L;
        output[109] = (_syncData2 & 144115188075855872L) != 0L;
        output[110] = (_syncData2 & 288230376151711744L) != 0L;
        output[111] = (_syncData2 & 576460752303423488L) != 0L;
        output[112] = (_syncData2 & 1152921504606846976L) != 0L;
        output[113] = (_syncData2 & 2305843009213693952L) != 0L;
        output[114] = (_syncData2 & 4611686018427387904L) != 0L;
        output[115] = (_syncData2 & -9223372036854775808L) != 0L;

        return output;
    }

    /// <summary>
    /// Reads out all the long1 booleans at once (preserving mapping compared to direct access)
    /// </summary>               
    /// <returns>Returns all the bools within the long1</returns>
    private bool[] GetBoolArrayLong1ONLY()
    {
        bool[] output = new bool[52];

        //Look a precomputed masks and no loops :)
        output[0] = (_syncData1 & 1L) != 0L;
        output[1] = (_syncData1 & 2L) != 0L;
        output[2] = (_syncData1 & 4L) != 0L;
        output[3] = (_syncData1 & 8L) != 0L;
        output[4] = (_syncData1 & 16L) != 0L;
        output[5] = (_syncData1 & 32L) != 0L;
        output[6] = (_syncData1 & 64L) != 0L;
        output[7] = (_syncData1 & 128L) != 0L;
        output[8] = (_syncData1 & 256L) != 0L;
        output[9] = (_syncData1 & 512L) != 0L;
        output[10] = (_syncData1 & 1024L) != 0L;
        output[11] = (_syncData1 & 2048L) != 0L;
        output[12] = (_syncData1 & 4096L) != 0L;
        output[13] = (_syncData1 & 8192L) != 0L;
        output[14] = (_syncData1 & 16384L) != 0L;
        output[15] = (_syncData1 & 32768L) != 0L;
        output[16] = (_syncData1 & 65536L) != 0L;
        output[17] = (_syncData1 & 131072L) != 0L;
        output[18] = (_syncData1 & 262144L) != 0L;
        output[19] = (_syncData1 & 524288L) != 0L;
        output[20] = (_syncData1 & 1048576L) != 0L;
        output[21] = (_syncData1 & 2097152L) != 0L;
        output[22] = (_syncData1 & 4194304L) != 0L;
        output[23] = (_syncData1 & 8388608L) != 0L;
        output[24] = (_syncData1 & 16777216L) != 0L;
        output[25] = (_syncData1 & 33554432L) != 0L;
        output[26] = (_syncData1 & 67108864L) != 0L;
        output[27] = (_syncData1 & 134217728L) != 0L;
        output[28] = (_syncData1 & 268435456L) != 0L;
        output[29] = (_syncData1 & 536870912L) != 0L;
        output[30] = (_syncData1 & 1073741824L) != 0L;
        output[31] = (_syncData1 & 2147483648L) != 0L;
        output[32] = (_syncData1 & 4294967296L) != 0L;
        output[33] = (_syncData1 & 8589934592L) != 0L;
        output[34] = (_syncData1 & 17179869184L) != 0L;
        output[35] = (_syncData1 & 34359738368L) != 0L;
        output[36] = (_syncData1 & 68719476736L) != 0L;
        output[37] = (_syncData1 & 137438953472L) != 0L;
        output[38] = (_syncData1 & 274877906944L) != 0L;
        output[39] = (_syncData1 & 549755813888L) != 0L;
        output[40] = (_syncData1 & 1099511627776L) != 0L;
        output[41] = (_syncData1 & 2199023255552L) != 0L;
        output[42] = (_syncData1 & 4398046511104L) != 0L;
        output[43] = (_syncData1 & 8796093022208L) != 0L;
        output[44] = (_syncData1 & 17592186044416L) != 0L;
        output[45] = (_syncData1 & 35184372088832L) != 0L;
        output[46] = (_syncData1 & 70368744177664L) != 0L;
        output[47] = (_syncData1 & 140737488355328L) != 0L;
        output[48] = (_syncData1 & 281474976710656L) != 0L;
        output[49] = (_syncData1 & 562949953421312L) != 0L;
        output[50] = (_syncData1 & 1125899906842624L) != 0L;
        output[51] = (_syncData1 & 2251799813685248L) != 0L;

        return output;
    }

    /// <summary>
    /// Reads out all the long2 booleans at once (preserving mapping compared to direct access)
    /// </summary>               
    /// <returns>Returns all the bools within long2</returns>
    private bool[] GetBoolArrayLong2ONLY()
    {
        bool[] output = new bool[116];

        //Look a precomputed masks and no loops :)
        output[52] = (_syncData2 & 1L) != 0L;
        output[53] = (_syncData2 & 2L) != 0L;
        output[54] = (_syncData2 & 4L) != 0L;
        output[55] = (_syncData2 & 8L) != 0L;
        output[56] = (_syncData2 & 16L) != 0L;
        output[57] = (_syncData2 & 32L) != 0L;
        output[58] = (_syncData2 & 64L) != 0L;
        output[59] = (_syncData2 & 128L) != 0L;
        output[60] = (_syncData2 & 256L) != 0L;
        output[61] = (_syncData2 & 512L) != 0L;
        output[62] = (_syncData2 & 1024L) != 0L;
        output[63] = (_syncData2 & 2048L) != 0L;
        output[64] = (_syncData2 & 4096L) != 0L;
        output[65] = (_syncData2 & 8192L) != 0L;
        output[66] = (_syncData2 & 16384L) != 0L;
        output[67] = (_syncData2 & 32768L) != 0L;
        output[68] = (_syncData2 & 65536L) != 0L;
        output[69] = (_syncData2 & 131072L) != 0L;
        output[70] = (_syncData2 & 262144L) != 0L;
        output[71] = (_syncData2 & 524288L) != 0L;
        output[72] = (_syncData2 & 1048576L) != 0L;
        output[73] = (_syncData2 & 2097152L) != 0L;
        output[74] = (_syncData2 & 4194304L) != 0L;
        output[75] = (_syncData2 & 8388608L) != 0L;
        output[76] = (_syncData2 & 16777216L) != 0L;
        output[77] = (_syncData2 & 33554432L) != 0L;
        output[78] = (_syncData2 & 67108864L) != 0L;
        output[79] = (_syncData2 & 134217728L) != 0L;
        output[80] = (_syncData2 & 268435456L) != 0L;
        output[81] = (_syncData2 & 536870912L) != 0L;
        output[82] = (_syncData2 & 1073741824L) != 0L;
        output[83] = (_syncData2 & 2147483648L) != 0L;
        output[84] = (_syncData2 & 4294967296L) != 0L;
        output[85] = (_syncData2 & 8589934592L) != 0L;
        output[86] = (_syncData2 & 17179869184L) != 0L;
        output[87] = (_syncData2 & 34359738368L) != 0L;
        output[88] = (_syncData2 & 68719476736L) != 0L;
        output[89] = (_syncData2 & 137438953472L) != 0L;
        output[90] = (_syncData2 & 274877906944L) != 0L;
        output[91] = (_syncData2 & 549755813888L) != 0L;
        output[92] = (_syncData2 & 1099511627776L) != 0L;
        output[93] = (_syncData2 & 2199023255552L) != 0L;
        output[94] = (_syncData2 & 4398046511104L) != 0L;
        output[95] = (_syncData2 & 8796093022208L) != 0L;
        output[96] = (_syncData2 & 17592186044416L) != 0L;
        output[97] = (_syncData2 & 35184372088832L) != 0L;
        output[98] = (_syncData2 & 70368744177664L) != 0L;
        output[99] = (_syncData2 & 140737488355328L) != 0L;
        output[100] = (_syncData2 & 281474976710656L) != 0L;
        output[101] = (_syncData2 & 562949953421312L) != 0L;
        output[102] = (_syncData2 & 1125899906842624L) != 0L;
        output[103] = (_syncData2 & 2251799813685248L) != 0L;
        output[104] = (_syncData2 & 4503599627370496L) != 0L;
        output[105] = (_syncData2 & 9007199254740992L) != 0L;
        output[106] = (_syncData2 & 18014398509481984L) != 0L;
        output[107] = (_syncData2 & 36028797018963968L) != 0L;
        output[108] = (_syncData2 & 72057594037927936L) != 0L;
        output[109] = (_syncData2 & 144115188075855872L) != 0L;
        output[110] = (_syncData2 & 288230376151711744L) != 0L;
        output[111] = (_syncData2 & 576460752303423488L) != 0L;
        output[112] = (_syncData2 & 1152921504606846976L) != 0L;
        output[113] = (_syncData2 & 2305843009213693952L) != 0L;
        output[114] = (_syncData2 & 4611686018427387904L) != 0L;
        output[115] = (_syncData2 & -9223372036854775808L) != 0L;

        return output;
    }

    /// <summary>
    /// Decodes and returns the floor number of the long
    /// </summary>           
    /// <param name="elevatorNumber">Number of the elevator 1-3</param>        
    /// <param name="floorNumber">value to set to the elevator variable</param>
    /// <returns>The updated uint</returns>
    private void MASTER_SetSyncElevatorFloor(int elevatorNumber, int floorNumber)
    {
        Debug.Log($"SYNC DATA elevator {elevatorNumber} floor setting to {floorNumber}");
        //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
        long locallong = _syncData1;
        //Debug.Log($"SYNC DATA_1 was {locallong}");
        //Sanitise the size of elevatorNumber
        if (elevatorNumber < 0 || elevatorNumber > 2)
        {
            //TODO: remove on live build
            Debug.Log($"uintConverter - 404 Elevator {elevatorNumber} does not exist");
            return;
        }

        //sanitise floorNumber
        if (floorNumber < 0 || floorNumber > 15)
        {
            //TODO: remove on live build
            Debug.Log($"uintConverter - Elevator  {elevatorNumber} number invalid");
            return;
        }
        long modifiedFloorNumber = (long)floorNumber;
        //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
        //Setting the variables using the following process        
        //1- Shift the data to the right bit section of the uint
        //2- Create mask to zero the right bits on the orginal uint
        //3- Zero the relevant bits on the original uint.
        //   Udon does not support bitwise NOT, so a clumsy mix of OR, XOR to zero it out...
        //4- Bitwise OR the two variables together to overlay the two, I guess you could also add them.                   
        if (elevatorNumber == 0)
        {
            modifiedFloorNumber = (modifiedFloorNumber << elevatorOneOffset);
            const long mask = (nibbleMask << elevatorOneOffset);
            locallong |= mask;
            locallong ^= mask;
            locallong |= modifiedFloorNumber;
        }
        else if (elevatorNumber == 1)
        {
            modifiedFloorNumber = (modifiedFloorNumber << elevatorTwoOffset);
            const long mask = (nibbleMask << elevatorTwoOffset);
            locallong |= mask;
            locallong ^= mask;
            locallong |= modifiedFloorNumber;
        }
        else  //Elevator 3
        {
            modifiedFloorNumber = (modifiedFloorNumber << elevatorThreeOffset);
            const long mask = (nibbleMask << elevatorThreeOffset);
            locallong |= mask;
            locallong ^= mask;
            locallong |= modifiedFloorNumber;
        }
        _syncData1 = locallong;
        //Debug.Log($"SYNC DATA_1 is now {locallong}");
    }

    /// <summary>
    /// Decodes and returns the floor number of the long
    /// </summary>              
    /// <param name="elevatorNumber">Number of the elevator 1-3</param>        
    /// <returns>Returns the floorNumber from the uint</returns>
    private int GetSyncElevatorFloor(int elevatorNumber)
    {
        //Debug.Log($"SYNC DATA_1 is now {_syncData1}");
        //Sanitise the size of elevatorNumber
        if (elevatorNumber < 0 || elevatorNumber > 2)
        {
            //TODO: remove on live build if needed
            Debug.Log($"uintConverter - 404 Elevator  {elevatorNumber} does not exist");
            return 0;
        }

        //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
        if (elevatorNumber == 0)
        {
            //Shift data
            long shiftedData = (_syncData1 >> elevatorOneOffset);
            //Mask away the higher bits
            shiftedData &= nibbleMask;
            return (int)(shiftedData & nibbleMask);
        }
        else if (elevatorNumber == 1)
        {
            //Shift data
            long shiftedData = (_syncData1 >> elevatorTwoOffset);
            //Mask away the higher bits
            shiftedData &= nibbleMask;
            return (int)(shiftedData & nibbleMask);
        }
        else  //Elevator 3
        {
            //Shift data
            long shiftedData = (_syncData1 >> elevatorThreeOffset);
            //Mask away the higher bits                
            return (int)(shiftedData & nibbleMask);
        }
    }
    #endregion SYNCBOOL_FUNCTIONS

    public long CastAwayAnyHopeToLong(ulong input)
    {
        long output = 0L;

        for (int i = 0; i < 64; i++)
        {
            //if long has bit
            if ((input & (1UL << i)) != 0L)
            {
                //set long bit to true
                output |= (1L << i);
            }
        }
        return output;
    }

    public ulong CastAwayAnyHopeToUlong(long input)
    {
        ulong output = 0L;

        for (int i = 0; i < 64; i++)
        {
            //if long has bit
            if ((input & (1L << i)) != 0L)
            {
                //set long bit to true
                output |= (1UL << i);
            }
        }
        return output;
    }
}

















////------------------------------------------------------------------------------------------------------------
////------------------------------------------ SyncBool lowlevel code ------------------------------------------
////------------------------------------------------------------------------------------------------------------

///// <summary>
///// This script sets and reads individual bits within a uint as well as encoding three numbers (nibbles) within the most significant bytes
///// 
///// The ulong maps as follows:-
/////  - 63-60 variable_1 (4bits)
/////  - 59-56 variable_2 (4bits)
/////  - 55-52 variable_2 (4bits)
/////  - 51-0 binary bools [0-51]
/////
///// The uint maps as follows:-
/////  - 31-0 binary bools [52-83(?)]
///// 
///// Script by NotFish
///// </summary>''
//private const byte elevatorOneOffset = 60;
//private const byte elevatorTwoOffset = 56;
//private const byte elevatorThreeOffset = 52;
//private const byte ulongBoolStartPosition = 51;
//private const uint nibbleMask = 15; // ...0000 0000 1111 
//private const int elevatorFloorNumberOffset = -2; //Keks floor hack offset

///// <summary>
///// Modifies a _syncData1 & _syncData2 on the bit level.
///// Sets "value" to bit "position" of "input".
///// </summary>       
///// <param name="input">uint to modify</param>
///// <param name="position">Bit position to modify (0-83)</param>
///// <param name="value">Value to set the bit</param>        
///// <returns>Returns the modified uint</returns>
//private void MASTER_SetSyncValue(int position, bool value)
//{
//    Debug.Log($"SYNC DATA bool {position} set to {value.ToString()}");
//    //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
//    ulong localUlong = _syncData1;
//    uint localUint = _syncData2;

//    //Sanitise position
//    if (position < 0 || position > 83)
//    {
//        Debug.LogError("uintConverter - Position out of range");
//        return;
//    }

//    //Index the positions back to front (negative index to be stored in the uint)
//    position = ulongBoolStartPosition - position;

//    if (position > 0)
//    {
//        //Store in the ulong
//        if (value)
//        {
//            //We want to set the value to true
//            //Set the bit using a bitwise OR. 
//            localUlong |= ((ulong)(1) << position);
//        }
//        else
//        {
//            //We want to set the value to false
//            //Udon does not currently support bitwise NOT
//            //Instead making sure bit is set to true and using a bitwise XOR.
//            ulong mask = ((ulong)(1) << position);
//            localUlong |= mask;
//            localUlong ^= mask;
//        }
//    }
//    else // position < 0
//    {
//        //Store in the uint
//        //Need to shift to to a valid address first!
//        position += 32;

//        if (value)
//        {
//            //We want to set the value to true
//            //Set the bit using a bitwise OR. 
//            localUint |= ((uint)(1) << position);
//        }
//        else
//        {
//            //We want to set the value to false
//            //Udon does not currently support bitwise NOT
//            //Instead making sure bit is set to true and using a bitwise XOR.
//            uint mask = ((uint)(1) << position);
//            localUint |= mask;
//            localUint ^= mask;
//        }
//    }

//    //Let's not forget to actually write it back to syncData!
//    _syncData1 = localUlong;
//    _syncData2 = localUint;
//}

///// <summary>
///// Reads the value of the bit at "position" of the combined syncData (_syncData1 & _syncData2).
///// </summary>       
///// <param name="input">uint to inspect</param>
///// <param name="position">Bit position to read (0-83)</param>
///// <returns>Boolean of specified bit position. Returns false on error.</returns>
//private bool GetSyncValue(int position)
//{
//    //Sanitise position
//    if (position < 0 || position > 83)
//    {
//        Debug.LogError("uintConverter - Position out of range");
//        return false;
//    }

//    //Index the positions back to front (negative index to be stored in the uint)
//    position = ulongBoolStartPosition - position;

//    if (position > 0)
//    {
//        //Read from the ulong
//        //Inspect using a bitwise AND and a mask.
//        //Branched in an IF statment for readability.
//        if ((_syncData1 & ((ulong)(1) << position)) != 0ul)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//    else // position < 0
//    {
//        //Read from the uint
//        //Need to shift to to a valid address first!
//        position += 32;

//        //Inspect using a bitwise AND and a mask.
//        //Branched in an IF statment for readability.
//        if ((_syncData2 & ((uint)(1) << position)) != 0ul)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//}

///// <summary>
///// Decodes and returns the floor number of the ulong
///// </summary>           
///// <param name="elevatorNumber">Number of the elevator 1-3</param>        
///// <param name="floorNumber">value to set to the elevator variable</param>
///// <returns>The updated uint</returns>
//private void MASTER_SetSyncElevatorFloor(int elevatorNumber, int floorNumber)
//{
//    Debug.Log($"SYNC DATA elevator {elevatorNumber} set to {floorNumber}");
//    //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
//    ulong localUlong = _syncData1;

//    //Sanitise the size of elevatorNumber
//    if (elevatorNumber < 0 || elevatorNumber > 2)
//    {
//        Debug.LogError($"uintConverter - 404 Elevator {elevatorNumber} does not exist");
//        return;
//    }

//    //floorNumber needs to be betweeen 0-15, so quick hack for negative floors and sanitise
//    int modifiedFloorNumberTempUint = (floorNumber - elevatorFloorNumberOffset);
//    if (modifiedFloorNumberTempUint < 0 || modifiedFloorNumberTempUint > 15)
//    {
//        Debug.LogError($"uintConverter - Elevator  {elevatorNumber} number invalid");
//        return;
//    }
//    ulong modifiedFloorNumber = (ulong)modifiedFloorNumberTempUint;
//    //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
//    //Setting the variables using the following process        
//    //1- Shift the data to the right bit section of the uint
//    //2- Create mask to zero the right bits on the orginal uint
//    //3- Zero the relevant bits on the original uint.
//    //   Udon does not support bitwise NOT, so a clumsy mix of OR, XOR to zero it out...
//    //4- Bitwise OR the two variables together to overlay the two, I guess you could also add them.                   
//    if (elevatorNumber == 0)
//    {
//        modifiedFloorNumber = (modifiedFloorNumber << elevatorOneOffset);
//        ulong mask = (nibbleMask << elevatorOneOffset);
//        _syncData1 |= mask;
//        _syncData1 ^= mask;
//        _syncData1 |= modifiedFloorNumber;
//    }
//    else if (elevatorNumber == 1)
//    {
//        modifiedFloorNumber = (modifiedFloorNumber << elevatorTwoOffset);
//        ulong mask = (nibbleMask << elevatorTwoOffset);
//        _syncData1 |= mask;
//        _syncData1 ^= mask;
//        _syncData1 |= modifiedFloorNumber;
//    }
//    else  //Elevator 3
//    {
//        modifiedFloorNumber = (modifiedFloorNumber << elevatorThreeOffset);
//        ulong mask = (nibbleMask << elevatorThreeOffset);
//        _syncData1 |= mask;
//        _syncData1 ^= mask;
//        _syncData1 |= modifiedFloorNumber;
//    }
//}

///// <summary>
///// Decodes and returns the floor number of the ulong
///// </summary>              
///// <param name="elevatorNumber">Number of the elevator 1-3</param>        
///// <returns>Returns the floorNumber from the uint</returns>
//private int GetSyncElevatorFloor(int elevatorNumber)
//{
//    //Sanitise the size of elevatorNumber
//    if (elevatorNumber < 0 || elevatorNumber > 2)
//    {
//        Debug.LogError($"uintConverter - 404 Elevator  {elevatorNumber} does not exist");
//        return 0;
//    }

//    //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
//    if (elevatorNumber == 0)
//    {
//        //No need to mask the higher bits, so a straight return.
//        int floorNumber = (int)(_syncData1 >> elevatorOneOffset);
//        floorNumber += elevatorFloorNumberOffset;
//        return floorNumber;
//    }
//    else if (elevatorNumber == 1)
//    {
//        //Shift data
//        ulong shiftedData = (_syncData1 >> elevatorTwoOffset);
//        //Mask away the higher bits
//        shiftedData &= nibbleMask;
//        int floorNumber = (int)(shiftedData) + elevatorFloorNumberOffset;
//        return floorNumber;
//    }
//    else  //Elevator 3
//    {
//        //Shift data
//        ulong shiftedData = (_syncData1 >> elevatorThreeOffset);
//        //Mask away the higher bits
//        shiftedData &= nibbleMask;
//        int floorNumber = (int)(shiftedData) + elevatorFloorNumberOffset;
//        return floorNumber;
//    }
//}





///// <summary>
///// Everyone can read bools from the synced states
///// </summary>
///// <returns></returns>
//private bool GetSyncValue(int position)
//{
//    //return BoolUIntConverter_GetValue(_syncData, position);
//}
///// <summary>
///// Only the master can call this function to change a bool state
///// </summary>
//private void MASTER_SetSyncValue(int position, bool value)
//{
//    Debug.Log("SYNC DATA position " + position + " set to " + value.ToString());
//    //_syncData = BoolUIntConverter_SetValue(_syncData, position, value);
//}
///// <summary>
///// Everyone can read bools from the synced states
///// </summary>
///// <returns></returns>
//private bool GetSyncValue(int position)
//{
//    if (position <= 63)
//    {
//        return BoolInt64Converter_GetValue(_syncData, position);
//    }
//    else //position must start at 13 when the value is at or above 64
//    {
//        return BoolUIntConverter_GetValue(_syncData, position - 51);
//    }
//}
///// <summary>
///// Only the master can call this function to change a bool state
///// </summary>
//private void MASTER_SetSyncValueReq(int position, bool value)
//{
//    Debug.Log("SYNC DATA REQ position " + position + " set to " + value.ToString());
//    if (position <= 63)
//    {
//        _syncDataReq = BoolInt46Converter_SetValue(_syncDataReq, position, value);
//    }
//    else //position must start at 13 when the value is at or above 64
//    {
//        _syncData = BoolUIntConverter_SetValue(_syncData, position - 51, value);
//    }
//}
///// <summary>
///// Allows master to set an elevator to a new floor
///// </summary>
//private void MASTER_SetSyncElevatorFloor(int elevatorNumber, int floorNumber)
//{
//    //_syncData1 = BoolUIntConverter_SetElevatorFloor(_syncData1, elevatorNumber, floorNumber);
//}
///// <summary>
///// returns the synced floor number on which an elevator currently is
///// </summary>
//private int GetSyncElevatorFloor(int elevatorNumber)
//{
//    //return BoolUIntConverter_GetElevatorFloor(_syncData1, elevatorNumber);
//}



//---------------------------------------------------- Bool To Int until 06.06.2020 ---------------------------------------------------------------------
///// <summary>
///// This script sets and reads individual bits within a uint as well as encoding/decoding three numbers (nibble sized) within the most significant bits
///// 
///// The uint maps as follows:-
/////  - 31-28 variable_1 (4bits)
/////  - 27-24 variable_2 (4bits)
/////  - 23-20 variable_3 (4bits)
/////  - 19-0 binary bools
/////  
///// Script by NotFish
///// </summary>

//private const byte elevatorOneOffset = 28;
//private const byte elevatorTwoOffset = 24;
//private const byte elevatorThreeOffset = 20;
//private const uint nibbleMask = 15; // ...0000 0000 1111 
//private const int elevatorFloorNumberOffset = -2; //Keks floor hack offset

///// <summary>
///// Modifies a uint on the bit level.
///// Sets "value" to bit "position" of "input".
///// </summary>       
///// <param name="input">uint to modify</param>
///// <param name="position">Bit position to modify (0-31)</param>
///// <param name="value">Value to set the bit</param>        
///// <returns>Returns the modified uint</returns>
//private uint BoolUIntConverter_SetValue(uint input, int position, bool value)
//{
//    //Sanitise position
//    if (position < 0 || position > 19)
//    {
//        Debug.LogError("[NetworkController] uintConverter - Position out of range");
//        return input;
//    }
//    if (value)
//    {
//        //We want to set the value to true
//        //Set the bit using a bitwise OR. 
//        input |= ((uint)(1) << position);
//    }
//    else
//    {
//        //We want to set the value to false
//        //Udon does not currently support bitwise NOT
//        //Instead making sure bit is set to true and using a bitwise XOR.
//        uint mask = ((uint)(1) << position);
//        input |= mask;
//        input ^= mask;
//    }

//    return input;
//}

///// <summary>
///// Reads the value of the bit at "position" of "input".
///// </summary>       
///// <param name="input">uint to inspect</param>
///// <param name="position">Bit position to read (0-31)</param>
///// <returns>Boolean of specified bit position. Returns false on error.</returns>
//private bool BoolUIntConverter_GetValue(uint input, int position)
//{
//    //Sanitise position
//    if (position < 0 || position > 19)
//    {
//        Debug.LogError("[NetworkController] uintConverter - Position out of range");
//        return false;
//    }

//    //Inspect using a bitwise AND and a mask.
//    //Branched in an IF statment for readability.
//    if ((input & ((uint)(1) << position)) != 0)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}

///// <summary>
///// Returns a bool array of bits within a uint. Use GetValue whenever possible.
///// </summary>       
///// <param name="input">uint to convert</param>    
///// <returns>A bool array representing the uint input</returns>
//private bool[] BoolUIntConverter_GetValues(uint input)
//{
//    bool[] boolArray = new bool[19];

//    //Iterate through all the bits and populate the array
//    for (byte i = 0; i < 19; i++)
//    {
//        boolArray[i] = BoolUIntConverter_GetValue(input, i);
//    }

//    return boolArray;
//}

///// <summary>
///// Takes a bool[] and overlays it on a uint. Use SetValue whenever possible.
///// </summary>       
///// <param name="input">uint to modify</param>    
///// <param name="values">A bool array up to length 32</param>    
///// <returns>A bool array representing the uint input</returns>
//private uint BoolUIntConverter_SetValues(uint input, bool[] values)
//{
//    //Sanitise the size of values
//    if (values == null || values.Length >= 19)
//    {
//        Debug.LogError("[NetworkController] uintConverter - Array null or too long");
//        return input;
//    }

//    //Iterate through all the bools and set the values
//    for (byte i = 0; i < values.Length; i++)
//    {
//        input = BoolUIntConverter_SetValue(input, i, values[i]);
//    }

//    return input;
//}

///// <summary>
///// Decodes and returns the floor number of the uint
///// </summary>       
///// <param name="input">uint to modify</param>    
///// <param name="elevatorNumber">Number of the elevator 1-3</param>        
///// <param name="floorNumber">value to set to the elevator variable</param>
///// <returns>The updated uint</returns>
//private uint BoolUIntConverter_SetElevatorFloor(uint input, int elevatorNumber, int floorNumber)
//{
//    //Sanitise the size of elevatorNumber
//    if (elevatorNumber < 0 || elevatorNumber > 2)
//    {
//        Debug.LogError("[NetworkController] uintConverter - 404 Elevator does not exist");
//        return input;
//    }

//    //floorNumber needs to be betweeen 0-15, so quick hack for negative floors and sanitise
//    uint modifiedFloorNumber = (uint)(floorNumber - elevatorFloorNumberOffset);
//    if (modifiedFloorNumber < 0 || modifiedFloorNumber > 15)
//    {
//        Debug.LogError("[NetworkController] uintConverter - Elevator number invalid");
//        return input;
//    }

//    //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
//    //Setting the variables using the following process        
//    //1- Shift the data to the right section of the uint
//    //2- Create mask to zero the variable's bits on the original uint
//    //3- Zero the relevant bits on the original uint.
//    //   Udon does not support bitwise NOT, so a clumsy mix of OR, XOR to zero it out...
//    //4- Bitwise OR the two variables together to overlay the two, I guess you could also add them.                   
//    if (elevatorNumber == 0)
//    {
//        modifiedFloorNumber = (modifiedFloorNumber << elevatorOneOffset);
//        uint mask = (nibbleMask << elevatorOneOffset);
//        input |= mask;
//        input ^= mask;
//        input |= modifiedFloorNumber;
//        return input;
//    }
//    else if (elevatorNumber == 1)
//    {
//        modifiedFloorNumber = (modifiedFloorNumber << elevatorTwoOffset);
//        uint mask = (nibbleMask << elevatorTwoOffset);
//        input |= mask;
//        input ^= mask;
//        input |= modifiedFloorNumber;
//        return input;
//    }
//    else  //Elevator 2
//    {
//        modifiedFloorNumber = (modifiedFloorNumber << elevatorThreeOffset);
//        uint mask = (nibbleMask << elevatorThreeOffset);
//        input |= mask;
//        input ^= mask;
//        input |= modifiedFloorNumber;
//        return input;
//    }
//}

///// <summary>
///// Decodes and returns the floor number of the uint
///// </summary>       
///// <param name="input">uint to decode</param>    
///// <param name="elevatorNumber">Number of the elevator 1-3</param>        
///// <returns>Returns the floorNumber from the uint</returns>
//private int BoolUIntConverter_GetElevatorFloor(uint input, int elevatorNumber)
//{
//    //Sanitise the size of elevatorNumber
//    if (elevatorNumber < 0 || elevatorNumber > 2)
//    {
//        Debug.LogError("[NetworkController] uintConverter - 404 Elevator does not exist");
//        return 0;
//    }

//    //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
//    if (elevatorNumber == 0)
//    {
//        //No need to mask the higher bits, so a straight return.
//        int floorNumber = (int)(input >> elevatorOneOffset);
//        floorNumber += elevatorFloorNumberOffset;
//        return floorNumber;
//    }
//    else if (elevatorNumber == 1)
//    {
//        //Shift data
//        uint shiftedData = (input >> elevatorTwoOffset);
//        //Mask away the higher bits
//        shiftedData &= nibbleMask;
//        int floorNumber = (int)(shiftedData) + elevatorFloorNumberOffset;
//        return floorNumber;
//    }
//    else  //Elevator 2
//    {
//        //Shift data
//        uint shiftedData = (input >> elevatorThreeOffset);
//        //Mask away the higher bits
//        shiftedData &= nibbleMask;
//        int floorNumber = (int)(shiftedData) + elevatorFloorNumberOffset;
//        return floorNumber;
//    }
//}
////-------------------------------------- end of bool to UInt32 converter ----------------------------------


//-------------------------------------- bool to UInt32 converter old----------------------------------
///// <summary>
///// Modifies a UInt32 on the bit level.
///// Sets "value" to bit "position" of "input".
///// </summary>       
///// <param name="input">UInt32 to modify</param>
///// <param name="position">Bit position to modify (0-31)</param>
///// <param name="value">Value to set the bit</param>        
///// <returns>Returns the modified UInt32</returns>
//public uint BoolUIntConverter_SetValue(uint input, int position, bool value)
//{
//    //Sanitise position
//    if (position < 0 || position > 31)
//    {
//        Debug.LogError("Uint32Converter - Position out of range");
//        return input;
//    }
//    if (value)
//    {
//        //Set the value to true
//        //Set the bit using a bitwise OR. 
//        input |= ((uint)(1) << position);
//    }
//    else
//    {
//        //Set the value to false
//        //Udon does not currently support bitwise NOT
//        //Instead making sure bit is first set to true and then using a bitwise XOR.
//        uint mask = ((uint)(1) << position);
//        input |= mask;
//        input ^= mask;
//    }
//    return input;
//}

///// <summary>
///// Reads the value of the bit at "position" of "input".
///// </summary>       
///// <param name="input">UInt32 to inspect</param>
///// <param name="position">Bit position to read (0-31)</param>
///// <returns>Boolean of specified bit position. Returns false on error.</returns>
//public bool BoolUIntConverter_GetValue(uint input, int position)
//{
//    //Sanitise position
//    if (position < 0 || position > 31)
//    {
//        Debug.LogError("Uint32Converter - Position out of range");
//        return false;
//    }

//    //Inspect using a bitwise AND and a mask.
//    //Branched in an IF statment for readability.
//    if ((input & ((uint)(1) << position)) != 0)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}

///// <summary>
///// Returns a bool array of bits within a UInt32. Use GetValue whenever possible.
///// </summary>       
///// <param name="input">UInt32 to convert</param>    
///// <returns>A bool array representing the UInt32 input</returns>
//public bool[] BoolUIntConverter_GetValues(uint input)
//{
//    bool[] boolArray = new bool[32];

//    //Iterate through all the bits and populate the array
//    for (byte i = 0; i < 32; i++)
//    {
//        boolArray[i] = BoolUIntConverter_GetValue(input, i);
//    }

//    return boolArray;
//}

///// <summary>
///// Takes a bool[] and overlays it on a UInt32. Use SetValue whenever possible.
///// </summary>       
///// <param name="input">UInt32 to overlay</param>    
///// <param name="values">A bool array up to length 32</param>    
///// <returns>A bool array representing the UInt32 input</returns>
//public uint BoolUIntConverter_SetValues(uint input, bool[] values)
//{
//    //Sanitise the size of values
//    if (values == null || values.Length > 32)
//    {
//        Debug.LogError("UintConverter - Array null or too long");
//        return 0;
//    }

//    //Iterate through all the bools and set the values
//    for (byte i = 0; i < values.Length; i++)
//    {
//        input = BoolUIntConverter_SetValue(input, i, values[i]);
//    }

//    return input;
//}