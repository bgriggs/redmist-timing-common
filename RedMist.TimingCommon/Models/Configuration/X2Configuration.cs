using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// X2 decoder server configuration.
/// </summary>
[MessagePackObject]
public class X2Configuration
{
    /// <summary>
    /// Gets or sets the username for authenticating with the X2 decoder server.
    /// Maximum length: 128 characters.
    /// </summary>
    [Key(0)]
    [MaxLength(128)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the password for authenticating with the X2 decoder server.
    /// Maximum length: 128 characters.
    /// </summary>
    [Key(1)]
    [MaxLength(128)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server address or hostname for the X2 decoder server.
    /// Maximum length: 128 characters.
    /// </summary>
    [Key(2)]
    [MaxLength(128)]
    public string Server { get; set; } = string.Empty;
}
