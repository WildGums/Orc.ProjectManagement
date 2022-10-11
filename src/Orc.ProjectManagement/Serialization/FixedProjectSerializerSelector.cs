namespace Orc.ProjectManagement.Serialization
{
    using System;
    using Catel.IoC;

    public class FixedProjectSerializerSelector<TReader, TWriter> : IProjectSerializerSelector
        where TReader : IProjectReader
        where TWriter : IProjectWriter
    {
        private readonly ITypeFactory _typeFactory;

        public FixedProjectSerializerSelector(ITypeFactory typeFactory)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            _typeFactory = typeFactory;
        }

        public IProjectReader GetReader(string location)
        {
            return _typeFactory.CreateRequiredInstance<TReader>();
        }

        public IProjectWriter GetWriter(string location)
        {
            return _typeFactory.CreateRequiredInstance<TWriter>();
        }
    }
}
