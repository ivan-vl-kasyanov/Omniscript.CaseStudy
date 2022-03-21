using System.Net;

using Microsoft.Extensions.Logging;

namespace Omniscript.CaseStudy.Server.Models.MessageModels
{
    /// <summary>
    /// Model of the simple server response.
    /// </summary>
    public sealed class ServerSimpleResponseMessageModel
    {
        /// <summary>
        /// Response HTTP result.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// (Optional) Response severity level.
        /// </summary>
        public LogLevel? Severity { get; set; }

        /// <summary>
        /// (Optional) Response status message.
        /// </summary>
        public string? Message { get; set; }
    }
}