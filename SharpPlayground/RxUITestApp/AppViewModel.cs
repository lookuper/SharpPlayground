using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Xml.Linq;
using HtmlAgilityPack;

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
        public List<FlickrPhoto> SearchResults
        {
            get { return _searchResults.Value; }
        }

        private ObservableAsPropertyHelper<Visibility> _spinnerVisibility;
        public Visibility SpinnerVisibility
        {
            get { return _spinnerVisibility.Value; }
        }

        public ReactiveCommand<Object> ExecuteSearch { get; protected set; }

        public AppViewModel(ReactiveCommand<Object> testExecuteSearchCommand = null, IObservable<List<FlickrPhoto>> testSearchResult = null)
        {
            ExecuteSearch = testExecuteSearchCommand ?? ReactiveCommand.Create();

            this.ObservableForProperty(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .DistinctUntilChanged()
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);

            _spinnerVisibility = ExecuteSearch.IsExecuting
                .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, x => x.SpinnerVisibility, Visibility.Hidden);

            IObservable<List<FlickrPhoto>> results;
            if (testSearchResult != null)
                results = testSearchResult;
            else
                results = ExecuteSearch.Select(term => GetSearchResultFromGoogle((string)term));
                //results = ExecuteSearch.Select(term => GetSearchResultFromFlickr((string)term));

            _searchResults = results.ToProperty(this, x => x.SearchResults, new List<FlickrPhoto>());
            SearchTerm = "cat";
        }

        public static List<FlickrPhoto> GetSearchResultFromGoogle(string searchTerm)
        {
            //var doc = XDocument.Load(String.Format(CultureInfo.InvariantCulture,
            //    "http://www.google.com.ua/#q={0}",
            //    HttpUtility.UrlEncode(searchTerm)));

            var web = new HtmlWeb();            
            var doc = web.Load(String.Format("http://www.google.com.ua/#q={0}&tbm=isch", searchTerm));

            //var nodes = GetUrls(doc.DocumentNode.InnerHtml);
            var xpath = "//*[@id=\"rg_s\"]/div";
            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"rg_s\"]/div")?.ToList();
            //*[@id="rg_s"]/div[1]/a/img
            //*[@id="rg_s"]/div[2]/a/img
            //*[@id="rg_s"]/div[1]/a/img

            return null;
        }

        private static List<string> GetUrls(string html)
        {
            var urls = new List<string>();
            int ndx = html.IndexOf("class=\"images_table\"", StringComparison.Ordinal);
            ndx = html.IndexOf("<img", ndx, StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("src=\"", ndx, StringComparison.Ordinal);
                ndx = ndx + 5;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("<img", ndx, StringComparison.Ordinal);
            }
            return urls;
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
