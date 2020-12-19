using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    // the operation which avatar could exceed
    public bool is_to_buy_ground; 
    public bool is_to_remove_building;
    //public GameObject information_menu;

    private bool show_buy_information;
    // Start is called before the first frame update
    void Start()
    {
        show_buy_information = false;
        is_to_remove_building = false;
        is_to_buy_ground = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (show_buy_information)
        {
            this.transform.Find("Sub Panel").gameObject.SetActive(true);
        }

        if (!show_buy_information)
        {
            this.transform.Find("Sub Panel").gameObject.SetActive(false);
        }
    }
    public void MenuButtonClick()
    {
        show_buy_information = true;
    }

    public void RemoveButtonClick()
    {
        is_to_remove_building = true;
    }

    public void MenuShutDownButtonClick()
    {
        show_buy_information = false;
    }

    public void BuyGroundButtonClick()
    {
        is_to_buy_ground = true;
    }
}
