using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterTestClick : MonoBehaviour
    
{
    public static bool WaterIsTesting = false;
    public GameObject testMenuUI;

    public void OpenPanel()
    {
        if (testMenuUI != null)
        {
                if (WaterIsTesting)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
         
        }
    }
    public void Resume()
    {
        testMenuUI.SetActive(false);
        Time.timeScale = 1f;
        WaterIsTesting = false;
    }
    void Pause()
    {
        testMenuUI.SetActive(true);
        Time.timeScale = 0f;
        WaterIsTesting = true;
    }

}


/*public class WaterTestClick : MonoBehaviour
{
    public Button waterTestButton;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = waterTestButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void TaskOnClick()
    {
        Debug.Log("TestWaterButtonClicked");
        //TestWaterMenu();
    }




if (Panel != null)
        {
            Panel.SetActive(true);
        }
}*/
