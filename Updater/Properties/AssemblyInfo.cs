using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Обновление ПО")]
[assembly: AssemblyDescription("Обновление программ")]
[assembly: AssemblyCompany("П3тр0виЧъ")]
[assembly: AssemblyProduct("Обновление ПО")]
[assembly: AssemblyCopyright("© П3тр0виЧъ")]
[assembly: AssemblyTrademark("П3тр0виЧъ™")]
[assembly: AssemblyCulture("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

[assembly: Guid("72748ba7-238e-479d-89d7-4d6c751455c4")]

[assembly: AssemblyVersion("1.2.*")]
[assembly: AssemblyFileVersion("1.2.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]