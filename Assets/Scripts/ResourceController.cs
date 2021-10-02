using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public AudioSource updatecoin;
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;
    private int _index;
    private ResourceConfig _config;
    private int _level
    {
        set
        {
            UserDataManager.Progress.ResourcesLevel[_index] = value;
            UserDataManager.Save();
        }
        get
        {
            if (!UserDataManager.HasResources(_index))
            {
                return 1;
            }
            return UserDataManager.Progress.ResourcesLevel[_index];
        }
    }
    public bool IsUnlocked { get; private set; }
    private void Start()

    {

        ResourceButton.onClick.AddListener(() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }
    public void SetConfig (int index,ResourceConfig config)
    {
        _index = index;
        _config = config;
        //To string("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{_config.Name} Lv. {_level}\n+{GetOutput().ToString("0")}";
        ResourceUnlockCost.text = $"Unlock Cost\n{_config.UnlockCost}";
        ResourceUpgradeCost.text = $"Upgrade Cost \n{GetUpgradeCost()}";

        SetUnlocked(_config.UnlockCost == 0||UserDataManager.HasResources(_index));
    }
    public double GetOutput()
    {
        return _config.Output * _level;
    }
    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * _level;
    }
    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }
    
    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if (UserDataManager.Progress.gold < upgradeCost)
        {
            return;
        }
        GameManager.Instance.AddGold(-upgradeCost);
        _level++;

        updatecoin.Play();  
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";

        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
            
    }
    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if (UserDataManager.Progress.gold< unlockCost)
        {
            return;
        }
        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();
        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);
        updatecoin.Play();
    }
    public void SetUnlocked(bool unlocked)
    {
        IsUnlocked = unlocked;
        if (unlocked)

        {

            // Jika resources baru di unlock dan belum ada di Progress Data, maka tambahkan data

            if (!UserDataManager.HasResources(_index))

            {

                UserDataManager.Progress.ResourcesLevel.Add(_level);

                UserDataManager.Save();

            }

        }
        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }
   
}
