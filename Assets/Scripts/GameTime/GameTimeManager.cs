using System;
using Unity.Netcode;
using UnityEngine;

namespace GameTime
{
    public class GameTimeManager : NetworkBehaviour
    {
        public static GameTimeManager Instance { get; private set; }

        public NetworkVariable<float> MinutesPassed = new NetworkVariable<float>();
        public int GetMinutesPassed() => Mathf.FloorToInt(MinutesPassed.Value);

        [SerializeField] private int startHour = 6;
        [SerializeField] private float gameHours = 16;
        [SerializeField] private float gameMinutesPerSecond = 1;

        private const int MINUTES_PER_HOUR = 60;
        private const int HOURS_PER_DAY = 24;

        private bool gameTimePassed = false;

        public static Action<float> OnGameTimePassed;
        public static Action OnGameTimeEnded;

        public float GetGameTimeProgress01()
        {
            float totalGameMinutes = gameHours * MINUTES_PER_HOUR;

            if (totalGameMinutes <= 0)
                return 1;

            return Mathf.Clamp01(MinutesPassed.Value / totalGameMinutes);
        }

        public float GetInGameTimeProgress01()
        {
            (int hour, int minute) inGameTime = GetInGameTime();

            int inGameMinutes = inGameTime.hour * MINUTES_PER_HOUR + inGameTime.minute;

            return inGameMinutes / (HOURS_PER_DAY * MINUTES_PER_HOUR);
        }

        public (int hour, int minute) GetInGameTime()
        {
            // Get total minutes passed
            int minutesPassed = GetMinutesPassed();
            
            // Calculate hours and minutes passed
            int hoursPassed = minutesPassed / MINUTES_PER_HOUR;
            minutesPassed %= MINUTES_PER_HOUR;

            // Take care of games spanning multiple days
            hoursPassed = (startHour + hoursPassed) % HOURS_PER_DAY;

            return (hoursPassed, minutesPassed);
        }

        public override void OnNetworkSpawn()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            MinutesPassed.OnValueChanged += MinutesPassedCallback;
        }

        private void Update()
        {
            if (!IsServer)
                return;

            if (gameTimePassed)
                return;

            HandleGameTimeUpdate();

            HandleGameEndCheckUpdate();
        }

        private void HandleGameTimeUpdate()
        {
            MinutesPassed.Value += Time.deltaTime * gameMinutesPerSecond;

            var time = GetInGameTime();
            Debug.Log($"InGame time - {time.hour}:{time.minute}");
        }

        private void HandleGameEndCheckUpdate()
        {
            if (MinutesPassed.Value < gameHours * MINUTES_PER_HOUR)
                return;

            gameTimePassed = true;

            OnGameTimePassedRpc();
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void OnGameTimePassedRpc()
        {
            Debug.Log("Game Time Ended");
            OnGameTimeEnded?.Invoke();
        }

        private void MinutesPassedCallback(float oldValue, float newValue)
        {
            float minutesPassed = newValue - oldValue;
            OnGameTimePassed?.Invoke(minutesPassed);
        }
    }
}