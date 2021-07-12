﻿using System;
using System.Collections.Generic;

using UnityEngine;

namespace Adrenak.UGX {
    [DisallowMultipleComponent]
    public sealed class UGXEventListener : MonoBehaviour {
        public bool debug;

        List<Action<UGXEvent>> handlers = new List<Action<UGXEvent>>();

        Dictionary<string, List<Func<GameObject, object, bool>>> idHandlers
            = new Dictionary<string, List<Func<GameObject, object, bool>>>();

        public void OnUGXEventWithID(string id, Func<GameObject, object, bool> handler) {
            if (idHandlers.ContainsKey(id))
                idHandlers[id].Add(handler);
            else {
                idHandlers.Add(id, new List<Func<GameObject, object, bool>>());
                idHandlers[id].Add(handler);
            }
            if (debug) {
                string msg = $"Subscribed to {id}. " +
                $"Total subscriptions for ID: {idHandlers[id].Count}";
                Debug.Log(msg, gameObject);
            }
        }

        public void OnUGXEvent(Action<UGXEvent> handler) {
            handlers.Add(handler);
            if (debug) {
                string msg = $"Subscribed to all events." +
                $"Total subscriptions: {handlers.Count}";
                Debug.Log(msg, gameObject);
            }
        }

        public bool ProcessUGXEvent(UGXEvent ugxEvent) {
            if (debug) {
                string msg = $"Received event: {ugxEvent}";
                Debug.Log(msg, gameObject);
            }

            if (idHandlers.ContainsKey(ugxEvent.id)) {
                bool propagate = true;
                foreach (var handler in idHandlers[ugxEvent.id]) {
                    var response = handler(ugxEvent.sender, ugxEvent.data);
                    if (!response)
                        propagate = false;
                }
                return propagate;
            }
            return true;
        }
    }
}