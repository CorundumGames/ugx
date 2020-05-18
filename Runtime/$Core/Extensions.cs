﻿using System.Collections.Generic;

namespace Adrenak.UPF {
    public static class Extensions {
        public static void AddFrom<T>(this IList<T> destination, IList<T> source) {
            foreach (var element in source)
                destination.Add(element);
        }
    }
}