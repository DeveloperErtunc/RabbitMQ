using System.ComponentModel;

namespace Common;

public class WeatherReport
{
    public WeatherReport()
    {
        
    }
    public WeatherReport(string dateTime, WeatherCondition weatherCondition)
    {
        DateTime = dateTime;
        WeatherCondition = weatherCondition;
    }

    public string DateTime { get; set; }
    public WeatherCondition WeatherCondition { get; set; }
    public static string GetDescription(Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }

}
