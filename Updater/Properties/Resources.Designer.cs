﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Updater.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Updater.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Файл с настройками повреждён..
        /// </summary>
        internal static string ErrorFileInfoBad {
            get {
                return ResourceManager.GetString("ErrorFileInfoBad", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Файл с настройками не существует..
        /// </summary>
        internal static string ErrorFileInfoNotExists {
            get {
                return ResourceManager.GetString("ErrorFileInfoNotExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to file not found in archive.
        /// </summary>
        internal static string ExceptionFileNotFoundInArchive {
            get {
                return ResourceManager.GetString("ExceptionFileNotFoundInArchive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://api.github.com.
        /// </summary>
        internal static string GitHubApiUrl {
            get {
                return ResourceManager.GetString("GitHubApiUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com.
        /// </summary>
        internal static string GitHubUrl {
            get {
                return ResourceManager.GetString("GitHubUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Прервать текущую операцию и закрыть программу?.
        /// </summary>
        internal static string QuestionClosing {
            get {
                return ResourceManager.GetString("QuestionClosing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Текущая версия уже установлена. Продолжить?.
        /// </summary>
        internal static string QuestionVersionCompare {
            get {
                return ResourceManager.GetString("QuestionVersionCompare", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Проверить.
        /// </summary>
        internal static string TextBtnCheck {
            get {
                return ResourceManager.GetString("TextBtnCheck", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Установить.
        /// </summary>
        internal static string TextBtnInstall {
            get {
                return ResourceManager.GetString("TextBtnInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Обновить.
        /// </summary>
        internal static string TextBtnUpdate {
            get {
                return ResourceManager.GetString("TextBtnUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to загрузка....
        /// </summary>
        internal static string TextVersionDownloading {
            get {
                return ResourceManager.GetString("TextVersionDownloading", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ошибка.
        /// </summary>
        internal static string TextVersionError {
            get {
                return ResourceManager.GetString("TextVersionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to нет.
        /// </summary>
        internal static string TextVersionNotExists {
            get {
                return ResourceManager.GetString("TextVersionNotExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to чтение....
        /// </summary>
        internal static string TextVersionReading {
            get {
                return ResourceManager.GetString("TextVersionReading", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to распаковка....
        /// </summary>
        internal static string TextVersionZipExtracting {
            get {
                return ResourceManager.GetString("TextVersionZipExtracting", resourceCulture);
            }
        }
    }
}
