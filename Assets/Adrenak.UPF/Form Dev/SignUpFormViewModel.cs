﻿using System;
using UnityEngine;
using UnityWeld.Binding;

namespace Adrenak.UPF.Examples {
    [Serializable]
    [Binding]
    public class SignUpFormViewModel : ViewModel {
        public event EventHandler OnSubmit;
        public event EventHandler OnCancel;

        [SerializeField] string email;
        [Binding]
        public string Email {
            get => email;
            set => Set(ref email, value);
        }

        [SerializeField] string password;
        [Binding]
        public string Password {
            get => password;
            set => Set(ref password, value);
        }

        [SerializeField] bool agreeWithTnc;
        [Binding]
        public bool AgreeWithTNC{
            get => agreeWithTnc;
            set => Set(ref agreeWithTnc, value);
        }

        public void Submit() {
            OnSubmit?.Invoke(this, EventArgs.Empty);
        }

        public void Cancel() {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
