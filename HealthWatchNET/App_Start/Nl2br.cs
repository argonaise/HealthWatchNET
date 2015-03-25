using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

public static class HtmlExtensions
{
    public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
    {
        if (string.IsNullOrEmpty(text))
            return MvcHtmlString.Create(text);
        else
        {
            StringBuilder builder = new StringBuilder();
            string[] lines = text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            Debug.WriteLine("Lines: " + lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    builder.Append("<br/>");
                builder.Append(HttpUtility.HtmlEncode(lines[i]));
            }
            Debug.WriteLine("Out: " + builder.ToString());
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}