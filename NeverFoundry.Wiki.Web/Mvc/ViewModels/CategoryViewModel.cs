using NeverFoundry.DataStorage;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    public class CategoryViewModel : WikiItemViewModel
    {
        public IList<IGrouping<string, CategoryPageViewModel>>? Articles { get; set; }
        public IList<IGrouping<string, CategoryFileViewModel>>? Files { get; set; }
        public IList<IGrouping<string, SubcategoryViewModel>>? Subcategories { get; set; }

        public CategoryViewModel(
            WikiRouteData data,
            string html,
            bool isDiff,
            IList<IGrouping<string, CategoryPageViewModel>>? articles,
            IList<IGrouping<string, CategoryFileViewModel>>? files,
            IList<IGrouping<string, SubcategoryViewModel>>? subcategories) : base(data, html, isDiff)
        {
            Articles = articles;
            Files = files;
            Subcategories = subcategories;
        }

        public static async Task<CategoryViewModel> NewAsync(WikiRouteData data, WikiItemViewModel vm)
        {
            var articles = new List<Article>();
            var files = new List<WikiFile>();
            var subCategories = new List<Category>();
            if (data.WikiItem is Category category)
            {
                foreach (var id in category.ChildIds)
                {
                    var child = await DataStore.GetItemAsync<Article>(id).ConfigureAwait(false);
                    if (child is null)
                    {
                        continue;
                    }
                    if (child is Category subCategory)
                    {
                        subCategories.Add(subCategory);
                    }
                    else if (child is WikiFile file)
                    {
                        files.Add(file);
                    }
                    else if (child is Article article)
                    {
                        articles.Add(article);
                    }
                }
            }

            return new CategoryViewModel(
                data,
                vm.Html,
                vm.IsDiff,
                articles
                    .Select(x => new CategoryPageViewModel(x.Title, x.WikiNamespace))
                    .GroupBy(x => StringInfo.GetNextTextElement(x.Title, 0))
                    .ToList(),
                files
                    .Select(x => new CategoryFileViewModel(x.Title, x.FileSize))
                    .GroupBy(x => StringInfo.GetNextTextElement(x.Title, 0))
                    .ToList(),
                subCategories
                    .Select(x => new SubcategoryViewModel(x.Title, x.ChildIds.Count))
                    .GroupBy(x => StringInfo.GetNextTextElement(x.Title, 0))
                    .ToList());
        }
    }
}
