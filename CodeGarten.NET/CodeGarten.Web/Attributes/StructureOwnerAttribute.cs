﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using CodeGarten.Data;

namespace CodeGarten.Web.Attributes
{
    public sealed class StructureOwnerAttribute : AuthorizeAttribute
    {
        private String StructureIdField { get; set; }

        public StructureOwnerAttribute(string structureIdField)
        {
            StructureIdField = structureIdField;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var _context = new Context();
            var user = _context.Users.Find(httpContext.User.Identity.Name);
            var structureId =
                Int32.Parse((httpContext.Request.RequestContext.RouteData.Values[StructureIdField] ??
                             httpContext.Request[StructureIdField]) as string);

            return user.Structures.Select(s => s.Id).Contains(structureId);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult(FormsAuthentication.DefaultUrl);
        }
    }
}