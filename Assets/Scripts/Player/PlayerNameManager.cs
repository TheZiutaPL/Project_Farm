using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    private static readonly string[][] randomNames = new string[][]
    {
        new string[]
        {
            "Disarmed",
            "Jolly",
            "Salty",
            "Jumping",
            "Heroic",
            "Comfy",
            "Petty",
            "Pool",
            "Magic",
            "Elite",
        },
        new string[] 
        {
            "Cookie",
            "Roger",
            "Sally",
            "Betty",
            "Potato",
            "Chris",
            "Peter",
            "Noodle",
            "Steve",
            "Banana",
        },
    };

    private static readonly StringBuilder nameBuilder = new StringBuilder();

    private static string GetRandomName()
    {
        nameBuilder.Clear();
        
        for (int i = 0; i < randomNames.Length; i++)
        {
            string namePart = randomNames[i][Random.Range(0, randomNames[i].Length)];
            nameBuilder.Append(namePart);
            nameBuilder.Append(' ');
        }

        nameBuilder.Append($"#{Random.Range(0, 100)}");

        return nameBuilder.ToString();
    }

    private static PlayerNameManager instance;

    private static string playerName;
    public static string PlayerName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(playerName))
                playerName = GetRandomName();
            
            return playerName;
        }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                playerName = GetRandomName();
        }
    }

    [SerializeField] private TMP_InputField playerNameInputField;

    private const string PLAYER_NAME_PREFS = "_playerName";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if(PlayerPrefs.HasKey(PLAYER_NAME_PREFS))
            PlayerName = PlayerPrefs.GetString(PLAYER_NAME_PREFS);

        playerNameInputField.text = PlayerName;

        playerNameInputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string newValue)
    {
        PlayerName = newValue;

        string newName = PlayerName;
        playerNameInputField.SetTextWithoutNotify(newName);
        PlayerPrefs.SetString(PLAYER_NAME_PREFS, newName);
    }
}
