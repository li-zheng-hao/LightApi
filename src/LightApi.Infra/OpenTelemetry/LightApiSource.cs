using System.Diagnostics;

namespace LightApi.Infra.OpenTelemetry;

public  class LightApiSource
{
    public static string SourceName = "LightApi";
    
    public static ActivitySource Source = new ActivitySource(SourceName);
}