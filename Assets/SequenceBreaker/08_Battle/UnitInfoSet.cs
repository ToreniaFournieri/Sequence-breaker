using UnityEngine;
using UnityEngine.UI;

sealed public class UnitInfoSet : MonoBehaviour
{
    //public GameObject iconMask;
    //public Image unitIcon;
    public Image hPBar;
    public Image shieldBar;
    public Text UnitInfoText;
    public GameObject barrierObject;
    public Text barrierRemains;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);


    }

    private void Awake()
    {

    }

}
