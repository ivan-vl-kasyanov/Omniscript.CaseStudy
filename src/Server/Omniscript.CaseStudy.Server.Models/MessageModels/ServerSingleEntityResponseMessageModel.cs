using System.Net;

using Microsoft.Extensions.Logging;

namespace Omniscript.CaseStudy.Server.Models.MessageModels
{
    /// <summary>
    /// Model of the single entity server response.
    /// </summary>
    public sealed class ServerSingleEntityResponseMessageModel
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
        /// Full type name of the responded entity.
        /// </summary>
        public string EntityFullTypeName { get; set; }

        /// <summary>
        /// (Optional) Responded entity.
        /// </summary>
        public object? Entity { get; set; }

        /// <summary>
        /// Constructor of the model of the single entity server response.
        /// </summary>
        /// <param name="statusCode">Response HTTP result.</param>
        /// <param name="severity">(Optional) Response severity level.</param>
        /// <param name="message">(Optional) Response status message.</param>
        /// <param name="entityFullTypeName">Full type name of the responded entity.</param>
        /// <param name="entity">(Optional) Responded entity.</param>
        public ServerSingleEntityResponseMessageModel(
            HttpStatusCode statusCode,
            LogLevel? severity,
            string? message,
            string entityFullTypeName,
            object? entity)
        {
            StatusCode = statusCode;
            Severity = severity;
            Message = message;
            EntityFullTypeName = entityFullTypeName;
            Entity = entity;
        }
    }
}