using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeTextManager : MonoBehaviour
{
    private TMP_InputField field;
    private int click = 0;
    private string fieldStr = "";

    // Start is called before the first frame update
    void Start()
    {
        field = Camera.main.GetComponent<InputfieldHolder>().field;

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(2 * gameObject.transform.position -  Camera.main.transform.position);
        //textManager.current.write("Updating textMesh");
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == null)
                textManager.current.write("No object selected");
            else if (hit.collider.gameObject == gameObject)
            {
                //GetComponentInChildren<TextMeshPro>().text = "click " + click++;
                field.SetTextWithoutNotify("");
                //gameObject.SetActive(false);

                field.gameObject.SetActive(true);
                //field.interactable = true;
                //EEventSystem.current.SetSelectedGameObject(myInputField.gameObject, null);
                //field.Select();
                field.ActivateInputField();

                //field.DeactivateInputField();
                field.onValueChanged.RemoveAllListeners();
                field.onValueChanged.AddListener(delegate { onValueChange(); });
                field.onDeselect.RemoveAllListeners();
                field.onDeselect.AddListener(delegate { clickedElsewhere(); });
               /* field.gameObject.SetActive(false);
                gameObject.SetActive(true);*/
            }
                    
            
        }
    }

    public void onValueChange()
    {
        GetComponentInChildren<TextMeshPro>().text = field.text;
        textManager.current.write("changed value :" + field.text + ".");
    }

    public void clickedElsewhere()
    {
        field.SetTextWithoutNotify("");
        field.gameObject.SetActive(false);
    }

}
