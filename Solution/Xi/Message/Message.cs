using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Xi
{
    /// <summary>
    /// A dynamic message.
    /// TODO: make messages able to be configured from a centralized message system via an XML file
    /// instead of from individual Simulatables.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Try to invoke messages in the given context.
        /// Message definition may be an empty string for no messages.
        /// Message definitions are separated by a '@' with no white space.
        /// </summary>
        public static void TryInvokeMessages(Simulatable context, string messageDefinitions, object eventArgument)
        {
            XiHelper.ArgumentNullCheck(context, messageDefinitions);
            if (messageDefinitions.Length == 0) return; // OPTIMIZATION
            if (!HasMultipleMessageDefinitions(messageDefinitions)) TryInvokeMessage(context, messageDefinitions, eventArgument); // OPTIMIZATION
            else
            {
                string[] messageDefinitionArray = ToMessageDefinitionArray(messageDefinitions);
                foreach (string messageDefinition in messageDefinitionArray) TryInvokeMessage(context, messageDefinition, eventArgument);
            }
        }

        /// <summary>
        /// Try to invoke a message in the given context.
        /// Message definition may be an empty string for no message.
        /// </summary>
        public static void TryInvokeMessage(Simulatable context, string messageDefinition, object eventArgument)
        {
            XiHelper.ArgumentNullCheck(context, messageDefinition);
            try
            {
                InvokeMessage(context, messageDefinition, eventArgument);
            }
            catch (Exception e)
            {
                Trace.Fail(e.Message);
            }
        }

        /// <summary>
        /// Invoke messages in the given context.
        /// Message definition may be an empty string for no messages.
        /// Message definitions are separated by a '@' with no white space.
        /// </summary>
        public static void InvokeMessages(Simulatable context, string messageDefinitions, object eventArgument)
        {
            XiHelper.ArgumentNullCheck(context, messageDefinitions);
            if (messageDefinitions.Length == 0) return; // OPTIMIZATION
            if (!HasMultipleMessageDefinitions(messageDefinitions)) InvokeMessage(context, messageDefinitions, eventArgument); // OPTIMIZATION
            else
            {
                string[] messageDefinitionArray = ToMessageDefinitionArray(messageDefinitions);
                foreach (string messageDefinition in messageDefinitionArray) InvokeMessage(context, messageDefinition, eventArgument);
            }
        }

        /// <summary>
        /// Invoke a message in the given context.
        /// Message definition may be an empty string for no message.
        /// </summary>
        public static void InvokeMessage(Simulatable context, string messageDefinition, object eventArgument)
        {
            XiHelper.ArgumentNullCheck(context, messageDefinition);
            if (messageDefinition.Length == 0) return;
            Message message;
            if (!messageDictionary.TryGetValue(messageDefinition, out message))
                messageDictionary.Add(messageDefinition, message = new Message(messageDefinition));
            message.Invoke(context, eventArgument);
        }

        /// <summary>
        /// Create a Message.
        /// </summary>
        /// <param name="messageDefinition">
        /// The message definition.
        /// TODO: expand on the format of a message definition.
        /// </param>
        public Message(string messageDefinition)
        {
            XiHelper.ArgumentNullCheck(messageDefinition);
            string[] messageParts = PartitionMessage(messageDefinition);
            SetUpDestinationParts(messageParts);
            SetUpName(messageParts);
            if (HasArguments(messageParts)) SetUpArguments(messageParts);
        }

        /// <summary>
        /// Invoke the message in the given invocation context.
        /// </summary>
        public void Invoke(Simulatable context, object eventArgument)
        {
            XiHelper.ArgumentNullCheck(context);
            Simulatable destination = ResolveDestination(context);
            if (destination != null) InvokeOn(context, destination, eventArgument); // TODO: consider logging if the destination isn't found
        }

        private string[] PartitionMessage(string messageDefinition)
        {
            string[] messageParts = messageDefinition.Split(':');
            ValidateMessageParts(messageDefinition, messageParts);
            return messageParts;
        }

        private void SetUpDestinationParts(string[] messageParts)
        {
            string destination = messageParts[0];
            ValidateDestination(destination);
            destinationParts = destination.Split('/');
        }

        private void SetUpName(string[] messageParts)
        {
            name = messageParts[1];
            ValidateName(name);
        }

        private void SetUpArguments(string[] messageParts)
        {
            string argumentsString = messageParts[2];
            arguments = new ArgumentList(argumentsString);
        }

        private Simulatable ResolveDestination(Simulatable context)
        {
            return context.GetSimulatableRelative(destinationParts);
        }

        private void InvokeOn(Simulatable context, object instance, object eventArgument)
        {
            MethodInfo method = ResolveMethod(instance);
            if (method == null) return;
            arguments.PopulateContext(context);
            arguments.PopulateEventArgument(eventArgument);
            try { method.Invoke(instance, arguments.ArgumentValues); }
            finally { arguments.ClearContext(context); }
        }

        private MethodInfo ResolveMethod(object instance)
        {
            Type type = instance.GetType();
            return type.GetMethod(name, arguments.ArgumentTypes);
        }

        private bool HasArguments(string[] messageParts)
        {
            return messageParts.Length > 2;
        }

        private static bool HasMultipleMessageDefinitions(string messageDefinitions)
        {
            return messageDefinitions.Contains("@");
        }

        private static string[] ToMessageDefinitionArray(string messageDefinitions)
        {
            // OPTIMIZATION: memoized to avoid garbage
            string[] messageDefinitionArray;
            if (!messageDefinitionArrays.TryGetValue(messageDefinitions, out messageDefinitionArray))
                messageDefinitionArrays.Add(messageDefinitions, messageDefinitionArray = messageDefinitions.Split('@'));
            return messageDefinitionArray;
        }

        private static void ValidateMessageParts(string messageDefinition, string[] parts)
        {
            if (parts.Length < 2 || parts.Length > 3)
                throw new ArgumentException("Invalid message definition '" + messageDefinition + "'.");
        }

        private static void ValidateDestination(string destination)
        {
            if (destination.Length == 0)
                throw new ArgumentException("Message destination must not be an empty string.");
        }

        private static void ValidateName(string name)
        {
            if (name.Length == 0)
                throw new ArgumentException("Message name must not be an empty string.");
        }

        private static readonly Dictionary<string, string[]> messageDefinitionArrays = new Dictionary<string,string[]>();
        private static readonly Dictionary<string, Message> messageDictionary = new Dictionary<string, Message>();
        private string[] destinationParts;
        private string name;
        private ArgumentList arguments;
    }
}
