using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BenchGunUi : MonoBehaviour
{

    public Slider AngleSlider;
    public TextMeshProUGUI AngleLabel;
    public TMP_InputField ProjectileMassInput;
    public TMP_InputField MuzzleVelocityInput;
    public TMP_InputField ProjectileRadiusInput;
    public TMP_InputField BallisticCoefficientInput;

    public Button FireButton;

    public BenchGun BenchGun;

    // Start is called before the first frame update
    void Start()
    {
        AngleSlider.onValueChanged.AddListener((val) => { BenchGun.TurretAngle = val; AngleLabel.text = val.ToString(); });
        FireButton.onClick.AddListener(() => {
            BenchGun.Fire(new ProjectileInfo
            {
                BallisticCoefficientG1 = float.Parse(BallisticCoefficientInput.text),
                MassGrams = float.Parse(ProjectileMassInput.text),
                MuzzleVelocity = float.Parse(MuzzleVelocityInput.text),
                RadiusMillimeters = float.Parse(ProjectileRadiusInput.text)
            }); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
