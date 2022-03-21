using System;

using Omniscript.CaseStudy.Server.Models.MessageModels;

namespace Omniscript.CaseStudy.Server.Models
{
    /// <summary>
    /// Provides resolving for message types.
    /// </summary>
    public static class MessageTypeResolver
    {
        /// <summary>
        /// Resolves message types.
        /// </summary>
        /// <param name="messageTypeName">Short message type name.</param>
        /// <returns>Message type.</returns>
        public static Type Resolve(this string? messageTypeName)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            var resolvedType = messageTypeName switch
            {
                null => throw new ArgumentNullException($"{nameof(messageTypeName)} cannot be NULL."),
                var mtn when mtn.Trim() == String.Empty => throw new ArgumentOutOfRangeException($"{nameof(messageTypeName)} cannot be empty."),

                var mtn when mtn.CompareNames<ServerSimpleResponseMessageModel>() => typeof(ServerSimpleResponseMessageModel),
                var mtn when mtn.CompareNames<ServerSingleEntityResponseMessageModel>() => typeof(ServerSingleEntityResponseMessageModel),
                var mtn when mtn.CompareNames<ServerMultipleEntityResponseMessageModel>() => typeof(ServerMultipleEntityResponseMessageModel),

                _ => throw new ArgumentException($"{nameof(messageTypeName)} is unknown: \"{messageTypeName}\".")
            };
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            return resolvedType;
        }

        private static bool CompareNames<T>(this string name)
        {
            return name.ToUpper() == typeof(T).Name.ToUpper();
        }
    }
}