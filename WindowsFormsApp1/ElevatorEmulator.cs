﻿using System;
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
    /// This is the class we need to work on, everything around it is basicly not relevant
    /// and has only the emulation purpose, thus it's poorly written
    /// 
    /// The whole purpose of this class is to run as fast as possible in Udon, this is why we can't use
    /// jagged arrays or ref-type arrays (both would definitely make our code much better). 
    /// We also can't use ENUMs, multiple classes, structs, static classes/functions etc. so we are very limited here.
    /// VRChat slows everything massively down with security checks, so we go for the least amount of CPU cycles 
    /// which sometimes makes Code a bit less readable. We keep a more "clean" version to make high-level changes 
    /// where the bitshifting part isn't included.
    /// 
    /// Please don't remove outcommented stuff here, this would break the Unity Version
    /// </summary>
    public class NetworkingController
    {
        #region justForEmulator
        //----- just added for emulator--------
        public ElevatorEmulator form1;
        private readonly Time time = new Time();
        private readonly bool _localPlayerisMaster = true;
        //-------------------------------------
        #endregion justForEmulator
        #region variables
        public ElevatorRequester _elevatorRequester;
        public ElevatorController _elevatorControllerReception;
        public ElevatorController _elevatorControllerArrivalArea;
        //public InsidePanelScriptForDesktop _insidePanelScriptElevator0Desktop;
        //public InsidePanelScriptForVR _InsidePanelScriptElevator0ForVR;
        /// <summary>
        /// Current floor level that localPlayer is on
        /// </summary>
        private int _localPlayerCurrentFloor = 7;
        /// <summary>
        /// elevator states, synced by master
        /// </summary>
        //NOPE [HideInInspector, UdonSynced(UdonSyncMode.None)]
        public long _syncData1 = 0;
        /// <summary>
        /// elevator request states, synced by master
        /// </summary>
        //NOPE [HideInInspector, UdonSynced(UdonSyncMode.None)]
        public long _syncData2 = 0;
        /// <summary>
        /// other public variables
        /// </summary>
        //NOPE VRCPlayerApi _localPlayer;
        private bool _finishedLocalSetup = false;
        private bool _isMaster = false;
        private bool _worldIsLoaded = false;
        /// <summary>
        /// Locally storing which elevator is currently working and which isn't, since we only need to read this 
        /// once from the SYNC state and it won't change, so it would be a waste to read it every time again
        /// 
        /// This is read by LOCAL but also used by MASTER
        /// 
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
        ///  0-115 binary bools
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
        /// <summary>
        /// Bools for rooms because theres room for bools...
        /// </summary>
        private const int SyncBool_Room0IsLocked = 87;
        private const int SyncBool_Room1IsLocked = 88;
        private const int SyncBool_Room2IsLocked = 89;
        private const int SyncBool_Room3IsLocked = 90;
        private const int SyncBool_Room4IsLocked = 91;
        private const int SyncBool_Room5IsLocked = 92;
        private const int SyncBool_Room6IsLocked = 93;
        private const int SyncBool_Room7IsLocked = 94;
        private const int SyncBool_Room8IsLocked = 95;
        private const int SyncBool_Room9IsLocked = 96;
        private const int SyncBool_Room10IsLocked = 97;
        private const int SyncBool_Room11IsLocked = 98;
        private const int SyncBool_Room12IsLocked = 99;
        private const int SyncBool_Room0IsAvailable = 100;
        private const int SyncBool_Room1IsAvailable = 101;
        private const int SyncBool_Room2IsAvailable = 102;
        private const int SyncBool_Room3IsAvailable = 103;
        private const int SyncBool_Room4IsAvailable = 104;
        private const int SyncBool_Room5IsAvailable = 105;
        private const int SyncBool_Room6IsAvailable = 106;
        private const int SyncBool_Room7IsAvailable = 107;
        private const int SyncBool_Room8IsAvailable = 108;
        private const int SyncBool_Room9IsAvailable = 109;
        private const int SyncBool_Room10IsAvailable = 110;
        private const int SyncBool_Room11IsAvailable = 111;
        private const int SyncBool_Room12IsAvailable = 112;
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

        //Bools for rooms
        private const int SyncBool_AddressLong2_RoomXIsLocked = 35;
        private const long SyncBool_MaskLong2_Room0IsLocked = (1L << 35);
        private const long SyncBool_MaskLong2_Room1IsLocked = (1L << 36);
        private const long SyncBool_MaskLong2_Room2IsLocked = (1L << 37);
        private const long SyncBool_MaskLong2_Room3IsLocked = (1L << 38);
        private const long SyncBool_MaskLong2_Room4IsLocked = (1L << 39);
        private const long SyncBool_MaskLong2_Room5IsLocked = (1L << 40);
        private const long SyncBool_MaskLong2_Room6IsLocked = (1L << 41);
        private const long SyncBool_MaskLong2_Room7IsLocked = (1L << 42);
        private const long SyncBool_MaskLong2_Room8IsLocked = (1L << 43);
        private const long SyncBool_MaskLong2_Room9IsLocked = (1L << 44);
        private const long SyncBool_MaskLong2_Room10IsLocked = (1L << 45);
        private const long SyncBool_MaskLong2_Room11IsLocked = (1L << 46);
        private const long SyncBool_MaskLong2_Room12IsLocked = (1L << 47);
        private const int SyncBool_AddressLong2_RoomXIsAvailable = 48;
        private const long SyncBool_MaskLong2_Room0IsAvailable = (1L << 48);
        private const long SyncBool_MaskLong2_Room1IsAvailable = (1L << 49);
        private const long SyncBool_MaskLong2_Room2IsAvailable = (1L << 50);
        private const long SyncBool_MaskLong2_Room3IsAvailable = (1L << 51);
        private const long SyncBool_MaskLong2_Room4IsAvailable = (1L << 52);
        private const long SyncBool_MaskLong2_Room5IsAvailable = (1L << 53);
        private const long SyncBool_MaskLong2_Room6IsAvailable = (1L << 54);
        private const long SyncBool_MaskLong2_Room7IsAvailable = (1L << 55);
        private const long SyncBool_MaskLong2_Room8IsAvailable = (1L << 56);
        private const long SyncBool_MaskLong2_Room9IsAvailable = (1L << 57);
        private const long SyncBool_MaskLong2_Room10IsAvailable = (1L << 58);
        private const long SyncBool_MaskLong2_Room11IsAvailable = (1L << 59);
        private const long SyncBool_MaskLong2_Room12IsAvailable = (1L << 60);

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
            time.sw.Start();
            Debug.Print("[NetworkController] NetworkingController is now in Start()");
            //NOPE _localPlayer = Networking.LocalPlayer;
            //the first master has to set the constant scene settings
            if (_localPlayerisMaster && 0L == (_syncData2 & (SyncBool_MaskLong2_Initialized)))
            {
                _isMaster = true;
                MASTER_SetConstSceneElevatorStates();
                MASTER_FirstMasterSetupElevatorControl();
            }
            //NOPE _elevatorControllerReception.CustomStart();
            //NOPE _insidePanelScriptElevator0Desktop.CustomStart();
            //NOPE _InsidePanelScriptElevator0ForVR.CustomStart();
            Debug.Print("[NetworkController] Elevator NetworkingController is now loaded");
            _worldIsLoaded = true;
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
                LOCAL_OnDeserialization();
                //only the current master does this
                MASTER_RunElevatorControl();
            }
            //Checking if local external call was handled or dropped
            LOCAL_CheckIfElevatorExternalCallWasReceived();
            //Checking if local internal call was handled or dropped
            LOCAL_CheckIfElevatorInternalCallWasReceived();
            //TODO: remove on live build
            TEST_DisplayElevatorStates();
        }
        //TODO: remove on live build
        private void TEST_DisplayElevatorStates()
        {            
            form1.DisplayElevatorState(0, 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0goingUp)), 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0idle)), 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator0open)));
            form1.DisplayElevatorState(1, 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1goingUp)), 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1idle)), 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator1open)));
            form1.DisplayElevatorState(2, 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2goingUp)), 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2idle)), 0L != (_syncData1 & (SyncBool_MaskLong1_Elevator2open)));
            form1.DisplayLocalPlayerFloor(_localPlayerCurrentFloor);
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
            Debug.Print("[NetworkController] FirstMasterSetupElevatorControl started");
            MASTER_SetSyncValue(SyncBool_Elevator0goingUp, false);
            MASTER_SetSyncValue(SyncBool_Elevator1goingUp, false);
            MASTER_SetSyncValue(SyncBool_Elevator2goingUp, false);
            MASTER_SetSyncValue(SyncBool_Elevator0idle, true);
            MASTER_SetSyncValue(SyncBool_Elevator1idle, true);
            MASTER_SetSyncValue(SyncBool_Elevator2idle, true);
            MASTER_SetSyncElevatorFloor(0, 13);
            MASTER_SetSyncElevatorFloor(1, 8);
            MASTER_SetSyncElevatorFloor(2, 2);            
            Debug.Print("[NetworkController] FirstMasterSetupElevatorControl finished");
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
                _timeAtCurrentFloorElevatorOpened_MASTER[i] = time.GetTime();
                _timeAtCurrentFloorElevatorClosed_MASTER[i] = time.GetTime();
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
                    Debug.Print("_elevator0FloorTargets_MASTER_COUNT:" + _elevator0FloorTargets_MASTER_COUNT);
                if (_elevator1FloorTargets_MASTER_COUNT != 0)
                    Debug.Print("_elevator1FloorTargets_MASTER_COUNT:" + _elevator1FloorTargets_MASTER_COUNT);
                if (_elevator2FloorTargets_MASTER_COUNT != 0)
                    Debug.Print("_elevator2FloorTargets_MASTER_COUNT:" + _elevator2FloorTargets_MASTER_COUNT);
                if (_calledToFloorToGoUp_MASTER_COUNT != 0)
                    Debug.Print("_calledToFloorToGoUp_MASTER_COUNT:" + _calledToFloorToGoUp_MASTER_COUNT);
                if (_calledToFloorToGoDown_MASTER_COUNT != 0)
                    Debug.Print("_calledToFloorToGoDown_MASTER_COUNT:" + _calledToFloorToGoDown_MASTER_COUNT);
            }
            if (_elevatorCheckTick_MASTER >= 4)
                _elevatorCheckTick_MASTER = 1;
        }
        private const float TIME_TO_STAY_CLOSED_AFTER_GOING_OUT_OF_IDLE = 3f;
        private const float TIME_TO_STAY_OPEN = 5f; // 10f is normal
        private const float TIME_TO_STAY_OPEN_RECEPTION = 5f; //30f is normal
        private const float TIME_TO_STAY_CLOSED = 4f; //MUST BE 4f IN UNITY! Because of the closing animation
        private const float TIME_TO_DRIVE_ONE_FLOOR = 2f;
        /// <summary>
        /// Running a single elevator, is only called by master in every Update
        /// 
        /// TODO: Moving down without internal target! (currently, setting an EREQ is a workaround for cheaper calculations)
        /// TODO: Ignore targets that generated fake-EREQs for other elevators
        /// 
        /// </summary>
        private void MASTER_RunElevator(int elevatorNumber, bool[] elevatorFloorTargets)
        {
            //Debug.Print("Elevator " + elevatorNumber + " has " + MASTER_GetInternalTargetCount(elevatorNumber) + " targets.");
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
                if (!(currentFloor == 0) && time.GetTime() - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > TIME_TO_STAY_OPEN || currentFloor == 0 && time.GetTime() - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > TIME_TO_STAY_OPEN_RECEPTION)
                {
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " closing on floor " + currentFloor);
                    //time to close this elevator
                    MASTER_SetSyncValue(SyncBool_Elevator0open + elevatorNumber, false);
                    _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime();
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
                if (time.GetTime() - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < TIME_TO_STAY_CLOSED)
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
            else if (time.GetTime() - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < TIME_TO_DRIVE_ONE_FLOOR)
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
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " opening on floor " + currentFloor);
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
                        Debug.Print("[NetworkController] Elevator " + elevatorNumber + " driving up to floor " + (int)(currentFloor + 1));
                        MASTER_SetSyncElevatorFloor(elevatorNumber, currentFloor + 1);
                        _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime(); //resetting the timer for next floor
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
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " opening on floor " + currentFloor);
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
                        Debug.Print("[NetworkController] Elevator " + elevatorNumber + " driving down to floor " + (int)(currentFloor - 1));
                        MASTER_SetSyncElevatorFloor(elevatorNumber, currentFloor - 1);
                        _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime(); //resetting the timer for next floor
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
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " opening again on floor " + currentFloor);
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
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an internal target and is going up");
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
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an internal target and is going down");
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
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now has an external target and is going up");
                    MASTER_SetElevatorDirection(elevatorNumber, goingUp: true);
                    //this elevator basicly belongs to that floor then, so both targets are handled, but this isn't perfect
                    Debug.Print("[NetworkController] We're faking an EREQ next to set an internal target");
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
                    Debug.Print("[NetworkController] Elevator " + elevatorNumber + " was idle but now as an external target and is going down");
                    MASTER_SetElevatorDirection(elevatorNumber, goingUp: false);
                    //this elevator basicly belongs to that floor then, so both targets are handled, but this isn't perfect
                    Debug.Print("[NetworkController] We're faking an EREQ next to set an internal target");
                    ELREQ_SetInternalTarget(elevatorNumber, nextTarget);
                    _floorHasFakeEREQ_MASTER[nextTarget] = true;
                    return;
                }
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
                _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] = time.GetTime() + TIME_TO_STAY_CLOSED - TIME_TO_STAY_CLOSED_AFTER_GOING_OUT_OF_IDLE;
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
            Debug.Print("ERROR: Unknown elevator number in MASTER_GetInternalTargetCount!");
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
            Debug.Print("[NetworkController] Random elevator states are now set by master");
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
        /// The long1 maps as follows:-
        ///  - 63-60 variable_1 (4bits)
        ///  - 59-56 variable_2 (4bits)
        ///  - 55-52 variable_3 (4bits)
        ///  - 0-51 binary bools [0-51]
        ///
        /// The long2 maps as follows:-
        ///  - 0-63 binary bools [52-115]
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
                bool[] cachedSync1Bools = GetBoolArraylong1ONLY();
                //the positions 0-51 are binary bools that might have changed
                for (int i = 1; i < 52; i++) //no need to check bool 0
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
                bool[] cachedSync2Bools = GetBoolArraylong2ONLY();
                //the positions 52-116 are binary bools that might have changed
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
                    LOCAL_SetElevatorCallButtonState(0, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_1:
                    LOCAL_SetElevatorCallButtonState(1, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_2:
                    LOCAL_SetElevatorCallButtonState(2, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_3:
                    LOCAL_SetElevatorCallButtonState(3, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_4:
                    LOCAL_SetElevatorCallButtonState(4, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_5:
                    LOCAL_SetElevatorCallButtonState(5, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_6:
                    LOCAL_SetElevatorCallButtonState(6, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_7:
                    LOCAL_SetElevatorCallButtonState(7, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_8:
                    LOCAL_SetElevatorCallButtonState(8, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_9:
                    LOCAL_SetElevatorCallButtonState(9, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_10:
                    LOCAL_SetElevatorCallButtonState(10, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_11:
                    LOCAL_SetElevatorCallButtonState(11, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_12:
                    LOCAL_SetElevatorCallButtonState(12, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledUp_13:
                    LOCAL_SetElevatorCallButtonState(13, buttonUp: true, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_0:
                    LOCAL_SetElevatorCallButtonState(0, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_1:
                    LOCAL_SetElevatorCallButtonState(1, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_2:
                    LOCAL_SetElevatorCallButtonState(2, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_3:
                    LOCAL_SetElevatorCallButtonState(3, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_4:
                    LOCAL_SetElevatorCallButtonState(4, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_5:
                    LOCAL_SetElevatorCallButtonState(5, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_6:
                    LOCAL_SetElevatorCallButtonState(6, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_7:
                    LOCAL_SetElevatorCallButtonState(7, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_8:
                    LOCAL_SetElevatorCallButtonState(8, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_9:
                    LOCAL_SetElevatorCallButtonState(9, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_10:
                    LOCAL_SetElevatorCallButtonState(10, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_11:
                    LOCAL_SetElevatorCallButtonState(11, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_12:
                    LOCAL_SetElevatorCallButtonState(12, buttonUp: false, called: newState);
                    break;
                case SyncBoolReq_ElevatorCalledDown_13:
                    LOCAL_SetElevatorCallButtonState(13, buttonUp: false, called: newState);
                    break;
                //case SyncBoolReq_Elevator#J1#CalledToFloor_#1#:
                //    LOCAL_SetElevatorInternalButtonState(#J1#,#1#, called: newState);
                //    break;
                case SyncBoolReq_Elevator0CalledToFloor_0:
                    LOCAL_SetElevatorInternalButtonState(0, 0, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_1:
                    LOCAL_SetElevatorInternalButtonState(0, 1, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_2:
                    LOCAL_SetElevatorInternalButtonState(0, 2, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_3:
                    LOCAL_SetElevatorInternalButtonState(0, 3, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_4:
                    LOCAL_SetElevatorInternalButtonState(0, 4, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_5:
                    LOCAL_SetElevatorInternalButtonState(0, 5, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_6:
                    LOCAL_SetElevatorInternalButtonState(0, 6, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_7:
                    LOCAL_SetElevatorInternalButtonState(0, 7, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_8:
                    LOCAL_SetElevatorInternalButtonState(0, 8, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_9:
                    LOCAL_SetElevatorInternalButtonState(0, 9, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_10:
                    LOCAL_SetElevatorInternalButtonState(0, 10, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_11:
                    LOCAL_SetElevatorInternalButtonState(0, 11, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_12:
                    LOCAL_SetElevatorInternalButtonState(0, 12, called: newState);
                    break;
                case SyncBoolReq_Elevator0CalledToFloor_13:
                    LOCAL_SetElevatorInternalButtonState(0, 13, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_0:
                    LOCAL_SetElevatorInternalButtonState(1, 0, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_1:
                    LOCAL_SetElevatorInternalButtonState(1, 1, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_2:
                    LOCAL_SetElevatorInternalButtonState(1, 2, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_3:
                    LOCAL_SetElevatorInternalButtonState(1, 3, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_4:
                    LOCAL_SetElevatorInternalButtonState(1, 4, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_5:
                    LOCAL_SetElevatorInternalButtonState(1, 5, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_6:
                    LOCAL_SetElevatorInternalButtonState(1, 6, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_7:
                    LOCAL_SetElevatorInternalButtonState(1, 7, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_8:
                    LOCAL_SetElevatorInternalButtonState(1, 8, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_9:
                    LOCAL_SetElevatorInternalButtonState(1, 9, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_10:
                    LOCAL_SetElevatorInternalButtonState(1, 10, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_11:
                    LOCAL_SetElevatorInternalButtonState(1, 11, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_12:
                    LOCAL_SetElevatorInternalButtonState(1, 12, called: newState);
                    break;
                case SyncBoolReq_Elevator1CalledToFloor_13:
                    LOCAL_SetElevatorInternalButtonState(1, 13, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_0:
                    LOCAL_SetElevatorInternalButtonState(2, 0, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_1:
                    LOCAL_SetElevatorInternalButtonState(2, 1, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_2:
                    LOCAL_SetElevatorInternalButtonState(2, 2, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_3:
                    LOCAL_SetElevatorInternalButtonState(2, 3, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_4:
                    LOCAL_SetElevatorInternalButtonState(2, 4, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_5:
                    LOCAL_SetElevatorInternalButtonState(2, 5, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_6:
                    LOCAL_SetElevatorInternalButtonState(2, 6, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_7:
                    LOCAL_SetElevatorInternalButtonState(2, 7, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_8:
                    LOCAL_SetElevatorInternalButtonState(2, 8, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_9:
                    LOCAL_SetElevatorInternalButtonState(2, 9, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_10:
                    LOCAL_SetElevatorInternalButtonState(2, 10, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_11:
                    LOCAL_SetElevatorInternalButtonState(2, 11, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_12:
                    LOCAL_SetElevatorInternalButtonState(2, 12, called: newState);
                    break;
                case SyncBoolReq_Elevator2CalledToFloor_13:
                    LOCAL_SetElevatorInternalButtonState(2, 13, called: newState);
                    break;
                case SyncBool_Elevator0IsDriving:
                    LOCAL_SetElevatorDrivingState(0, isDriving: newState);
                    break;
                case SyncBool_Elevator1IsDriving:
                    LOCAL_SetElevatorDrivingState(1, isDriving: newState);
                    break;
                case SyncBool_Elevator2IsDriving:
                    LOCAL_SetElevatorDrivingState(2, isDriving: newState);
                    break;
                default:
                    Debug.Print("ERROR: UNKNOWN BOOL HAS CHANGED IN SYNCBOOL, position: " + syncBoolPosition);
                    break;
            }
        }
        /// <summary>
        /// Setting if an elevator is currently driving
        /// </summary>
        private void LOCAL_SetElevatorDrivingState(int elevatorNumber, bool isDriving)
        {
            _elevatorControllerArrivalArea.SetElevatorDrivingState(elevatorNumber, isDriving);
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
        /// <summary>
        /// Setting a button inside an elevator to a different state
        /// </summary>
        private void LOCAL_SetElevatorInternalButtonState(int elevatorNumber, int floorNumber, bool called)
        {
            _elevatorControllerReception.SetElevatorFloorButtonState(elevatorNumber, floorNumber, called);
        }
        /// <summary>
        /// When a state of a floor callbutton changed, we need to update that button to on or off
        /// </summary>
        private void LOCAL_SetElevatorCallButtonState(int floorNumber, bool buttonUp, bool called)
        {
            if (buttonUp)
            {
                if (called)
                {
                    _elevatorControllerReception.SetElevatorCalledUp(floorNumber);
                }
                else
                {
                    _elevatorControllerReception.SetElevatorNotCalledUp(floorNumber);
                }
            }
            else
            {
                if (called)
                {
                    _elevatorControllerReception.SetElevatorCalledDown(floorNumber);
                }
                else
                {
                    _elevatorControllerReception.SetElevatorNotCalledDown(floorNumber);
                }
            }
        }
        /// <summary>
        /// Is run ONCE by localPlayer on scene load.
        /// Setting up the scene at startup or when it isn't setup yet
        /// </summary>
        private void LOCAL_ReadConstSceneElevatorStates()
        {
            Debug.Print("[NetworkController] Setting random elevator states for reception by localPlayer");
            _elevator0Working = 0L != (_syncData2 & (SyncBool_MaskLong2_Elevator0working));
            _elevator1Working = 0L != (_syncData2 & (SyncBool_MaskLong2_Elevator1working));
            _elevator2Working = 0L != (_syncData2 & (SyncBool_MaskLong2_Elevator2working));
            form1.DisplayElevatorBroken(_elevator0Working, _elevator1Working, _elevator2Working);
            //NOPE _elevatorControllerReception._elevator1working = _elevator0Working;
            //NOPE _elevatorControllerReception._elevator2working = _elevator1Working;
            //NOPE _elevatorControllerReception._elevator3working = _elevator2Working;
            //NOPE _elevatorControllerReception.SetupElevatorStates();
            Debug.Print("[NetworkController] Random elevator states for reception are now set by localPlayer");
        }
        /// <summary>
        /// is called when network packets are received (only happens when there are more players except Master in the scene
        /// </summary>
        public void OnDeserialization()
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
                if (time.GetTime() < 1f) //no scene setup before at least 1 second has passed to ensure the update loop has already started
                    return;
                Debug.Print("[NetworkController] Local setup was started");
                if (0L != (_syncData2 & (SyncBool_MaskLong2_Initialized)))
                {
                    LOCAL_ReadConstSceneElevatorStates();
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
                Debug.Print("[NetworkController] LocalPlayer received to open elevator " + elevatorNumber + " on floor " + floorNumber);
                if (floorNumber == _localPlayerCurrentFloor)
                {
                    //Passes elevatorNumber, (if going up), (if idle)
                    _elevatorControllerReception.OpenElevatorLocalPlayerFloor(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
                    //Debug.Print("[NetworkController] Elevator " + elevatorNumber + " is currently at floor " + floorNumber + " so Reception won't open.");
                }
                else if (floorNumber == 0)
                {
                    //Passes elevatorNumber, (if going up), (if idle)
                    _elevatorControllerReception.OpenElevatorReception(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
                }
                //TODO: make sure to store the floor states for people driving to that floor later
            }
            else
            {
                Debug.Print("[NetworkController] LocalPlayer received to close elevator " + elevatorNumber + " on floor " + floorNumber);
                if (floorNumber == _localPlayerCurrentFloor)
                {
                    _elevatorControllerReception.CloseElevatorLocalPlayerFloor(elevatorNumber);
                    //Debug.Print("[NetworkController] Elevator " + elevatorNumber + " is currently at floor " + floorNumber + " so Reception won't close.");
                }
                else if (floorNumber == 0)
                {
                    _elevatorControllerReception.CloseElevatorReception(elevatorNumber);
                }
                //TODO: make sure to store the floor states for people driving to that floor later
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
                Debug.Print("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " IDLE=" + isIdle.ToString() + ", but it isn't open");
                return;
            }
            int floorNumber = GetSyncElevatorFloor(elevatorNumber);
            Debug.Print("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " IDLE=" + isIdle.ToString() + " on floor " + floorNumber);
            if (floorNumber != 0)
            {
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " is currently at floor " + floorNumber + " so Reception won't change IDLE state.");
            }
            else
            {
                //Passes elevatorNumber, (if going up), isIdle
                _elevatorControllerReception.SetElevatorDirectionDisplay(elevatorNumber, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), isIdle);
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
                Debug.Print("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " GoingUp=" + goingUp.ToString() + ", but it isn't open");
                return;
            }
            int floorNumber = GetSyncElevatorFloor(elevatorNumber);
            Debug.Print("[NetworkController] LocalPlayer received to set elevator " + elevatorNumber + " GoingUp=" + goingUp.ToString() + " on floor " + floorNumber);
            if (floorNumber != 0)
            {
                Debug.Print("[NetworkController] Elevator " + elevatorNumber + " is currently at floor " + floorNumber + " so Reception won't change GoingUp state.");
            }
            else
            {
                //Passes elevatorNumber, goingUp, (if idle)
                _elevatorControllerReception.SetElevatorDirectionDisplay(elevatorNumber, goingUp, 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
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
                //Debug.Print("There is " + _pendingCallUp_COUNT + " pending call up.");
                for (int floor = 0; floor <= 13; floor++)
                {
                    //Check if there is a pending request
                    if (_pendingCallUp_LOCAL_EXT[floor] && time.GetTime() - _pendingCallTimeUp_LOCAL_EXT[floor] > 1.5f)
                    {
                        _pendingCallUp_LOCAL_EXT[floor] = false;
                        _pendingCallUp_COUNT_LOCAL_EXT--;
                        
                        //if NOT called up to floor X
                        if (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + floor))))
                        {
                            //TODO: link all elevator controllers here in Unity later
                            if (floor == 0)
                            {
                                Debug.Print("Dropped request, SetElevatorNotCalledUp() floor " + floor + " after " + (time.GetTime() - _pendingCallTimeUp_LOCAL_EXT[floor]).ToString() + " seconds.");
                                _elevatorControllerReception.SetElevatorNotCalledUp(floor);
                            }
                            else
                            {
                                Debug.Print("Dropped request, SetElevatorNotCalledUp() floor " + floor + " after " + (time.GetTime() - _pendingCallTimeUp_LOCAL_EXT[floor]).ToString() + " seconds.");
                                _elevatorControllerReception.SetElevatorNotCalledUp(floor);
                            }
                        }
                    }
                }
            }
            if (_pendingCallDown_COUNT_LOCAL_EXT != 0)
            {
                //Debug.Print("There is " + _pendingCallUp_COUNT + " pending call down.");
                for (int floor = 0; floor <= 13; floor++)
                {
                    if (_pendingCallDown_LOCAL_EXT[floor] && time.GetTime() - _pendingCallTimeDown_LOCAL_EXT[floor] > 1.5f)
                    {
                        _pendingCallDown_LOCAL_EXT[floor] = false;
                        _pendingCallDown_COUNT_LOCAL_EXT--;

                        //if NOT called down to floor X
                        if (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + floor))))
                        {
                            //TODO: link all elevator controllers here in Unity later
                            if (floor == 0)
                            {
                                Debug.Print("Dropped request, SetElevatorNotCalledDown() floor " + floor + " after " + (time.GetTime() - _pendingCallTimeDown_LOCAL_EXT[floor]).ToString() + " seconds.");
                                _elevatorControllerReception.SetElevatorNotCalledDown(floor);
                            }
                            else
                            {
                                Debug.Print("Dropped request, SetElevatorNotCalledDown() floor " + floor + " after " + (time.GetTime() - _pendingCallTimeDown_LOCAL_EXT[floor]).ToString() + " seconds.");
                                _elevatorControllerReception.SetElevatorNotCalledDown(floor);
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
            if (_pendingCallElevator1_COUNT_LOCAL_INT != 0)
            {
                //Debug.Print("There is " + _pendingCallElevator1_COUNT + " pending call internally.");
                for (int floor = 0; floor <= 13; floor++)
                {
                    if (_pendingCallElevator1_LOCAL_INT[floor] && time.GetTime() - _pendingCallElevator1Time_LOCAL_INT[floor] > 1.5f)
                    {
                        _pendingCallElevator1_LOCAL_INT[floor] = false;
                        _pendingCallElevator1_COUNT_LOCAL_INT--;

                        //if NOT elevator1 called to floor X
                        if (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator1CalledToFloor + floor))))
                        {
                            Debug.Print("Dropped request, SetElevatorInternalButtonState() button " + floor + " after " + (time.GetTime() - _pendingCallElevator1Time_LOCAL_INT[floor]).ToString() + " seconds.");
                            LOCAL_SetElevatorInternalButtonState(0, floor, called: false);
                        }
                    }
                }
            }
            if (_pendingCallElevator0_COUNT_LOCAL_INT != 0)
            {
                //Debug.Print("There is " + _pendingCallElevator0_COUNT + " pending call internally.");
                for (int floor = 0; floor <= 13; floor++)
                {
                    if (_pendingCallElevator0_LOCAL_INT[floor] && time.GetTime() - _pendingCallElevator0Time_LOCAL_INT[floor] > 1.5f)
                    {
                        _pendingCallElevator0_LOCAL_INT[floor] = false;
                        _pendingCallElevator0_COUNT_LOCAL_INT--;
                        if (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_Elevator0CalledToFloor + floor))))
                        {
                            Debug.Print("Dropped request, SetElevatorInternalButtonState() button " + floor + " after " + (time.GetTime() - _pendingCallElevator0Time_LOCAL_INT[floor]).ToString() + " seconds.");
                            LOCAL_SetElevatorInternalButtonState(0, floor, called: false);
                        }
                    }
                }
            }
            if (_pendingCallElevator2_COUNT_LOCAL_INT != 0)
            {
                //Debug.Print("There is " + _pendingCallElevator2_COUNT + " pending call internally.");
                for (int floor = 0; floor <= 13; floor++)
                {
                    if (_pendingCallElevator2_LOCAL_INT[floor] && time.GetTime() - _pendingCallElevator2Time_LOCAL_INT[floor] > 1.5f)
                    {
                        _pendingCallElevator2_LOCAL_INT[floor] = false;
                        _pendingCallElevator2_COUNT_LOCAL_INT--;
                        //if NOT elevator0 called to floor X
                        if (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator2CalledToFloor + floor))))
                        {
                            Debug.Print("Dropped request, SetElevatorInternalButtonState() button " + floor + " after " + (time.GetTime() - _pendingCallElevator2Time_LOCAL_INT[floor]).ToString() + " seconds.");
                            LOCAL_SetElevatorInternalButtonState(0, floor, called: false);
                        }
                    }
                }
            }
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
                Debug.Print("[NetworkController] Elevator called to floor " + floorNumber + " by localPlayer (Up)");
                //if something with an array OR Elevator called up on floor X
                if (_pendingCallUp_LOCAL_EXT[floorNumber] || 0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledUp + floorNumber))))
                    return;
                _pendingCallUp_LOCAL_EXT[floorNumber] = true;
                _pendingCallTimeUp_LOCAL_EXT[floorNumber] = time.GetTime();
                _pendingCallUp_COUNT_LOCAL_EXT++;
                _elevatorRequester.RequestElevatorFloorButton(directionUp, floorNumber);
            }
            else
            {
                Debug.Print("[NetworkController] Elevator called to floor " + floorNumber + " by localPlayer (Down)");
                //if something with an array OR Elevator called down on floor X
                if (_pendingCallDown_LOCAL_EXT[floorNumber] || 0L != (_syncData1 & (1L << (SyncBoolReq_AddressLong1_ElevatorCalledDown + floorNumber))))
                    return;
                _pendingCallDown_LOCAL_EXT[floorNumber] = true;
                _pendingCallTimeDown_LOCAL_EXT[floorNumber] = time.GetTime();
                _pendingCallDown_COUNT_LOCAL_EXT++;
                _elevatorRequester.RequestElevatorFloorButton(directionUp, floorNumber);
            }
        }
        /// <summary>
        /// When localPlayer pressed a button INSIDE the elevator
        /// </summary>
        public void API_LocalPlayerPressedElevatorButton(int elevatorNumber, int buttonNumber)
        {
            Debug.Print($"[NetworkController] LocalPlayer pressed button {buttonNumber} in elevator {elevatorNumber}");
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
            int floorNumber = buttonNumber - 4;
            switch (elevatorNumber)
            {
                case 0:
                    _pendingCallElevator0_LOCAL_INT[floorNumber] = true; ;
                    _pendingCallElevator0_COUNT_LOCAL_INT++;
                    _pendingCallElevator0Time_LOCAL_INT[floorNumber] = time.GetTime();
                    break;
                case 1:
                    _pendingCallElevator1_LOCAL_INT[floorNumber] = true; ;
                    _pendingCallElevator1_COUNT_LOCAL_INT++;
                    _pendingCallElevator1Time_LOCAL_INT[floorNumber] = time.GetTime();
                    break;
                case 2:
                    _pendingCallElevator2_LOCAL_INT[floorNumber] = true; ;
                    _pendingCallElevator2_COUNT_LOCAL_INT++;
                    _pendingCallElevator2Time_LOCAL_INT[floorNumber] = time.GetTime();
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
            Debug.Print("[NetworkingController] Master received Elevator called to floor " + floor + " by localPlayer (DirectionUp: " + directionUp.ToString() + ")");
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
            float test = time.GetTime() - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber];
            Debug.Print("Master received CallToChangeDoorState for elevator " + elevatorNumber + " (Direction open: " + open.ToString() + ") Elevator driving:" + (0L != (_syncData2 & (1L << (SyncBool_AddressLong2_ElevatorXIsDriving + elevatorNumber)))));
            
            //if (open AND elevator X idle) OR (some timing stuff AND NOT driving)
            if (open && 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))) || (time.GetTime() - _timeAtCurrentFloorElevatorClosed_MASTER[elevatorNumber] < 2.5f && (0L == (_syncData2 & (1L << (SyncBool_AddressLong2_ElevatorXIsDriving + elevatorNumber))))))
            {
                MASTER_HandleFloorDoorOpening(elevatorNumber, GetSyncElevatorFloor(elevatorNumber), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXgoingUp + elevatorNumber))), 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXidle + elevatorNumber))));
            }
            //if NOT open AND elevator X idle AND some timing stuff
            else if (!open && 0L != (_syncData1 & (1L << (SyncBool_AddressLong1_ElevatorXopen + elevatorNumber))) && time.GetTime() - _timeAtCurrentFloorElevatorOpened_MASTER[elevatorNumber] > 6f)
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

            Debug.Print("[NetworkController] Master received client request to set target for elevator " + elevatorNumber + " to floor " + floorNumber);
            //if elevatorNumber0 AND NOT elevator0 called to floor X
            if (elevatorNumber == 0 && (0L == (_syncData1 & (1L << (SyncBoolReq_AddressLong1_Elevator0CalledToFloor + floorNumber)))))
            {
                Debug.Print("Internal target was now set.");
                MASTER_SetSyncValue(SyncBoolReq_Elevator0CalledToFloor_0 + floorNumber, true);
                _elevator0FloorTargets_MASTER[floorNumber] = true;
                _elevator0FloorTargets_MASTER_COUNT++;
                return;
            }
            //if elevatorNumber1 AND NOT elevator1 called to floor X
            else if (elevatorNumber == 1 && (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator1CalledToFloor + floorNumber)))))
            {
                Debug.Print("Internal target was now set.");
                MASTER_SetSyncValue(SyncBoolReq_Elevator1CalledToFloor_0 + floorNumber, true);
                _elevator1FloorTargets_MASTER[floorNumber] = true;
                _elevator1FloorTargets_MASTER_COUNT++;
                return;
            }
            //if elevatorNumber2 AND NOT elevator2 called to floor X
            else if (elevatorNumber == 2 && (0L == (_syncData2 & (1L << (SyncBoolReq_AddressLong2_Elevator2CalledToFloor + floorNumber)))))
            {
                Debug.Print("Internal target was now set.");
                MASTER_SetSyncValue(SyncBoolReq_Elevator2CalledToFloor_0 + floorNumber, true);
                _elevator2FloorTargets_MASTER[floorNumber] = true;
                _elevator2FloorTargets_MASTER_COUNT++;
                return;
            }
            Debug.Print("No target was set since the elevator is already called to that floor");
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
        /// This script sets and reads individual bits within a uint as well as encoding three numbers (nibbles) within the most significant bytes
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
        /// Modifies a _syncData1 & _syncData2 on the bit level.
        /// Sets "value" to bit "position" of "input".
        /// </summary>       
        /// <param name="input">uint to modify</param>
        /// <param name="position">Bit position to modify (0-115)</param>
        /// <param name="value">Value to set the bit</param>        
        /// <returns>Returns the modified uint</returns>
        private void MASTER_SetSyncValue(int position, bool value)
        {
            Debug.Print($"SYNC DATA bool {position} set to {value.ToString()}");
            //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
            long locallong1 = _syncData1;
            long locallong2 = _syncData2;

            //Sanitise position
            if (position < 0 || position > 115)
            {
                //TODO: remove on live build
                Debug.Print("uintConverter - Position out of range");
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
        /// <param name="position">Bit position to read (0-115)</param>
        /// <returns>Boolean of specified bit position. Returns false on error.</returns>
        private bool GetSyncValue(int position)
        {
            //Sanitise position
            if (position < 0 || position > 115)
            {
                //TODO: remove on live build
                Debug.Print("uintConverter - Position out of range");
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
            output[115] = (_syncData2 >> 63) != 0L;

            return output;
        }

        /// <summary>
        /// Reads out all the long1 booleans at once (preserving mapping compared to direct access)
        /// </summary>               
        /// <returns>Returns all the bools within the long1</returns>
        private bool[] GetBoolArraylong1ONLY()
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
        private bool[] GetBoolArraylong2ONLY()
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
            output[115] = (_syncData2 >> 63) != 0L;

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
            Debug.Print($"SYNC DATA elevator {elevatorNumber} floor setting to {floorNumber}");
            //Not sure if there is something multi-threaded going on in the background, so creating working copies just in case.
            long locallong = _syncData1;
            //Debug.Print($"SYNC DATA_1 was {locallong}");
            //Sanitise the size of elevatorNumber
            if (elevatorNumber < 0 || elevatorNumber > 2)
            {
                //TODO: remove on live build
                Debug.Print($"uintConverter - 404 Elevator {elevatorNumber} does not exist");
                return;
            }

            //sanitise floorNumber
            if (floorNumber < 0 || floorNumber > 15)
            {
                //TODO: remove on live build
                Debug.Print($"uintConverter - Elevator  {elevatorNumber} number invalid");
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
            //Debug.Print($"SYNC DATA_1 is now {locallong}");
        }

        /// <summary>
        /// Decodes and returns the floor number of the long
        /// </summary>              
        /// <param name="elevatorNumber">Number of the elevator 1-3</param>        
        /// <returns>Returns the floorNumber from the uint</returns>
        private int GetSyncElevatorFloor(int elevatorNumber)
        {
            //Debug.Print($"SYNC DATA_1 is now {_syncData1}");
            //Sanitise the size of elevatorNumber
            if (elevatorNumber < 0 || elevatorNumber > 2)
            {
                //TODO: remove on live build if needed
                Debug.Print($"uintConverter - 404 Elevator  {elevatorNumber} does not exist");
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
    }
    #endregion SYNCBOOL_FUNCTIONS
    //----------------------------------------------------------------------------------------------------------------
    //--------------------------------------- Emulator classes -------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    #region EMULATOR_FUNCTIONS
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
    /// Simplified emulator of our elevatorController which itself has several thousand lines of code, this here is very high-level
    /// </summary>
    public class ElevatorController
    {
        public ElevatorEmulator form1;

        internal void CloseElevatorReception(int elevatorNumber)
        {
            form1.CloseElevatorReception(elevatorNumber);
        }

        internal void SetElevatorDirectionDisplay(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            form1.SetElevatorDirectionDisplay(elevatorNumber, isGoingUp, isIdle);
        }

        internal void OpenElevatorReception(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            form1.OpenElevatorReception(elevatorNumber, isGoingUp, isIdle);
        }

        internal void DisplayLocalPlayerFloor(int floor)
        {
            form1.DisplayLocalPlayerFloor(floor);
        }
        internal void OpenElevatorLocalPlayerFloor(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            form1.OpenElevatorLocalPlayerFloor(elevatorNumber, isGoingUp, isIdle);
        }
        internal void CloseElevatorLocalPlayerFloor(int elevatorNumber)
        {
            form1.CloseElevatorLocalPlayerFloor(elevatorNumber);
        }

        internal void SetElevatorFloorButtonState(int elevatorNumber, int floorNumber, bool called)
        {
            form1.SetElevatorButtonColor(called, floorNumber + 4, elevatorNumber);
            //Debug.Print("Elevator " + elevatorNumber + " would change floor " + floorNumber + " button to called:" + called.ToString() + ", but this function isn't implemented yet.");
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
        internal void SetElevatorCalledDown(int floor)
        {
            form1.SetElevatorCalledDown(floor);
        }
        internal void SetElevatorCalledUp(int floor)
        {
            form1.SetElevatorCalledUp(floor);
        }

        internal void SetElevatorDrivingState(int elevatorNumber, bool isDriving)
        {
            //not needed, just for Udon version
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
    #endregion EMULATOR_FUNCTIONS
    //----------------------------------------------------------------------------------------------------------------
    //------------------- GUI Class ----------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    #region GUI_class
    /// <summary>
    /// This is the GUI component section which controls the buttons and textbox components
    /// It's not part of the Unity build and thus is totally irrelevant outside of providing a quick debug GUI
    /// </summary>
    public partial class ElevatorEmulator : Form
    {
        NetworkingController _controller;
        /// <summary>
        /// The intervall in which the <see cref="_controller"/> performs an Update() loop
        /// </summary>
        private const int _tickTimeInMilliseconds = 1;
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
            if (isPause)
                return;
            _controller.Update();
            SetElevatorCurrentPositionsInGui();
        }
        //------------------------------------------- GUI interface ------------------------------------
        internal void SetElevatorCurrentPositionsInGui()
        {
            panelFloor0.BackColor = Color.DarkGreen;
            panelFloor1.BackColor = Color.DarkGreen;
            panelFloor2.BackColor = Color.DarkGreen;
            panelFloor3.BackColor = Color.DarkGreen;
            panelFloor4.BackColor = Color.DarkGreen;
            panelFloor5.BackColor = Color.DarkGreen;
            panelFloor6.BackColor = Color.DarkGreen;
            panelFloor7.BackColor = Color.DarkGreen;
            panelFloor8.BackColor = Color.DarkGreen;
            panelFloor9.BackColor = Color.DarkGreen;
            panelFloor10.BackColor = Color.DarkGreen;
            panelFloor11.BackColor = Color.DarkGreen;
            panelFloor12.BackColor = Color.DarkGreen;
            panelFloor13.BackColor = Color.DarkGreen;
            if (textBoxElevator1State.Text.Contains("OPEN"))
            {
                SetFloorColor(positionElevator0, Color.OrangeRed);
                textBoxElevator1State.BackColor = Color.OrangeRed;
            }
            else if (textBoxElevator1OpenReception.Text != "BROKEN")
            {
                SetFloorColor(positionElevator0, Color.Purple);
                textBoxElevator1State.BackColor = Color.Purple;
            }
            else if (textBoxElevator1OpenReception.Text == "BROKEN")
            {
                textBoxElevator1State.BackColor = Color.Red;
            }

            if (textBoxElevator2State.Text.Contains("OPEN"))
            {
                SetFloorColor(positionElevator1, Color.OrangeRed);
                textBoxElevator2State.BackColor = Color.OrangeRed;
            }
            else if (textBoxElevator2OpenReception.Text != "BROKEN")
            {
                SetFloorColor(positionElevator1, Color.Purple);
                textBoxElevator2State.BackColor = Color.Purple;
            }
            else if (textBoxElevator2OpenReception.Text == "BROKEN")
            {
                textBoxElevator2State.BackColor = Color.Red;
            }

            if (textBoxElevator3State.Text.Contains("OPEN"))
            {
                SetFloorColor(positionElevator2, Color.OrangeRed);
                textBoxElevator3State.BackColor = Color.OrangeRed;
            }
            else if (textBoxElevator3OpenReception.Text != "BROKEN")
            {
                SetFloorColor(positionElevator2, Color.Purple);
                textBoxElevator3State.BackColor = Color.Purple;
            }
            else if (textBoxElevator3OpenReception.Text == "BROKEN")
            {
                textBoxElevator3State.BackColor = Color.Red;
            }
        }
        private void SetFloorColor(int floor, Color color)
        {
            switch (floor)
            {
                case 0:
                    panelFloor0.BackColor = color;
                    break;
                case 1:
                    panelFloor1.BackColor = color;
                    break;
                case 2:
                    panelFloor2.BackColor = color;
                    break;
                case 3:
                    panelFloor3.BackColor = color;
                    break;
                case 4:
                    panelFloor4.BackColor = color;
                    break;
                case 5:
                    panelFloor5.BackColor = color;
                    break;
                case 6:
                    panelFloor6.BackColor = color;
                    break;
                case 7:
                    panelFloor7.BackColor = color;
                    break;
                case 8:
                    panelFloor8.BackColor = color;
                    break;
                case 9:
                    panelFloor9.BackColor = color;
                    break;
                case 10:
                    panelFloor10.BackColor = color;
                    break;
                case 11:
                    panelFloor11.BackColor = color;
                    break;
                case 12:
                    panelFloor12.BackColor = color;
                    break;
                case 13:
                    panelFloor13.BackColor = color;
                    break;
            }
        }
        private int positionElevator0 = 4;
        private int positionElevator1 = 4;
        private int positionElevator2 = 4;
        internal void SetElevatorLevelOnDisplay(int floorNumber, int elevator)
        {
            switch (elevator)
            {
                case 0:
                    elevator1.Text = floorNumber.ToString();
                    positionElevator0 = floorNumber;
                    break;
                case 1:
                    elevator2.Text = floorNumber.ToString();
                    positionElevator1 = floorNumber;
                    break;
                case 2:
                    elevator3.Text = floorNumber.ToString();
                    positionElevator2 = floorNumber;
                    break;
            }
        }
        internal void SetElevatorCalledDown(int floor)
        {
            Debug.Print("SetElevatorCalledDown " + floor);
            SetButtonColor(false, true, floor);
        }
        internal void SetElevatorCalledUp(int floor)
        {
            Debug.Print("SetElevatorCalledUp " + floor);
            SetButtonColor(true, true, floor);
        }
        internal void SetElevatorNotCalledDown(int floor)
        {
            Debug.Print("SetElevatorNotCalledDown " + floor);
            SetButtonColor(false, false, floor);
        }
        internal void SetElevatorNotCalledUp(int floor)
        {
            Debug.Print("SetElevatorNotCalledUp " + floor);
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
            Button button0;
            Button button1;
            Button button2;
            Button button3;
            Button button4;
            Button button5;
            Button button6;
            Button button7;
            Button button8;
            Button button9;
            Button button10;
            Button button11;
            Button button12;
            Button button13;
            if (directionUp)
            {
                button0 = buttonCallUp_0;
                button1 = buttonCallUp_1;
                button2 = buttonCallUp_2;
                button3 = buttonCallUp_3;
                button4 = buttonCallUp_4;
                button5 = buttonCallUp_5;
                button6 = buttonCallUp_6;
                button7 = buttonCallUp_7;
                button8 = buttonCallUp_8;
                button9 = buttonCallUp_9;
                button10 = buttonCallUp_10;
                button11 = buttonCallUp_11;
                button12 = buttonCallUp_12;
                button13 = buttonCallUp_13;
            }
            else
            {
                button0 = buttonCallDown_0;
                button1 = buttonCallDown_1;
                button2 = buttonCallDown_2;
                button3 = buttonCallDown_3;
                button4 = buttonCallDown_4;
                button5 = buttonCallDown_5;
                button6 = buttonCallDown_6;
                button7 = buttonCallDown_7;
                button8 = buttonCallDown_8;
                button9 = buttonCallDown_9;
                button10 = buttonCallDown_10;
                button11 = buttonCallDown_11;
                button12 = buttonCallDown_12;
                button13 = buttonCallDown_13;
            }
            switch (floor)
            {
                case 0:
                    button0.BackColor = color;
                    break;
                case 1:
                    button1.BackColor = color;
                    break;
                case 2:
                    button2.BackColor = color;
                    break;
                case 3:
                    button3.BackColor = color;
                    break;
                case 4:
                    button4.BackColor = color;
                    break;
                case 5:
                    button5.BackColor = color;
                    break;
                case 6:
                    button6.BackColor = color;
                    break;
                case 7:
                    button7.BackColor = color;
                    break;
                case 8:
                    button8.BackColor = color;
                    break;
                case 9:
                    button9.BackColor = color;
                    break;
                case 10:
                    button10.BackColor = color;
                    break;
                case 11:
                    button11.BackColor = color;
                    break;
                case 12:
                    button12.BackColor = color;
                    break;
                case 13:
                    button13.BackColor = color;
                    break;
            }
        }
        /// <summary>
        /// Setting the elevator direction of an open elevator
        /// </summary>
        internal void SetElevatorDirectionDisplay(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            DisplayElevatorDoorState(isIdle, isGoingUp, elevatorNumber, textBoxElevator1OpenReception, textBoxElevator2OpenReception, textBoxElevator3OpenReception, true);
        }
        /// <summary>
        /// Opening elevators on floor 0 / reception
        /// </summary>
        internal void OpenElevatorReception(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            DisplayElevatorDoorState(isIdle, isGoingUp, elevatorNumber, textBoxElevator1OpenReception, textBoxElevator2OpenReception, textBoxElevator3OpenReception, true);
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
                    textBoxElevator1OpenReception.BackColor = Color.Purple;
                    break;
                case 1:
                    textBoxElevator2OpenReception.Text = "CLOSED";
                    textBoxElevator2OpenReception.BackColor = Color.Purple;
                    break;
                case 2:
                    textBoxElevator3OpenReception.Text = "CLOSED";
                    textBoxElevator3OpenReception.BackColor = Color.Purple;
                    break;
            }
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
                    if (isOpen)
                        texBox1.BackColor = Color.OrangeRed;
                    if (!isOpen)
                        texBox1.BackColor = Color.Purple;
                    break;
                case 1:
                    texBox2.Text = $"{state}";
                    if (isOpen)
                        texBox2.BackColor = Color.OrangeRed;
                    if (!isOpen)
                        texBox2.BackColor = Color.Purple;
                    break;
                case 2:
                    texBox3.Text = $"{state}";
                    if (isOpen)
                        texBox3.BackColor = Color.OrangeRed;
                    if (!isOpen)
                        texBox3.BackColor = Color.Purple;
                    break;
            }
        }
        /// <summary>
        /// Interface to display the general elevator state for debugging purposes
        /// </summary>
        internal void DisplayElevatorState(int elevatorNumber, bool isGoingUp, bool isIdle, bool isOpen)
        {
            DisplayElevatorDoorState(isIdle, isGoingUp, elevatorNumber, textBoxElevator1State, textBoxElevator2State, textBoxElevator3State, isOpen);
        }
        internal void DisplayElevatorBroken(bool elevator0Working, bool elevator1Working, bool elevator2Working)
        {
            if (!elevator0Working)
            {
                textBoxElevator1OpenReception.Text = "BROKEN";
                textBoxElevator1OpenReception.BackColor = Color.Red;
            }
            if (!elevator1Working)
            {
                textBoxElevator2OpenReception.Text = "BROKEN";
                textBoxElevator2OpenReception.BackColor = Color.Red;
            }
            if (!elevator2Working)
            {
                textBoxElevator3OpenReception.Text = "BROKEN";
                textBoxElevator3OpenReception.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// Setting a button bg color inside the elevator
        /// </summary>
        public void SetElevatorButtonColor(bool stateCalled, int buttonNumber, int elevatorNumber)
        {
            Debug.Print("[UI] Set internal button " + buttonNumber + " called:" + stateCalled.ToString() + " on elevator " + elevatorNumber);
            Color color;
            if (stateCalled)
            {
                color = Color.Red;
            }
            else
            {
                color = Color.LightGray;
            }
            Button elevatorButton0;
            Button elevatorButton1;
            Button elevatorButton4;
            Button elevatorButton5;
            Button elevatorButton6;
            Button elevatorButton7;
            Button elevatorButton8;
            Button elevatorButton9;
            Button elevatorButton10;
            Button elevatorButton11;
            Button elevatorButton12;
            Button elevatorButton13;
            Button elevatorButton14;
            Button elevatorButton15;
            Button elevatorButton16;
            Button elevatorButton17;
            switch (elevatorNumber)
            {
                case 0:
                    elevatorButton0 = button0;
                    elevatorButton1 = button1;
                    elevatorButton4 = button4;
                    elevatorButton5 = button5;
                    elevatorButton6 = button6;
                    elevatorButton7 = button7;
                    elevatorButton8 = button8;
                    elevatorButton9 = button9;
                    elevatorButton10 = button10;
                    elevatorButton11 = button11;
                    elevatorButton12 = button12;
                    elevatorButton13 = button13;
                    elevatorButton14 = button14;
                    elevatorButton15 = button15;
                    elevatorButton16 = button16;
                    elevatorButton17 = button17;
                    break;
                case 1:
                    elevatorButton0 = button0_1;
                    elevatorButton1 = button1_1;
                    elevatorButton4 = button4_1;
                    elevatorButton5 = button5_1;
                    elevatorButton6 = button6_1;
                    elevatorButton7 = button7_1;
                    elevatorButton8 = button8_1;
                    elevatorButton9 = button9_1;
                    elevatorButton10 = button10_1;
                    elevatorButton11 = button11_1;
                    elevatorButton12 = button12_1;
                    elevatorButton13 = button13_1;
                    elevatorButton14 = button14_1;
                    elevatorButton15 = button15_1;
                    elevatorButton16 = button16_1;
                    elevatorButton17 = button17_1;
                    break;
                default:
                    elevatorButton0 = button0_2;
                    elevatorButton1 = button1_2;
                    elevatorButton4 = button4_2;
                    elevatorButton5 = button5_2;
                    elevatorButton6 = button6_2;
                    elevatorButton7 = button7_2;
                    elevatorButton8 = button8_2;
                    elevatorButton9 = button9_2;
                    elevatorButton10 = button10_2;
                    elevatorButton11 = button11_2;
                    elevatorButton12 = button12_2;
                    elevatorButton13 = button13_2;
                    elevatorButton14 = button14_2;
                    elevatorButton15 = button15_2;
                    elevatorButton16 = button16_2;
                    elevatorButton17 = button17_2;
                    break;
            }
            switch (buttonNumber)
            {
                case 0:
                    elevatorButton0.BackColor = color;
                    break;
                case 1:
                    elevatorButton1.BackColor = color;
                    break;
                case 4:
                    elevatorButton4.BackColor = color;
                    break;
                case 5:
                    elevatorButton5.BackColor = color;
                    break;
                case 6:
                    elevatorButton6.BackColor = color;
                    break;
                case 7:
                    elevatorButton7.BackColor = color;
                    break;
                case 8:
                    elevatorButton8.BackColor = color;
                    break;
                case 9:
                    elevatorButton9.BackColor = color;
                    break;
                case 10:
                    elevatorButton10.BackColor = color;
                    break;
                case 11:
                    elevatorButton11.BackColor = color;
                    break;
                case 12:
                    elevatorButton12.BackColor = color;
                    break;
                case 13:
                    elevatorButton13.BackColor = color;
                    break;
                case 14:
                    elevatorButton14.BackColor = color;
                    break;
                case 15:
                    elevatorButton15.BackColor = color;
                    break;
                case 16:
                    elevatorButton16.BackColor = color;
                    break;
                case 17:
                    elevatorButton17.BackColor = color;
                    break;
            }
        }
        #region internalElevatorButtons
        //internal elevator buttons
        private void button1_Click(object sender, EventArgs e)
        {
            //close
            //SetElevatorButtonColor(stateCalled: true, buttonNumber: 1);
            _controller.API_LocalPlayerPressedElevatorButton(0, 1);
        }
        private void button0_Click(object sender, EventArgs e)
        {
            //open
            //SetElevatorButtonColor(stateCalled: true, buttonNumber: 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 0);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            //floor 0
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 4, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 4);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            // floor 1
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 5, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 5);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            //2
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 6, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 6);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            //3
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 7, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 7);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            //4
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 8, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 8);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            //5
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 9, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 9);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            //6
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 10, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 10);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            //7
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 11, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 11);
        }
        private void button12_Click(object sender, EventArgs e)
        {
            //8
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 12, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 12);
        }
        private void button13_Click(object sender, EventArgs e)
        {
            //9
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 13, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 13);
        }
        private void button14_Click(object sender, EventArgs e)
        {
            //10
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 14, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 14);
        }
        private void button15_Click(object sender, EventArgs e)
        {
            //11
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 15, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 15);
        }
        private void button16_Click(object sender, EventArgs e)
        {
            //12
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 16, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 16);
        }
        private void button17_Click(object sender, EventArgs e)
        {
            //13
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 17, 0);
            _controller.API_LocalPlayerPressedElevatorButton(0, 17);
        }
        #endregion internalElevatorButtons
        #region ExternalCallButtons
        private void buttonCallDown_0_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 0);
            _controller.API_LocalPlayerPressedCallButton(0, false);
        }
        private void buttonCallUp_0_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 0);
            _controller.API_LocalPlayerPressedCallButton(0, true);
        }
        private void buttonCallUp_1_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 1);
            _controller.API_LocalPlayerPressedCallButton(1, true);
        }
        private void buttonCallDown_1_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 1);
            _controller.API_LocalPlayerPressedCallButton(1, false);
        }
        private void buttonCallUp_2_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 2);
            _controller.API_LocalPlayerPressedCallButton(2, true);
        }
        private void buttonCallDown_2_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 2);
            _controller.API_LocalPlayerPressedCallButton(2, false);
        }
        private void buttonCallUp_3_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 3);
            _controller.API_LocalPlayerPressedCallButton(3, true);
        }
        private void buttonCallDown_3_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 3);
            _controller.API_LocalPlayerPressedCallButton(3, false);
        }
        private void buttonCallDown_4_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 4);
            _controller.API_LocalPlayerPressedCallButton(4, false);
        }
        private void buttonCallUp_4_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 4);
            _controller.API_LocalPlayerPressedCallButton(4, true);
        }
        private void buttonCallDown_5_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 5);
            _controller.API_LocalPlayerPressedCallButton(5, false);
        }
        private void buttonCallUp_5_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 5);
            _controller.API_LocalPlayerPressedCallButton(5, true);
        }
        private void buttonCallUp_6_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 6);
            _controller.API_LocalPlayerPressedCallButton(6, true);
        }
        private void buttonCallDown_6_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 6);
            _controller.API_LocalPlayerPressedCallButton(6, false);
        }
        private void buttonCallUp_7_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 7);
            _controller.API_LocalPlayerPressedCallButton(7, true);
        }
        private void buttonCallDown_7_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 7);
            _controller.API_LocalPlayerPressedCallButton(7, false);
        }
        private void buttonCallUp_8_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 8);
            _controller.API_LocalPlayerPressedCallButton(8, true);
        }
        private void buttonCallDown_8_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 8);
            _controller.API_LocalPlayerPressedCallButton(8, false);
        }
        private void buttonCallUp_9_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 9);
            _controller.API_LocalPlayerPressedCallButton(9, true);
        }
        private void buttonCallDown_9_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 9);
            _controller.API_LocalPlayerPressedCallButton(9, false);
        }
        private void buttonCallUp_10_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 10);
            _controller.API_LocalPlayerPressedCallButton(10, true);
        }
        private void buttonCallDown_10_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 10);
            _controller.API_LocalPlayerPressedCallButton(10, false);
        }
        private void buttonCallUp_11_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 11);
            _controller.API_LocalPlayerPressedCallButton(11, true);
        }
        private void buttonCallDown_11_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 11);
            _controller.API_LocalPlayerPressedCallButton(11, false);
        }
        private void buttonCallUp_12_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 12);
            _controller.API_LocalPlayerPressedCallButton(12, true);
        }
        private void buttonCallDown_12_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 12);
            _controller.API_LocalPlayerPressedCallButton(12, false);
        }
        private void buttonCallDown_13_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: false, stateCalled: true, floor: 13);
            _controller.API_LocalPlayerPressedCallButton(13, false);
        }
        private void buttonCallUp_13_Click(object sender, EventArgs e)
        {
            SetButtonColor(directionUp: true, stateCalled: true, floor: 13);
            _controller.API_LocalPlayerPressedCallButton(13, true);
        }
        #endregion ExternalCallButtons
        #region InternalButtonsElevator1
        private void button0_1_Click(object sender, EventArgs e)
        {
            //SetElevatorButtonColor(stateCalled: true, buttonNumber: 0, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 0);
        }
        private void button1_1_Click(object sender, EventArgs e)
        {
            //SetElevatorButtonColor(stateCalled: true, buttonNumber: 1, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 1);
        }
        private void button4_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 4, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 4);
        }
        private void button5_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 5, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 5);
        }
        private void button6_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 6, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 6);
        }
        private void button7_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 7, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 7);
        }
        private void button8_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 8, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 8);
        }
        private void button9_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 9, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 9);
        }
        private void button10_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 10, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 10);
        }
        private void button11_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 11, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 11);
        }
        private void button12_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 12, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 12);
        }
        private void button13_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 13, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 13);
        }
        private void button14_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 14, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 14);
        }
        private void button15_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 15, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 15);
        }
        private void button16_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 16, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 16);
        }
        private void button17_1_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 17, 1);
            _controller.API_LocalPlayerPressedElevatorButton(1, 17);
        }
        #endregion InternalButtonsElevator1
        #region InternalButtonsElevator2
        private void button0_2_Click(object sender, EventArgs e)
        {
            //SetElevatorButtonColor(stateCalled: true, buttonNumber: 0, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 0);
        }
        private void button1_2_Click(object sender, EventArgs e)
        {
            //SetElevatorButtonColor(stateCalled: true, buttonNumber: 1, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 1);
        }
        private void button4_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 4, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 4);
        }
        private void button5_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 5, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 5);
        }
        private void button6_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 6, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 6);
        }
        private void button7_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 7, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 7);
        }
        private void button8_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 8, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 8);
        }
        private void button9_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 9, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 9);
        }
        private void button10_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 10, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 10);
        }
        private void button11_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 11, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 11);
        }
        private void button12_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 12, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 12);
        }
        private void button13_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 13, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 13);
        }
        private void button14_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 14, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 14);
        }
        private void button15_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 15, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 15);
        }
        private void button16_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 16, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 16);
        }
        private void button17_2_Click(object sender, EventArgs e)
        {
            SetElevatorButtonColor(stateCalled: true, buttonNumber: 17, 2);
            _controller.API_LocalPlayerPressedElevatorButton(2, 17);
        }
        #endregion InternalButtonsElevator2
        private bool isPause = false;
        private void buttonPlayPause_Click(object sender, EventArgs e)
        {
            isPause = !isPause;
            if (isPause)
            {
                buttonPlayPause.BackColor = Color.Red;
            }
            else
            {
                buttonPlayPause.BackColor = Color.LightGray;
            }
        }
        internal void DisplayLocalPlayerFloor(int floor)
        {
            textBoxPlayerFloor.Text = floor.ToString();
        }
        /// <summary>
        /// Opening elevators on floor 0 / reception
        /// </summary>
        internal void OpenElevatorLocalPlayerFloor(int elevatorNumber, bool isGoingUp, bool isIdle)
        {
            DisplayElevatorDoorState(isIdle, isGoingUp, elevatorNumber, textBoxStateFloor1, textBoxStateFloor2, textBoxStateFloor3, true);
        }
        /// <summary>
        /// Closing elevators on floor 0 / reception
        /// </summary>
        internal void CloseElevatorLocalPlayerFloor(int elevatorNumber)
        {
            switch (elevatorNumber)
            {
                case 0:
                    textBoxStateFloor1.Text = "CLOSED";
                    textBoxStateFloor1.BackColor = Color.Purple;
                    break;
                case 1:
                    textBoxStateFloor2.Text = "CLOSED";
                    textBoxStateFloor2.BackColor = Color.Purple;
                    break;
                case 2:
                    textBoxStateFloor3.Text = "CLOSED";
                    textBoxStateFloor3.BackColor = Color.Purple;
                    break;
            }
        }
    }
    #endregion GUI_class
}