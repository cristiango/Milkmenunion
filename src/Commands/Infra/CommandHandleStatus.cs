using System;
using System.Collections.Generic;

namespace MilkmenUnion.Commands.Infra
{
    public class CommandHandleStatus
    {
        public static readonly CommandHandleStatus Successs = new CommandHandleStatus();

        public CommandHandleStatus()
        {
            Success = true;
        }

        public CommandHandleStatus(
            CommandErrorType errorType,
            string errorSummary = null,
            IEnumerable<KeyValuePair<string, string>> errorFields = null,
            string systemError = null,
            DomainError domainError = null)
        {
            ErrorType = errorType;
            ErrorSummary = errorSummary;
            DomainError = domainError;
            ErrorFields = errorFields ?? new List<KeyValuePair<string, string>>();
            SystemError = systemError ?? string.Empty;
        }

        /// <summary>
        ///     Indicates whether the command was successfully handled.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        ///     The identity of the error for de-referencing. Null if successful.
        /// </summary>
        public Guid? ErrorId { get; }

        /// <summary>
        ///     The error type.
        /// </summary>
        public CommandErrorType ErrorType { get; }

        /// <summary>
        ///     Error summary. Null if successful.
        /// </summary>
        public string ErrorSummary { get; }

        /// <summary>
        ///     A collection of error  message related to field.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ErrorFields { get; }

        /// <summary>
        ///     The system error if any. System errors are written to log and not displayed to user.
        /// </summary>
        public string SystemError { get; }

        /// <summary>
        ///     The Domain Error, id any. Will be null if ErrorType != CommandErrorType.Domain
        /// </summary>
        public DomainError DomainError { get; }

        public override string ToString()
        {
            return $"{nameof(Success)}: {Success}, {nameof(ErrorType)}: {ErrorType}, " +
                   $"{nameof(ErrorFields)}: {ErrorFields.ToFormattedString()}, " +
                   $"{nameof(SystemError)}: {SystemError}";
        }
    }
}