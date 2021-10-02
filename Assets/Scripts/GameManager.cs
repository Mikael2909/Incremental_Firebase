using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager _instance = null;
  
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    //fungsi [Range(min,max)] ialah agar menjaga value agar tetap berada di antara min dan max nya
    [Range(0f, 1f)]
    public float AutoCollectPercentage = 0.1f;
    public ResourceConfig[] ResourceConfigs;
    public Sprite[] ResourcesSprites;

    public Transform ResourcesParent;
    public ResourceController ResourcePrefab;
    public TapText TapTextPrefab;
    public Transform CoinIcon;
    public Text GoldInfo;
    public Text AutoCollectInfo;
    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;
    
    public double _totalGold { get; private set; }
    // Start is called before the first frame update
   private void Start()
    {
        AddAllResources();
        GoldInfo.text = $"Gold: {UserDataManager.Progress.gold.ToString("0")}";
    }

    // Update is called once per frame
    private void Update()
    {
        //fungsi untuk mengeksekusi collectPerSecond setiap detik
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }
        CheckResourceCost();
        CoinIcon.transform.localScale = Vector3.LerpUnclamped(CoinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        CoinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);//berputar ke kiri dalam satuan waktu
    }
    public void AddGold(double value)
    {
        UserDataManager.Progress.gold += value;
        _totalGold += value;
        GoldInfo.text = $"Gold : {UserDataManager.Progress.gold.ToString("0")}";
    }

    private void AddAllResources()
    {
        bool showResources = true;
        int index = 0;
        foreach (ResourceConfig config in ResourceConfigs)
        {
            GameObject obj = Instantiate(ResourcePrefab.gameObject, ResourcesParent, false);//fungsi memunculkan prefab game object
            ResourceController resource = obj.GetComponent<ResourceController>();//mendapatkan komponen resource controller
            resource.SetConfig(index,config);
            obj.gameObject.SetActive(showResources);
            if(showResources && !resource.IsUnlocked)
            {
                showResources = false;
            }
            _activeResources.Add(resource);//menambahkan resource
            index++;
        }
    }
  public void ShowNextResource()
    {
        foreach(ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }
    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)

        {
            bool isBuyable = false;
            if (resource.IsUnlocked)
            {
                isBuyable = UserDataManager.Progress.gold >= resource.GetUpgradeCost();
            }
            else
            {
                isBuyable =UserDataManager.Progress.gold >= resource.GetUnlockCost();
            }
            resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];
        }
    }
    private void CollectPerSecond()
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();//resource bertambah satu
            }
        }
        output *= AutoCollectPercentage;//output dikali satu
        //fungsi To String ("F1") ialah membulatkan angka menjadi desimal yang memiliki 1 angka di belakang koma
        AutoCollectInfo.text = $"Auto Collect: {output.ToString("F1")}/second";
        AddGold(output);
    }
   
    public void CollectByTap (Vector3 tapPosition,Transform parent)
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();
            }
        }
        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;
        tapText.Text.text = $"+{output.ToString("0")}";
        tapText.gameObject.SetActive(true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;
        AddGold(output);
    }
    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if (tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }
        return tapText;
    }
    

}
//fungsi sistem serializable adalah agar object bisa di serialize 
//dan value dapat di-set dari inspector
[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;
}
