// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2017 by Andrey Noskov

using System;
using System.Text.RegularExpressions;
using Aspose.JavaAttributes;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    /// <summary>
    /// A simple ExpectedExceptionAttribute
    /// </summary>
    [JavaDelete("Autoported directly to Java's TestNG analog. No need in Java.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExpectedExceptionAttribute : NUnitAttribute, IWrapTestMethod
    {
        public ExpectedExceptionAttribute(Type type)
        {
            mExpectedExceptionType = type;
        }

        public string ExpectedMessage
        {
            get { return mExpectedMessage; }
            set { mExpectedMessage = value; }
        }

        public MessageMatch MatchType
        {
            get { return mMatchType; }
            set { mMatchType = value; }
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new ExpectedExceptionCommand(command, mExpectedExceptionType, mExpectedMessage, mMatchType);
        }

        private class ExpectedExceptionCommand : DelegatingTestCommand
        {
            private readonly Type _expectedType;
            private readonly string _expectedMessage;
            private readonly MessageMatch _matchType;

            public ExpectedExceptionCommand(TestCommand innerCommand, Type expectedType, string message, MessageMatch matchType)
                : base(innerCommand)
            {
                _expectedType = expectedType;
                _expectedMessage = message;
                _matchType = matchType;
            }

            public override TestResult Execute(TestExecutionContext context)
            {
                Type caughtType = null;
                string caughtMessage = String.Empty;

                try
                {
                    innerCommand.Execute(context);
                }
                catch (Exception ex)
                {
                    if (ex is NUnitException)
                    {
                        ex = ex.InnerException;
                        if (ex is IgnoreException)
                            throw;
                    }
                    caughtType = ex.GetType();
                    caughtMessage = ex.Message;

                }

                if (caughtType == _expectedType)
                    context.CurrentResult.SetResult(ResultState.Success);
                else if (caughtType != null)
                    context.CurrentResult.SetResult(ResultState.Failure,
                        string.Format("Expected {0} but got {1}", _expectedType.Name, caughtType.Name));
                else
                    context.CurrentResult.SetResult(ResultState.Failure,
                        string.Format("Expected {0} but no exception was thrown", _expectedType.Name));

                if (Aspose.StringUtil.HasChars(_expectedMessage))
                {
                    switch (_matchType)
                    {
                        case MessageMatch.Contains:
                        {
                            if (caughtMessage.Contains(_expectedMessage))
                                context.CurrentResult.SetResult(ResultState.Success);
                            else
                                context.CurrentResult.SetResult(ResultState.Failure,
                                    string.Format("Expected {0} but got {1}", _expectedMessage, caughtMessage));
                            break;
                        }
                        case MessageMatch.Regex:
                        {
                            if (Regex.IsMatch(caughtMessage, _expectedMessage))
                                context.CurrentResult.SetResult(ResultState.Success);
                            else
                                context.CurrentResult.SetResult(ResultState.Failure,
                                    string.Format("Expected {0} but got {1}", _expectedMessage, caughtMessage));
                            break;
                        }
                        case MessageMatch.StartsWith:
                        {
                            if (caughtMessage.StartsWith(_expectedMessage))
                                context.CurrentResult.SetResult(ResultState.Success);
                            else
                                context.CurrentResult.SetResult(ResultState.Failure,
                                    string.Format("Expected {0} but got {1}", _expectedMessage, caughtMessage));
                            break;
                        }
                        case MessageMatch.Exact:
                        default:
                        {
                            if (caughtMessage == _expectedMessage)
                                context.CurrentResult.SetResult(ResultState.Success);
                            else
                                context.CurrentResult.SetResult(ResultState.Failure,
                                    string.Format("Expected {0} but got {1}", _expectedMessage, caughtMessage));
                            break;
                        }
                    }
                }

                return context.CurrentResult;
            }
        }

        private readonly Type mExpectedExceptionType;
        private string mExpectedMessage;
        private MessageMatch mMatchType;
    }

    [JavaDelete("Autoported directly to Java's TestNG analog. No need in Java.")]
    public enum MessageMatch
    {
        /// <summary>
        /// Expect an exact match
        /// </summary>
        Exact,

        /// <summary>
        /// Expect a message containing the parameter string
        /// </summary>
        Contains,

        /// <summary>
        /// Match the regular expression provided as a parameter
        /// </summary>
        Regex,

        /// <summary>
        /// Expect a message starting with the parameter string
        /// </summary>
        StartsWith
    }
}
