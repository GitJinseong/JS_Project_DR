using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스탯 데이터를 담을 클래스
[System.Serializable]
public class StatData
{ 
    public List<UpgradePC> upgradeHp;
    public List<UpgradePC> upgradeGainExp;
    public List<UpgradePC> upgradeGaingold;
    public List<UpgradeWeapon> upgradeAtk;
    public List<UpgradeWeapon> upgradeCrit;
    public List<UpgradeWeapon> upgradeCritDmg;
    public List<UpgradeWeapon> upgradeAtkSpd;
}

// PC업그레이드 값을 담을 클래스
[System.Serializable]
public class UpgradePC      
{
    public int level;       // 현재 레벨
    public float value;     // 증가값
    public float sum;       // 누적 
    public int exp;         // 소모 경험치
}
[System.Serializable]
public class UpgradeWeapon
{
    public int level;       // 현재 레벨
    public float value1;
    public float sum1;
    public float value2;
    public float sum2;
    public float value3;
    public float sum3;
    public float value4;
    public float sum4;
    public int exp;         // 소모 경험치
}


public class StatusData : MonoBehaviour
{
    // 스프레드 시트의 데이터 ID
    [Header("Data ID")]
    public int hpID = 901101;
    public int gainExpID = 901201;
    public int gainGoldID = 901301;

    public int atkID = 911101;
    public int critID = 911201;
    public int critDmgID = 911301;
    public int atkspd = 911401;

    [Header("Data")]
    public StatData data;           // 데이터를 모은 변수


   

    public void Start()
    {
        //Debug.Log(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        GetData();

    }

    // 데이터 가져오기
    public void GetData()
    {
        data.upgradeHp = GetPcData(hpID);
        data.upgradeGainExp = GetPcData(gainExpID);
        data.upgradeGaingold = GetPcData(gainGoldID);

        data.upgradeAtk = GetWeaponData(atkID);
        data.upgradeCrit = GetWeaponData(critID);
        data.upgradeCritDmg = GetWeaponData(critDmgID);
        data.upgradeAtkSpd = GetWeaponData(atkspd);
    }


    // PC 데이터를 가져오는 메서드
    public List<UpgradePC> GetPcData(int id)
    {
        List<UpgradePC> list = new List<UpgradePC>();

        for (int i = 0; i < 10; i++)
        {
            UpgradePC newData = new UpgradePC();
            //Debug.Log(id+i);

            newData.level = i + 1;
            newData.value = (float)DataManager.instance.GetData(id + i, "Value1", typeof(float));
            newData.sum = (float)DataManager.instance.GetData(id + i, "Value2", typeof(float));
            newData.exp = (int)DataManager.instance.GetData(id + i, "EXP", typeof(int));
            list.Add(newData);
        }

        return list;
    }

    // 무기 데이터를 가져오는 메서드
    public List<UpgradeWeapon> GetWeaponData(int id)
    {
        List<UpgradeWeapon> list = new List<UpgradeWeapon>();

        for (int i = 0; i < 10; i++)
        {
            UpgradeWeapon newData = new UpgradeWeapon();
            //Debug.Log(id + i);

            newData.level = (int)DataManager.instance.GetData(id + i, "LV", typeof(int));
            newData.value1 = (float)DataManager.instance.GetData(id + i, "Value1", typeof(float));
            newData.sum1 = (float)DataManager.instance.GetData(id + i, "Value2", typeof(float));
            newData.value2 = (float)DataManager.instance.GetData(id + i, "Value3", typeof(float));
            newData.sum2 = (float)DataManager.instance.GetData(id + i, "Value4", typeof(float));
            newData.exp = (int)DataManager.instance.GetData(id + i, "EXP", typeof(int));

            list.Add(newData);
        }
        return list;
    }

}