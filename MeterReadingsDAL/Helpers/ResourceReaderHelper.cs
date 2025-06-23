using System.Reflection;

namespace MeterReadingsDAL.Helpers;

public static class ResourceReaderHelper
{
    /// <summary>
    /// uses reflection to read an embedded file and returns the content
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static StreamReader GetEmbeddedCsv(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(fileName));

        if (resourceName == null)
            throw new FileNotFoundException($"Embedded resource '{fileName}' not found.");

        var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"Unable to load embedded resource stream for '{resourceName}'.");

        return new StreamReader(stream);
    }
}
