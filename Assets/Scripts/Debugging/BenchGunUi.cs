using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TMP_InputField StepCountInput;
    public TMP_InputField StepResolutionInput;
    public TMP_Dropdown ProjectileTypeDropdown;

    public Button FireButton;
    public BenchGun BenchGun;

    public NpcAmmunitionData[] AmmunitionTypes;
    private List<string> AmmunitionSelectionNames;

    // Start is called before the first frame update
    void Start()
    {
        AmmunitionSelectionNames = AmmunitionTypes.Select(x => x.Name).ToList();
        ProjectileTypeDropdown.AddOptions(AmmunitionSelectionNames);
        ProjectileTypeDropdown.onValueChanged.AddListener((selection) =>
        {
            UpdateAmmunitionInputs(ProjectileTypeDropdown.options[selection].text);
        });

        AngleSlider.onValueChanged.AddListener((val) => { BenchGun.TurretAngle = val; AngleLabel.text = val.ToString(); });
        FireButton.onClick.AddListener(() =>
        {
            BenchGun.Fire(new ProjectileInfo
            {
                BallisticCoefficientG1 = float.Parse(BallisticCoefficientInput.text),
                MassGrams = float.Parse(ProjectileMassInput.text),
                MuzzleVelocity = float.Parse(MuzzleVelocityInput.text),
                RadiusMillimeters = float.Parse(ProjectileRadiusInput.text)
            }, int.Parse(StepCountInput.text), float.Parse(StepResolutionInput.text));
        });
        UpdateAmmunitionInputs(ProjectileTypeDropdown.options[0].text);
    }

    private void UpdateAmmunitionInputs(string selection)
    {
        NpcAmmunitionData ammoSelection = null;
        foreach (var ammoType in AmmunitionTypes)
        {
            if (selection == ammoType.Name)
            {
                ammoSelection = ammoType;
                break;
            }
        }
        BallisticCoefficientInput.text = ammoSelection.BallisticCoefficientG1.ToString();
        ProjectileMassInput.text = ammoSelection.ProjectileMassGrams.ToString();
        MuzzleVelocityInput.text = ammoSelection.MuzzleVelocity.ToString();
        ProjectileRadiusInput.text = ammoSelection.RadiusMillimeters.ToString();
    }
}
