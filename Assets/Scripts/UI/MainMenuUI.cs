using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;

    [System.Serializable]
    public class MenuTab
    {
        public string name;
        public GameObject tab;
    }

    [SerializeField] private MenuTab[] menuTabs = new MenuTab[0];
    private MenuTab currentTab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            menuTabs[i].tab.SetActive(false);
        }

        SetTab(menuTabs[0]);
    }

    public MenuTab GetMenuTab(string tabName)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].name == tabName)
                return menuTabs[i];
        }

        return null;
    }

    public void SetTab(string tabName) => SetTab(GetMenuTab(tabName));
    public void SetTab(MenuTab tab)
    {
        if (currentTab == tab)
            return;

        if (currentTab != null)
            currentTab.tab.SetActive(false);

        if(tab != null)
            tab.tab.SetActive(true);

        currentTab = tab;
    }
}
