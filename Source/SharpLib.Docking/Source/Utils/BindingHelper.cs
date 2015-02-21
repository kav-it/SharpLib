using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace SharpLib.Docking.Controls
{
    internal class BindingHelper
    {
        #region Методы

        public static void RebindInactiveBindings(DependencyObject dependencyObject)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(dependencyObject.GetType()))
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(property);
                if (dpd != null)
                {
                    var binding = BindingOperations.GetBindingExpressionBase(dependencyObject, dpd.DependencyProperty);
                    if (binding != null)
                    {

                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
                            {
                                dependencyObject.ClearValue(dpd.DependencyProperty);
                                BindingOperations.SetBinding(dependencyObject, dpd.DependencyProperty, binding.ParentBindingBase);
                            });
                        }
                    }
                }
            }
        }

        #endregion
    }
}