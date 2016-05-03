using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GenericPaymentService.ActionResults
{
    public class FormIntegrationActionResult : ContentResult
    {
        public FormIntegrationActionResult(Dictionary<string, string> form, string postUrl) : base()
        {
            var sb = new StringBuilder();
            sb.Append("<html><head></head><body>");

            sb.Append(RenderForm(form, postUrl));
            sb.Append(RenderAutoSubmitScript());

            sb.Append("</body></html>");

            Content = sb.ToString();
            ContentType = "text/html";
            ContentEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Renders the form with hidden inputs
        /// </summary>
        /// <param name="form"></param>
        /// <param name="postUrl"></param>
        /// <returns></returns>
        public string RenderForm(Dictionary<string, string> form, string postUrl)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<form id='autoSubmitForm' action='{0}' method='post'>", postUrl);
            foreach(var key in form.Keys)
            {
                sb.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, form[key]);
            }
            sb.Append("</form>");
            return sb.ToString();
        }

        /// <summary>
        /// Renders a snippet of javascript that will trigger the auto submit
        /// </summary>
        /// <returns></returns>
        public string RenderAutoSubmitScript()
        {
            return @"
<script type'text/javascript'>
    document.getElementById('autoSubmitForm').submit();
</script>";
        }
    }
}
