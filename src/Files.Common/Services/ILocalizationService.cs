namespace Files.Common.Services
{
    public interface ILocalizationService
    {
        string LocalizeFromResourceKey(string resourceKey);
    }
}