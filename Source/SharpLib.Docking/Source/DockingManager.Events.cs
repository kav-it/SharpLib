using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using SharpLib.Docking.Controls;
using SharpLib.Docking;

namespace SharpLib.Docking
{
    /// <summary>
    /// События "Dependency properties"
    /// </summary>
    public partial class DockingManager
    {
        #region Методы

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
        }

        private static object CoerceLayoutValue(DependencyObject d, object value)
        {
            if (value == null)
            {
                return new LayoutRoot
                {
                    RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
                };
            }

            ((DockingManager)d).OnLayoutChanging();

            return value;
        }

        private static void OnDocumentPaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentPaneTemplateChanged(e);
        }

        private static void OnDocumentPaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentPaneControlStyleChanged(e);
        }

        private static void OnAnchorablePaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorablePaneTemplateChanged(e);
        }

        private static void OnAnchorablePaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorablePaneControlStyleChanged(e);
        }

        private static void OnDocumentHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentHeaderTemplateChanged(e);
        }

        private static object CoerceDocumentHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null && d.GetValue(DocumentHeaderTemplateSelectorProperty) != null)
            {
                return null;
            }
            return value;
        }

        private static void OnDocumentHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentHeaderTemplateSelectorChanged(e);
        }

        private static object CoerceDocumentHeaderTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnDocumentTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentTitleTemplateChanged(e);
        }

        private static object CoerceDocumentTitleTemplateValue(DependencyObject d, object value)
        {
            if (value != null && d.GetValue(DocumentTitleTemplateSelectorProperty) != null)
            {
                return null;
            }

            return value;
        }

        private static void OnDocumentTitleTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentTitleTemplateSelectorChanged(e);
        }

        private static object CoerceDocumentTitleTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnAnchorableTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorableTitleTemplateChanged(e);
        }

        private static object CoerceAnchorableTitleTemplateValue(DependencyObject d, object value)
        {
            if (value != null && d.GetValue(AnchorableTitleTemplateSelectorProperty) != null)
            {
                return null;
            }
            return value;
        }

        private static void OnAnchorableTitleTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorableTitleTemplateSelectorChanged(e);
        }

        private static void OnAnchorableHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorableHeaderTemplateChanged(e);
        }

        private static object CoerceAnchorableHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null && d.GetValue(AnchorableHeaderTemplateSelectorProperty) != null)
            {
                return null;
            }

            return value;
        }

        private static void OnAnchorableHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorableHeaderTemplateSelectorChanged(e);
        }

        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLayoutRootPanelChanged(e);
        }

        private static void OnRightSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseRightSidePanelChanged(e);
        }

        private static void OnLeftSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLeftSidePanelChanged(e);
        }

        private static void OnTopSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseTopSidePanelChanged(e);
        }

        private static void OnBottomSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseBottomSidePanelChanged(e);
        }

        private static void OnLayoutItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLayoutItemContainerStyleChanged(e);
        }

        private static void OnAutoHideWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAutoHideWindowChanged(e);
        }

        private static void OnLayoutItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLayoutItemTemplateChanged(e);
        }

        private static void OnLayoutItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLayoutItemTemplateSelectorChanged(e);
        }

        private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentsSourceChanged(e);
        }

        private static void OnAnchorablesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseAnchorablesSourceChanged(e);
        }

        private static void OnActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).InternalSetActiveContent(e.NewValue);
            ((DockingManager)d).RaiseActiveContentChanged(e);
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseThemeChanged(e);
        }

        private static void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseDocumentPaneMenuItemHeaderTemplateChanged(e);
        }

        private static object CoerceDocumentPaneMenuItemHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty) != null)
            {
                return null;
            }
            if (value == null)
            {
                return d.GetValue(DocumentHeaderTemplateProperty);
            }

            return value;
        }

        private static void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RiaseDocumentPaneMenuItemHeaderTemplateSelectorChanged(e);
        }

        private void RiaseDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && DocumentPaneMenuItemHeaderTemplate != null)
            {
                DocumentPaneMenuItemHeaderTemplate = null;
            }
        }

        private static object CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnLayoutItemContainerStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).RaiseLayoutItemContainerStyleSelectorChanged(e);
        }

        private void RaiseLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
        {
            if (oldLayout != null)
            {
                oldLayout.PropertyChanged -= OnLayoutRootPropertyChanged;
                oldLayout.Updated -= OnLayoutRootUpdated;
            }

            foreach (var fwc in _fwList.ToArray())
            {
                fwc.KeepContentVisibleOnClose = true;
                fwc.InternalClose();
            }

            _fwList.Clear();

            DetachDocumentsSource(oldLayout, DocumentsSource);
            DetachAnchorablesSource(oldLayout, AnchorablesSource);

            if (oldLayout != null && Equals(oldLayout.Manager, this))
            {
                oldLayout.Manager = null;
            }

            ClearLogicalChildrenList();
            DetachLayoutItems();

            Layout.Manager = this;

            AttachLayoutItems();
            AttachDocumentsSource(newLayout, DocumentsSource);
            AttachAnchorablesSource(newLayout, AnchorablesSource);

            if (IsLoaded)
            {
                LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
                TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
                RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
                BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;

                foreach (var fw in Layout.FloatingWindows.ToArray())
                {
                    if (fw.IsValid)
                    {
                        _fwList.Add(CreateUIElementForModel(fw) as LayoutFloatingWindowControl);
                    }
                }
            }

            if (newLayout != null)
            {
                newLayout.PropertyChanged += OnLayoutRootPropertyChanged;
                newLayout.Updated += OnLayoutRootUpdated;
            }

            if (LayoutChanged != null)
            {
                LayoutChanged(this, EventArgs.Empty);
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void RaiseDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseDocumentHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseDocumentHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && DocumentHeaderTemplate != null)
            {
                DocumentHeaderTemplate = null;
            }

            if (DocumentPaneMenuItemHeaderTemplateSelector == null)
            {
                DocumentPaneMenuItemHeaderTemplateSelector = DocumentHeaderTemplateSelector;
            }
        }

        private void RaiseDocumentTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseDocumentTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentTitleTemplate = null;
            }
        }

        private void RaiseAnchorableTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseAnchorableTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && AnchorableTitleTemplate != null)
            {
                AnchorableTitleTemplate = null;
            }
        }

        private void RaiseAnchorableHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseAnchorableHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                AnchorableHeaderTemplate = null;
            }
        }

        private void RaiseLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private void RaiseRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private void RaiseLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private void RaiseTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private void RaiseBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private void RaiseAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private void RaiseLayoutItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseLayoutItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachDocumentsSource(Layout, e.OldValue as IEnumerable);
            AttachDocumentsSource(Layout, e.NewValue as IEnumerable);
        }

        private void RaiseAnchorablesSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachAnchorablesSource(Layout, e.OldValue as IEnumerable);
            AttachAnchorablesSource(Layout, e.NewValue as IEnumerable);
        }

        private void RaiseActiveContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ActiveContentChanged != null)
            {
                ActiveContentChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseThemeChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldTheme = e.OldValue as Theme;
            var newTheme = e.NewValue as Theme;
            var resources = Resources;
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                {
                    resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
                }
            }

            if (newTheme != null)
            {
                resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = newTheme.GetResourceUri()
                });
            }

            foreach (var fwc in _fwList)
            {
                fwc.UpdateThemeResources(oldTheme);
            }

            if (_navigatorWindow != null)
            {
                _navigatorWindow.UpdateThemeResources();
            }

            if (_overlayWindow != null)
            {
                _overlayWindow.UpdateThemeResources();
            }
        }

        private void RaiseDocumentPaneMenuItemHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void RaiseLayoutItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachLayoutItems();
        }

        private void RaiseLayoutItemContainerStyleSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachLayoutItems();
        }

        #endregion
    }
}