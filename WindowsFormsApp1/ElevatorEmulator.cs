using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WindowsFormsApp1
{
    /// <summary>
    /// This is the GUI component section which controls the buttons and textbox components
    /// It's not part of the Unity build
    /// </summary>
    public partial class ElevatorEmulator : Form
    {
        NetworkingController _controller;
        /// <summary>
        /// The intervall in which the <see cref="_controller"/> performs an Update() loop
        /// </summary>
        private const int _tickTimeInMilliseconds = 500;
        public ElevatorEmulator()
        {
            InitializeComponent();
            //we need to fake the Unity script assignements that normally happen in the Unity editor
            _controller = new NetworkingController();
            _controller.form1 = this;
            _controller._elevatorRequester = new ElevatorRequester();
            _controller._elevatorRequester._networkingController = _controller;
            _controller._elevatorControllerReception = new ElevatorController();
            _controller.Start();
            _controller._elevatorControllerReception.form1 = this;
            timer1.Interval = _tickTimeInMilliseconds;
            timer1.Start();
        }
        /// <summary>
        /// To make debugging more simple, we don't run this script 90 times per second, just each <see cref="_tickTimeInMilliseconds"/>
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            _controller.Update();
        }
        //------------------------------------------- GUI interface ------------------------------------
        internal void SetElevatorLevelOnDisplay(int floorNumber, int elevator)
        {
            switch (elevator)
            {
                case 0:
                    elevator1.Text = floorNumber.ToString();
                    break;
                case 1:
                    elevator2.Text = floorNumber.ToString();
                    break;
                case 2:
                    elevator3.Text = floorNumber.ToString();
                    break;
            }
        }
        internal void SetElevatorCalledDown(int floor)
        {
            SetButtonColor(false, true, floor);
        }
        internal void SetElevatorCalledUp(int floor)
        {
            SetButtonColor(true, true, floor);
        }
        internal void SetElevatorNotCalledDown(int floor)
        {
            SetButtonColor(false, false, floor);
        }
        internal void SetElevatorNotCalledUp(int floor)
        {
            SetButtonColor(true, false, floor);
        }
        /// <summary>
        /// Setting a button color for either the up button if <see cref="directionUp"/> is true or the down button
        /// on a certain <see cref="floor"/>. White if <see cref="stateCalled"/> is false, else red
        /// </summary>
        private void SetButtonColor(bool directionUp, bool stateCalled, int floor)
        {
            Debug.Print("[UI] Set button " + floor + " directionUp:" + directionUp.ToString() + " stateCalled:" + stateCalled.ToString());
            Color color;
            if (stateCalled)
            {
                color = Color.Red;
            }
            else
            {
                color = Color.LightGray;
            }
            Button button1;
            Button button5;
            Button button13;
            if (directionUp)
            {
                button1 = buttonCallUp_0;
                button5 = buttonCallUp_5;
                button13 = buttonCallUp_13;
            }
            else
            {
                button1 = buttonCallDown_0;
                button5 = buttonCallDown_5;
                button13 = buttonCallDown_13;
            }
            switch (floor)
            {
                case 0:
                    button1.BackColor = color;
                    break;
                case 5:
                    button5.BackColor = color;
                    break;
                case 13:
                    button13.BackColor = color;
                    break;
            }
        }
        /// <summary>
        /// Closing elevators on floor 0 / reception
        /// </summary>
        internal void CloseElevatorReception(int elevatorNumber)
        {
            switch (elevatorNumber)
            {
                case 0:
                    textBoxElevator1OpenReception.Text = "CLOSED";
                    break;
                case 1:
                    textBoxElevator2OpenReception.Text = "CLOSED";
                    break;
                case 2:
                    textBoxElevator3OpenReception.Text = "CLOSED";
                    break;
            }
        }
        /// <summary>
        /// Opening elevators on floor 0 / reception
        /// </summary>
        internal void OpenElevatorReception(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            DisplayElevatorDoorState(isIdle, isGoingUp, elevatorNumber, textBoxElevator1OpenReception, textBoxElevator2OpenReception, textBoxElevator3OpenReception, true);
        }
        private void DisplayElevatorDoorState(bool isIdle, bool isGoingUp, int elevatorNumber, TextBox texBox1, TextBox texBox2, TextBox texBox3, bool isOpen)
        {
            string state;

            if (isOpen)
            {
                state = "OPEN ( ";
            }
            else
            {
                state = "CLOSED ( ";
            }

            if (isIdle)
            {
                state += "idle)";
            }
            else if (isGoingUp)
            {
                state += "going UP)";
            }
            else
            {
                state += "going DOWN)";
            }
            switch (elevatorNumber)
            {
                case 0:
                    texBox1.Text = $"{state}";
                    break;
                case 1:
                    texBox2.Text = $"{state}";
                    break;
                case 2:
                    texBox3.Text = $"{state}";
                    break;
            }
        }
        /// <summary>
        /// User presses button "DOWN" on floor 0 / reception
        /// </summary>
        private void buttonCallDown_0_Click(object sender, EventArgs e)
        {
            _controller.API_LocalPlayerPressedCallButton(0, false);
        }
        /// <summary>
        /// User presses button "UP" on floor 0 / reception
        /// </summary>
        private void buttonCallUp_0_Click(object sender, EventArgs e)
        {
            _controller.API_LocalPlayerPressedCallButton(0, true);
        }
        /// <summary>
        /// User presses button "UP" on floor 5
        /// </summary>
        private void buttonCallUp_5_Click(object sender, EventArgs e)
        {
            _controller.API_LocalPlayerPressedCallButton(5, true);
        }
        /// <summary>
        /// User presses button "DOWN" on floor 5
        /// </summary>
        private void buttonCallDown_5_Click(object sender, EventArgs e)
        {
            _controller.API_LocalPlayerPressedCallButton(5, false);
        }
        /// <summary>
        /// User presses button "DOWN" on floor 13 (topmost floor)
        /// </summary>
        private void buttonCallDown_13_Click(object sender, EventArgs e)
        {
            _controller.API_LocalPlayerPressedCallButton(13, false);
        }
        /// <summary>
        /// User presses button "UP" on floor 13 (topmost floor)
        /// </summary>
        private void buttonCallUp_13_Click(object sender, EventArgs e)
        {
            _controller.API_LocalPlayerPressedCallButton(13, true);
        }
        /// <summary>
        /// Interface to display the general elevator state for debugging purposes
        /// </summary>
        internal void DisplayElevatorState(int elevatorNumber, bool isGoingUp, bool isIdle, bool isOpen)
        {
            DisplayElevatorDoorState(isIdle, isGoingUp, elevatorNumber, textBoxElevator1Open, textBoxElevator2Open, textBoxElevator3Open, isOpen);
        }

        internal void DisplayElevatorBroken(bool elevator0Working, bool elevator1Working, bool elevator2Working)
        {
            if (!elevator0Working)
                textBoxElevator1OpenReception.Text = "BROKEN";
            if (!elevator1Working)
                textBoxElevator2OpenReception.Text = "BROKEN";
            if (!elevator2Working)
                textBoxElevator3OpenReception.Text = "BROKEN";
        }
    }
    /// <summary>
    /// Please don't removed outcommented stuff here, this would break the Unity Version
    /// This is the class we need to work on, everything around it is basicly not relevant
    /// and has only the emulation purpose
    /// </summary>
    public class NetworkingController
    {
        //----- just added for emulator--------
        public ElevatorEmulator form1;
        private readonly Time time = new Time();
        private readonly bool _localPlayerisMaster = true;
        //-------------------------------------
        public ElevatorRequester _elevatorRequester;
        public ElevatorController _elevatorControllerReception;
        public ElevatorController _elevatorControllerArrivalArea;
        //public InsidePanelScriptForDesktop _insidePanelScriptElevator0Desktop;
        //public InsidePanelScriptForVR _InsidePanelScriptElevator0ForVR;
        /// <summary>
        /// elevator states, synced by master
        /// </summary>
        //NOPE [HideInInspector, UdonSynced(UdonSyncMode.None)]
        public ulong _syncData1 = 0;
        /// <summary>
        /// elevator request states, synced by master
        /// </summary>
        //NOPE [HideInInspector, UdonSynced(UdonSyncMode.None)]
        public uint _syncData2 = 0;
        /// <summary>
        /// other public variables
        /// </summary>
        //NOPE VRCPlayerApi _localPlayer;
        private bool _finishedLocalSetup = false;
        private bool _isMaster = false;
        private bool _worldIsLoaded = false;
        /// <summary>
        /// "ENUM" of different bools that are synced in _syncData
        /// (ENUM isn't possible in Udon, so we use this here)
        /// </summary>
        private const int SyncBool_Initialized = 0;
        private const int SyncBool_Elevator0working = 1;
        private const int SyncBool_Elevator1working = 2;
        private const int SyncBool_Elevator2working = 3;
        private const int SyncBool_Elevator0open = 4;
        private const int SyncBool_Elevator1open = 5;
        private const int SyncBool_Elevator2open = 6;
        private const int SyncBool_Elevator0idle = 7;
        private const int SyncBool_Elevator1idle = 8;
        private const int SyncBool_Elevator2idle = 9;
        private const int SyncBool_Elevator0goingUp = 10;
        private const int SyncBool_Elevator1goingUp = 11;
        private const int SyncBool_Elevator2goingUp = 12;
        /// <summary>
        /// Sync-data positions for elevator call up buttons
        /// </summary>
        private const int SyncBoolReq_ElevatorCalledUp_0 = 13;
        private const int SyncBoolReq_ElevatorCalledUp_1 = 14;
        private const int SyncBoolReq_ElevatorCalledUp_2 = 15;
        private const int SyncBoolReq_ElevatorCalledUp_3 = 16;
        private const int SyncBoolReq_ElevatorCalledUp_4 = 17;
        private const int SyncBoolReq_ElevatorCalledUp_5 = 18;
        private const int SyncBoolReq_ElevatorCalledUp_6 = 19;
        private const int SyncBoolReq_ElevatorCalledUp_7 = 20;
        private const int SyncBoolReq_ElevatorCalledUp_8 = 21;
        private const int SyncBoolReq_ElevatorCalledUp_9 = 22;
        private const int SyncBoolReq_ElevatorCalledUp_10 = 23;
        private const int SyncBoolReq_ElevatorCalledUp_11 = 24;
        private const int SyncBoolReq_ElevatorCalledUp_12 = 25;
        private const int SyncBoolReq_ElevatorCalledUp_13 = 26;
        /// <summary>
        /// Sync-data positions for elevator call down buttons
        /// </summary>
        private const int SyncBoolReq_ElevatorCalledDown_0 = 27;
        private const int SyncBoolReq_ElevatorCalledDown_1 = 28;
        private const int SyncBoolReq_ElevatorCalledDown_2 = 29;
        private const int SyncBoolReq_ElevatorCalledDown_3 = 30;
        private const int SyncBoolReq_ElevatorCalledDown_4 = 31;
        private const int SyncBoolReq_ElevatorCalledDown_5 = 32;
        private const int SyncBoolReq_ElevatorCalledDown_6 = 33;
        private const int SyncBoolReq_ElevatorCalledDown_7 = 34;
        private const int SyncBoolReq_ElevatorCalledDown_8 = 35;
        private const int SyncBoolReq_ElevatorCalledDown_9 = 36;
        private const int SyncBoolReq_ElevatorCalledDown_10 = 37;
        private const int SyncBoolReq_ElevatorCalledDown_11 = 38;
        private const int SyncBoolReq_ElevatorCalledDown_12 = 39;
        private const int SyncBoolReq_ElevatorCalledDown_13 = 40;
        /// <summary>
        /// Sync-data positions for internal elevator 0 buttons
        /// </summary>
        private const int SyncBoolReq_Elevator0CalledToFloor_0 = 41;
        private const int SyncBoolReq_Elevator0CalledToFloor_1 = 42;
        private const int SyncBoolReq_Elevator0CalledToFloor_2 = 43;
        private const int SyncBoolReq_Elevator0CalledToFloor_3 = 44;
        private const int SyncBoolReq_Elevator0CalledToFloor_4 = 45;
        private const int SyncBoolReq_Elevator0CalledToFloor_5 = 46;
        private const int SyncBoolReq_Elevator0CalledToFloor_6 = 47;
        private const int SyncBoolReq_Elevator0CalledToFloor_7 = 48;
        private const int SyncBoolReq_Elevator0CalledToFloor_8 = 49;
        private const int SyncBoolReq_Elevator0CalledToFloor_9 = 50;
        private const int SyncBoolReq_Elevator0CalledToFloor_10 = 51;
        private const int SyncBoolReq_Elevator0CalledToFloor_11 = 52;
        private const int SyncBoolReq_Elevator0CalledToFloor_12 = 53;
        private const int SyncBoolReq_Elevator0CalledToFloor_13 = 54;
        /// <summary>
        /// Sync-data positions for internal elevator 1 buttons
        /// </summary>
        private const int SyncBoolReq_Elevator1CalledToFloor_0 = 55;
        private const int SyncBoolReq_Elevator1CalledToFloor_1 = 56;
        private const int SyncBoolReq_Elevator1CalledToFloor_2 = 57;
        private const int SyncBoolReq_Elevator1CalledToFloor_3 = 58;
        private const int SyncBoolReq_Elevator1CalledToFloor_4 = 59;
        private const int SyncBoolReq_Elevator1CalledToFloor_5 = 60;
        private const int SyncBoolReq_Elevator1CalledToFloor_6 = 61;
        private const int SyncBoolReq_Elevator1CalledToFloor_7 = 62;
        private const int SyncBoolReq_Elevator1CalledToFloor_8 = 63;
        private const int SyncBoolReq_Elevator1CalledToFloor_9 = 64;
        private const int SyncBoolReq_Elevator1CalledToFloor_10 = 65;
        private const int SyncBoolReq_Elevator1CalledToFloor_11 = 66;
        private const int SyncBoolReq_Elevator1CalledToFloor_12 = 67;
        private const int SyncBoolReq_Elevator1CalledToFloor_13 = 68;
        /// <summary>
        /// Sync-data positions for internal elevator 2 buttons
        /// </summary>
        private const int SyncBoolReq_Elevator2CalledToFloor_0 = 69;
        private const int SyncBoolReq_Elevator2CalledToFloor_1 = 70;
        private const int SyncBoolReq_Elevator2CalledToFloor_2 = 71;
        private const int SyncBoolReq_Elevator2CalledToFloor_3 = 72;
        private const int SyncBoolReq_Elevator2CalledToFloor_4 = 73;
        private const int SyncBoolReq_Elevator2CalledToFloor_5 = 74;
        private const int SyncBoolReq_Elevator2CalledToFloor_6 = 75;
        private const int SyncBoolReq_Elevator2CalledToFloor_7 = 76;
        private const int SyncBoolReq_Elevator2CalledToFloor_8 = 77;
        private const int SyncBoolReq_Elevator2CalledToFloor_9 = 78;
        private const int SyncBoolReq_Elevator2CalledToFloor_10 = 79;
        private const int SyncBoolReq_Elevator2CalledToFloor_11 = 80;
        private const int SyncBoolReq_Elevator2CalledToFloor_12 = 81;
        private const int SyncBoolReq_Elevator2CalledToFloor_13 = 82;
        /// <summary>
        /// State of the elevator bell
        /// </summary>
        private const int SyncBoolReq_BellOn = 83;
        /// <summary>
        /// Initializing the scene
        /// </summary>
        public void Start()
        {
            time.sw.Start();
            Debug.Print("[NetworkController] NetworkingController is now in Start()");
            //NOPE _localPlayer = Networking.LocalPlayer;
            //the first master has to set the constant scene settings
            if (_localPlayerisMaster && !GetSyncValue(SyncBool_Initialized))
            {
                _isMaster = true;
                MASTER_SetConstSceneElevatorStates();
                FirstMasterSetupElevatorControl();
            }
            //NOPE _elevatorControllerReception.CustomStart();
            //NOPE _insidePanelScriptElevator0Desktop.CustomStart();
            //NOPE _InsidePanelScriptElevator0ForVR.CustomStart();
            Debug.Print("[NetworkController] Elevator NetworkingController is now loaded");
            _worldIsLoaded = true;
        }
        /// <summary>
        /// Locally storing which elevator is currently working and which isn't, since we only need to read this 
        /// once from the SYNC state and it won't change, so it would be a waste to read it every time again
        /// </summary>
        private bool _elevator0Working = false;
        private bool _elevator1Working = false;
        private bool _elevator2Working = false;
        /// <summary>
        /// Setting up the scene at startup or when it isn't setup yet
        /// </summary>
        private void ReadConstSceneElevatorStates()
        {
            Debug.Print("[NetworkController] Setting random elevator states for reception by localPlayer");
            _elevator0Working = GetSyncValue(SyncBool_Elevator0working);
            _elevator1Working = GetSyncValue(SyncBool_Elevator1working);
            _elevator2Working = GetSyncValue(SyncBool_Elevator2working);
            form1.DisplayElevatorBroken(_elevator0Working, _elevator1Working, _elevator2Working);
            //NOPE _elevatorControllerReception._elevator1working = _elevator0Working;
            //NOPE _elevatorControllerReception._elevator2working = _elevator1Working;
            //NOPE _elevatorControllerReception._elevator3working = _elevator2Working;
            //NOPE _elevatorControllerReception.SetupElevatorStates();
            Debug.Print("[NetworkController] Random elevator states for reception are now set by localPlayer");
        }
        /// <summary> Elevator 0 open/close (localPlayer world view that is in sync with ElevatorController of Level 0)</summary>
        private bool _elevator0isOpenReception = false;
        /// <summary> Elevator 1 open/close (localPlayer world view that is in sync with ElevatorController of Level 0)</summary>
        private bool _elevator1isOpenReception = false;
        /// <summary> Elevator 2 open/close (localPlayer world view that is in sync with ElevatorController of Level 0)</summary>
        private bool _elevator2isOpenReception = false;
        /// <summary>
        /// is called when network packets are received (only happens when there are more players except Master in the scene
        /// </summary>
        public void OnDeserialization()
        {
            if (!_worldIsLoaded)
                return;
            LocalOnDeserialization(); //do nothing else in here or shit will break!
        }
        /// <summary>
        /// Can be called by master (locally in Update) or by everyone else OnDeserialization (when SyncBool states change)
        /// </summary>
        private void LocalOnDeserialization()
        {
            if (!_finishedLocalSetup)
            {
                if (time.GetTime() < 1f)
                    return;
                Debug.Print("[NetworkController] Local setup was started");
                if (GetSyncValue(SyncBool_Initialized))
                {
                    ReadConstSceneElevatorStates();
                    _finishedLocalSetup = true;
                    Debug.Print("[NetworkController] Local setup was finished");
                }
                else
                {
                    return;
                }
            }
            else
            {
                LOCAL_CheckElevatorStates();
                LOCAL_CheckElevatorCallStateReception();
                LOCAL_CheckElevatorLevels();
            }
        }
        /// <summary>
        /// locally storing where each elevator is and has to go, these need to be checked against SyncBool states 
        /// </summary>
        private bool[] _calledToFloorToGoUp_MASTER = new bool[14];
        private bool[] _calledToFloorToGoDown_MASTER = new bool[14];
        //private int[] _elevatorCurrentFloor = new int[3];
        private bool[] _elevator0FloorTargets_MASTER = new bool[14];
        private int _elevator0FloorTargets_MASTER_COUNT = 0;
        private bool[] _elevator1FloorTargets_MASTER = new bool[14];
        private int _elevator1FloorTargets_MASTER_COUNT = 0;
        private bool[] _elevator2FloorTargets_MASTER = new bool[14];
        private int _elevator2FloorTargets_MASTER_COUNT = 0;
        private float[] _timeAtCurrentFloorElevatorOpened_MASTER = new float[3];
        private float[] _timeAtCurrentFloorElevatorClosed_MASTER = new float[3];
        private int _elevatorCheckTick_MASTER = 1;
        /// <summary>
        /// The first Master (on instance start) needs to run this once to set the initial elevator states and positions
        /// </summary>
        private void FirstMasterSetupElevatorControl()
        {
            Debug.Print("[NetworkController] FirstMasterSetupElevatorControl started");
            MASTER_SetSyncValue(SyncBool_Elevator0goingUp, false);
            MASTER_SetSyncValue(SyncBool_Elevator1goingUp, false);
            MASTER_SetSyncValue(SyncBool_Elevator2goingUp, false);
            MASTER_SetSyncValue(SyncBool_Elevator0idle, true);
            MASTER_SetSyncValue(SyncBool_Elevator1idle, true);
            MASTER_SetSyncValue(SyncBool_Elevator2idle, true);
            MASTER_SetSyncElevatorFloor(0, 4);
            MASTER_SetSyncElevatorFloor(1, 4);
            MASTER_SetSyncElevatorFloor(2, 4);
            Debug.Print("[NetworkController] FirstMasterSetupElevatorControl finished");
        }
        /// <summary>
        /// The master runs this elevator controller in every Update()
        /// The load is splitted accross 3 frames to have a better performance
        /// </summary>
        private void MASTER_RunElevatorControl()
        {
            if (_elevator0Working && _elevatorCheckTick_MASTER == 1)
            {
                MASTER_RunElevator(0, SyncBool_Elevator0open, SyncBool_Elevator0idle, SyncBool_Elevator0goingUp, _elevator0FloorTargets_MASTER);
            }
            if (_elevator1Working && _elevatorCheckTick_MASTER == 2)
            {
                MASTER_RunElevator(1, SyncBool_Elevator1open, SyncBool_Elevator1idle, SyncBool_Elevator1goingUp, _elevator1FloorTargets_MASTER);
            }
            if (_elevator2Working && _elevatorCheckTick_MASTER == 3)
            {
                MASTER_RunElevator(2, SyncBool_Elevator2open, SyncBool_Elevator2idle, SyncBool_Elevator2goingUp, _elevator2FloorTargets_MASTER);
            }
            _elevatorCheckTick_MASTER++;
            if (_elevatorCheckTick_MASTER >= 4)
                _elevatorCheckTick_MASTER = 1;
        }

        private const float TIME_TO_STAY_OPEN = 5f; // 10f is normal
        private const float TIME_TO_STAY_OPEN_RECEPTION = 5f; //30f is normal
        /// <summary>
        /// Running a single elevator, is only called by master in every Update
        /// </summary>
        private void MASTER_RunElevator(int elevatorNumber, int SyncBoolElevatorOpen, int SyncBoolElevatorIdle, int SyncBoolElevatorGoingUp, bool[] elevatorFloorTargets)
        {
            Debug.Print("Elevator " + elevatorNumber + " has " + MASTER_GetInternalTargetCount(elevatorNumber) + " targets.");
            int currentFloor = GetSyncElevatorFloor(elevatorNumber);
            bool elevatorIdle = GetSyncValue(SyncBoolElevatorIdle);
            bool elevatorGoingUp = GetSyncValue(SyncBoolElevatorGoingUp);
            bool targetFound = false;
            //we can't handle people blocking the elevator, so we will ignore ongoing requests and save them for later
            if (GetSyncValue(SyncBoolElevatorOpen))
            {
                //an elevator must stay open for 10 seconds
                if (!(currentFloor == 0) && time.GetTime() - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > TIME_TO_STAY_OPEN || currentFloor == 0 && time.GetTime() - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > TIME_TO_STAY_OPEN_RECEPTION)
                {
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " closing on floor " + currentFloor);
                    //time to close this elevator
                    MASTER_SetSyncValue(SyncBoolElevatorOpen, false);
                    _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime();
                    //closing it was enough for now
                    return;
                }
                else
                {
                    //before opening, check if there is another internal target, else set
                    if (!elevatorIdle && MASTER_GetInternalTargetCount(elevatorNumber) == 0)
                    {
                        MASTER_SetElevatorIdle(elevatorNumber);
                    }
                    //handle all targets that are pointing in the current direction on the current floor
                    MASTER_HandleFloorTarget(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                    //we can't move an open elevator, so the code ends here
                    return;
                }
            }
            else if (time.GetTime() - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < 4f)
            {
                //an elevator must stay closed for the duration of the closing animation
                //however, we could still process user-requests to open it again here
                //we can't move an elevator that isn't fully closed yet
                return;
            }
            //an elevator that is going up will only handle internal targets and up requests
            if (!elevatorIdle && elevatorGoingUp)
            {
                //when the current floor was requested and we've arrived
                if (elevatorFloorTargets[currentFloor] || _calledToFloorToGoUp_MASTER[currentFloor])
                {
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " opening on floor " + currentFloor);
                    MASTER_HandleFloorDoorOpening(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                    //the code must end here since we just stopped the elevator
                    return;
                }
                else //checking for next target
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
                        Debug.Print("[NetworkController] Elevator " + elevatorNumber + " driving up to floor " + (int)(currentFloor + 1));
                        MASTER_SetSyncElevatorFloor(elevatorNumber, currentFloor + 1);
                        _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime(); //resetting the timer for next floor
                        //the code must end here since we are now travelling further
                        return;
                    }
                    else
                    {
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
            }
            if (!elevatorIdle && !elevatorGoingUp) //this elevator can handle internal targets and external down-requests
            {
                //when the current floor was internally or externally requested and we've arrived
                if (elevatorFloorTargets[currentFloor] || _calledToFloorToGoDown_MASTER[currentFloor])
                {
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " opening on floor " + currentFloor);
                    MASTER_HandleFloorDoorOpening(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                    //the code must end here since we just opened the elevator
                    return;
                }
                else //checking for next target
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
                        Debug.Print("[NetworkController] Elevator " + elevatorNumber + " driving down to floor " + (int)(currentFloor - 1));
                        MASTER_SetSyncElevatorFloor(elevatorNumber, currentFloor - 1);
                        _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime(); //resetting the timer for next floor
                        //the code must end here since we are now travelling further
                        return;
                    }
                    else
                    {
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
                            MASTER_SetSyncValue(SyncBoolElevatorGoingUp, true);
                            // since the next loop code will handle this direction, we need to stop execution now
                            return;
                        }
                    }
                }
            }
            //when we reach this line of code, the elevator found no internal target and is closed
            //when the current floor was requested by anyone and we are already there, we open the elevator
            if (elevatorFloorTargets[currentFloor] || _calledToFloorToGoUp_MASTER[currentFloor] || _calledToFloorToGoDown_MASTER[currentFloor])
            {
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " opening again on floor " + currentFloor);
                MASTER_HandleFloorDoorOpening(elevatorNumber, currentFloor, elevatorGoingUp, elevatorIdle);
                //the code must end here since we just stopped the elevator
                return;
            }
            //if we reach this line, we need to find the next target first
            int nextTarget = 0;
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
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now as an internal target and is going up");
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
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now as an internal target and is going down");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp: false);
                return;
            }
            //--------------------------------------------------------------------------------------------------
            //if we reach this code line, there is no internal target and we need to check external targets next
            for (int i = currentFloor + 1; i <= 13; i++)
            {
                if (_calledToFloorToGoUp_MASTER[i] || _calledToFloorToGoDown_MASTER[i]) //those are external targets
                {
                    targetFound = true;
                    nextTarget = i;
                    break;
                }
            }
            //if we found an internal target, we go out of idle mode and set the new direction up
            if (targetFound)
            {
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an external target and is going up");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp:true);
                //this elevator basicly belongs to that floor then, so both targets are handled, but this isn't perfect
                Debug.Print("[NetworkController] We're faking an EREQ next to set an internal target");
                ELREQ_SetInternalTarget(elevatorNumber, nextTarget);
                _calledToFloorToGoUp_MASTER[nextTarget] = false;
                _calledToFloorToGoDown_MASTER[nextTarget] = false;
                return;
            }
            //Now we need to check if there is an external target on the way down
            for (int i = currentFloor - 1; i >= 0; i--)
            {
                if (_calledToFloorToGoUp_MASTER[i] || _calledToFloorToGoDown_MASTER[i]) //those are external targets
                {
                    targetFound = true;
                    nextTarget = i;
                    break;
                }
            }
            //if we found an internal target, we go out of idle mode and set the new direction down
            if (targetFound)
            {
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now as an external target and is going down");
                MASTER_SetElevatorDirection(elevatorNumber, goingUp: false);
                //this elevator basicly belongs to that floor then, so both targets are handled, but this isn't perfect
                Debug.Print("[NetworkController] We're faking an EREQ next to set an internal target");
                ELREQ_SetInternalTarget(elevatorNumber, nextTarget);
                _calledToFloorToGoUp_MASTER[nextTarget] = false;
                _calledToFloorToGoDown_MASTER[nextTarget] = false;
                return;
            }
            //------------------------------------
            //reaching this code line means there is no next target and the elevator must go into idle mode
            if (!elevatorIdle)
            {
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " is now idle since there are no targets.");
                MASTER_SetElevatorIdle(elevatorNumber);
            }
        }
        /// <summary>
        /// Resets the elevator call button network when the elevator opens
        /// </summary>
        /// <param name="floor"></param>
        private void MASTER_HandleFloorDoorOpening(int elevatorNumber, int currentFloor, bool directionUp, bool isIdle)
        {
            MASTER_HandleFloorTarget(elevatorNumber, currentFloor, directionUp, isIdle);
            MASTER_SetSyncValue(SyncBool_Elevator0open + elevatorNumber, true); //opening the elevator
            _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] = time.GetTime();
        }
        /// <summary>
        /// Handles all floor targets on the current floor and direction
        /// </summary>
        /// <param name="floor"></param>
        private void MASTER_HandleFloorTarget(int elevatorNumber, int currentFloor, bool directionUp, bool isIdle)
        {
            if ((directionUp || isIdle) && _calledToFloorToGoUp_MASTER[currentFloor])
            {
                _calledToFloorToGoUp_MASTER[currentFloor] = false; //this target was now handled
                MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + currentFloor, false);
            }
            if ((!directionUp || isIdle) && _calledToFloorToGoDown_MASTER[currentFloor])
            {
                _calledToFloorToGoDown_MASTER[currentFloor] = false; //this target was now handled
                MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + currentFloor, false);
            }
            if (elevatorNumber == 0 && _elevator0FloorTargets_MASTER[currentFloor])
            {
                _elevator0FloorTargets_MASTER[currentFloor] = false;
                _elevator0FloorTargets_MASTER_COUNT--;
            }
            else if (elevatorNumber == 1 && _elevator1FloorTargets_MASTER[currentFloor])
            {
                _elevator1FloorTargets_MASTER[currentFloor] = false;
                _elevator1FloorTargets_MASTER_COUNT--;
            }
            else if (elevatorNumber == 2 && _elevator2FloorTargets_MASTER[currentFloor])
            {
                _elevator2FloorTargets_MASTER[currentFloor] = false;
                _elevator2FloorTargets_MASTER_COUNT--;
            }
        }
        /// <summary>
        /// Sets the elevator travel direction
        /// </summary>
        private void MASTER_SetElevatorDirection(int elevatorNumber, bool goingUp)
        {
            MASTER_SetSyncValue(SyncBool_Elevator0goingUp + elevatorNumber, goingUp);
            MASTER_SetSyncValue(SyncBool_Elevator0idle + elevatorNumber, false);
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
            Debug.Print("ERROR: Unknown elevator number in MASTER_GetInternalTargetCount!");
            return 0; // to make the compiler happy
        }
        /// <summary>
        /// Run by LocalPlayer
        /// Checking all bits and see if the real world alligns with it
        /// </summary>
        private void LOCAL_CheckElevatorStates()
        {
            form1.DisplayElevatorState(0, GetSyncElevatorGoingUp(0), GetSyncValue(SyncBool_Elevator0idle), GetSyncValue(SyncBool_Elevator0open));
            form1.DisplayElevatorState(1, GetSyncElevatorGoingUp(1), GetSyncValue(SyncBool_Elevator1idle), GetSyncValue(SyncBool_Elevator1open));
            form1.DisplayElevatorState(2, GetSyncElevatorGoingUp(2), GetSyncValue(SyncBool_Elevator2idle), GetSyncValue(SyncBool_Elevator2open));
            //Elevator 0 open/close
            if (!_elevator0isOpenReception && GetSyncValue(SyncBool_Elevator0open))   //TODO: Check floor position on synced uint!
            {
                if (GetSyncElevatorFloor(0) != 0)
                {
                    Debug.Print("[NetworkController] Elevator 0 is currently at floor " + GetSyncElevatorFloor(0) + " so Reception won't open.");
                }
                else
                {
                    Debug.Print("[NetworkController] LocalPlayer received to open elevator 0");
                    _elevatorControllerReception.OpenElevatorReception(0, GetSyncElevatorGoingUp(0), GetSyncValue(SyncBool_Elevator0idle));
                    _elevator0isOpenReception = true;
                }
            }
            else if (_elevator0isOpenReception && !GetSyncValue(SyncBool_Elevator0open))   //TODO: Check floor position on synced uint!
            {
                Debug.Print("[NetworkController] LocalPlayer received to close elevator 0");
                _elevatorControllerReception.CloseElevatorReception(0);
                _elevator0isOpenReception = false;
            }
            //Elevator 1 open/close
            if (!_elevator1isOpenReception && GetSyncValue(SyncBool_Elevator1open))   //TODO: Check floor position on synced uint!
            {
                if (GetSyncElevatorFloor(1) != 0)
                {
                    Debug.Print("[NetworkController] Elevator 1 is currently at floor " + GetSyncElevatorFloor(1) + " so Reception won't open.");
                }
                else
                {
                    Debug.Print("[NetworkController] LocalPlayer received to open elevator 1");
                    _elevatorControllerReception.OpenElevatorReception(1, GetSyncElevatorGoingUp(1), GetSyncValue(SyncBool_Elevator1idle));
                    _elevator1isOpenReception = true;
                }
            }
            else if (_elevator1isOpenReception && !GetSyncValue(SyncBool_Elevator1open))
            {
                Debug.Print("[NetworkController] LocalPlayer received to close elevator 1");
                _elevatorControllerReception.CloseElevatorReception(1);
                _elevator1isOpenReception = false;
            }
            //Elevator 2 open/close
            if (!_elevator2isOpenReception && GetSyncValue(SyncBool_Elevator2open) && GetSyncElevatorFloor(2) == 0)
            {
                if (GetSyncElevatorFloor(2) != 0)
                {
                    Debug.Print("[NetworkController] Elevator 2 is currently at floor " + GetSyncElevatorFloor(2) + " so Reception won't open.");
                }
                else
                {
                    Debug.Print("[NetworkController] LocalPlayer received to open elevator 2");
                    _elevatorControllerReception.OpenElevatorReception(2, GetSyncElevatorGoingUp(2), GetSyncValue(SyncBool_Elevator2idle));
                    _elevator2isOpenReception = true;
                }
            }
            else if (_elevator2isOpenReception && !GetSyncValue(SyncBool_Elevator2open))
            {
                Debug.Print("[NetworkController] LocalPlayer received to open elevator 2");
                _elevatorControllerReception.CloseElevatorReception(2);
                _elevator2isOpenReception = false;
            }
        }
        /// <summary>
        /// This update is run every frame
        /// </summary>
        public void Update()
        {
            //Debug.Print("Time: " + time.GetTime());
            if (_localPlayerisMaster)
            {
                if (!_isMaster)
                {
                    Debug.Print("[NetworkController] Master has changed!");
                    MASTER_OnMasterChanged();
                    _isMaster = true;
                }
                //first process network events because Master can't do that else
                LocalOnDeserialization();
                //only the current master does this
                MASTER_RunElevatorControl();
            }
            //Checking if local call was handled or dropped
            LOCAL_CheckIfElevatorCallWasReceived();
        }
        /// <summary>
        /// When the master changes, we need to load the SyncBool states into local copies to run the elevator controller correct
        /// </summary>
        private void MASTER_OnMasterChanged()
        {
            //taking all content from SyncedData into local arrays
            //TODO: Implement this
            //setting _calledToFloor
            for (int i = 0; i <= 13; i++)
            {
                //_calledToFloor[i] = GetSyncValue( );
            }
            _timeAtCurrentFloorElevatorOpened_MASTER[0] = time.GetTime();
            _timeAtCurrentFloorElevatorOpened_MASTER[1] = time.GetTime();
            _timeAtCurrentFloorElevatorOpened_MASTER[2] = time.GetTime();
            _timeAtCurrentFloorElevatorClosed_MASTER[0] = time.GetTime();
            _timeAtCurrentFloorElevatorClosed_MASTER[1] = time.GetTime();
            _timeAtCurrentFloorElevatorClosed_MASTER[2] = time.GetTime();
            _elevatorCheckTick_MASTER = 1;
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///  sets the elevators into the random state which is determined by master user uwu
        ///  this can't be "random", but we have a pool of 7 allowed states
        /// </summary>
        private void MASTER_SetConstSceneElevatorStates()
        {
            //to make testing easier, we only allow one state right now
            int random = 4; // UnityEngine.Random.Range(0, 7);
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
            Debug.Print("[NetworkController] Random elevator states are now set by master");
        }
        //Local copies of elevator state to check them against SyncBool states
        private int _localPlayerFloor = 0; //this is only relevant once we figure out instancing in Unity
        private bool[] _elevatorIsCalledDown = new bool[14];
        private bool[] _elevatorIsCalledUp = new bool[14];
        private bool[] _locallyIsCalledDown = new bool[14];
        private bool[] _locallyIsCalledUp = new bool[14];
        private float[] _elevatorCallTimeUp = new float[14];
        private float[] _elevatorCallTimeDown = new float[14];
        /// <summary>
        /// Checking if the elevator was successfully called by master after we've called it, else we drop the request
        /// </summary>
        private void LOCAL_CheckIfElevatorCallWasReceived()
        {
            for (int floor = 0; floor <= 13; floor++)
            {
                //Check if there is a pending request
                if (_locallyIsCalledUp[floor] && !GetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + floor) && time.GetTime() - _elevatorCallTimeUp[floor] > 1f)
                {
                    _locallyIsCalledUp[floor] = false;
                    //TODO: link all elevator controllers here in Unity later
                    if (floor == 0)
                    {
                        _elevatorControllerReception.SetElevatorNotCalledUp(floor);
                    }
                    else
                    {
                        _elevatorControllerArrivalArea.SetElevatorNotCalledUp(floor);
                    }
                }
                if (_locallyIsCalledDown[floor] && !GetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + floor) && time.GetTime() - _elevatorCallTimeDown[floor] > 1f)
                {
                    _locallyIsCalledDown[floor] = false;
                    //TODO: link all elevator controllers here in Unity later
                    if (floor == 0)
                    {
                        _elevatorControllerReception.SetElevatorNotCalledDown(floor);
                    }
                    else
                    {
                        _elevatorControllerArrivalArea.SetElevatorNotCalledDown(floor);
                    }
                }
            }
        }
        /// <summary>
        /// Checking elevator callsign states (the callbutton-panels / UP-DOWN buttons)
        /// </summary>
        private void LOCAL_CheckElevatorCallStateReception()
        {
            for (int floor = 0; floor <= 13; floor++)
            {
                //Check if the master has set an elevator call to up
                bool networkBoolState = GetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + floor);
                if (_elevatorIsCalledUp[floor] && !networkBoolState)
                {
                    _elevatorIsCalledUp[floor] = false;
                    _elevatorControllerReception.SetElevatorNotCalledUp(floor);
                }
                else if (!_elevatorIsCalledUp[floor] && networkBoolState)
                {
                    _elevatorIsCalledUp[floor] = true;
                    _locallyIsCalledUp[floor] = false; //local call can be dropped now
                    //TODO: link all elevator controllers here in Unity later
                    if (floor == 0)
                    {
                        _elevatorControllerReception.SetElevatorCalledUp(floor);
                    }
                    else
                    {
                        //TODO: Replace with a different controller in Unity
                        _elevatorControllerReception.SetElevatorCalledUp(floor);
                    }
                }
                //Check if the master has set an elevator call to down from reception
                networkBoolState = GetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + floor);
                if (_elevatorIsCalledDown[floor] && !networkBoolState)
                {
                    _elevatorIsCalledDown[floor] = false;
                    _elevatorControllerReception.SetElevatorNotCalledDown(floor);
                }
                else if (!_elevatorIsCalledDown[floor] && networkBoolState)
                {
                    _elevatorIsCalledDown[floor] = true;
                    _locallyIsCalledDown[floor] = false; //local call can be dropped now
                    //TODO: link all elevator controllers here in Unity later
                    if (floor == 0)
                    {
                        _elevatorControllerReception.SetElevatorCalledDown(floor);
                    }
                    else
                    {
                        //TODO: Replace with a different controller in Unity
                        _elevatorControllerArrivalArea.SetElevatorCalledDown(floor);
                    }
                }
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
                _localElevator0Level = floorNumber;
            }
            if (_elevator1Working && _localElevator1Level != GetSyncElevatorFloor(1))
            {
                int floorNumber = GetSyncElevatorFloor(1);
                _elevatorControllerReception.SetElevatorLevelOnDisplay(floorNumber, 1);
                _localElevator1Level = floorNumber;
            }
            if (_elevator2Working && _localElevator2Level != GetSyncElevatorFloor(2))
            {
                int floorNumber = GetSyncElevatorFloor(2);
                _elevatorControllerReception.SetElevatorLevelOnDisplay(floorNumber, 2);
                _localElevator2Level = floorNumber;
            }
        }
        //------------------------------- API ---------------------------------
        public void API_LocalPlayerPressedCallButton(int floorNumber, bool directionUp)
        {
            if (directionUp)
            {
                Debug.Print("[NetworkController] Elevator called to floor " + floorNumber + " by localPlayer (Up)");
                if (_elevatorIsCalledUp[floorNumber] || _locallyIsCalledUp[floorNumber])
                    return;
                _locallyIsCalledUp[floorNumber] = true;
                _elevatorCallTimeUp[floorNumber] = time.GetTime();
                _elevatorRequester.RequestElevatorFloorButton(directionUp, floorNumber);
            }
            else
            {
                Debug.Print("[NetworkController] Elevator called to floor " + floorNumber + " by localPlayer (Down)");
                if (_elevatorIsCalledDown[floorNumber] || _locallyIsCalledDown[floorNumber])
                    return;
                _locallyIsCalledDown[floorNumber] = true;
                _elevatorCallTimeDown[floorNumber] = time.GetTime();
            }
            _elevatorRequester.RequestElevatorFloorButton(directionUp, floorNumber);
        }
        /// <summary>
        /// When localPlayer pressed a button INSIDE the elevator
        /// </summary>
        public void API_LocalPlayerPressedElevatorButton(int elevatorNumber, int buttonNumber)
        {
            Debug.Print($"[NetworkController] LocalPlayer pressed button {buttonNumber} in elevator {elevatorNumber}");
            if (buttonNumber == 0) //OPEN
            {
                if (!GetIfElevatorIsOpen(elevatorNumber))
                {
                    _elevatorRequester.RequestElevatorDoorStateChange(elevatorNumber, true);
                }
                return;
            }
            if (buttonNumber == 1) //CLOSE
            {
                if (GetIfElevatorIsOpen(elevatorNumber))
                {
                    _elevatorRequester.RequestElevatorDoorStateChange(elevatorNumber, false);
                }
                return;
            }
            if (buttonNumber == 2) // (m2) mirror-button
            {
                //toggle loli stairs locally in ElevatorController
                //_elevatorControllerReception.ToggleLoliStairs(elevatorNumber);
                return;
            }
            if (buttonNumber == 3) // (m1) loli-stairs button
            {
                //toggle mirror locally in ElevatorController
                //_elevatorControllerReception.ToggleMirror(elevatorNumber);
                return;
            }
            if (buttonNumber == 18) // RING-button
            {
                //This button does absolutely nothing atm
                return;
            }
            //every other button is an internal floor request, button 4 is floor 0 etc.
            _elevatorRequester.RequestElevatorInternalTarget(elevatorNumber, buttonNumber - 4);
        }
        //---------------------------- Network Call Receivers-----------------------------------------
        /// <summary>
        /// This function receives a client request (and is run by master-only)
        /// </summary>
        public void ELREQ_CallFromFloor(bool directionUp, int floor)
        {
            Debug.Print("[NetworkingController] Master received Elevator called to floor " + floor + " by localPlayer (DirectionUp: " + directionUp.ToString() + ")");
            if (directionUp && !GetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + floor))
            {
                if (!MASTER_ElevatorAlreadyThereAndOpen(floor, true))
                {
                    MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledUp_0 + floor, true);
                    _calledToFloorToGoUp_MASTER[floor] = true;
                }
            }
            else if (!directionUp && !GetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + floor))
            {
                if (!MASTER_ElevatorAlreadyThereAndOpen(floor, true))
                {
                    MASTER_SetSyncValue(SyncBoolReq_ElevatorCalledDown_0 + floor, true);
                    _calledToFloorToGoDown_MASTER[floor] = true;
                }
            }
        }
        /// <summary>
        /// Only Master receives this, it's called by ElevatorRequester
        /// </summary>
        public void ELREQ_CallToChangeDoorState(int elevatorNumber, bool open)
        {
            Debug.Print("Master received CallToChangeDoorState for elevator " + elevatorNumber + " (Direction open: " + open.ToString() + ")");
            if (open && GetSyncValue(SyncBool_Elevator0idle + elevatorNumber) || time.GetTime() - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < 2.5f)
            {
                MASTER_HandleFloorDoorOpening(elevatorNumber, GetSyncElevatorFloor(elevatorNumber), GetSyncValue(SyncBool_Elevator0goingUp + elevatorNumber), GetSyncValue(SyncBool_Elevator0idle + elevatorNumber));
            }
            else if (!open && GetSyncValue(SyncBool_Elevator0open + elevatorNumber) && time.GetTime() - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > 6f)
            {
                MASTER_SetSyncValue(SyncBool_Elevator0open + elevatorNumber, false);
                _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime();
            }
        }
        /// <summary>
        /// Only Master receives this, it's called by ElevatorRequester
        /// </summary>
        public void ELREQ_SetInternalTarget(int elevatorNumber, int floorNumber)
        {
            Debug.Print("[NetworkController] Master receiver client request to set target for elevator " + elevatorNumber + " to floor " + floorNumber);
            if (elevatorNumber == 0 && !GetSyncValue(SyncBoolReq_Elevator0CalledToFloor_0 + floorNumber))
            {
                MASTER_SetSyncValue(SyncBoolReq_Elevator0CalledToFloor_0 + floorNumber, true);
                _elevator0FloorTargets_MASTER[floorNumber] = true;
                _elevator0FloorTargets_MASTER_COUNT++;
            }
            else if (elevatorNumber == 1 && !GetSyncValue(SyncBoolReq_Elevator1CalledToFloor_0 + floorNumber))
            {
                MASTER_SetSyncValue(SyncBoolReq_Elevator1CalledToFloor_0 + floorNumber, true);
                _elevator1FloorTargets_MASTER[floorNumber] = true;
                _elevator1FloorTargets_MASTER_COUNT++;
            }
            else if (elevatorNumber == 2 && !GetSyncValue(SyncBoolReq_Elevator2CalledToFloor_0 + floorNumber))
            {
                MASTER_SetSyncValue(SyncBoolReq_Elevator2CalledToFloor_0 + floorNumber, true);
                _elevator2FloorTargets_MASTER[floorNumber] = true;
                _elevator2FloorTargets_MASTER_COUNT++;
            }
        }
        /// <summary>
        /// Checks if there is already an open elevator on this floor which is going in the target direction
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="directionUp"></param>
        /// <returns></returns>
        private bool MASTER_ElevatorAlreadyThereAndOpen(int floor, bool directionUp)
        {
            if (_elevator0Working && GetSyncElevatorFloor(0) == 0 && GetSyncValue(SyncBool_Elevator0open))
            {
                return true;
            }
            if (_elevator1Working && GetSyncElevatorFloor(1) == 0 && GetSyncValue(SyncBool_Elevator1open))
            {
                return true;
            }
            if (_elevator2Working && GetSyncElevatorFloor(2) == 0 && GetSyncValue(SyncBool_Elevator2open))
            {
                return true;
            }
            return false;
        }


        //------------------------------------------------------------------------------------------------------------
        //------------------------------------------ SyncBool Interface ----------------------------------------------
        //------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Checks if that elevator is currently open
        /// </summary>
        private bool GetIfElevatorIsOpen(int elevatorNumber)
        {
            return GetSyncValue(SyncBool_Elevator0open + elevatorNumber);
        }
        /// <summary>
        /// Returns the synced elevator direction, true is up / false is down
        /// </summary>
        private bool GetSyncElevatorGoingUp(int elevatorNumber)
        {
            return GetSyncValue(SyncBool_Elevator0goingUp + elevatorNumber);
        }
        //------------------------------------------------------------------------------------------------------------
        //------------------------------------------ SyncBool lowlevel code ------------------------------------------
        //------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This script sets and reads individual bits within a uint as well as encoding three numbers (nibbles) within the most significant bytes
        /// 
        /// The ulong maps as follows:-
        ///  - 63-60 variable_1 (4bits)
        ///  - 59-56 variable_2 (4bits)
        ///  - 55-52 variable_2 (4bits)
        ///  - 51-0 binary bools [0-51]
        ///
        /// The uint maps as follows:-
        ///  - 31-0 binary bools [52-83(?)]
        /// 
        /// Script by NotFish
        /// </summary>''
        private const ulong ulongzero = 0;
        private const byte elevatorOneOffset = 60;
        private const byte elevatorTwoOffset = 56;
        private const byte elevatorThreeOffset = 52;
        private const byte ulongBoolStartPosition = 51;
        private const ulong nibbleMask = 15; // ...0000 0000 1111
        private const int elevatorFloorNumberOffset = -2; //Keks floor hack offset

        /// <summary>
        /// Modifies a _syncData1 & _syncData2 on the bit level.
        /// Sets "value" to bit "position" of "input".
        /// </summary>       
        /// <param name="input">uint to modify</param>
        /// <param name="position">Bit position to modify (0-83)</param>
        /// <param name="value">Value to set the bit</param>        
        /// <returns>Returns the modified uint</returns>
        private void MASTER_SetSyncValue(int position, bool value)
        {
            Debug.Print($"SYNC DATA bool {position} set to {value.ToString()}");
            //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
            ulong localUlong = _syncData1;
            uint localUint = _syncData2;

            //Sanitise position
            if (position < 0 || position > 83)
            {
                Debug.Print("uintConverter - Position out of range");
                return;
            }

            //Index the positions back to front (negative index to be stored in the uint)
            position = ulongBoolStartPosition - position;

            if (position > 0)
            {
                //Store in the ulong
                if (value)
                {
                    //We want to set the value to true
                    //Set the bit using a bitwise OR. 
                    localUlong |= ((ulong)(1) << position);
                }
                else
                {
                    //We want to set the value to false
                    //Udon does not currently support bitwise NOT
                    //Instead making sure bit is set to true and using a bitwise XOR.
                    ulong mask = ((ulong)(1) << position);
                    localUlong |= mask;
                    localUlong ^= mask;
                }
            }
            else // position < 0
            {
                //Store in the uint
                //Need to shift to to a valid address first!
                position += 32;

                if (value)
                {
                    //We want to set the value to true
                    //Set the bit using a bitwise OR. 
                    localUint |= ((uint)(1) << position);
                }
                else
                {
                    //We want to set the value to false
                    //Udon does not currently support bitwise NOT
                    //Instead making sure bit is set to true and using a bitwise XOR.
                    uint mask = ((uint)(1) << position);
                    localUint |= mask;
                    localUint ^= mask;
                }
            }

            //Let's not forget to actually write it back to syncData!
            _syncData1 = localUlong;
            _syncData2 = localUint;
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
            if (position < 0 || position > 83)
            {
                Debug.Print("uintConverter - Position out of range");
                return false;
            }

            //Index the positions back to front (negative index to be stored in the uint)
            position = ulongBoolStartPosition - position;

            if (position > 0)
            {
                //Read from the ulong
                //Inspect using a bitwise AND and a mask.
                //Branched in an IF statment for readability.
                if ((_syncData1 & ((ulong)(1) << position)) != ulongzero)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else // position < 0
            {
                //Read from the uint
                //Need to shift to to a valid address first!
                position += 32;

                //Inspect using a bitwise AND and a mask.
                //Branched in an IF statment for readability.
                if ((_syncData2 & ((uint)(1) << position)) != 0)
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
        /// Decodes and returns the floor number of the ulong
        /// </summary>           
        /// <param name="elevatorNumber">Number of the elevator 1-3</param>        
        /// <param name="floorNumber">value to set to the elevator variable</param>
        /// <returns>The updated uint</returns>
        private void MASTER_SetSyncElevatorFloor(int elevatorNumber, int floorNumber)
        {
            Debug.Print($"SYNC DATA elevator {elevatorNumber} floor setting to {floorNumber}");
            //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
            ulong localUlong = _syncData1;
            //Debug.Print($"SYNC DATA_1 was {localUlong}");
            //Sanitise the size of elevatorNumber
            if (elevatorNumber < 0 || elevatorNumber > 2)
            {
                Debug.Print($"uintConverter - 404 Elevator {elevatorNumber} does not exist");
                return;
            }

            //floorNumber needs to be betweeen 0-15, so quick hack for negative floors and sanitise
            int modifiedFloorNumberTempUint = (floorNumber - elevatorFloorNumberOffset);
            if (modifiedFloorNumberTempUint < 0 || modifiedFloorNumberTempUint > 15)
            {
                Debug.Print($"uintConverter - Elevator  {elevatorNumber} number invalid");
                return;
            }
            ulong modifiedFloorNumber = (ulong)modifiedFloorNumberTempUint;
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
                ulong mask = (nibbleMask << elevatorOneOffset);
                localUlong |= mask;
                localUlong ^= mask;
                localUlong |= modifiedFloorNumber;
            }
            else if (elevatorNumber == 1)
            {
                modifiedFloorNumber = (modifiedFloorNumber << elevatorTwoOffset);
                ulong mask = (nibbleMask << elevatorTwoOffset);
                localUlong |= mask;
                localUlong ^= mask;
                localUlong |= modifiedFloorNumber;
            }
            else  //Elevator 3
            {
                modifiedFloorNumber = (modifiedFloorNumber << elevatorThreeOffset);
                ulong mask = (nibbleMask << elevatorThreeOffset);
                localUlong |= mask;
                localUlong ^= mask;
                localUlong |= modifiedFloorNumber;
            }
            _syncData1 = localUlong;
            //Debug.Print($"SYNC DATA_1 is now {localUlong}");
        }

        /// <summary>
        /// Decodes and returns the floor number of the ulong
        /// </summary>              
        /// <param name="elevatorNumber">Number of the elevator 1-3</param>        
        /// <returns>Returns the floorNumber from the uint</returns>
        private int GetSyncElevatorFloor(int elevatorNumber)
        {
            //Debug.Print($"SYNC DATA_1 is now {_syncData1}");
            //Sanitise the size of elevatorNumber
            if (elevatorNumber < 0 || elevatorNumber > 2)
            {
                Debug.Print($"uintConverter - 404 Elevator  {elevatorNumber} does not exist");
                return 0;
            }

            //Not sure if Udon likes SWITCH cases, so just doing this with IF statments
            if (elevatorNumber == 0)
            {
                //No need to mask the higher bits, so a straight return.
                int floorNumber = (int)(_syncData1 >> elevatorOneOffset);
                floorNumber += elevatorFloorNumberOffset;
                //Debug.Print($"SYNC DATA elevator {elevatorNumber} floor is read as {floorNumber}");
                return floorNumber;
            }
            else if (elevatorNumber == 1)
            {
                //Shift data
                ulong shiftedData = (_syncData1 >> elevatorTwoOffset);
                //Mask away the higher bits
                shiftedData &= nibbleMask;
                int floorNumber = (int)(shiftedData) + elevatorFloorNumberOffset;
                //Debug.Print($"SYNC DATA elevator {elevatorNumber} floor is read as {floorNumber}");
                return floorNumber;
            }
            else  //Elevator 3
            {
                //Shift data
                ulong shiftedData = (_syncData1 >> elevatorThreeOffset);
                //Mask away the higher bits
                shiftedData &= nibbleMask;
                int floorNumber = (int)(shiftedData) + elevatorFloorNumberOffset;
                //Debug.Print($"SYNC DATA elevator {elevatorNumber} floor is read as {floorNumber}");
                return floorNumber;
            }
        }
    }

    //------------------------------------ Emulator classes ----------------------------------

    /// <summary>
    /// Emulator of unity time
    /// </summary>
    public class Time
    {
        public Stopwatch sw = Stopwatch.StartNew();
        /// <summary>
        /// Returning the time since application start in seconds
        /// </summary>
        /// <returns></returns>
        public long GetTime()
        {
            return sw.ElapsedMilliseconds / 1000;
        }
    }
    /// <summary>
    /// Simplified emulator of our elevatorController
    /// </summary>
    public class ElevatorController
    {
        public ElevatorEmulator form1;

        internal void CloseElevatorReception(int elevatorNumber)
        {
            form1.CloseElevatorReception(elevatorNumber);
        }

        internal void OpenElevatorReception(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            form1.OpenElevatorReception(elevatorNumber, isGoingUp, isIdle);
        }

        internal void SetElevatorCalledDown(int floor)
        {
            form1.SetElevatorCalledDown(floor);
        }

        internal void SetElevatorCalledUp(int floor)
        {
            form1.SetElevatorCalledUp(floor);
        }

        internal void SetElevatorLevelOnDisplay(int floorNumber, int v)
        {
            form1.SetElevatorLevelOnDisplay(floorNumber, v);
        }

        internal void SetElevatorNotCalledDown(int floor)
        {
            form1.SetElevatorNotCalledDown(floor);
        }

        internal void SetElevatorNotCalledUp(int floor)
        {
            form1.SetElevatorNotCalledUp(floor);
        }
    }
    /// <summary>
    /// Simplified emulator of our elevatorRequester
    /// In Unity, this is where a clients sends a request to the master user
    /// </summary>
    public class ElevatorRequester
    {
        public NetworkingController _networkingController;
        public void RequestElevatorFloorButton(bool directionUp, int floor)
        {
            Debug.Print("[ElevatorRequester] Elevator called to floor " + floor + " by localPlayer (DirectionUp: " + directionUp.ToString() + ")");
            //master doesn't need to send a request over network
            _networkingController.ELREQ_CallFromFloor(directionUp, floor);
        }

        public void RequestElevatorDoorStateChange(int elevatorNumber, bool open)
        {
            Debug.Print("[ElevatorRequester] Elevator " + elevatorNumber + " requested by localPlayer to open/close (DirectionOpen: " + open.ToString() + ")");
            //master doesn't need to send a request over network
            _networkingController.ELREQ_CallToChangeDoorState(elevatorNumber, open);
        }

        public void RequestElevatorInternalTarget(int elevatorNumber, int floor)
        {
            Debug.Print("[ElevatorRequester] Request internal target for elevator " + elevatorNumber.ToString() + " by localPlayer (target: " + floor.ToString() + ")");
            //master doesn't need to send a request over network
            _networkingController.ELREQ_SetInternalTarget(elevatorNumber, floor);
        }
    }

}