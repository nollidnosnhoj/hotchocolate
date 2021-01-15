using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;

namespace StrawberryShake.CodeGeneration.Analyzers.Models
{
    public class OperationModel
    {
        public OperationModel(
            NameString name,
            ObjectType type,
            DocumentNode document,
            OperationDefinitionNode operation,
            IReadOnlyList<ArgumentModel> arguments,
            OutputTypeModel resultType,
            IReadOnlyList<LeafTypeModel> leafTypes,
            IReadOnlyList<InputObjectTypeModel> inputObjectTypes,
            IReadOnlyList<OutputTypeModel> outputTypeModels)
        {
            Name = name.EnsureNotEmpty(nameof(name));
            Type = type ?? 
                throw new ArgumentNullException(nameof(type));
            Document = document ?? 
                throw new ArgumentNullException(nameof(document));
            Operation = operation ?? 
                throw new ArgumentNullException(nameof(operation));
            Arguments = arguments ?? 
                throw new ArgumentNullException(nameof(arguments));
            ResultType = resultType ?? 
                throw new ArgumentNullException(nameof(resultType));
            LeafTypes = leafTypes ?? 
                throw new ArgumentNullException(nameof(leafTypes));
            InputObjectTypes = inputObjectTypes ?? 
                throw new ArgumentNullException(nameof(inputObjectTypes));
            OutputTypeModels = outputTypeModels ?? 
                throw new ArgumentNullException(nameof(outputTypeModels));
        }

        public string Name { get; }

        public ObjectType Type { get; }

        public DocumentNode Document { get; }

        public OperationDefinitionNode Operation { get; }

        public IReadOnlyList<ArgumentModel> Arguments { get; }

        public OutputTypeModel ResultType { get; }

        public IReadOnlyList<LeafTypeModel> LeafTypes { get; }

        public IReadOnlyList<InputObjectTypeModel> InputObjectTypes { get; }

        public IReadOnlyList<OutputTypeModel> OutputTypeModels { get; }

        public IEnumerable<OutputTypeModel> GetImplementations(OutputTypeModel outputTypeModel)
        {
            if (outputTypeModel is null)
            {
                throw new ArgumentNullException(nameof(outputTypeModel));
            }

            foreach (var model in OutputTypeModels)
            {
                if (model.Implements.Contains(outputTypeModel))
                {
                    yield return model;
                }
            }
        }

    }
}
