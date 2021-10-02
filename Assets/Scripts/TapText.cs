using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapText : MonoBehaviour
{
    public float SpawnTime = 0.5f;//spawn setiap 0.5 detik
    public Text Text;

    private float _spawnTime;

    private void OnEnable()
    {
        _spawnTime = SpawnTime;
    }



    private void Update()
    {
        _spawnTime -= Time.unscaledDeltaTime;//nge spawn secara real time
        if(_spawnTime<=0f)//jika tidak diklik maka spawn tidak berjalan
        {
            gameObject.SetActive(false);
        }
        else
        {
            Text.CrossFadeAlpha(0f, 0.5f, false);
            if (Text.color.a == 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
