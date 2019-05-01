using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSearchBar : MonoBehaviour
{
    public bool IsOpen = false;

    private void Awake()
    {
        this.transform.gameObject.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
    }


    public void SetSearchBarOpen (bool isOpen)
    {
        IsOpen = isOpen;
        this.transform.gameObject.SetActive(IsOpen);
    }


}
