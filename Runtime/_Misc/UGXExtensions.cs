﻿using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace Adrenak.UGX {
    public static class UGXExtensions {
        public static void SetColor(this Image image, Color color){
            image.color = color;
        }

        public static void AddRange<T>(this IList<T> destination, IList<T> source) {
            foreach (var element in source)
                destination.Add(element);
        }

        public static float GetLeftExit(this RectTransform rt) {
            var parentRect = rt.parent.GetComponent<RectTransform>().rect;
            return -parentRect.width / 2 - rt.rect.width / 2;
        }

        public static float GetRightExit(this RectTransform rt) {
            var parentRect = rt.parent.GetComponent<RectTransform>().rect;
            return parentRect.width / 2 + rt.rect.width / 2;
        }

        public static float GetTopExit(this RectTransform rt) {
            var parentRect = rt.parent.GetComponent<RectTransform>().rect;
            return parentRect.height / 2 + rt.rect.height / 2;
        }

        public static float GetBottomExit(this RectTransform rt) {
            var parentRect = rt.parent.GetComponent<RectTransform>().rect;
            return -parentRect.width / 2 - rt.rect.height / 2;
        }

        public static float GetLeft(this RectTransform rt) {
            return rt.position.x - rt.rect.width * rt.lossyScale.x / 2;
        }

        public static float GetRight(this RectTransform rt) {
            return rt.position.x + rt.rect.width * rt.lossyScale.x / 2;
        }

        public static float GetTop(this RectTransform rt) {
            return rt.position.y + rt.rect.height * rt.lossyScale.y / 2;
        }

        public static float GetBottom(this RectTransform rt) {
            return rt.position.y - rt.rect.height * rt.lossyScale.y / 2;
        }

        public static Vector2 GetTopLeft(this RectTransform rt) {
            var left = rt.GetLeft();
            var top = rt.GetTop();
            return new Vector2(left, top);
        }

        public static Vector2 GetTopRight(this RectTransform rt) {
            var right = rt.GetRight();
            var top = rt.GetTop();
            return new Vector2(right, top);
        }

        public static Vector2 GetBottomLeft(this RectTransform rt) {
            var left = rt.GetLeft();
            var bottom = rt.GetBottom();
            return new Vector2(left, bottom);
        }

        public static Vector2 GetBottomRight(this RectTransform rt) {
            var right = rt.GetRight();
            var bottom = rt.GetBottom();
            return new Vector2(right, bottom);
        }

        public static bool IsVisible(this RectTransform rt, out bool? fully) {
            bool IsInHorizontalBand(Vector2 point) =>
                point.y >= -1 && point.y <= Screen.height + 1;

            bool IsInVerticalBand(Vector2 point) =>
                point.x >= -1 && point.x <= Screen.width + 1;

            var points = new Vector2[]{
                rt.GetTopLeft(),
                rt.GetTopRight(),
                rt.GetBottomRight(),
                rt.GetBottomLeft()
            };

            int count = 0;
            foreach(var  point in points) 
                count += IsInHorizontalBand(point) && IsInVerticalBand(point) ? 1 : 0;

            if(count == 4) {
                fully = true;
                return true;
			}

            bool IsAboveScreen(Vector2 v) =>
                v.y > Screen.height + 1;

            bool IsBelowScreen(Vector2 v) =>
                v.y < -1;

            bool IsLeftOfScreen(Vector2 v) =>
                v.x < -1;

            bool IsRightOfScreen(Vector2 v) =>
                v.x > Screen.width + 1;

			List<Func<Vector2, bool>> checks = new List<Func<Vector2, bool>> {
				IsAboveScreen,
				IsBelowScreen,
				IsLeftOfScreen,
				IsRightOfScreen
			};

			foreach (var check in checks) {
                count = 0;
                foreach(var point in points) 
                    count += check(point) ? 1 : 0;
                if(count == 4) {
                    fully = false;
                    return false;
				}			    
			}

            fully = false;
            return true;
        }

        public static void EnsureKey<T, K>(this IDictionary<T, K> dict, T t, K k){
            if (!dict.ContainsKey(t))
                dict.Add(t, k);
        }

        public static void EnsureExists<T>(this List<T> list, T t){
            if (!list.Contains(t))
                list.Add(t);
        }

        public static void EnsureDoesntExist<T>(this List<T> list, T t){
            if (list.Contains(t))
                list.Remove(t);
        }
    }
}