//------------------------------------------------------------------------------
// <copyright file="PasteFromJsonSchema.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace JsonToCSharp
{
    using System;
    using System.ComponentModel.Design;
    using System.Windows;
    using EnvDTE;
    using EnvDTE80;
    using JsonSchema;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class PasteFromJsonSchema
    {
        /// <summary>
        /// Json Command ID.
        /// </summary>
        private const int JsonCommandId = 0x0200;

        /// <summary>
        /// Shema Command ID.
        /// </summary>
        private const int SchemaCommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid SchemaCommandSet = new Guid("c984471a-f28b-4def-9680-825d40045942");

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid JsonCommandSet = new Guid("c984471a-f28b-4def-9680-825d40045943");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasteFromJsonSchema" /> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PasteFromJsonSchema(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                AddMenuItem(commandService, SchemaMenuItemCallback, SchemaCommandSet, SchemaCommandId);
                AddMenuItem(commandService, JsonMenuItemCallback, JsonCommandSet, JsonCommandId);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        private static PasteFromJsonSchema Instance { get; set; }

        private JsonSchemaOptionModel Options => (package as PasteFromJsonSchemaPackage)?.Options;

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new PasteFromJsonSchema(package);
        }

        private void AddMenuItem(OleMenuCommandService commandService, EventHandler menuItemCallback, Guid commandSet, int commandId)
        {
            var menuCommandId = new CommandID(commandSet, commandId);
            var menuItem = new OleMenuCommand(menuItemCallback, menuCommandId);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private DTE2 GetDte() => _dte ?? (_dte = Package.GetGlobalService(typeof(SDTE)) as DTE2);

        private void JsonMenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var schemaText = Clipboard.GetText();
                var doc = JToken.Parse(schemaText, new JsonLoadSettings { CommentHandling = CommentHandling.Load });
                var classText = JTokenConverter.Convert(doc, Options.ToConverterOptions());

                var dte = GetDte();
                try
                {
                    dte.UndoContext.Open("Paste JSON Schema as Class");
                    var selection = (TextSelection)dte.ActiveDocument.Selection;
                    if (selection != null)
                    {
                        selection.Insert(classText);
                        dte.ActiveDocument.Activate();
                        dte.ExecuteCommand("Edit.FormatDocument");
                    }
                }
                finally
                {
                    dte.UndoContext.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Copied text is not a JSON!\r\n" + ex.Message, "JSON Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var pasteCommand = (OleMenuCommand)sender;

            // hidden by default
            pasteCommand.Visible = false;
            pasteCommand.Enabled = false;

            var activeDoc = GetDte().ActiveDocument;
            if (activeDoc?.ProjectItem?.ContainingProject != null)
            {
                if (activeDoc.Language.Equals("CSharp"))
                {
                    // show command if active document is a csharp file.
                    pasteCommand.Visible = true;
                }

                // enable command, if command is visible and there is text on the clipboard
                pasteCommand.Enabled = pasteCommand.Visible && Clipboard.ContainsText();
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void SchemaMenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var schemaText = Clipboard.GetText();
                var obj = JsonConvert.DeserializeObject<JsonObject>(schemaText);
                var classText = obj.ToString(Options.ToConverterOptions());

                var dte = GetDte();
                try
                {
                    dte.UndoContext.Open("Paste JSON Schema as Class");
                    var selection = (TextSelection)dte.ActiveDocument.Selection;
                    if (selection != null)
                    {
                        selection.Insert(classText);
                        dte.ActiveDocument.Activate();
                        dte.ExecuteCommand("Edit.FormatDocument");
                    }
                }
                finally
                {
                    dte.UndoContext.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Copied text is not a JSON Schema!\r\n" + ex.Message, "JSON Schema Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}