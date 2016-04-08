using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Les informations générales relatives à un assembly dépendent de 
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associées à un assembly.
[assembly: AssemblyTitle(AssemblyInfo.AssemblyTitle)]
[assembly: AssemblyDescription(AssemblyInfo.AssemblyDescription)]
[assembly: AssemblyConfiguration(AssemblyInfo.AssemblyConfiguration)]
[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct(AssemblyInfo.AssemblyProduct)]
[assembly: AssemblyCopyright(AssemblyInfo.AssemblyCopyright)]
[assembly: AssemblyTrademark(AssemblyInfo.AssemblyTrademark)]
[assembly: AssemblyCulture(AssemblyInfo.AssemblyCulture)]

// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly 
// aux composants COM. Si vous devez accéder à un type dans cet assembly à partir de 
// COM, affectez la valeur true à l'attribut ComVisible sur ce type.
[assembly: ComVisible(false)]

// Le GUID suivant est pour l'ID de la typelib si ce projet est exposé à COM
[assembly: Guid(AssemblyInfo.AssemblyGuid)]

// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire 
//      Numéro de build
//      Révision
//
// Vous pouvez spécifier toutes les valeurs ou indiquer les numéros de révision et de build par défaut 
// en utilisant '*', comme indiqué ci-dessous :
[assembly: AssemblyVersion(AssemblyInfo.AssemblyVersion)]
[assembly: AssemblyFileVersion(AssemblyInfo.AssemblyFileVersion)]

internal class AssemblyInfo
{
    public const string AssemblyTitle = "Oyosoft.AgenceImmobiliere.WebHost";
    public const string AssemblyDescription = "Hôte web du service WCF permettant la gestion d'une agence immobilière.";
    public const string AssemblyConfiguration = "";
    public const string AssemblyCompany = "Oyosoft";
    public const string AssemblyProduct = "Oyosoft.AgenceImmobiliere";
    public const string AssemblyCopyright = "Copyright ©  2015-2016";
    public const string AssemblyTrademark = "Oyosoft";
    public const string AssemblyCulture = "";

    public const string AssemblyGuid = "d04aecae-1cce-4197-968a-f85c28a004ae";

    public const string AssemblyVersion = "1.0.0.0";
    public const string AssemblyFileVersion = "1.0.0.0";
}