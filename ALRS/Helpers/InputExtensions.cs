using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Linq.Expressions; 
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Principal; 

namespace ALRSSystem.Helpers
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {

        public MyAuthorizeAttribute(params string[] roleKeys)
        {
            List<string> roles = new List<string>(roleKeys.Length);

            var allRoles = (NameValueCollection)ConfigurationManager.GetSection("roles");
            foreach (var roleKey in roleKeys)
            {
                roles.Add(allRoles[roleKey]);
            }

            this.Roles = string.Join(",", roles);
        }
    }
    
    public class aJudgeList
    {
        public bool ID { get; set; }
        public String Name { get; set; }
    }

    public class BooleanRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value is bool)
                return (bool)value;
            else
                return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "booleanrequired"
            };
        }
    }
    

    public static class InputExtensions
    {

        public static string IsUserInGroup(string groupName)
        {
            string result = "";

            WindowsIdentity currentUser = (WindowsIdentity)HttpContext.Current.User.Identity; //WindowsIdentity.GetCurrent();

            result = currentUser.Name;

            IdentityReferenceCollection userGroups = currentUser.Groups;

            foreach (IdentityReference group in userGroups)
            {
                IdentityReference translated = group.Translate(typeof(NTAccount));

                result = result + "-" + translated.Value;
                if (groupName.Equals(translated.Value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return "true";
                }
            }

            //return "false";
            return result;
        }

        public static string AbsoluteAction(this UrlHelper url,
            string actionName, string controllerName, object routeValues = null)
        {
            string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;

            return url.Action(actionName, controllerName, routeValues, scheme);
        }

        
        public static MvcHtmlString RadioButtonForSelectList<TModel, TProperty>(
                    this HtmlHelper<TModel> htmlHelper,
                    Expression<Func<TModel, TProperty>> expression,
                    IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list 
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers 
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();

                    // Create the html string that will be returned to the client 
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat("<div class=\"RadioButton\">{0}  {1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        } 

        public static IHtmlString UnValuedCheckBox(this HtmlHelper helper, string name, string value)
        {
            string html = @"<input type=""checkbox"" name=""{0}"" value=""{1}""/>";
            return new MvcHtmlString(string.Format(html, name, value));
        }

        public static IHtmlString DisplayALRSImage(this HtmlHelper helper, string name)
        {
            if (name == "New")
            {
                //return new MvcHtmlString(string.Format("<p><span style='float:left;'><input type='image' class='NewALRS' style='height:100%;widht:100%' /></span> </p>"));
                return new MvcHtmlString(string.Format("<table border='0' cellspacing='0' cellpadding='4' style='font-family: Arial; font-size: 11pt; background-color: gainsboro; width: 100%;'> <tr> <td> <input type='image' class='NewALRS' style='height: 78px; width: 100%'/> </td> </tr> </table>"));
                //return new MvcHtmlString(string.Format("<div id='image-container'><input type='image' class='NewALRS' style='height: 78px; width: 100%'/> </div>"));

            //<div id="image-container"><img id="image" src="image1.jpg" alt=""/></div>
            }
            else if (name == "Pending")
            {
                return new MvcHtmlString(string.Format("<table border='0' cellspacing='0' cellpadding='4' style='font-family: Arial; font-size: 11pt; background-color: gainsboro; width: 100%;'> <tr> <td> <input type='image' class='PendingALRS' style='height: 78px; width: 100%'/> </td> </tr> </table>"));
            }
            else if (name == "Cancelled")
            {
                return new MvcHtmlString(string.Format("<table border='0' cellspacing='0' cellpadding='4' style='font-family: Arial; font-size: 11pt; background-color: gainsboro; width: 100%;'> <tr> <td> <input type='image' class='CanxALRS' style='height: 78px; width: 100%'/> </td> </tr> </table>"));
            }
            else if (name == "Approved")
            {
                return new MvcHtmlString(string.Format("<table border='0' cellspacing='0' cellpadding='4' style='font-family: Arial; font-size: 11pt; background-color: gainsboro; width: 100%;'> <tr> <td> <input type='image' class='ApprovedALRS' style='height: 78px; width: 100%'/> </td> </tr> </table>"));
            }
            else if (name == "Unsuccessful")
            {
                return new MvcHtmlString(string.Format("<table border='0' cellspacing='0' cellpadding='4' style='font-family: Arial; font-size: 11pt; background-color: gainsboro; width: 100%;'> <tr> <td> <input type='image' class='UnsuccessfulALRS' style='height: 78px; width: 100%'/> </td> </tr> </table>"));
            }
            else
            {
                return new MvcHtmlString(string.Format("<table border='0' cellspacing='0' cellpadding='4' style='font-family: Arial; font-size: 11pt; background-color: gainsboro; width: 100%;'> <tr> <td> <input type='image' class='NewALRS' style='height: 78px; width: 100%'/> </td> </tr> </table>"));
            }
        }


        public static IHtmlString ReadOnlyDisplay(this HtmlHelper helper, string target, string text)
        {
            return new MvcHtmlString(string.Format("<input type='text' name='{1}' id='{1}' style='width:100%;' value='{0}' readonly>", target, text));
        }

        public static IHtmlString HiddenDisplay(this HtmlHelper helper, string target, string text)
        {
            return new MvcHtmlString(string.Format("<input type='hidden' name='{1}' id='{1}' style='width:100%;' value='{0}'>", target, text));
        }

        public static IHtmlString ALRSNameDisplay(this HtmlHelper helper, string target, string text)
        {
            return new MvcHtmlString(string.Format("<input type='text' name='{1}' id='{1}' style='width:100%;' value='{0}' readonly>", target, text));
        }

        public static IHtmlString ALRSDurationDisplay(this HtmlHelper helper, string target, string text)
        {

            return new MvcHtmlString(string.Format("<input type='text' name='{1}' id='{1}' style='width:100%;' value='{0}' readonly>", target, text));
        }

        public static IHtmlString ShortDate(this HtmlHelper helper, DateTime thisDate, string name, bool edit)
        {
            var myDate = thisDate.ToString("dd/MM/yyyy");   

            if (myDate.ToString() == "01/01/0001")
            {
                myDate = DateTime.Now.ToShortDateString();
            }


            // TValue valueOfBar = expression.Compile()(html.ViewData.Model); //thisDate.ToShortDateString().ToString();
            if (edit)
                return new MvcHtmlString(string.Format("<input type='text' name='{1}' id='{1}' class='datepicker' style='width:100%;' value='{0}'>", myDate, name));
            else
                return new MvcHtmlString(string.Format("<comment style='text-align: left' id='{1}'>{0}</comment>", myDate, name));
        }

    }
}