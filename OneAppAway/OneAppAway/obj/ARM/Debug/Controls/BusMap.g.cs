﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway\OneAppAway\OneAppAway\Controls\BusMap.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E3D8F398C49B164BD117F2ACE864C0A8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneAppAway
{
    partial class BusMap : 
        global::Windows.UI.Xaml.Controls.UserControl, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.MainGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.MainMap = (global::Windows.UI.Xaml.Controls.Maps.MapControl)(target);
                    #line 14 "..\..\..\Controls\BusMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).MapElementClick += this.MainMap_MapElementClick;
                    #line 14 "..\..\..\Controls\BusMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).SizeChanged += this.MainMap_SizeChanged;
                    #line 14 "..\..\..\Controls\BusMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).ZoomLevelChanged += this.MainMap_ZoomLevelChanged;
                    #line 14 "..\..\..\Controls\BusMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).CenterChanged += this.MainMap_CenterChanged;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

