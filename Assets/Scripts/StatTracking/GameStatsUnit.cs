using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace StatTracking
{
    public struct GameStatUnit : INetworkSerializable, IEquatable<GameStatUnit>
    {
        public GameStats gameStat;
        public int statValue;

        public GameStatUnit(GameStats stat)
        {
            gameStat = stat;
            statValue = 0;
        }

        public bool Equals(GameStatUnit other) =>
            gameStat == other.gameStat &&
            statValue == other.statValue;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref gameStat);
            serializer.SerializeValue(ref statValue);
        }
    }
}