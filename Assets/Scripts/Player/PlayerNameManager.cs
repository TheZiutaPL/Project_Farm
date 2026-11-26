using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using WebSocketSharp;

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
            if(string.IsNullOrWhiteSpace(playerName))
                SetName(playerName);

            return playerName;
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

        // Set saved name
        SetName(PlayerPrefs.GetString(PLAYER_NAME_PREFS, string.Empty));

        playerNameInputField.text = PlayerName;

        playerNameInputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnEndEdit(string newName)
    {
        SetName(newName);

        newName = PlayerName;
        if (playerNameInputField.text == newName)
            return;

        playerNameInputField.SetTextWithoutNotify(newName);
    }

    public static void SetName(string newName)
    {
        newName = newName.Trim();

        if (!string.IsNullOrWhiteSpace(playerName) && playerName == newName)
            return;

        if(string.IsNullOrWhiteSpace(newName))
        {
            newName = GetRandomName();
            PlayerPrefs.DeleteKey(PLAYER_NAME_PREFS);
        }
        else
            PlayerPrefs.SetString(PLAYER_NAME_PREFS, newName);

        playerName = newName;
    }
}