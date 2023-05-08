using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ES3Internal;
using System;
public class ShowSavePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button saveButton;
    [SerializeField] Button confirmButton;
    [SerializeField] Button dataButton;
    [SerializeField] Button recoButton;


    public Dictionary<string, string> data = new Dictionary<string, string>();
    [SerializeField] TMP_Text popUpText;
    [SerializeField] Animator popUpAnim;
    [SerializeField] float popUpShowingTime = 3f;


    private void Start()
    {
        ES3.Save<Dictionary<string, string>>("data", data);
    }

    public bool panelShowing = false;

    public void ShowAndHidePanel()
    {
        if (panelShowing)
            HidePanel();
        else if (!panelShowing)
            ShowPanel();
        else
            Debug.LogWarning("PanelShowing status Error");
    }
    void HidePanel()
    {
        inputField.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);

        recoButton.gameObject.SetActive(true);
        dataButton.gameObject.SetActive(false);
        panelShowing = false;
    }

    void ShowPanel()
    {
        inputField.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);

        recoButton.gameObject.SetActive(false);
        dataButton.gameObject.SetActive(true);
        panelShowing = true;
    }

    public void SaveData()
    {
        string num = inputField.text;

        if (string.IsNullOrEmpty(Classification.resText.text))
        {
            string type = Classification.resText.text;

            if (data.ContainsKey(num))
            {
                data[num] = type;
                StartCoroutine(ShowSubPopupInfo(num, type));
            }
            else
            {
                data.Add(num, type);
                StartCoroutine(ShowSavePopupInfo(num, type));
            }
            ES3.Save<Dictionary<string, string>>("data", data);
        }
        else
            Debug.LogWarning("Classification.resText.text IsNullOrEmpty");
    }

    IEnumerator ShowSavePopupInfo(string num, string type)
    {
        // ID: 123123
        // Type: 露筋
        // 数据已保存
        popUpAnim.Play("Show");
        if (num == "")
            popUpText.text = "ID: " + DateTime.UtcNow.ToString() + "\n"
            + "Type: " + type + "\n"
            + "ID为空 时间戳作为ID 数据已保存";
        else
            popUpText.text = "ID: " + num + "\n"
                + "Type: " + type + "\n"
                + "数据已保存";
        yield return new WaitForSeconds(popUpShowingTime);
        popUpAnim.Play("Hide");
    }
    IEnumerator ShowSubPopupInfo(string num, string type)
    {
        // ID: 123123
        // Type: 露筋
        // 数据已保存
        popUpAnim.Play("Show");
        if (num == "")
            popUpText.text = "ID: " + DateTime.UtcNow.ToString() + "\n"
            + "Type: " + type + "\n"
            + "ID已经存在 数据更新成功";
        else
            popUpText.text = "ID: " + num + "\n"
                + "Type: " + type + "\n"
                + "ID已经存在 数据更新成功";
        yield return new WaitForSeconds(popUpShowingTime);
        popUpAnim.Play("Hide");
    }

    [SerializeField] GameObject DataPanel;
    
    [SerializeField] Transform ContentContainer;
    [SerializeField] GameObject dataItenPrefab;


    public void ShowDataPanel()
    {
      
        data = ES3.Load<Dictionary<string, string>>("data");
        ClearContent(ContentContainer);
        foreach (var dataItem in data)
        {
            GameObject dataItemObj = GameObject.Instantiate(dataItenPrefab, ContentContainer);
            if (dataItenPrefab.transform.Find("text").TryGetComponent<TMP_Text>(out TMP_Text dataItemText))
            {
                dataItemText.text = "ID: " + dataItem.Key + "  -  Type: " + dataItem.Value;
            }
        }
          DataPanel.SetActive(true);
    }



    void ClearContent(Transform parent)
    {
        foreach (Transform tr in parent)
        {
            Destroy(tr.gameObject);
        }
    }

    public void CloseDataPanel()
    {
        DataPanel.SetActive(false);
    }

}
