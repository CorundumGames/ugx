﻿using System;
using UnityWeld.Binding;

namespace Adrenak.UPF {
    [Serializable]
    [Binding]
    public abstract class FormViewModel : ViewModel {
        public event EventHandler OnSubmit;
        public event EventHandler OnCancel;

        public void Submit() {
            OnSubmit?.Invoke(this, EventArgs.Empty);
        }

        public void Cancel() {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
