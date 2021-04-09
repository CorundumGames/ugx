﻿using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Adrenak.UGX {
    [System.Serializable]
    public class PopupState : WindowState { }

    [System.Serializable]
    public class PopupResponse { }

    public abstract class Popup<T, K> : Window<T> where T : PopupState where K : PopupResponse {
        static GameObject activePopup;

        [Obsolete("It is recommended that you use Popup.Display instead as it manages popup instancing too.")]
        public new void OpenWindow() => base.OpenWindow();

        [Obsolete("It is recommended that you use Popup.Display instead as it manages popup instancing too.")]
        public new UniTask OpenWindowAsync() => base.OpenWindowAsync();

        [Obsolete("It is recommended that you use Popup.Display instead as it manages popup instancing too.")]
        async public UniTask<K> WaitForResponse() {
            await UniTask.SwitchToMainThread();
            var response = await WaitForResponseImpl();
            await UniTask.SwitchToMainThread();
            return response;
        }

        public static void Display(string path, Action<T> cloneState, Action<K> resultCallback){
            var instance = InstantiateUGXBehaviourResource<Popup<T, K>>(path);
            instance.Display(cloneState, resultCallback);
        }

        async public void Display(Action<T> cloneState, Action<K> resultCallback) {
            var result = await Display(cloneState);
            resultCallback?.Invoke(result);
        }

        async public static UniTask<K> Display(string path, Action<T> cloneState){
            var instance = InstantiateUGXBehaviourResource<Popup<T, K>>(path);
            return await instance.Display(cloneState);
        }

        async public UniTask<K> Display(Action<T> cloneState) {
            await UniTask.WaitWhile(() => activePopup != null);

            var instance = GetClone();
            activePopup = instance.gameObject;
            cloneState?.Invoke(instance.CurrentState);

            await (instance as Window).OpenWindowAsync();
#pragma warning disable 0618
            var response = await instance.WaitForResponse();
#pragma warning restore 0618
            await instance.CloseWindowAsync();

            activePopup = null;
            Destroy(instance.gameObject);

            return response;
        }

        Popup<T, K> GetClone() {
            var instance = MonoBehaviour.Instantiate(gameObject).GetComponent<Popup<T, K>>();
            instance.transform.SetParent(transform.parent, false);
            return instance;
        }

        protected abstract UniTask<K> WaitForResponseImpl();

        sealed protected override void HandleWindowStateSet() => HandlePopupStateSet();
        protected abstract void HandlePopupStateSet();
    }
}