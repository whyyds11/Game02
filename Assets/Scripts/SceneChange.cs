using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class wyj : MonoBehaviour
{
    //public GameObject levelPanel;
    // Use this for initialization
    public Button thebutton;
    void Start()
    {
        thebutton = this.GetComponent<Button>();
        thebutton.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        SceneManager.LoadSceneAsync(1);//level1Ϊ����Ҫ�л����ĳ���
        //levelPanel.SetActive(true);
        Debug.Log("click");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
