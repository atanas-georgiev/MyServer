namespace MyServer.Services.Content
{
    using MyServer.Common;

    public interface IContentService
    {
        string Get(string key, LanguageEnum language);

        void Update(string key, LanguageEnum language, string value);
    }
}
