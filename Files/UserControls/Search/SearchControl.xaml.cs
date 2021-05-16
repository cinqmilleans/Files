using Files.Filesystem.Search;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls.Search
{
    public sealed partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            this.InitializeComponent();
        }
    }

    public class TextBoxEx : RichEditBox
    {
        private readonly FolderSearchOption option = new FolderSearchOption();

        public TextBoxEx()
        {
            TextChanged += TextBoxEx_TextChanged;
        }

        private void TextBoxEx_TextChanged(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            var text = string.Join(" ", option.Parameters.Select(parameter => ToRichTextFormat(parameter)));
            Document.SetText(TextSetOptions.FormatRtf, @"{\rtf1\ansi " + text + "}");

            static string ToRichTextFormat (IFolderSearchParameter parameter)
            {
                if (parameter is INamedFolderSearchParameter named)
                {
                    return $"\\b {named.Name} \\b0 : \\b {named.Value} \\b0";
                }
                return parameter.Value;
            }
        }
    }
}
