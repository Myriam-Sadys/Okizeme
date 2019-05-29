using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAttackButton : MonoBehaviour
{
    public RawImage Details;
    public RawImage DetailsAttack;
    public Text Name;
    public Text Description;

    private string Capacity;
    private string OGDescription;
    private Hability _hability;
    public Hability Hability
    {
        get { return _hability; }
        set
        {
            if (value != _hability)
                _hability = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OverButton()
    {
        if (Hability != null)
        {
            Text[] texts = DetailsAttack.GetComponentsInChildren<Text>();
            texts[0].text = Hability.Damages.ToString();
            texts[1].text = Hability.ComboCost.ToString();
            Capacity = Name.text;
            OGDescription = Description.text;
            Name.text = Hability.Name;
            Description.text = Hability.Description;
            Details.gameObject.SetActive(false);
            DetailsAttack.gameObject.SetActive(true);
        }
    }

    public void OutOffButton()
    {
        if (!string.IsNullOrEmpty(Capacity))
        {
            Name.text = Capacity;
            Description.text = OGDescription;
        }
        DetailsAttack.gameObject.SetActive(false);
        Details.gameObject.SetActive(true);
    }

}
