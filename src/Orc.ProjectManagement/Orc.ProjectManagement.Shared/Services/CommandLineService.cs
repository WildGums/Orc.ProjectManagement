// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Services
{
    public class CommandLineService : ICommandLineService
    {
        /// <summary>
        /// The command argument list.
        /// </summary>
        private string[] _arguments;

        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        public string[] Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    var commandLine = System.Environment.GetCommandLineArgs();

                    _arguments = new string[commandLine.Length - 1];

                    if (_arguments.Length > 0)
                    {
                        System.Array.Copy(commandLine, 1, _arguments, 0, _arguments.Length);
                    }
                }

                return _arguments;
            }
        }
    }
}