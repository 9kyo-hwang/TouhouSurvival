using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeDataSO", menuName = "Scriptable Objects/AttributeDataSO")]
public class AttributeDataSO : ScriptableObject
{
    [Header("Attribute Settings")]
    public string attributeName;
    public float baseValue;                         // 스탯 데이터 테이블 등으로부터 세팅되는 기본 값. 변경되지 않음
    [HideInInspector] public float currentValue;    // 버프 등의 이유로 변동되는 값.
    public float minValue;         // currentValue가 가질 수 있는 최소 값.
    public float maxValue;         // currentValue가 가질 수 있는 최대 값.

    // 스크립트가 로드될 때나 인스펙터 상에서 값이 변경됐을 때마다 호출. currentValue의 기본값을 baseValue로 세팅
    private void OnValidate()
    {
        currentValue = baseValue;

        if (Mathf.Approximately(minValue, -1f))
        {
            minValue = float.MinValue;
        }

        if (Mathf.Approximately(maxValue, -1f))
        {
            maxValue = float.MaxValue;
        }
    }
    
    public void ResetCurrentValue()
    {
        currentValue = baseValue;
    }
}
