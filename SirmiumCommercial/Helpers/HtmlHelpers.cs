﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SirmiumCommercial.Helpers
{
    public static class HtmlHelpers
    {
        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string IsSelected(this IHtmlHelper html, string controller = null,
            string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
            {
                cssClass = "active text-info";
            }

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
            {
                controller = currentController;
            }
            if (String.IsNullOrEmpty(action))
            {
                action = currentAction;
            }

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string IsSelectedContentType(this IHtmlHelper html, string contentType = "All",
            string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
            {
                cssClass = " text-bold text-primary";
            }
            string currentContentType = (string)html.ViewContext.ViewData["ContentType"];


            return contentType == currentContentType ? cssClass : String.Empty;
        }

        public static string IsSelectedAdmin(this IHtmlHelper html, string controller = null,
            string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
            {
                cssClass = " text-bold text-primary";
            }

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
            {
                controller = currentController;
            }
            if (String.IsNullOrEmpty(action))
            {
                action = currentAction;
            }

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
    }
}
