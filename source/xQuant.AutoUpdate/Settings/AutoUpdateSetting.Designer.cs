﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.5466
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace xQuant.AutoUpdate.Settings {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class AutoUpdateSetting : global::System.Configuration.ApplicationSettingsBase {
        
        private static AutoUpdateSetting defaultInstance = ((AutoUpdateSetting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AutoUpdateSetting())));
        
        public static AutoUpdateSetting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConnectionString.xeq_app")]
        public string APP {
            get {
                return ((string)(this["APP"]));
            }
            set {
                this["APP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConnectionString.xeq_md")]
        public string Md {
            get {
                return ((string)(this["Md"]));
            }
            set {
                this["Md"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConnectionString.xeq_trd")]
        public string Trd {
            get {
                return ((string)(this["Trd"]));
            }
            set {
                this["Trd"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConnectionString.xeq_trd_exh")]
        public string Exh {
            get {
                return ((string)(this["Exh"]));
            }
            set {
                this["Exh"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("xQuant.Run.Server.exe.config")]
        public string ConfigName {
            get {
                return ((string)(this["ConfigName"]));
            }
            set {
                this["ConfigName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("xQuant.Run.Server.exe")]
        public string ServerProgramName {
            get {
                return ((string)(this["ServerProgramName"]));
            }
            set {
                this["ServerProgramName"] = value;
            }
        }
    }
}
