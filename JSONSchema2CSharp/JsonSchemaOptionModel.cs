//------------------------------------------------------------------------------
// <copyright file="PasteFromJsonSchemaPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace JsonToCSharp
{
    using System.Runtime.InteropServices;
    using System.Windows;
    using JsonSchema;
    using Microsoft.VisualStudio.Shell;

    [Guid("d55612b6-1cfd-406c-a875-90b0f309eba5")]
    public class JsonSchemaOptionModel : UIElementDialogPage
    {
        public bool AddNamespace { get; set; } = true;

        public bool AddJsonProperty { get; set; } = true;

        protected override UIElement Child => new JsonSchemaOptionPage(this);

        public ConverterOptions ToConverterOptions() => new ConverterOptions
        {
            AddNamespace = AddNamespace,
            AddJsonProperty = AddJsonProperty
        };
    }
}
