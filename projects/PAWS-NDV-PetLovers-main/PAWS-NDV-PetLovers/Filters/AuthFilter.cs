using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PAWS_NDV_PetLovers.Data;
using System.Collections.Generic;
using System.Linq;

public class AuthFilter : ActionFilterAttribute
{
    private readonly PAWS_NDV_PetLoversContext _context;

    public AuthFilter(PAWS_NDV_PetLoversContext context)
    {
        _context = context;
    }

    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        // Disable caching for this page to protect sensitive information
        context.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
        context.HttpContext.Response.Headers.Add("Pragma", "no-cache");
        context.HttpContext.Response.Headers.Add("Expires", "0");

        // Check if the user is authenticated; if not, redirect to login page
        var isAuthenticated = context.HttpContext.Session.GetString("IsAuthenticated");
        if (string.IsNullOrEmpty(isAuthenticated))
        {
            context.Result = new RedirectToActionResult("Login", "Logins", null);
            return;
        }

        // Get user's role and password status from the session
        var passwordChanged = context.HttpContext.Session.GetString("PasswordChanged");
        var userRole = context.HttpContext.Session.GetString("UserRole");

        // If user role is missing, redirect to login
        if (string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Logins", null);
            return;
        }

        // If password has not been changed, only allow access to the Edit Account page
        var isEditAccountPage = context.RouteData.Values["action"]?.ToString() == "SignUp" &&
                                context.RouteData.Values["controller"]?.ToString() == "UserAccounts";
        if (passwordChanged == "false" && !isEditAccountPage)
        {
            context.Result = new RedirectToActionResult("SignUp", "UserAccounts", null);
            return;
        }

        // Allow admins unrestricted access to all pages
        if (userRole == "Admin")
        {
            base.OnActionExecuting(context);
            return;
        }

        // List of specific pages staff users cann  ot access
        var restrictedActionsForStaff = new List<(string controller, string action)>
    {
           // Staff cannot access PawsReport TransacSumm
        ("PawsReport", "TransacSumm"),
        ("UserAccounts","Account"),
        ("ManageAccount","UserAccounts"),
        ("Account","UserAccounts"),
        ("CreateStaff","UserAccounts"),
        ("Diagnosis","Billing"),
        ("Billing","Diagnosis"),
        ("Billing","CreateDiagnosis"),
        ("Owners","Edit"),
        ("Pets","DiagnosHistory"),
        ("Billing","EditDetails")

    };

        // List of controllers where all actions are restricted for staff
        var restrictedControllersForStaff = new List<string>
    {
        "Services"  // Staff cannot access any action in Services controller
    };

        // For staff users, check if they are trying to access a restricted page
        if (userRole == "Staff")
        {
            var currentController = context.RouteData.Values["controller"]?.ToString();
            var currentAction = context.RouteData.Values["action"]?.ToString();

            // If accessing a restricted action or controller, redirect to Access Denied
            if (restrictedControllersForStaff.Contains(currentController) ||
                restrictedActionsForStaff.Any(r => r.controller == currentController && r.action == currentAction))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            // Continue with the action if no restrictions apply
            base.OnActionExecuting(context);
        }
    }
}