namespace JsonToCSharp
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class JsonSchemaOptionPage : UserControl
    {
        private JsonSchemaOptionModel model;
        public JsonSchemaOptionPage(JsonSchemaOptionModel model)
        {
            InitializeComponent();
            this.model = model;
            DataContext = model;
        }

        public JsonSchemaOptionModel OptionsPage { get; internal set; }
    }
}
