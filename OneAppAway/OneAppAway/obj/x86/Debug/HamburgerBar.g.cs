﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway\OneAppAway\OneAppAway\HamburgerBar.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B91C9A18AAD5B30B4718DC8A679E3D8C"
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
    partial class HamburgerBar : 
        global::Windows.UI.Xaml.Controls.ContentControl, 
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
                    global::Windows.UI.Xaml.Controls.Button element1 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 51 "..\..\..\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element1).Click += this.HamburgerButton_Click;
                    #line default
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.Button element2 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 52 "..\..\..\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element2).Click += this.BandwidthButton_Click;
                    #line default
                }
                break;
            case 3:
                {
                    global::Windows.UI.Xaml.Controls.Button element3 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 72 "..\..\..\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element3).Click += this.CenterOnLocationButton_Click;
                    #line default
                }
                break;
            case 4:
                {
                    global::Windows.UI.Xaml.Controls.Button element4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 96 "..\..\..\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element4).Click += this.BandwidthButton_Click;
                    #line default
                }
                break;
            case 5:
                {
                    global::Windows.UI.Xaml.Controls.Button element5 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 121 "..\..\..\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element5).Click += this.CenterOnLocationButton_Click;
                    #line default
                }
                break;
            case 6:
                {
                    global::Windows.UI.Xaml.Controls.Button element6 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 88 "..\..\..\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element6).Click += this.HamburgerButton_Click;
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

