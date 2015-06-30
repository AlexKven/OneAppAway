﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



namespace OneAppAway
{
    public partial class App : global::Windows.UI.Xaml.Markup.IXamlMetadataProvider
    {
        private global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider _provider;

        /// <summary>
        /// GetXamlType(Type)
        /// </summary>
        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlType(global::System.Type type)
        {
            if(_provider == null)
            {
                _provider = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider();
            }
            return _provider.GetXamlTypeByType(type);
        }

        /// <summary>
        /// GetXamlType(String)
        /// </summary>
        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlType(string fullName)
        {
            if(_provider == null)
            {
                _provider = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider();
            }
            return _provider.GetXamlTypeByName(fullName);
        }

        /// <summary>
        /// GetXmlnsDefinitions()
        /// </summary>
        public global::Windows.UI.Xaml.Markup.XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return new global::Windows.UI.Xaml.Markup.XmlnsDefinition[0];
        }
    }
}

namespace OneAppAway.OneAppAway_XamlTypeInfo
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal partial class XamlTypeInfoProvider
    {
        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlTypeByType(global::System.Type type)
        {
            global::Windows.UI.Xaml.Markup.IXamlType xamlType;
            if (_xamlTypeCacheByType.TryGetValue(type, out xamlType))
            {
                return xamlType;
            }
            int typeIndex = LookupTypeIndexByType(type);
            if(typeIndex != -1)
            {
                xamlType = CreateXamlType(typeIndex);
            }
            if (xamlType != null)
            {
                _xamlTypeCacheByName.Add(xamlType.FullName, xamlType);
                _xamlTypeCacheByType.Add(xamlType.UnderlyingType, xamlType);
            }
            return xamlType;
        }

        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            global::Windows.UI.Xaml.Markup.IXamlType xamlType;
            if (_xamlTypeCacheByName.TryGetValue(typeName, out xamlType))
            {
                return xamlType;
            }
            int typeIndex = LookupTypeIndexByName(typeName);
            if(typeIndex != -1)
            {
                xamlType = CreateXamlType(typeIndex);
            }
            if (xamlType != null)
            {
                _xamlTypeCacheByName.Add(xamlType.FullName, xamlType);
                _xamlTypeCacheByType.Add(xamlType.UnderlyingType, xamlType);
            }
            return xamlType;
        }

        public global::Windows.UI.Xaml.Markup.IXamlMember GetMemberByLongName(string longMemberName)
        {
            if (string.IsNullOrEmpty(longMemberName))
            {
                return null;
            }
            global::Windows.UI.Xaml.Markup.IXamlMember xamlMember;
            if (_xamlMembers.TryGetValue(longMemberName, out xamlMember))
            {
                return xamlMember;
            }
            xamlMember = CreateXamlMember(longMemberName);
            if (xamlMember != null)
            {
                _xamlMembers.Add(longMemberName, xamlMember);
            }
            return xamlMember;
        }

        global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlType>
                _xamlTypeCacheByName = new global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlType>();

        global::System.Collections.Generic.Dictionary<global::System.Type, global::Windows.UI.Xaml.Markup.IXamlType>
                _xamlTypeCacheByType = new global::System.Collections.Generic.Dictionary<global::System.Type, global::Windows.UI.Xaml.Markup.IXamlType>();

        global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlMember>
                _xamlMembers = new global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlMember>();

        string[] _typeNameTable = null;
        global::System.Type[] _typeTable = null;

        private void InitTypeTables()
        {
            _typeNameTable = new string[17];
            _typeNameTable[0] = "OneAppAway.BusMap";
            _typeNameTable[1] = "Windows.UI.Xaml.Controls.UserControl";
            _typeNameTable[2] = "System.Collections.Generic.ICollection`1<OneAppAway.BusStop>";
            _typeNameTable[3] = "OneAppAway.BusStop";
            _typeNameTable[4] = "System.ValueType";
            _typeNameTable[5] = "Object";
            _typeNameTable[6] = "OneAppAway.StopDirection";
            _typeNameTable[7] = "System.Enum";
            _typeNameTable[8] = "Windows.Devices.Geolocation.BasicGeoposition";
            _typeNameTable[9] = "String";
            _typeNameTable[10] = "Int32";
            _typeNameTable[11] = "String[]";
            _typeNameTable[12] = "System.Array";
            _typeNameTable[13] = "Double";
            _typeNameTable[14] = "OneAppAway.Converters.BasicGeopositionToStringConverter";
            _typeNameTable[15] = "OneAppAway.MainPage";
            _typeNameTable[16] = "Windows.UI.Xaml.Controls.Page";

            _typeTable = new global::System.Type[17];
            _typeTable[0] = typeof(global::OneAppAway.BusMap);
            _typeTable[1] = typeof(global::Windows.UI.Xaml.Controls.UserControl);
            _typeTable[2] = typeof(global::System.Collections.Generic.ICollection<global::OneAppAway.BusStop>);
            _typeTable[3] = typeof(global::OneAppAway.BusStop);
            _typeTable[4] = typeof(global::System.ValueType);
            _typeTable[5] = typeof(global::System.Object);
            _typeTable[6] = typeof(global::OneAppAway.StopDirection);
            _typeTable[7] = typeof(global::System.Enum);
            _typeTable[8] = typeof(global::Windows.Devices.Geolocation.BasicGeoposition);
            _typeTable[9] = typeof(global::System.String);
            _typeTable[10] = typeof(global::System.Int32);
            _typeTable[11] = typeof(global::System.String[]);
            _typeTable[12] = typeof(global::System.Array);
            _typeTable[13] = typeof(global::System.Double);
            _typeTable[14] = typeof(global::OneAppAway.Converters.BasicGeopositionToStringConverter);
            _typeTable[15] = typeof(global::OneAppAway.MainPage);
            _typeTable[16] = typeof(global::Windows.UI.Xaml.Controls.Page);
        }

        private int LookupTypeIndexByName(string typeName)
        {
            if (_typeNameTable == null)
            {
                InitTypeTables();
            }
            for (int i=0; i<_typeNameTable.Length; i++)
            {
                if(0 == string.CompareOrdinal(_typeNameTable[i], typeName))
                {
                    return i;
                }
            }
            return -1;
        }

        private int LookupTypeIndexByType(global::System.Type type)
        {
            if (_typeTable == null)
            {
                InitTypeTables();
            }
            for(int i=0; i<_typeTable.Length; i++)
            {
                if(type == _typeTable[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private object Activate_0_BusMap() { return new global::OneAppAway.BusMap(); }
        private object Activate_14_BasicGeopositionToStringConverter() { return new global::OneAppAway.Converters.BasicGeopositionToStringConverter(); }
        private object Activate_15_MainPage() { return new global::OneAppAway.MainPage(); }
        private void VectorAdd_2_ICollection(object instance, object item)
        {
            var collection = (global::System.Collections.Generic.ICollection<global::OneAppAway.BusStop>)instance;
            var newItem = (global::OneAppAway.BusStop)item;
            collection.Add(newItem);
        }

        private global::Windows.UI.Xaml.Markup.IXamlType CreateXamlType(int typeIndex)
        {
            global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType xamlType = null;
            global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType userType;
            string typeName = _typeNameTable[typeIndex];
            global::System.Type type = _typeTable[typeIndex];

            switch (typeIndex)
            {

            case 0:   //  OneAppAway.BusMap
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_0_BusMap;
                userType.AddMemberName("ShownStops");
                userType.AddMemberName("Center");
                userType.AddMemberName("TopLeft");
                userType.AddMemberName("BottomRight");
                userType.AddMemberName("LatitudePerPixel");
                userType.AddMemberName("LongitudePerPixel");
                userType.AddMemberName("ZoomLevel");
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 1:   //  Windows.UI.Xaml.Controls.UserControl
                xamlType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 2:   //  System.Collections.Generic.ICollection`1<OneAppAway.BusStop>
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, null);
                userType.CollectionAdd = VectorAdd_2_ICollection;
                userType.SetIsReturnTypeStub();
                xamlType = userType;
                break;

            case 3:   //  OneAppAway.BusStop
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("System.ValueType"));
                userType.AddMemberName("Direction");
                userType.AddMemberName("Position");
                userType.AddMemberName("ID");
                userType.AddMemberName("Name");
                userType.AddMemberName("Code");
                userType.AddMemberName("LocationType");
                userType.AddMemberName("RouteIds");
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 4:   //  System.ValueType
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Object"));
                xamlType = userType;
                break;

            case 5:   //  Object
                xamlType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 6:   //  OneAppAway.StopDirection
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("System.Enum"));
                userType.AddEnumValue("Unspecified", global::OneAppAway.StopDirection.Unspecified);
                userType.AddEnumValue("N", global::OneAppAway.StopDirection.N);
                userType.AddEnumValue("NE", global::OneAppAway.StopDirection.NE);
                userType.AddEnumValue("E", global::OneAppAway.StopDirection.E);
                userType.AddEnumValue("SE", global::OneAppAway.StopDirection.SE);
                userType.AddEnumValue("S", global::OneAppAway.StopDirection.S);
                userType.AddEnumValue("SW", global::OneAppAway.StopDirection.SW);
                userType.AddEnumValue("W", global::OneAppAway.StopDirection.W);
                userType.AddEnumValue("NW", global::OneAppAway.StopDirection.NW);
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 7:   //  System.Enum
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("System.ValueType"));
                xamlType = userType;
                break;

            case 8:   //  Windows.Devices.Geolocation.BasicGeoposition
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("System.ValueType"));
                userType.SetIsReturnTypeStub();
                xamlType = userType;
                break;

            case 9:   //  String
                xamlType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 10:   //  Int32
                xamlType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 11:   //  String[]
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("System.Array"));
                userType.SetIsReturnTypeStub();
                xamlType = userType;
                break;

            case 12:   //  System.Array
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Object"));
                xamlType = userType;
                break;

            case 13:   //  Double
                xamlType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 14:   //  OneAppAway.Converters.BasicGeopositionToStringConverter
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Object"));
                userType.Activator = Activate_14_BasicGeopositionToStringConverter;
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 15:   //  OneAppAway.MainPage
                userType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.Page"));
                userType.Activator = Activate_15_MainPage;
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 16:   //  Windows.UI.Xaml.Controls.Page
                xamlType = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;
            }
            return xamlType;
        }


        private object get_0_BusMap_ShownStops(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.ShownStops;
        }
        private object get_1_BusStop_Direction(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.Direction;
        }
        private void set_1_BusStop_Direction(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.Direction = (global::OneAppAway.StopDirection)Value;
        }
        private object get_2_BusStop_Position(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.Position;
        }
        private void set_2_BusStop_Position(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.Position = (global::Windows.Devices.Geolocation.BasicGeoposition)Value;
        }
        private object get_3_BusStop_ID(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.ID;
        }
        private void set_3_BusStop_ID(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.ID = (global::System.String)Value;
        }
        private object get_4_BusStop_Name(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.Name;
        }
        private void set_4_BusStop_Name(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.Name = (global::System.String)Value;
        }
        private object get_5_BusStop_Code(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.Code;
        }
        private void set_5_BusStop_Code(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.Code = (global::System.String)Value;
        }
        private object get_6_BusStop_LocationType(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.LocationType;
        }
        private void set_6_BusStop_LocationType(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.LocationType = (global::System.Int32)Value;
        }
        private object get_7_BusStop_RouteIds(object instance)
        {
            var that = (global::OneAppAway.BusStop)instance;
            return that.RouteIds;
        }
        private void set_7_BusStop_RouteIds(object instance, object Value)
        {
            var that = (global::OneAppAway.BusStop)instance;
            that.RouteIds = (global::System.String[])Value;
        }
        private object get_8_BusMap_Center(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.Center;
        }
        private void set_8_BusMap_Center(object instance, object Value)
        {
            var that = (global::OneAppAway.BusMap)instance;
            that.Center = (global::Windows.Devices.Geolocation.BasicGeoposition)Value;
        }
        private object get_9_BusMap_TopLeft(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.TopLeft;
        }
        private object get_10_BusMap_BottomRight(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.BottomRight;
        }
        private object get_11_BusMap_LatitudePerPixel(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.LatitudePerPixel;
        }
        private object get_12_BusMap_LongitudePerPixel(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.LongitudePerPixel;
        }
        private object get_13_BusMap_ZoomLevel(object instance)
        {
            var that = (global::OneAppAway.BusMap)instance;
            return that.ZoomLevel;
        }

        private global::Windows.UI.Xaml.Markup.IXamlMember CreateXamlMember(string longMemberName)
        {
            global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember xamlMember = null;
            global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType userType;

            switch (longMemberName)
            {
            case "OneAppAway.BusMap.ShownStops":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "ShownStops", "System.Collections.Generic.ICollection`1<OneAppAway.BusStop>");
                xamlMember.Getter = get_0_BusMap_ShownStops;
                xamlMember.SetIsReadOnly();
                break;
            case "OneAppAway.BusStop.Direction":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "Direction", "OneAppAway.StopDirection");
                xamlMember.Getter = get_1_BusStop_Direction;
                xamlMember.Setter = set_1_BusStop_Direction;
                break;
            case "OneAppAway.BusStop.Position":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "Position", "Windows.Devices.Geolocation.BasicGeoposition");
                xamlMember.Getter = get_2_BusStop_Position;
                xamlMember.Setter = set_2_BusStop_Position;
                break;
            case "OneAppAway.BusStop.ID":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "ID", "String");
                xamlMember.Getter = get_3_BusStop_ID;
                xamlMember.Setter = set_3_BusStop_ID;
                break;
            case "OneAppAway.BusStop.Name":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "Name", "String");
                xamlMember.Getter = get_4_BusStop_Name;
                xamlMember.Setter = set_4_BusStop_Name;
                break;
            case "OneAppAway.BusStop.Code":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "Code", "String");
                xamlMember.Getter = get_5_BusStop_Code;
                xamlMember.Setter = set_5_BusStop_Code;
                break;
            case "OneAppAway.BusStop.LocationType":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "LocationType", "Int32");
                xamlMember.Getter = get_6_BusStop_LocationType;
                xamlMember.Setter = set_6_BusStop_LocationType;
                break;
            case "OneAppAway.BusStop.RouteIds":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusStop");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "RouteIds", "String[]");
                xamlMember.Getter = get_7_BusStop_RouteIds;
                xamlMember.Setter = set_7_BusStop_RouteIds;
                break;
            case "OneAppAway.BusMap.Center":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "Center", "Windows.Devices.Geolocation.BasicGeoposition");
                xamlMember.Getter = get_8_BusMap_Center;
                xamlMember.Setter = set_8_BusMap_Center;
                break;
            case "OneAppAway.BusMap.TopLeft":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "TopLeft", "Windows.Devices.Geolocation.BasicGeoposition");
                xamlMember.Getter = get_9_BusMap_TopLeft;
                xamlMember.SetIsReadOnly();
                break;
            case "OneAppAway.BusMap.BottomRight":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "BottomRight", "Windows.Devices.Geolocation.BasicGeoposition");
                xamlMember.Getter = get_10_BusMap_BottomRight;
                xamlMember.SetIsReadOnly();
                break;
            case "OneAppAway.BusMap.LatitudePerPixel":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "LatitudePerPixel", "Double");
                xamlMember.Getter = get_11_BusMap_LatitudePerPixel;
                xamlMember.SetIsReadOnly();
                break;
            case "OneAppAway.BusMap.LongitudePerPixel":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "LongitudePerPixel", "Double");
                xamlMember.Getter = get_12_BusMap_LongitudePerPixel;
                xamlMember.SetIsReadOnly();
                break;
            case "OneAppAway.BusMap.ZoomLevel":
                userType = (global::OneAppAway.OneAppAway_XamlTypeInfo.XamlUserType)GetXamlTypeByName("OneAppAway.BusMap");
                xamlMember = new global::OneAppAway.OneAppAway_XamlTypeInfo.XamlMember(this, "ZoomLevel", "Double");
                xamlMember.Getter = get_13_BusMap_ZoomLevel;
                xamlMember.SetIsReadOnly();
                break;
            }
            return xamlMember;
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal class XamlSystemBaseType : global::Windows.UI.Xaml.Markup.IXamlType
    {
        string _fullName;
        global::System.Type _underlyingType;

        public XamlSystemBaseType(string fullName, global::System.Type underlyingType)
        {
            _fullName = fullName;
            _underlyingType = underlyingType;
        }

        public string FullName { get { return _fullName; } }

        public global::System.Type UnderlyingType
        {
            get
            {
                return _underlyingType;
            }
        }

        virtual public global::Windows.UI.Xaml.Markup.IXamlType BaseType { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlMember ContentProperty { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlMember GetMember(string name) { throw new global::System.NotImplementedException(); }
        virtual public bool IsArray { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsCollection { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsConstructible { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsDictionary { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsMarkupExtension { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsBindable { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsReturnTypeStub { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsLocalType { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlType ItemType { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlType KeyType { get { throw new global::System.NotImplementedException(); } }
        virtual public object ActivateInstance() { throw new global::System.NotImplementedException(); }
        virtual public void AddToMap(object instance, object key, object item)  { throw new global::System.NotImplementedException(); }
        virtual public void AddToVector(object instance, object item)  { throw new global::System.NotImplementedException(); }
        virtual public void RunInitializer()   { throw new global::System.NotImplementedException(); }
        virtual public object CreateFromString(string input)   { throw new global::System.NotImplementedException(); }
    }
    
    internal delegate object Activator();
    internal delegate void AddToCollection(object instance, object item);
    internal delegate void AddToDictionary(object instance, object key, object item);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal class XamlUserType : global::OneAppAway.OneAppAway_XamlTypeInfo.XamlSystemBaseType
    {
        global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider _provider;
        global::Windows.UI.Xaml.Markup.IXamlType _baseType;
        bool _isArray;
        bool _isMarkupExtension;
        bool _isBindable;
        bool _isReturnTypeStub;
        bool _isLocalType;

        string _contentPropertyName;
        string _itemTypeName;
        string _keyTypeName;
        global::System.Collections.Generic.Dictionary<string, string> _memberNames;
        global::System.Collections.Generic.Dictionary<string, object> _enumValues;

        public XamlUserType(global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider provider, string fullName, global::System.Type fullType, global::Windows.UI.Xaml.Markup.IXamlType baseType)
            :base(fullName, fullType)
        {
            _provider = provider;
            _baseType = baseType;
        }

        // --- Interface methods ----

        override public global::Windows.UI.Xaml.Markup.IXamlType BaseType { get { return _baseType; } }
        override public bool IsArray { get { return _isArray; } }
        override public bool IsCollection { get { return (CollectionAdd != null); } }
        override public bool IsConstructible { get { return (Activator != null); } }
        override public bool IsDictionary { get { return (DictionaryAdd != null); } }
        override public bool IsMarkupExtension { get { return _isMarkupExtension; } }
        override public bool IsBindable { get { return _isBindable; } }
        override public bool IsReturnTypeStub { get { return _isReturnTypeStub; } }
        override public bool IsLocalType { get { return _isLocalType; } }

        override public global::Windows.UI.Xaml.Markup.IXamlMember ContentProperty
        {
            get { return _provider.GetMemberByLongName(_contentPropertyName); }
        }

        override public global::Windows.UI.Xaml.Markup.IXamlType ItemType
        {
            get { return _provider.GetXamlTypeByName(_itemTypeName); }
        }

        override public global::Windows.UI.Xaml.Markup.IXamlType KeyType
        {
            get { return _provider.GetXamlTypeByName(_keyTypeName); }
        }

        override public global::Windows.UI.Xaml.Markup.IXamlMember GetMember(string name)
        {
            if (_memberNames == null)
            {
                return null;
            }
            string longName;
            if (_memberNames.TryGetValue(name, out longName))
            {
                return _provider.GetMemberByLongName(longName);
            }
            return null;
        }

        override public object ActivateInstance()
        {
            return Activator(); 
        }

        override public void AddToMap(object instance, object key, object item) 
        {
            DictionaryAdd(instance, key, item);
        }

        override public void AddToVector(object instance, object item)
        {
            CollectionAdd(instance, item);
        }

        override public void RunInitializer() 
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(UnderlyingType.TypeHandle);
        }

        override public object CreateFromString(string input)
        {
            if (_enumValues != null)
            {
                int value = 0;

                string[] valueParts = input.Split(',');

                foreach (string valuePart in valueParts) 
                {
                    object partValue;
                    int enumFieldValue = 0;
                    try
                    {
                        if (_enumValues.TryGetValue(valuePart.Trim(), out partValue))
                        {
                            enumFieldValue = global::System.Convert.ToInt32(partValue);
                        }
                        else
                        {
                            try
                            {
                                enumFieldValue = global::System.Convert.ToInt32(valuePart.Trim());
                            }
                            catch( global::System.FormatException )
                            {
                                foreach( string key in _enumValues.Keys )
                                {
                                    if( string.Compare(valuePart.Trim(), key, global::System.StringComparison.OrdinalIgnoreCase) == 0 )
                                    {
                                        if( _enumValues.TryGetValue(key.Trim(), out partValue) )
                                        {
                                            enumFieldValue = global::System.Convert.ToInt32(partValue);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        value |= enumFieldValue; 
                    }
                    catch( global::System.FormatException )
                    {
                        throw new global::System.ArgumentException(input, FullName);
                    }
                }

                return value; 
            }
            throw new global::System.ArgumentException(input, FullName);
        }

        // --- End of Interface methods

        public Activator Activator { get; set; }
        public AddToCollection CollectionAdd { get; set; }
        public AddToDictionary DictionaryAdd { get; set; }

        public void SetContentPropertyName(string contentPropertyName)
        {
            _contentPropertyName = contentPropertyName;
        }

        public void SetIsArray()
        {
            _isArray = true; 
        }

        public void SetIsMarkupExtension()
        {
            _isMarkupExtension = true;
        }

        public void SetIsBindable()
        {
            _isBindable = true;
        }

        public void SetIsReturnTypeStub()
        {
            _isReturnTypeStub = true;
        }

        public void SetIsLocalType()
        {
            _isLocalType = true;
        }

        public void SetItemTypeName(string itemTypeName)
        {
            _itemTypeName = itemTypeName;
        }

        public void SetKeyTypeName(string keyTypeName)
        {
            _keyTypeName = keyTypeName;
        }

        public void AddMemberName(string shortName)
        {
            if(_memberNames == null)
            {
                _memberNames =  new global::System.Collections.Generic.Dictionary<string,string>();
            }
            _memberNames.Add(shortName, FullName + "." + shortName);
        }

        public void AddEnumValue(string name, object value)
        {
            if (_enumValues == null)
            {
                _enumValues = new global::System.Collections.Generic.Dictionary<string, object>();
            }
            _enumValues.Add(name, value);
        }
    }

    internal delegate object Getter(object instance);
    internal delegate void Setter(object instance, object value);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal class XamlMember : global::Windows.UI.Xaml.Markup.IXamlMember
    {
        global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider _provider;
        string _name;
        bool _isAttachable;
        bool _isDependencyProperty;
        bool _isReadOnly;

        string _typeName;
        string _targetTypeName;

        public XamlMember(global::OneAppAway.OneAppAway_XamlTypeInfo.XamlTypeInfoProvider provider, string name, string typeName)
        {
            _name = name;
            _typeName = typeName;
            _provider = provider;
        }

        public string Name { get { return _name; } }

        public global::Windows.UI.Xaml.Markup.IXamlType Type
        {
            get { return _provider.GetXamlTypeByName(_typeName); }
        }

        public void SetTargetTypeName(string targetTypeName)
        {
            _targetTypeName = targetTypeName;
        }
        public global::Windows.UI.Xaml.Markup.IXamlType TargetType
        {
            get { return _provider.GetXamlTypeByName(_targetTypeName); }
        }

        public void SetIsAttachable() { _isAttachable = true; }
        public bool IsAttachable { get { return _isAttachable; } }

        public void SetIsDependencyProperty() { _isDependencyProperty = true; }
        public bool IsDependencyProperty { get { return _isDependencyProperty; } }

        public void SetIsReadOnly() { _isReadOnly = true; }
        public bool IsReadOnly { get { return _isReadOnly; } }

        public Getter Getter { get; set; }
        public object GetValue(object instance)
        {
            if (Getter != null)
                return Getter(instance);
            else
                throw new global::System.InvalidOperationException("GetValue");
        }

        public Setter Setter { get; set; }
        public void SetValue(object instance, object value)
        {
            if (Setter != null)
                Setter(instance, value);
            else
                throw new global::System.InvalidOperationException("SetValue");
        }
    }
}

