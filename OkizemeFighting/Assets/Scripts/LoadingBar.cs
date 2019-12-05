using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour {

    private Image FullLoadingBar;
    private Image EmptyLoadingBar;
    private Image Background;

    private bool startLoading;
    public bool StartLoading
    {
        get { return startLoading; }
        set { startLoading = value; }
    }

    // Use this for initialization
    void Start () {
        FullLoadingBar = this.transform.Find("FullBar").GetComponent<Image>();
        EmptyLoadingBar = this.transform.Find("EmptyBar").GetComponent<Image>();
        Background = this.transform.Find("Image").GetComponent<Image>();
        Hide();
    }

    void Awake()
    {
        FullLoadingBar = this.transform.Find("FullBar").GetComponent<Image>();
        EmptyLoadingBar = this.transform.Find("EmptyBar").GetComponent<Image>();
        Background = this.transform.Find("Image").GetComponent<Image>();
        Hide();
    }

    // Update is called once per frame
    void Update () {
        if (StartLoading)
        {
            if (FullLoadingBar.fillAmount == 1.0f)
                FullLoadingBar.fillAmount = 0;
            FullLoadingBar.fillAmount += 1.0f / 5.0f * Time.deltaTime;
        }
        else
            FullLoadingBar.fillAmount = 0;
    }

    public void Display()
    {
        StartLoading = true;
        FullLoadingBar.gameObject.SetActive(true);
        EmptyLoadingBar.gameObject.SetActive(true);
        Background.gameObject.SetActive(true);
    }

    public void Hide()
    {
        StartLoading = false;
        FullLoadingBar.gameObject.SetActive(false);
        EmptyLoadingBar.gameObject.SetActive(false);
        Background.gameObject.SetActive(false);
    }

}
