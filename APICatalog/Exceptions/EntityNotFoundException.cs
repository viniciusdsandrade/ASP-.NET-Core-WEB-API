using System.Runtime.Serialization;

namespace APICatalog.Exceptions;

[Serializable]
public class EntityNotFoundException : Exception
{
    // Constructors without serialization are fine
    public EntityNotFoundException() { }
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }

    // Updated serialization constructor
    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        // Retrieve serialized data (adjust property names if needed)
        var message = info.GetString("Message");
        var innerException = (Exception?)info.GetValue("InnerException", typeof(Exception));

        // Initialize base class with deserialized data
        HResult = info.GetInt32("HResult"); // Preserve the HResult value
        base.HelpLink = info.GetString("HelpURL"); // Set the help link if applicable
        if (innerException != null) base.Source = innerException.Source; // Set the source if applicable
    }

    // Method to serialize the exception data
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        // Add exception data to the SerializationInfo
        info.AddValue("Message", Message);
        info.AddValue("InnerException", InnerException);
        info.AddValue("HResult", HResult);
    }
}