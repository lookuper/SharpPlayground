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
using Bing;
using System.Net;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace RxUITestApp
{
    public class AppViewModel : ReactiveObject
    {
        static string key = "moAb5YRNPZQNjUvDpy3ckYbFLIJo+6mMbXdTEQn8iqU";
        static BingSearchContainer bing = new BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/Search/"))
        { Credentials = new NetworkCredential(key, key) };

        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { this.RaiseAndSetIfChanged(ref _searchTerm, value); }
        }

        private ObservableAsPropertyHelper<ObservableCollection<FlickrPhoto>> _searchResults;
        public ObservableCollection<FlickrPhoto> SearchResults
        {
            get { return _searchResults.Value; }
        }

        private ObservableAsPropertyHelper<Visibility> _spinnerVisibility;
        public Visibility SpinnerVisibility
        {
            get { return _spinnerVisibility.Value; }
        }

        public ReactiveCommand<Object> ExecuteSearch { get; protected set; }
        public ReactiveCommand<Object> LoadMoreItems { get; protected set; }

        public AppViewModel(ReactiveCommand<Object> testExecuteSearchCommand = null, IObservable<ObservableCollection<FlickrPhoto>> testSearchResult = null)
        {       
            ExecuteSearch = testExecuteSearchCommand ?? ReactiveCommand.Create();
            LoadMoreItems = ReactiveCommand.Create();

            this.ObservableForProperty(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .DistinctUntilChanged()
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);

            _spinnerVisibility = ExecuteSearch.IsExecuting
                .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, x => x.SpinnerVisibility, Visibility.Hidden);

            IObservable<ObservableCollection<FlickrPhoto>> results;
            if (testSearchResult != null)
                results = testSearchResult;
            else
                results = ExecuteSearch.Select(term => GetSearchResultFromBing((string)term));

            _searchResults = results.ToProperty(this, x => x.SearchResults, new ObservableCollection<FlickrPhoto>());

            LoadMoreItems
                .Select(x => LoadMore((int)x))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    foreach (var item in x)
                    {
                        SearchResults.Add(item);
                    }
                });


            SearchTerm = "british cat";
        }

        public static ObservableCollection<FlickrPhoto> GetSearchResultFromBing(string searchTerm)
        {
            var query = bing.Image(searchTerm, null, null, null, null, null, "Size:Small");
            query = query.AddQueryOption("$top", 7);
            //query = query.AddQueryOption("$size", "small");

            var results = query.Execute();            

            var res = results.Select(x => new FlickrPhoto
            {
                Title = x.Title,
                Description = x.Title,
                Url = x.MediaUrl
            }).ToList();

            return new ObservableCollection<FlickrPhoto>(res);
        }

        public ObservableCollection<FlickrPhoto> LoadMore(int count)
        {
            var query = bing.Image(SearchTerm, null, null, null, null, null, "Size:Small");
            query = query.AddQueryOption("$skip", SearchResults.Count);
            query = query.AddQueryOption("$top", 7);

            var results = query.Execute();

            var res = results.Select(x => new FlickrPhoto
            {
                Title = x.Title,
                Description = x.Title,
                Url = x.MediaUrl
            }).ToList();

            return new ObservableCollection<FlickrPhoto>(res);
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
