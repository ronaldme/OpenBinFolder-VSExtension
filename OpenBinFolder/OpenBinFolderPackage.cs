﻿//------------------------------------------------------------------------------
// <copyright file="OpenBinFolderPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;
using System.IO;

namespace OpenBinFolder
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0")] // Info on this package for Help/About
    [Guid(OpenBinFolderPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class OpenBinFolderPackage : AsyncPackage
    {
        /// <summary>
        /// OpenBinFolderPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "21572e2d-a591-4cd1-b073-c4ae5e3f6be6";

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenBinFolderPackage"/> class.
        /// </summary>
        public OpenBinFolderPackage()
        {
            //get the menu service
            OleMenuCommandService _menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            //create the click method for the menu command we are adding to the right click of the project menu
            CommandID _openBinFolderCommand = new CommandID(Guid.Parse("{02AB237F-F580-4278-A02B-8DA88483528E}"), int.Parse("3B9ACA01", System.Globalization.NumberStyles.HexNumber));
            MenuCommand _openBinFolderMenu = new MenuCommand(OpenBinFolderWithFileExplorer, _openBinFolderCommand);
            _menuCommandService.AddCommand(_openBinFolderMenu);
        }

        private void OpenBinFolderWithFileExplorer(object sender, EventArgs e)
        {
            //grab the DTE object
            var dte = (DTE2)this.GetService(typeof(DTE));
            //Get the active projects within the solution.
            Array _activeProjects = (Array)dte.ActiveSolutionProjects;

            //loop through each active project
            foreach (Project _activeProject in _activeProjects)
            {
                //get the directory path based on the project file.
                string _projectPath = Path.GetDirectoryName(_activeProject.FullName);
                //get the output path based on the active configuration
                string _projectOutputPath = _activeProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
                //combine the project path and output path to get the bin path
                string _projectBinPath = Path.Combine(_projectPath, _projectOutputPath);

                //if the directory exists (already built) then open that directory
                //in windows explorer using the diagnostics.process object
                if (Directory.Exists(_projectBinPath))
                {
                    System.Diagnostics.Process.Start(_projectBinPath);
                }
                else
                {
                    //if the directory doesnt exist, open the project directory.
                    System.Diagnostics.Process.Start(_projectPath);
                }
            }
        }
    }
}
