using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Enumeracna trieda reprezentujuca typ packetu
/// </summary>
public enum PacketMessageType
{
    Connect = 0,
    Send = 1,
    Disconnect = 2,
    ChatMessage = 3,
    RequestConnClientsData = 4,
    Movement = 5,
    LevelData = 6,
    RequestLevelInitData = 7,
    LevelWonChanged = 8, //Level bol vyhrany a ide sa zmenit
    DefaultPosChanged = 9,
    GameFinished = 10
}

/// <summary>
/// Enumeracna trieda, ktora blizsie specifikuje typ packetu o info requeste.
/// </summary>
public enum PacketInfoRequestType
{
    Init_Connect = 0,
    Request = 1
}

/// <summary>
/// Enumeracna trieda, ktora specifikuje o ake data o pohybe klienta ide.
/// </summary>
public enum ClientMovementDataType
{
    Regular = 0,
    ErrorCorrect = 1
}