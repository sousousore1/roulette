using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Roulette.Helpers
{
    [HtmlTargetElement("span", Attributes = EmailAttributeName)]
    public class GravatarImageTagHelper : TagHelper
    {
        private const string SizeAttributeName = "image-size";
        private const string EmailAttributeName = "gravatar-email";
        private const string AltTextAttributeName = "alt";

        private const string GravatarBaseUrl = "http://www.gravatar.com/avatar.php?";

        [HtmlAttributeName(EmailAttributeName)]
        public string Email { get; set; }

        [HtmlAttributeName(AltTextAttributeName)]
        public string AltText { set; get; }

        [HtmlAttributeName(SizeAttributeName)]
        public int? Size { get; set; }

        private static string ToGravatarHash(string email)
        {
            var encoder = new UTF8Encoding();
            var md5 = MD5.Create();
            var hashedBytes = md5.ComputeHash(encoder.GetBytes(email.ToLower()));
            var sb = new StringBuilder(hashedBytes.Length * 2);

            foreach (var t in hashedBytes)
                sb.Append(t.ToString("X2"));

            return sb.ToString().ToLower();
        }

        public static string ToGravatarUrl(string email, int? size = 200)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()))
                throw new ArgumentException("The email is empty.", nameof(email));

            var sb = ToGravatarHash(email);

            var imageUrl = GravatarBaseUrl + "gravatar_id=" + sb;
            if (size.HasValue)
                imageUrl += "?s=" + size.Value;

            return imageUrl;

        }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var str = new StringBuilder();
            var url = ToGravatarUrl(this.Email, this.Size);
            str.AppendFormat("<img src='{0}' alt='{1}' />", url, AltText);
            output.Content.AppendHtml(str.ToString());

        }
    }
}
