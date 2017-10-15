namespace MyServer.Services.Content
{
    using System;
    using System.Linq;

    using MyServer.Common;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class ContentService : IContentService
    {
        private readonly IRepository<StaticContent, Guid> staticContent;

        public ContentService(IRepository<StaticContent, Guid> staticContent)
        {
            this.staticContent = staticContent;
        }

        public string Get(string key, LanguageEnum language)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var entry = this.staticContent.All().FirstOrDefault(x => x.ContentKey == key);

            if (entry == null)
            {
                return null;
            }
            
            switch (language)
            {
                case LanguageEnum.En:
                    return entry.ContentValueEn;
                case LanguageEnum.Bg:
                    return entry.ContentValueBg;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }   
        }

        public void Update(string key, LanguageEnum language, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var entry = this.staticContent.All().FirstOrDefault(x => x.ContentKey == key);

            if (entry == null)
            {
                entry = new StaticContent();

                switch (language)
                {
                    case LanguageEnum.En:
                        entry.ContentValueEn = value;
                        break;
                    case LanguageEnum.Bg:
                        entry.ContentValueBg = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(language), language, null);
                }

                entry.ContentKey = key;
                entry.ContentValueEn = entry.ContentValueEn ?? string.Empty;
                entry.ContentValueBg = entry.ContentValueBg ?? string.Empty;

                this.staticContent.Add(entry);
            }
            else
            {
                switch (language)
                {
                    case LanguageEnum.En:
                        entry.ContentValueEn = value;
                        break;
                    case LanguageEnum.Bg:
                        entry.ContentValueBg = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(language), language, null);
                }

                this.staticContent.Update(entry);
            }
        }
    }
}