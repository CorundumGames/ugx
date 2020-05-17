﻿using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Specialized;

using UnityEngine.UI;
using System.Collections.ObjectModel;
using NaughtyAttributes;

namespace Adrenak.UPF {
    [Serializable]
    public abstract class LayoutView<TModel, TView> : View where TModel : Model where TView : View<TModel> {
        public class ItemSelectedEventArgs : EventArgs {
            public TModel Item { get; private set; }
            public ItemSelectedEventArgs(TModel item) {
                Item = item;
            }
        }

        public event EventHandler<ItemSelectedEventArgs> OnLayoutItemSelected;
        public event EventHandler OnPulledToRefresh;

#pragma warning disable 0649
        [Header("Pull Down Refresh")]
        [SerializeField] bool pullToRefresh;
        [SerializeField] float pullRefreshDistance;
        [SerializeField] RefreshIndicator indicator;

        [Header("Unity UI Components")]
        [SerializeField] ScrollRect _scrollRect;
        public ScrollRect ScrollRect => _scrollRect;

        [SerializeField] LayoutGroup _layoutGroup;
        public LayoutGroup LayoutGroup => _layoutGroup;

        [Header("Instantiation")]
        [SerializeField] TView prefab;

        [SerializeField] Transform _container;
        public Transform Container => _container;

        ObservableCollection<TModel> items = new ObservableCollection<TModel>();
        public IList<TModel> Items {
            get => items;
            set {
                items.Clear();
                items.AddFrom(value);
            }
        }

        readonly List<TView> instantiated = new List<TView>();

        public Func<TView, string> InstanceNamer;
#pragma warning restore 0649

        void Awake() {
            items.CollectionChanged += (sender, args) => {
                switch (args.Action) {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var newItem in args.NewItems)
                            Instantiate(newItem as TModel);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var removed in args.OldItems)
                            Destroy(removed as TModel);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        foreach (var instance in instantiated)
                            Destroy(instance.Model);
                        break;
                }
            };
        }

        void Instantiate(TModel t) {
            var instance = Instantiate(prefab, _container);
            instance.Model = t;

            instance.name = InstanceNamer != null ?
                InstanceNamer(instance) :
                "#" + instance.transform.GetSiblingIndex();

            void OnSelected(object sender, EventArgs r) {
                OnLayoutItemSelected?.Invoke(this, new ItemSelectedEventArgs(instance.Model));
            }

            instance.OnViewSelected += OnSelected;
            instance.OnViewDestroyed += (sender, e) =>
                instance.OnViewSelected -= OnSelected;

            instantiated.Add(instance);
            Init(instance.Model);
        }

        void Destroy(TModel t) {
            foreach (var instance in instantiated) {
                if (instance.Model == t) {
                    Deinit(instance.Model);
                    Destroy(instance.gameObject);
                }
            }
        }

        virtual protected void Init(TModel cell) { }
        virtual protected void Deinit(TModel cell) { }

        void Update() {
            TryPullRefresh();
        }

        // ================================================
        // PULL TO REFRESH
        // ================================================
        bool isRefreshing;
        bool markForRefresh;

        void TryPullRefresh() {
            if (!pullToRefresh) return;

            // Make the list move smoothly
            if (isRefreshing)
                TopPadding = (int)Mathf.Clamp(pullRefreshDistance - NegativeDragDistance, 0, Mathf.Infinity);
            else
                TopPadding = (int)Mathf.Lerp(TopPadding, 0, Time.deltaTime / _scrollRect.decelerationRate);

            // If we're not refreshing, set the value that represents the fraction
            // of how much the list has been draged towards refresh. We only call 
            // this when not refreshing as when it IS refreshing, the Drag Distance
            // will go to 0 
            if (!isRefreshing)
                indicator.SetValue(NegativeDragDistance / pullRefreshDistance);

            // We need to do this false->true to force 
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;

            // If we have dragged it beyond the distance and it's not refreshing 
            // then we mark it for refresh. It's like a refresh dirty flag
            if (InRefreshZone && !isRefreshing && IsDragging)
                markForRefresh = true;

            // If we have marked for refresh, are under refresh zone, not dragging
            // and not refreshing => then we start refreshing
            if (markForRefresh && !InRefreshZone && !IsDragging && !isRefreshing) {
                isRefreshing = true;
                markForRefresh = false;
                indicator.SetRefreshing(true);
                OnPulledToRefresh?.Invoke(this, EventArgs.Empty);
            }
        }

        bool InRefreshZone {
            get => NegativeDragDistance > pullRefreshDistance;
        }

        // How many pixels has the scroll rect been dragged down beyond 0
        float NegativeDragDistance {
            get => -_container.GetComponent<RectTransform>().localPosition.y;
        }

        int TopPadding {
            get => _layoutGroup.padding.top;
            set => _layoutGroup.padding.top = value;
        }

        // Uses reflection to find out if the scroll rect is being dragged
        bool IsDragging {
            get => (bool)typeof(ScrollRect).GetField("m_Dragging", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_scrollRect);
        }

        public void StopRefresh() {
            isRefreshing = false;
            indicator.SetRefreshing(false);
        }
    }
}
