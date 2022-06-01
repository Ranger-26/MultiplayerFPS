using UnityEngine;

public class TabsUI : MonoBehaviour
{
    public GameObject[] tabs;

    private void OnEnable()
    {
        if (tabs.Length != 0)
            ActivateTab(0);
    }

    public void ActivateTab(int id)
    {
        foreach (GameObject go in tabs)
        {
            go.SetActive(false);
        }

        tabs[id].SetActive(true);
    }
}
