﻿using System;

namespace Adrenak.UPF {
    [Serializable]
    public abstract class ViewModel : Bindable {
        public object Identifier;
    }
}