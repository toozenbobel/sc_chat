using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace Krempel.WP7.Core.Controls
{
    [TemplatePart(Name=HtmlTextBlock.PART_ItemsControl, Type=typeof(ItemsControl))]
    public class HtmlTextBlock : Control
    {
        private const string PART_ItemsControl = "PART_ItemsControl";

        static HtmlTextBlock()
        {
            HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(HtmlTextBlock),  new PropertyMetadata(HtmlChanged));
            
        }

        public HtmlTextBlock()
        {
            DefaultStyleKey = typeof(HtmlTextBlock);
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        public static readonly DependencyProperty HtmlProperty;

        private ItemsControl internalItemsControl;

        private static void HtmlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HtmlTextBlock instance = (HtmlTextBlock)o;
            if(instance.internalItemsControl != null)
                instance.AppendHtml(e.NewValue.ToString());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            internalItemsControl = (ItemsControl)base.GetTemplateChild(HtmlTextBlock.PART_ItemsControl);
            
            if (!String.IsNullOrWhiteSpace(Html))
            {
                if (textBoxes == null || textBoxes.Count == 0)
                {
                    AppendHtml(Html);
                }
                else
                {
                    foreach (var rtb in textBoxes)
                    {
                        internalItemsControl.Items.Add(rtb);
                    }
                }
            }
        }

        private List<RichTextBox> textBoxes = null;

        private void AppendHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            if (textBoxes == null)
                textBoxes = new List<RichTextBox>();
            textBoxes.Clear();
            internalItemsControl.Items.Clear();

            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                RichTextBox rtb = new RichTextBox();
                textBoxes.Add(rtb);
                internalItemsControl.Items.Add(rtb);
                AppendParagraph(node, rtb);
            }
        }

        private void AppendParagraph(HtmlNode node, RichTextBox rtb)
        {
            Paragraph paragraph = new Paragraph();
            rtb.Blocks.Add(paragraph);
            if (node.Name == "p")
            {
                AppendChildren(node, paragraph, null);
            }
            else
            {
                AppendFromHtml(node, paragraph, null);
            }
        }

        private void AppendChildren(HtmlNode htmlNode, Paragraph paragraph, Span span)
        {
            foreach (var node in htmlNode.ChildNodes)
            {
                AppendFromHtml(node, paragraph, span);
            }
        }

        private void AppendFromHtml(HtmlNode node, Paragraph paragraph, Span span)
        {
            switch (node.Name)
            {
                case "h1":
                case "h2":
                case "h3":
                    AppendSpan(node, paragraph, span, node.Name);
                    break;
                case "p":
                case "blockquote":
                case "ul":
                    AppendSpan(node, paragraph, span, null);
                    break;
                case "#text":
                    AppendRun(node, paragraph, span);
                    break;
                case "a":
                    AppendHyperlink(node, paragraph, span);
                    break;
                case "br":
                case "li":
                    AppendLineBreak(node, paragraph, span);
                    break;
                case "image":
                case "img":
                    AppendImage(node, paragraph);
                    break;
                default:
                    Debug.WriteLine(String.Format("Element {0} not implemented", node.Name));
                    break;
            }
        }

        private void AppendLineBreak(HtmlNode node, Paragraph paragraph, Span span)
        {
            LineBreak lineBreak = new LineBreak();
            
            if (span != null)
            {
                span.Inlines.Add(lineBreak);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(lineBreak);
            }

            AppendChildren(node, paragraph, span);
        }

        private void AppendImage(HtmlNode node, Paragraph paragraph)
        {
            InlineUIContainer inlineContainer = new InlineUIContainer();
            Image image = new Image();
            ImageSourceConverter converter = new ImageSourceConverter();
            ImageSource source = (ImageSource)converter.ConvertFromString(node.Attributes["src"].Value);
            image.Source = source;
            inlineContainer.Child = image;

            if (paragraph != null)
            {
                paragraph.Inlines.Add(inlineContainer);
            }

            AppendChildren(node, paragraph, null);
        }

        private void AppendHyperlink(HtmlNode node, Paragraph paragraph, Span span)
        {
            Hyperlink hyperlink = new Hyperlink();

            if (span != null)
            {
                span.Inlines.Add(hyperlink);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(hyperlink);
            }

            AppendChildren(node, paragraph, hyperlink);
        }

        private void AppendSpan(HtmlNode node, Paragraph paragraph, Span span, string style)
        {
            Span span2 = new Span();

            if (span != null)
            {
                span.Inlines.Add(span2);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(span2);
            }

            AppendChildren(node, paragraph, span2);
        }

        private void AppendRun(HtmlNode node, Paragraph paragraph, Span span)
        {
            Run run = new Run();
            run.Text = DecodeAndCleanupHtml(node.InnerText);

            if (span != null)
            {
                span.Inlines.Add(run);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(run);
            }

            AppendChildren(node, paragraph, span);
        }


        private string DecodeAndCleanupHtml(string html)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HttpUtility.HtmlDecode(html));

            builder.Replace("&nbsp;", " ");

            return builder.ToString();
        }
    }
}
