using System;

namespace Xi
{
    /// <summary>
    /// The argument list for a message.
    /// </summary>
    public class ArgumentList
    {
        /// <summary>
        /// Create an ArgumentList.
        /// </summary>
        /// <param name="argumentsString">Defines the argument list.</param>
        public ArgumentList(string argumentsString)
        {
            XiHelper.ArgumentNullCheck(argumentsString);
            // TODO: make sure '>' won't screw up the parser when used in an XML file
            string[] argumentStrings = argumentsString.Split('>');
            argumentTypes = new Type[argumentStrings.Length];
            argumentValues = new object[argumentStrings.Length];
            SetUpArguments(argumentStrings);
        }

        /// <summary>
        /// The types of the arguments from first to last.
        /// </summary>
        public Type[] ArgumentTypes { get { return argumentTypes; } }

        /// <summary>
        /// The values of the arguments from first to last.
        /// </summary>
        public object[] ArgumentValues { get { return argumentValues; } }

        /// <summary>
        /// Populate the calling context where needed.
        /// </summary>
        public void PopulateContext(Simulatable context)
        {
            for (int i = 0; i < argumentValues.Length; ++i)
                if (argumentValues[i] == ArgumentSymbol.Context)
                    argumentValues[i] = context;
        }

        /// <summary>
        /// Populate the event argument where needed.
        /// </summary>
        public void PopulateEventArgument(object eventArgument)
        {
            for (int i = 0; i < argumentValues.Length; ++i)
                if (argumentValues[i] == ArgumentSymbol.Event)
                    argumentValues[i] = eventArgument;
        }

        /// <summary>
        /// Clear the calling context.
        /// Important to do to avoid keeping around stale object references.
        /// </summary>
        public void ClearContext(Simulatable context)
        {
            for (int i = 0; i < argumentValues.Length; ++i)
                if (argumentValues[i] == context)
                    argumentValues[i] = ArgumentSymbol.Context;
        }

        private void SetUpArguments(string[] argumentStrings)
        {
            for (int i = 0; i < argumentStrings.Length; ++i)
                SetUpArgument(argumentStrings[i], i);
        }

        private void SetUpArgument(string argumentString, int argumentIndex)
        {
            Argument argument = new Argument();
            argument.Populate(argumentString);
            argumentTypes[argumentIndex] = argument.Type;
            argumentValues[argumentIndex] = argument.Value;
        }

        private readonly Type[] argumentTypes;
        private readonly object[] argumentValues;
    }
}
