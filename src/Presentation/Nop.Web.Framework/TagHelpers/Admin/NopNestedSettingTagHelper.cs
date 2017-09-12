using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    [HtmlTargetElement("nop-nested-setting", Attributes = ForAttributeName)]
    public class NopNestedSettingTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// HtmlGenerator
        /// </summary>
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopNestedSettingTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var parentSettingName = For.Name;

            var random = CommonHelper.GenerateRandomInteger();
            var nestedSettingId = $"nestedSetting{random}";
            var parentSettingId = $"parentSetting{random}";

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "nested-setting");
            output.Attributes.Add("id", nestedSettingId);

            //decorative line
            var lineMarkup = "<div class=\"form-group\"><div class=\"col-md-9 col-md-offset-3 nested-setting-line-wrapper\"><div class=\"nested-setting-line\"></div></div></div>";
            output.PreContent.AppendHtml(lineMarkup);

            //script
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                            $"$('input[name=\"{parentSettingName}\"]').closest('.form-group').attr('id', '{parentSettingId}');" +
                                            $"var lineHeight{nestedSettingId} = 0;" +
                                            $"for(i = 1; i < $('#{nestedSettingId} .form-group').length-1; i++){{lineHeight{nestedSettingId} += $($('#{nestedSettingId} .form-group')[i]).height() + 5;}}" +
                                            $"lineHeight{nestedSettingId} += 20;" +
                                            $"lineHeight{nestedSettingId} += $('#{parentSettingId}').height() - 15;" +
                                            $"$('#{nestedSettingId} .nested-setting-line').height(lineHeight{nestedSettingId});" +
                                            $"$('#{nestedSettingId} .nested-setting-line').css('top', '-' + ($('#{parentSettingId}').height() - 20) + 'px');" +
                                            $"function toggleNestedSetting() {{if ($('input[name=\"{parentSettingName}\"]').is(':checked')) {{$('#{nestedSettingId}').addClass('opened')}} else {{$('#{nestedSettingId}').removeClass('opened')}}}}" +
                                            $"$('input[name=\"{parentSettingName}\"]').click(toggleNestedSetting);" +
                                            "toggleNestedSetting();" +
                                        "});");
            output.PostContent.SetHtmlContent(script.RenderHtmlContent());
        }
    }
}