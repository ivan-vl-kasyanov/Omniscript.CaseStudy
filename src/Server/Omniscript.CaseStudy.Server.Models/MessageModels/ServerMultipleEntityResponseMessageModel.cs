using System.Collections.Generic;
using System.Net;

using Microsoft.Extensions.Logging;

namespace Omniscript.CaseStudy.Server.Models.MessageModels
{
    /// <summary>
    /// Model of the multiple entity server response.
    /// </summary>
    public sealed class ServerMultipleEntityResponseMessageModel
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

        /// <summary>
        /// Full type name of the single responded entity.
        /// </summary>
        public string SingleEntityFullTypeName { get; set; }

        /// <summary>
        /// (Optional) Responded entities.
        /// </summary>
        public IEnumerable<object>? Entities { get; set; }

        /// <summary>
        /// (Optional) Full type name of the responded metadata.
        /// </summary>
        public string? MetadataFullTypeName { get; set; }

        /// <summary>
        /// (Optional) Responded metadata.
        /// </summary>
        public object? Metadata { get; set; }

        /// <summary>
        /// Constructor of the model of the single entity server response.
        /// </summary>
        /// <param name="statusCode">Response HTTP result.</param>
        /// <param name="severity">(Optional) Response severity level.</param>
        /// <param name="message">(Optional) Response status message.</param>
        /// <param name="singleEntityFullTypeName">Full type name of the single responded entity.</param>
        /// <param name="entities">(Optional) Responded entities.</param>
        /// <param name="metadataFullTypeName">(Optional) Full type name of the responded metadata.</param>
        /// <param name="metadata">(Optional) Responded metadata.</param>
        public ServerMultipleEntityResponseMessageModel(
            HttpStatusCode statusCode,
            LogLevel? severity,
            string? message,
            string singleEntityFullTypeName,
            IEnumerable<object>? entities,
            string? metadataFullTypeName,
            object? metadata)
        {
            StatusCode = statusCode;
            Severity = severity;
            Message = message;
            SingleEntityFullTypeName = singleEntityFullTypeName;
            Entities = entities;
            MetadataFullTypeName = metadataFullTypeName;
            Metadata = metadata;
        }
    }
}