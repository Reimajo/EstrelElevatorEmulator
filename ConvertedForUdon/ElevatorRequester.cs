
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ElevatorRequester : UdonSharpBehaviour
{
    public NetworkingController _networkingController;
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
}
