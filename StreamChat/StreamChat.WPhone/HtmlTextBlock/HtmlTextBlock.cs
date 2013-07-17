using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using HtmlAgilityPack;

namespace StreamChat.WPhone.HtmlTextBlock
{
	[TemplatePart(Name = HtmlTextBlock.PART_ItemsControl, Type = typeof(ItemsControl))]
	public class HtmlTextBlock : Control
	{
		private const string PART_ItemsControl = "PART_ItemsControl";

		static HtmlTextBlock()
		{
			HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(HtmlTextBlock), new PropertyMetadata(HtmlChanged));
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
			if (instance.internalItemsControl != null)
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

			Span mainSpan = new Span();

			var mainParagraph = new Paragraph();
			mainParagraph.Inlines.Add(mainSpan);

			RichTextBox rtb = new RichTextBox();
			rtb.Blocks.Add(mainParagraph);
			textBoxes.Add(rtb);
			internalItemsControl.Items.Add(rtb);

			foreach (var node in htmlDoc.DocumentNode.ChildNodes)
			{
				AppendFromHtml(node, mainParagraph, mainSpan);
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

				case "b":
					AppendBold(node, paragraph, span);
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
			hyperlink.NavigateUri = new Uri(node.Attributes["href"].Value, UriKind.Absolute);
			hyperlink.TargetName = "_blank";

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

		private const string URL_REPLACER = "%LINKLINK%";

		private void AppendRun(HtmlNode node, Paragraph paragraph, Span span)
		{
			List<Uri> urls = new List<Uri>();
			var decoded = DecodeAndCleanupHtml(node.InnerText);

			Regex uriDetector = new Regex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

			decoded = uriDetector.Replace(decoded, me =>
			{
				urls.Add(new Uri(me.Value, UriKind.Absolute));
				return URL_REPLACER + " ";
			});

			var parts = decoded.Split(new string[] { URL_REPLACER }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var part in parts)
			{
				Run run = new Run();
				run.Text = part;

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

			if (span != null)
			{
				span.Inlines.Add("\n");
			}
			else if (paragraph != null)
			{
				paragraph.Inlines.Add("\n");
			}

			foreach (var url in urls)
			{
				Hyperlink hyperlink = new Hyperlink();
				hyperlink.NavigateUri = url;
				hyperlink.TargetName = "_blank";

				hyperlink.Inlines.Add("link");

				if (span != null)
				{
					span.Inlines.Add(hyperlink);
				}
				else if (paragraph != null)
				{
					paragraph.Inlines.Add(hyperlink);
				}
			}
		}

		private void AppendBold(HtmlNode node, Paragraph paragraph, Span span)
		{
			Run run = new Run();
			run.Text = DecodeAndCleanupHtml(node.InnerText);
			run.FontWeight = FontWeights.Bold;

			if (span != null)
			{
				span.Inlines.Add(run);
			}
			else if (paragraph != null)
			{
				paragraph.Inlines.Add(run);
			}

			//			AppendChildren(node, paragraph, span);
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
