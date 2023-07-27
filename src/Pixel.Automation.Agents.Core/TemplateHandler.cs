using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Pixel.Automation.Agents.Core;

[DataContract]
public abstract class Document
{   
    [DataMember(IsRequired = true, Order = 1000)]
    public string Id { get; set; }

    [DataMember(IsRequired = true, Order = 1010)]
    public int Revision { get; set; } = 1;

    [DataMember(IsRequired = false, Order = 1020)]
    public bool IsDeleted { get; set; }

    [DataMember(IsRequired = true, Order = 1030)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DataMember(IsRequired = true, Order = 1040)]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Template Handler
/// </summary>
[DataContract]
[JsonDerivedType(typeof(DockerTemplateHandler), typeDiscriminator: nameof(DockerTemplateHandler))]
[JsonDerivedType(typeof(WindowsTemplateHandler), typeDiscriminator: nameof(WindowsTemplateHandler))]
[JsonDerivedType(typeof(LinuxTemplateHandler), typeDiscriminator: nameof(LinuxTemplateHandler))]
public abstract class TemplateHandler : Document
{
    /// <summary>
    /// Name of the handler
    /// </summary>
    [DataMember(Order = 10)]
    public string Name { get; set; }

    /// <summary>
    /// Parameters associated with the template handler
    /// </summary>
    [DataMember(Order = 20, IsRequired = true)]
    public Dictionary<string, string> Parameters { get; set; } = new();

    /// <summary>
    /// Description of the handler
    /// </summary>
    [DataMember(Order = 30, IsRequired = false)]
    public string Description { get; set; }

}

/// <summary>
/// Template handler for execution in docker environment
/// </summary>
[DataContract]
[JsonDerivedType(typeof(DockerTemplateHandler), typeDiscriminator: nameof(DockerTemplateHandler))]
public class DockerTemplateHandler : TemplateHandler
{
    /// <summary>
    /// Name of the docker compose file to use with handler
    /// </summary>
    [DataMember(Order = 100, IsRequired = true)]
    public string DockerComposeFileName { get; set; }
}

/// <summary>
/// Template handler for execution in Windows environment
/// </summary>
[DataContract]
[JsonDerivedType(typeof(WindowsTemplateHandler), typeDiscriminator: nameof(WindowsTemplateHandler))]
public class WindowsTemplateHandler : TemplateHandler
{

}

/// <summary>
/// Template handler for execution in Linux environment
/// </summary>
[DataContract]
[JsonDerivedType(typeof(LinuxTemplateHandler), typeDiscriminator: nameof(LinuxTemplateHandler))]
public class LinuxTemplateHandler : TemplateHandler
{

}

