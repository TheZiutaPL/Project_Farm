using UnityEngine;

namespace Databases
{
    public interface IDataIdentifiable<T> where T : ScriptableObject
    {
        public int DataID { get; protected set; }
    }
}