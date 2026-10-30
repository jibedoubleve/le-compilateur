namespace Compilateur.Web.Helpers;

public static class FileHelpers
{
    
    public static string GetFileName(string name) => name[(name.LastIndexOfAny(['/', '\\']) + 1)..];

    public static string GetFolderName(string name)
    {
        var index = name.LastIndexOfAny(['/', '\\']);
        return index < 0 ? "general" : name[..index];
    }
}