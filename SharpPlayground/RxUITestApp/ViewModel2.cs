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
    public class ViewModel2 : ReactiveObject
    {
        static string key = "moAb5YRNPZQNjUvDpy3ckYbFLIJo+6mMbXdTEQn8iqU";
        static BingSearchContainer bing = new BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/Search/"))
        { Credentials = new NetworkCredential(key, key) };


        public ReactiveList<FlickrPhoto> SearchResults { get; set; }

        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { this.RaiseAndSetIfChanged(ref _searchTerm, value); }
        }

        public ReactiveCommand<List<FlickrPhoto>> Search { get; set; }
        public ReactiveCommand<List<FlickrPhoto>> LoadMoreItems { get; set; }

        public ViewModel2()
        {
            SearchResults = new ReactiveList<FlickrPhoto>();
            var canSearch = this.WhenAny(x => x.SearchTerm, x => !String.IsNullOrWhiteSpace(x.Value));

            Search = ReactiveCommand.CreateAsyncTask(canSearch, async _ =>  {  return await GetSearchResultFromBing(this.SearchTerm); });
            LoadMoreItems = ReactiveCommand.CreateAsyncTask(canSearch, async x => { return await LoadMore((int)x);  });

            Search.Subscribe(results =>
            {
                SearchResults.Clear();
                foreach (var item in results)
                {
                    SearchResults.Add(item);
                }
            });

            LoadMoreItems.Subscribe(results =>
            {
                foreach (var item in results)
                {
                    SearchResults.Add(item);
                }
            });

            Search.ThrownExceptions.Subscribe(ex => { UserError.Throw("Potential network connectivity Error", ex); });
            LoadMoreItems.ThrownExceptions.Subscribe(ex => { UserError.Throw("Problem when downloading additional items", ex); });

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .InvokeCommand(this, x => x.Search);

            //SearchTerm = "british cat";
        }

        public Task<List<FlickrPhoto>> GetSearchResultFromBing(string searchTerm)
        {
            var t = Task.Run(() =>
            {
                var query = bing.Image(searchTerm, null, null, null, null, null, "Size:Small");
                query = query.AddQueryOption("$top", 7);

                var results = query.Execute();

                var res = results.Select(x => new FlickrPhoto
                {
                    Title = x.Title,
                    Description = x.Title,
                    Url = x.MediaUrl
                }).ToList();

                return res;
            });


            return t;
        }

        public Task<List<FlickrPhoto>> LoadMore(int count)
        {
            var t = Task.Run(() =>
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

                return res;
            });

            return t;
        }
    }
}
