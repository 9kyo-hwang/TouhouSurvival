using System;

[Serializable]
public class AttributeData
{
    public string attributeName;
    public float baseValue;     // 스탯 데이터 테이블 등으로부터 세팅되는 기본 값. 변경되지 않음
    public float currentValue;  // 버프 등의 이유로 변동되는 값.
    public float minValue;      // currentValue가 가질 수 있는 최소 값.
    public float maxValue;      // currentValue가 가질 수 있는 최대 값.

    public AttributeData(string attributeName, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        this.attributeName = attributeName;
        this.baseValue = baseValue;
        this.currentValue = baseValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}