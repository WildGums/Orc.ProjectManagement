// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedProjectSerializerSelector.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Serialization
{
    using Catel.IoC;

    public class FixedProjectSerializerSelector<TReader, TWriter> : IProjectSerializerSelector
        where TReader : IProjectReader
        where TWriter : IProjectWriter
    {
        #region IProjectSerializerSelector Members
        public IProjectReader GetReader(string location)
        {
            var typeFactory = TypeFactory.Default;
            return typeFactory.CreateInstance<TReader>();
        }

        public IProjectWriter GetWriter(string location)
        {
            var typeFactory = TypeFactory.Default;
            return typeFactory.CreateInstance<TWriter>();
        }
        #endregion
    }
}