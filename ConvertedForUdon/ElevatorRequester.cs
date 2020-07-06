
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ElevatorRequester : UdonSharpBehaviour
{
    public NetworkingController _networkingController;

    #region ElevatorRequests
    /// <summary>
    /// When the player pressed a button outside on the floor
    /// </summary>
    public void RequestElevatorFloorButton(bool directionUp, int floor)
    {
        Debug.Log("[ElevatorRequester] Elevator called to floor " + floor + " by localPlayer (DirectionUp: " + directionUp.ToString() + ")");
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ELREQ_CallFromFloor(directionUp, floor);
            return;
        }
        //function calls for calling it up
        if (directionUp == true)
        {
            string functionName = $"MOCUP{floor}";
            Debug.Log($"Sending Network function {functionName} to master");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
        }
        else
        {
            //function calls for calling it down
            string functionName = $"MOCDWN{floor}";
            Debug.Log($"Sending Network function {functionName} to master");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
        }
    }
    #region RequestElevatorFloorButton
    //functions for calling the elevator up
    public void MOCUP0()
    {
        _networkingController.ELREQ_CallFromFloor(true, 0);
    }
    public void MOCUP1()
    {
        _networkingController.ELREQ_CallFromFloor(true, 1);
    }
    public void MOCUP2()
    {
        _networkingController.ELREQ_CallFromFloor(true, 2);
    }
    public void MOCUP3()
    {
        _networkingController.ELREQ_CallFromFloor(true, 3);
    }
    public void MOCUP4()
    {
        _networkingController.ELREQ_CallFromFloor(true, 4);
    }
    public void MOCUP5()
    {
        _networkingController.ELREQ_CallFromFloor(true, 5);
    }
    public void MOCUP6()
    {
        _networkingController.ELREQ_CallFromFloor(true, 6);
    }
    public void MOCUP7()
    {
        _networkingController.ELREQ_CallFromFloor(true, 7);
    }
    public void MOCUP8()
    {
        _networkingController.ELREQ_CallFromFloor(true, 8);
    }
    public void MOCUP9()
    {
        _networkingController.ELREQ_CallFromFloor(true, 9);
    }
    public void MOCUP10()
    {
        _networkingController.ELREQ_CallFromFloor(true, 10);
    }
    public void MOCUP11()
    {
        _networkingController.ELREQ_CallFromFloor(true, 11);
    }
    public void MOCUP12()
    {
        _networkingController.ELREQ_CallFromFloor(true, 12);
    }
    public void MOCUP13()
    {
        _networkingController.ELREQ_CallFromFloor(true, 13);
    }
    //functions for calling it down
    public void MOCDWN0()
    {
        _networkingController.ELREQ_CallFromFloor(false, 0);
    }
    public void MOCDWN1()
    {
        _networkingController.ELREQ_CallFromFloor(false, 1);
    }
    public void MOCDWN2()
    {
        _networkingController.ELREQ_CallFromFloor(false, 2);
    }
    public void MOCDWN3()
    {
        _networkingController.ELREQ_CallFromFloor(false, 3);
    }
    public void MOCDWN4()
    {
        _networkingController.ELREQ_CallFromFloor(false, 4);
    }
    public void MOCDWN5()
    {
        _networkingController.ELREQ_CallFromFloor(false, 5);
    }
    public void MOCDWN6()
    {
        _networkingController.ELREQ_CallFromFloor(false, 6);
    }
    public void MOCDWN7()
    {
        _networkingController.ELREQ_CallFromFloor(false, 7);
    }
    public void MOCDWN8()
    {
        _networkingController.ELREQ_CallFromFloor(false, 8);
    }
    public void MOCDWN9()
    {
        _networkingController.ELREQ_CallFromFloor(false, 9);
    }
    public void MOCDWN10()
    {
        _networkingController.ELREQ_CallFromFloor(false, 10);
    }
    public void MOCDWN11()
    {
        _networkingController.ELREQ_CallFromFloor(false, 11);
    }
    public void MOCDWN12()
    {
        _networkingController.ELREQ_CallFromFloor(false, 12);
    }
    public void MOCDWN13()
    {
        _networkingController.ELREQ_CallFromFloor(false, 13);
    }
    #endregion RequestElevatorFloorButton

    public void RequestElevatorDoorStateChange(int elevatorNumber, bool open)
    {
        Debug.Log("[ElevatorRequester] Elevator " + elevatorNumber + " requested by localPlayer to open/close (DirectionOpen: " + open.ToString() + ")");
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ELREQ_CallToChangeDoorState(elevatorNumber, open);
            return;
        }
        //function calls for calling it to open
        if (open == true)
        {
            string functionName = $"MOOPEN{elevatorNumber}";
            Debug.Log($"Sending Network function {functionName} to master");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
        }
        else
        {
            //function calls for calling it to close
            string functionName = $"MOCLOSE{elevatorNumber}";
            Debug.Log($"Sending Network function {functionName} to master");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
        }
    }
    #region RequestElevatorDoorStateChange
    //network receivers
    public void MOCLOSE0()
    {
        _networkingController.ELREQ_CallToChangeDoorState(0, false);
    }
    public void MOCLOSE1()
    {
        _networkingController.ELREQ_CallToChangeDoorState(1, false);
    }
    public void MOCLOSE2()
    {
        _networkingController.ELREQ_CallToChangeDoorState(2, false);
    }
    public void MOOPEN0()
    {
        _networkingController.ELREQ_CallToChangeDoorState(0, true);
    }
    public void MOOPEN1()
    {
        _networkingController.ELREQ_CallToChangeDoorState(1, true);
    }
    public void MOOPEN2()
    {
        _networkingController.ELREQ_CallToChangeDoorState(2, true);
    }
    #endregion RequestElevatorDoorStateChange

    public void RequestElevatorInternalTarget(int elevatorNumber, int floor)
    {
        Debug.Log("[ElevatorRequester] Request internal target for elevator " + elevatorNumber.ToString() + " by localPlayer (target: " + floor.ToString() + ")");
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ELREQ_SetInternalTarget(elevatorNumber, floor);
            return;
        }
        string functionName = $"MOE{elevatorNumber}T{floor}";
        Debug.Log($"Sending Network function {functionName} to master");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
    }
    #region RequestElevatorInternalTarget
    //Elevator internal calls (elevator 1)
    public void MOE0T0()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 0);
    }
    public void MOE0T1()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 1);
    }
    public void MOE0T2()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 2);
    }
    public void MOE0T3()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 3);
    }
    public void MOE0T4()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 4);
    }
    public void MOE0T5()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 5);
    }
    public void MOE0T6()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 6);
    }
    public void MOE0T7()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 7);
    }
    public void MOE0T8()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 8);
    }
    public void MOE0T9()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 9);
    }
    public void MOE0T10()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 10);
    }
    public void MOE0T11()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 11);
    }
    public void MOE0T12()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 12);
    }
    public void MOE0T13()
    {
        _networkingController.ELREQ_SetInternalTarget(0, 13);
    }
    //Elevator internal calls (elevator 2)
    public void MOE1T0()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 0);
    }
    public void MOE1T1()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 1);
    }
    public void MOE1T2()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 2);
    }
    public void MOE1T3()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 3);
    }
    public void MOE1T4()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 4);
    }
    public void MOE1T5()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 5);
    }
    public void MOE1T6()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 6);
    }
    public void MOE1T7()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 7);
    }
    public void MOE1T8()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 8);
    }
    public void MOE1T9()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 9);
    }
    public void MOE1T10()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 10);
    }
    public void MOE1T11()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 11);
    }
    public void MOE1T12()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 12);
    }
    public void MOE1T13()
    {
        _networkingController.ELREQ_SetInternalTarget(1, 13);
    }
    //Elevator internal calls (elevator 3)
    public void MOE2T0()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 0);
    }
    public void MOE2T1()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 1);
    }
    public void MOE2T2()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 2);
    }
    public void MOE2T3()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 3);
    }
    public void MOE2T4()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 4);
    }
    public void MOE2T5()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 5);
    }
    public void MOE2T6()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 6);
    }
    public void MOE2T7()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 7);
    }
    public void MOE2T8()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 8);
    }
    public void MOE2T9()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 9);
    }
    public void MOE2T10()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 10);
    }
    public void MOE2T11()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 11);
    }
    public void MOE2T12()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 12);
    }
    public void MOE2T13()
    {
        _networkingController.ELREQ_SetInternalTarget(2, 13);
    }
    #endregion RequestElevatorInternalTarget

    #endregion ElevatorRequests

    #region RoomRequests
    /// <summary>
    /// Requests a room booking from the master
    /// </summary>
    /// <param name="roomNumber">Room to book</param>
    public void RequestRoomBooking(int roomNumber)
    {
        Debug.Log("[ElevatorRequester] Request for room booking. Room: " + roomNumber.ToString());
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ROOMREQ_BookRoom(roomNumber);
            return;
        }
        string functionName = $"ROB{roomNumber}";
        Debug.Log($"Sending Network function {functionName} to master");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
    }
    #region RequestRoomBooking
    public void ROB0()
    {
        _networkingController.ROOMREQ_BookRoom(0);
    }
    public void ROB1()
    {
        _networkingController.ROOMREQ_BookRoom(1);
    }
    public void ROB2()
    {
        _networkingController.ROOMREQ_BookRoom(2);
    }
    public void ROB3()
    {
        _networkingController.ROOMREQ_BookRoom(3);
    }
    public void ROB4()
    {
        _networkingController.ROOMREQ_BookRoom(4);
    }
    public void ROB5()
    {
        _networkingController.ROOMREQ_BookRoom(5);
    }
    public void ROB6()
    {
        _networkingController.ROOMREQ_BookRoom(6);
    }
    public void ROB7()
    {
        _networkingController.ROOMREQ_BookRoom(7);
    }
    public void ROB8()
    {
        _networkingController.ROOMREQ_BookRoom(8);
    }
    public void ROB9()
    {
        _networkingController.ROOMREQ_BookRoom(9);
    }
    public void ROB10()
    {
        _networkingController.ROOMREQ_BookRoom(10);
    }
    public void ROB11()
    {
        _networkingController.ROOMREQ_BookRoom(11);
    }
    public void ROB12()
    {
        _networkingController.ROOMREQ_BookRoom(12);
    }
    #endregion RequestRoomBooking

    /// <summary>
    /// Return room card and releases the room
    /// </summary>
    /// <param name="roomNumber">Room to release</param>
    public void RequestRoomRelease(int roomNumber)
    {
        Debug.Log("[ElevatorRequester] Request returning a keycard for room: " + roomNumber.ToString());
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ROOMREQ_ReleaseRoom(roomNumber);
            return;
        }
        string functionName = $"ROR{roomNumber}";
        Debug.Log($"Sending Network function {functionName} to master");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
    }
    #region ReleaseRoomBooking
    public void ROR0()
    {
        _networkingController.ROOMREQ_ReleaseRoom(0);
    }
    public void ROR1()
    {
        _networkingController.ROOMREQ_ReleaseRoom(1);
    }
    public void ROR2()
    {
        _networkingController.ROOMREQ_ReleaseRoom(2);
    }
    public void ROR3()
    {
        _networkingController.ROOMREQ_ReleaseRoom(3);
    }
    public void ROR4()
    {
        _networkingController.ROOMREQ_ReleaseRoom(4);
    }
    public void ROR5()
    {
        _networkingController.ROOMREQ_ReleaseRoom(5);
    }
    public void ROR6()
    {
        _networkingController.ROOMREQ_ReleaseRoom(6);
    }
    public void ROR7()
    {
        _networkingController.ROOMREQ_ReleaseRoom(7);
    }
    public void ROR8()
    {
        _networkingController.ROOMREQ_ReleaseRoom(8);
    }
    public void ROR9()
    {
        _networkingController.ROOMREQ_ReleaseRoom(9);
    }
    public void ROR10()
    {
        _networkingController.ROOMREQ_ReleaseRoom(10);
    }
    public void ROR11()
    {
        _networkingController.ROOMREQ_ReleaseRoom(11);
    }
    public void ROR12()
    {
        _networkingController.ROOMREQ_ReleaseRoom(12);
    }
    #endregion ReleaseRoomBooking
    /// <summary>
    /// Request to lock a room
    /// </summary>
    /// <param name="roomNumber">Room to lock</param>
    public void RequestRoomLock(int roomNumber)
    {
        Debug.Log("[ElevatorRequester] Request to lock room: " + roomNumber.ToString());
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ROOMREQ_ChangeLockState(roomNumber, true);
            return;
        }
        string functionName = $"ROL{roomNumber}";
        Debug.Log($"Sending Network function {functionName} to master");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
    }
    /// <summary>
    /// Request to unlock a room
    /// </summary>
    /// <param name="roomNumber">Room to unlock</param>
    public void RequestRoomUnlock(int roomNumber)
    {
        Debug.Log("[ElevatorRequester] Request to unlock room: " + roomNumber.ToString());
        if (Networking.LocalPlayer.isMaster)
        {
            //master doesn't need to send a request over network
            _networkingController.ROOMREQ_ChangeLockState(roomNumber, false);
            return;
        }
        string functionName = $"ROU{roomNumber}";
        Debug.Log($"Sending Network function {functionName} to master");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
    }
    #region RoomLockUnlockCallbacks
    public void ROL0()
    {
        _networkingController.ROOMREQ_ChangeLockState(0, true);
    }
    public void ROL1()
    {
        _networkingController.ROOMREQ_ChangeLockState(1, true);
    }
    public void ROL2()
    {
        _networkingController.ROOMREQ_ChangeLockState(2, true);
    }
    public void ROL3()
    {
        _networkingController.ROOMREQ_ChangeLockState(3, true);
    }
    public void ROL4()
    {
        _networkingController.ROOMREQ_ChangeLockState(4, true);
    }
    public void ROL5()
    {
        _networkingController.ROOMREQ_ChangeLockState(5, true);
    }
    public void ROL6()
    {
        _networkingController.ROOMREQ_ChangeLockState(6, true);
    }
    public void ROL7()
    {
        _networkingController.ROOMREQ_ChangeLockState(7, true);
    }
    public void ROL8()
    {
        _networkingController.ROOMREQ_ChangeLockState(8, true);
    }
    public void ROL9()
    {
        _networkingController.ROOMREQ_ChangeLockState(9, true);
    }
    public void ROL10()
    {
        _networkingController.ROOMREQ_ChangeLockState(10, true);
    }
    public void ROL11()
    {
        _networkingController.ROOMREQ_ChangeLockState(11, true);
    }
    public void ROL12()
    {
        _networkingController.ROOMREQ_ChangeLockState(12, true);
    }
    public void ROU0()
    {
        _networkingController.ROOMREQ_ChangeLockState(0, false);
    }
    public void ROU1()
    {
        _networkingController.ROOMREQ_ChangeLockState(1, false);
    }
    public void ROU2()
    {
        _networkingController.ROOMREQ_ChangeLockState(2, false);
    }
    public void ROU3()
    {
        _networkingController.ROOMREQ_ChangeLockState(3, false);
    }
    public void ROU4()
    {
        _networkingController.ROOMREQ_ChangeLockState(4, false);
    }
    public void ROU5()
    {
        _networkingController.ROOMREQ_ChangeLockState(5, false);
    }
    public void ROU6()
    {
        _networkingController.ROOMREQ_ChangeLockState(6, false);
    }
    public void ROU7()
    {
        _networkingController.ROOMREQ_ChangeLockState(7, false);
    }
    public void ROU8()
    {
        _networkingController.ROOMREQ_ChangeLockState(8, false);
    }
    public void ROU9()
    {
        _networkingController.ROOMREQ_ChangeLockState(9, false);
    }
    public void ROU10()
    {
        _networkingController.ROOMREQ_ChangeLockState(10, false);
    }
    public void ROU11()
    {
        _networkingController.ROOMREQ_ChangeLockState(11, false);
    }
    public void ROU12()
    {
        _networkingController.ROOMREQ_ChangeLockState(12, false);
    }
    #endregion RoomLockUnlockCallbacks

    #endregion RoomRequests
}
