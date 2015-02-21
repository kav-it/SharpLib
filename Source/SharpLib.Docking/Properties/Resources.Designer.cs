namespace SharpLib.Docking.Properties
{
    using System;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {
        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SharpLib.Docking.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture
        {
            get { return resourceCulture; } 
            set{ resourceCulture = value; }
        }

        public static string Anchorable_AutoHide
        {
            get { return ResourceManager.GetString("Anchorable_AutoHide", resourceCulture); }
        }

        public static string Anchorable_BtnAutoHide_Hint
        {
            get { return ResourceManager.GetString("Anchorable_BtnAutoHide_Hint", resourceCulture); }
        }

        public static string Anchorable_BtnClose_Hint
        {
            get { return ResourceManager.GetString("Anchorable_BtnClose_Hint", resourceCulture); }
        }

        public static string Anchorable_CxMenu_Hint
        {
            get { return ResourceManager.GetString("Anchorable_CxMenu_Hint", resourceCulture); }
        }

        public static string Anchorable_Dock
        {
            get { return ResourceManager.GetString("Anchorable_Dock", resourceCulture); }
        }

        public static string Anchorable_DockAsDocument
        {
            get{ return ResourceManager.GetString("Anchorable_DockAsDocument", resourceCulture); }
        }

        public static string Anchorable_Float
        {
            get { return ResourceManager.GetString("Anchorable_Float", resourceCulture); }
        }

        public static string Anchorable_Hide
        {
            get { return ResourceManager.GetString("Anchorable_Hide", resourceCulture); }
        }

        public static string Document_Close
        {
            get { return ResourceManager.GetString("Document_Close", resourceCulture); }
        }

        public static string Document_CloseAllButThis
        {
            get { return ResourceManager.GetString("Document_CloseAllButThis", resourceCulture); }
        }

        public static string Document_CxMenu_Hint
        {
            get{  return ResourceManager.GetString("Document_CxMenu_Hint", resourceCulture); }
        }

        public static string Document_DockAsDocument
        {
            get { return ResourceManager.GetString("Document_DockAsDocument", resourceCulture); }
        }

        public static string Document_Float
        {
            get { return ResourceManager.GetString("Document_Float", resourceCulture); }
        }

        public static string Document_MoveToNextTabGroup
        {
            get { return ResourceManager.GetString("Document_MoveToNextTabGroup", resourceCulture); }
        }

        public static string Document_MoveToPreviousTabGroup
        {
            get { return ResourceManager.GetString("Document_MoveToPreviousTabGroup", resourceCulture); }
        }

        public static string Document_NewHorizontalTabGroup
        {
            get { return ResourceManager.GetString("Document_NewHorizontalTabGroup", resourceCulture); }
        }

        public static string Document_NewVerticalTabGroup
        {
            get { return ResourceManager.GetString("Document_NewVerticalTabGroup", resourceCulture); }
        }

        public static string Window_Maximize
        {
            get { return ResourceManager.GetString("Window_Maximize", resourceCulture); }
        }

        public static string Window_Restore
        {
            get { return ResourceManager.GetString("Window_Restore", resourceCulture); }
        }
    }
}
