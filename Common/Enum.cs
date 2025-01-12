using System.ComponentModel;

namespace Common;


public enum WeatherCondition
{
    [Description("Güneşli")]
    Sunny,

    [Description("Yağmurlu")]
    Rainy,

    [Description("Fırtınalı")]
    Stormy,

    [Description("Afet Olabilir Dikkat!")]
    DisasterPossible
}