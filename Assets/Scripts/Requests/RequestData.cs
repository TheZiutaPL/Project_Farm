using System;
using Unity.Netcode;

namespace Requests
{
    [Serializable]
    public struct RequestData : INetworkSerializable, IEquatable<RequestData>
    {
        public int RequestID;
        public int RequestedItemID;

        public bool IsCompleted;

        public bool Equals(RequestData other)
        {
            return
                RequestID == other.RequestID &&
                RequestedItemID == other.RequestedItemID &&
                IsCompleted == other.IsCompleted;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref RequestID);

            serializer.SerializeValue(ref RequestedItemID);

            serializer.SerializeValue(ref IsCompleted);
        }
    }
}