using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UITabs : MonoBehaviour
{
    public GameObject[] pages;

    int index = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        for(int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[0].SetActive(true);
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInputs.inputREF.nextTabInput)
        {
            NextPage();
        }

        if(UserInputs.inputREF.prevTabInput)
        {
            PreviousPage();
        }

    }

    void NextPage()
    {
        pages[index].SetActive(false);
        
        index++;

        if(index >= pages.Length)
        {
            index = 0;
        }

        pages[index].SetActive(true);
    }
    void PreviousPage()
    {
        pages[index].SetActive(false);
        
        index--;

        if (index < 0)
        {
            index = pages.Length -1;
        }

        pages[index].SetActive(true);
    }
}

