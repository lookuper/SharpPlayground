using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Xml.Linq;

namespace RxUITestApp
{
    public class AppViewModel : ReactiveObject
    {
        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { this.RaiseAndSetIfChanged(ref _searchTerm, value); }
        }

        private ObservableAsPropertyHelper<List<FlickrPhoto>> _searchResults;
        public List<FlickrPhoto> SearchResult
        {
            get { return _searchResults.Value; }
        }

        private ObservableAsPropertyHelper<Visibility> _spinnerVisibility;
        public Visibility SpinnerVisibility
        {
            get { return _spinnerVisibility.Value; }
        }

        public ReactiveCommand<Object> ExecuteSearch { get; protected set; }

        public AppViewModel(ReactiveCommand<Object> testExecuteSearchCommand = null, IObservable<FlickrPhoto> testSearchResult = null)
        {
            var i = 5;
            //GetSearchResultFromFlickr("cat");
        }

        public static List<FlickrPhoto> GetSearchResultFromFlickr(string searchTerm)
        {
            var doc = XDocument.Load(String.Format(CultureInfo.InvariantCulture,
                "http://api.flickr.com/services/feeds/photos_public.gne?tags={0}&format=rss_200",
                HttpUtility.UrlEncode(searchTerm)));

            if (doc.Root == null)
                return null;

            var titles = doc.Root.Descendants("{http://search.yahoo.com/mrss/}title")
                .Select(x => x.Value)
                .ToList();

            var tagRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var descriptions = doc.Root.Descendants("{http://search.yahoo.com/mrss/}description")
                .Select(x => tagRegex.Replace(HttpUtility.HtmlDecode(x.Value), ""))
                .ToList();

            var items = titles.Zip(descriptions,
                (t, d) => new FlickrPhoto { Title = t, Description = d })
                .ToArray();

            var urls = doc.Root.Descendants("{http://search.yahoo.com/mrss/}thumbnail")
                .Select(x => x.Attributes("url").First().Value)
                .ToList();

            var res = items.Zip(urls, (item, url) => { item.Url = url; return item; })
                .ToList();

            return res;
        }

    }

    public class FlickrPhoto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
