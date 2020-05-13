﻿using UnityEngine;

namespace Adrenak.UPF.Examples{
    public class MovieExample : MonoBehaviour {
        public MasterDetailPage page;

        public Movie[] models;

        void Start() {
            var listView = page.Master.Content as MovieListView;

            foreach (var model in models)
                listView.ItemsSource.Add(model);

            listView.OnClick += (sender, args) => 
                (page.Detail.Content as MovieView).Context = sender as Movie;
        }
    }
}