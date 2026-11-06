using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Requests
{
    public class RequestObjectSpawner : MonoBehaviour
    {
        [SerializeField] private RequestObject requestObjectPrefab;

        [SerializeField] private Transform[] requestObjectPositions = new Transform[0];
        private Dictionary<Transform, RequestObject> requestObjectDictionary;

        private void Awake()
        {
            InitializeRequestObjects();
        }

        private void InitializeRequestObjects()
        {
            if (requestObjectDictionary != null)
                return;

            requestObjectDictionary = requestObjectPositions.ToDictionary(x => x, x => Instantiate(requestObjectPrefab, x));
        }

        private void OnEnable()
        {
            RequestManager.OnRequestsUpdated += OnRequestsUpdated;
            OnRequestsUpdated();
        }

        private void OnDisable()
        {
            RequestManager.OnRequestsUpdated -= OnRequestsUpdated;
            HideAllRequestObjects();
        }

        private void OnRequestsUpdated()
        {
            if (RequestManager.Instance == null)
            {
                HideAllRequestObjects();
                return;
            }

            HashSet<int> requestIDs = RequestManager.Instance.GetActiveRequestIDs();

            Dictionary<int, RequestObject> pairedObjects = new Dictionary<int, RequestObject>();
            Queue<RequestObject> unusedObjects = new Queue<RequestObject>();

            foreach (var obj in requestObjectDictionary.Values)
            {
                // If is first object of desired ID, then assign it
                if (requestIDs.Contains(obj.RequestID) && pairedObjects.TryAdd(obj.RequestID, obj))
                {
                    obj.gameObject.SetActive(true);
                    continue;
                }
                
                // Mark for change
                unusedObjects.Enqueue(obj);
            }

            // Assign unused objects
            foreach (int id in requestIDs)
            {
                if (pairedObjects.ContainsKey(id))
                    continue;

                if (unusedObjects.Count == 0)
                    break;

                RequestObject requestObject = unusedObjects.Dequeue();
                requestObject.SetRequestID(id);
                requestObject.gameObject.SetActive(true);

                pairedObjects.Add(id, requestObject);
            }

            // Turn off unused objects
            while(unusedObjects.Count > 0)
            {
                RequestObject requestObject = unusedObjects.Dequeue();

                requestObject.SetRequestID(-1);
                requestObject.gameObject.SetActive(false);
            }
        }

        private void HideAllRequestObjects()
        {
            foreach (var obj in requestObjectDictionary.Values)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }
}