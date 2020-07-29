﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI{
    public class ScrollRectPropagated : ScrollRect {
        bool propagate = true;

        /// <summary>
        /// Do action for all parents
        /// </summary>
        private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler {
            Transform parent = transform.parent;
            while (parent != null) {
                foreach (var component in parent.GetComponents<Component>()) {
                    if (component is T)
                        action((T)(IEventSystemHandler)component);
                }
                parent = parent.parent;
            }
        }

        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public override void OnInitializePotentialDrag(PointerEventData eventData) {
            DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
            base.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData) {
            if (propagate)
                DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
            else
                base.OnDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData) {
            if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                propagate = true;
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                propagate = true;
            else
                propagate = false;

            if (propagate)
                DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
            else
                base.OnBeginDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData) {
            if (propagate)
                DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
            else
                base.OnEndDrag(eventData);
            propagate = false;
        }
    }
}