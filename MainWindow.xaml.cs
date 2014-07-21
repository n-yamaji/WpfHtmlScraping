using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHtmlScraping
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task<string> GetHtmlAsync(string url)
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        private IEnumerable<string> GetHtmlContext(string html, string xpath)
        {
            HtmlAgilityPack.HtmlDocument doc = GetHtmlDocument(html);
            HtmlAgilityPack.HtmlNodeCollection nodes = null;
            try
            {
                nodes = doc.DocumentNode.SelectNodes(xpath);
            }
            catch (Exception)
            {
            }

            if (nodes == null)
            {
                yield return string.Empty;
            }

            foreach (var node in nodes)
            {
                yield return node.InnerHtml;
            }
        }

        private IEnumerable<string> GetHtmlAttribute(string html, string xpath, string attribute)
        {
            HtmlAgilityPack.HtmlDocument doc = GetHtmlDocument(html);
            HtmlAgilityPack.HtmlNodeCollection nodes = null;
            try
            {
                nodes = doc.DocumentNode.SelectNodes(xpath);
            }
            catch (Exception)
            {
            }

            if (nodes == null)
            {
                yield return string.Empty;
            }

            foreach (var node in nodes)
            {
                yield return node.GetAttributeValue(attribute, string.Empty);
            }
        }

        private HtmlAgilityPack.HtmlDocument GetHtmlDocument(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(html);
            return doc;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(txtUrl.Text))
            //{
            //    return;
            //}

            string url = txtUrl.Text;
            HttpClient client = new HttpClient();
            string html = await client.GetStringAsync(url);

            //string html = this.GetHtmlAsync(url).Result;
            //string tag = this.txtTag.Text;
            //string attribute = this.txtAttribute.Text;
            string xpath = this.txtXPath.Text;
            string attribute = this.txtAttribute.Text;
            Mode mode = Mode.要素;
            if (this.chkAttribute.IsChecked == true)
            {
                mode = Mode.属性;
            }

            txtResult.Dispatcher.Invoke(new Action(() =>
            {
                StringBuilder result = new StringBuilder();
                if (mode == Mode.要素)
                {
                    foreach (var text in this.GetHtmlContext(html, xpath))
                    {
                        result.AppendLine(text);
                        result.AppendLine();
                    }
                }
                else
                {
                    foreach (var text in this.GetHtmlAttribute(html, xpath, attribute))
                    {
                        result.AppendLine(text);
                        result.AppendLine();
                    }
                }

                txtResult.Text = result.ToString(); ;
            }));
        }

        private enum Mode
        {
            要素 = 0,
            属性 = 1,
        }
    }
}
