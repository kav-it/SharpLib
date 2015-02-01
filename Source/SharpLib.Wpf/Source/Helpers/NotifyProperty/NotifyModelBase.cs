using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SharpLib.Wpf
{
    public abstract class NotifyModelBase : INotifyPropertyChanged
    {
        #region События

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Методы

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaisePropertyChanged<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            RaisePropertyChanged(property.GetMemberInfo().Name);
        }

        #endregion
    }
}