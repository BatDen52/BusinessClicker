using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Configs/LocalizationConfig")]
public class LocalizationConfig : ScriptableObject
{
    public BusinessLocalization[] BusinessLocalizations;
    public LocalizationLables Lables;
}
