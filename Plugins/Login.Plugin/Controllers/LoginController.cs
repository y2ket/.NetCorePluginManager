﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  .Net Core Plugin Manager is distributed under the GNU General Public License version 3 and  
 *  is also available under alternative licenses negotiated directly with Simon Carter.  
 *  If you obtained Service Manager under the GPL, then the GPL applies to all loadable 
 *  Service Manager modules used on your system as well. The GPL (version 3) is 
 *  available at https://opensource.org/licenses/GPL-3.0
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *  See the GNU General Public License for more details.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2018 - 2020 Simon Carter.  All Rights Reserved.
 *
 *  Product:  Login Plugin
 *  
 *  File: LoginController.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  19/11/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.IO;
using System.Security.Claims;

using LoginPlugin.Classes;
using LoginPlugin.Models;

using Microsoft.AspNetCore.Mvc;

using Middleware;

using PluginManager.Abstractions;

using Shared.Classes;

using SharedPluginFeatures;

using static Middleware.Constants;
using static Shared.Utilities;

#pragma warning disable CS1591

namespace LoginPlugin.Controllers
{
    /// <summary>
    /// Login controller, allows users to login using a standard interface implemented by ILoginProvider interface.
    /// </summary>
    [LoggedOut]
    public class LoginController : BaseController
    {
        #region Private Members

        private readonly ILoginProvider _loginProvider;
        private readonly LoginControllerSettings _settings;
        private readonly IClaimsProvider _claimsProvider;
        //private readonly IAuthenticationService _authenticationService;

        private static readonly CacheManager _loginCache = new CacheManager("Login Cache", new TimeSpan(0, 30, 0));

        #endregion Private Members

        #region Constructors

        public LoginController(ILoginProvider loginProvider, ISettingsProvider settingsProvider,
            IClaimsProvider claimsProvider/*, IAuthenticationService authenticationService*/)
        {
            if (settingsProvider == null)
                throw new ArgumentNullException(nameof(settingsProvider));

            _loginProvider = loginProvider ?? throw new ArgumentNullException(nameof(loginProvider));
            _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
            //_authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _settings = settingsProvider.GetSettings<LoginControllerSettings>(nameof(LoginPlugin));
        }

        #endregion Constructors

        #region Public Action Methods

        [HttpGet]
        [Breadcrumb(nameof(Languages.LanguageStrings.Login))]
        [SmokeTest(200, PostType.Form, "loginForm", inputData: "Username=joe&Password=bloggs&RememberMe=False", searchData: "Don't have an account yet", submitSearchData: "Invalid User Name/Password;Please try again")]
        [SmokeTest(200, PostType.Form, "loginForm", inputData: "Username=dennis&Password=mennace", searchData: "Don't have an account yet", submitSearchData: "Invalid User Name/Password;Please try again")]
        public IActionResult Index(string returnUrl)
        {
            // has the user been remembered?
            if (ValidateRememberedLogin())
            {
                if (String.IsNullOrEmpty(returnUrl))
                    return Redirect(_settings.LoginSuccessUrl);
                else
                    return Redirect(returnUrl);
            }

            LoginViewModel model = new LoginViewModel(GetModelData(),
                String.IsNullOrEmpty(returnUrl) ? _settings.LoginSuccessUrl : returnUrl,
                _settings.ShowRememberMe);


            LoginCacheItem loginCacheItem = GetCachedLoginAttempt(false);

            if (loginCacheItem != null)
            {
                model.ShowCaptchaImage = loginCacheItem.LoginAttempts >= _settings.CaptchaShowFailCount;
                loginCacheItem.CaptchaText = GetRandomWord(_settings.CaptchaWordLength, CaptchaCharacters);
            }

            return View(model);
        }

        [BadEgg]
        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            LoginCacheItem loginCacheItem = GetCachedLoginAttempt(true);

            if (!String.IsNullOrEmpty(loginCacheItem.CaptchaText))
            {
                if (!loginCacheItem.CaptchaText.Equals(model.CaptchaText))
                    ModelState.AddModelError(String.Empty, Languages.LanguageStrings.CodeNotValid);
            }

            loginCacheItem.LoginAttempts++;

            model.ShowCaptchaImage = loginCacheItem.LoginAttempts >= _settings.CaptchaShowFailCount;

            UserLoginDetails loginDetails = new UserLoginDetails();

            model.Breadcrumbs = GetBreadcrumbs();
            model.CartSummary = GetCartSummary();

            LoginResult loginResult = _loginProvider.Login(model.Username, model.Password, GetIpAddress(),
                loginCacheItem.LoginAttempts, ref loginDetails);

            switch (loginResult)
            {
                case LoginResult.Success:
                case LoginResult.PasswordChangeRequired:
                    RemoveLoginAttempt();

                    UserSession session = GetUserSession();

                    if (session != null)
                        session.Login(loginDetails.UserId, loginDetails.Username, loginDetails.Email);

                    if (model.RememberMe)
                    {
                        CookieAdd(_settings.RememberMeCookieName, Encrypt(loginDetails.UserId.ToString(),
                            _settings.EncryptionKey), _settings.LoginDays);
                    }


                    GetAuthenticationService().SignInAsync(HttpContext,
                        _settings.AuthenticationScheme,
                        new ClaimsPrincipal(_claimsProvider.GetUserClaims(loginDetails.UserId)),
                        _claimsProvider.GetAuthenticationProperties());

                    if (loginResult == LoginResult.PasswordChangeRequired)
                    {
                        return Redirect(_settings.ChangePasswordUrl);
                    }

                    return Redirect(model.ReturnUrl);

                case LoginResult.AccountLocked:
                    return RedirectToAction(nameof(AccountLocked), new { username = model.Username });

                case LoginResult.InvalidCredentials:
                    ModelState.AddModelError(String.Empty, Languages.LanguageStrings.InvalidUsernameOrPassword);
                    break;
            }

            if (model.ShowCaptchaImage)
                loginCacheItem.CaptchaText = GetRandomWord(_settings.CaptchaWordLength, CaptchaCharacters);

            return View(model);
        }

        [HttpGet]
        [Breadcrumb(nameof(Languages.LanguageStrings.AccountLocked))]
        [SmokeTest(302, PostType.Form, parameters: "username=", redirectUrl: "/Login")]
        [SmokeTest(200, PostType.Form, parameters: "username=fred@bloggs", searchData: "Your account has been temporarily locked due to authentication failures")]
        [SmokeTest(200, PostType.Form, formId: "frmUnlockAccount", inputData: "Username=fred@bloggs.com&UnlockCode=123456", parameters: "username=fred@bloggs", searchData: "Please enter your Username/Email Address")]
        public IActionResult AccountLocked(string username)
        {
            if (String.IsNullOrEmpty(username))
                return RedirectToAction(nameof(Index));

            AccountLockedViewModel model = new AccountLockedViewModel(GetModelData(), username);

            return View(model);
        }

        [HttpPost]
        [BadEgg]
        public IActionResult AccountLocked(AccountLockedViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (_loginProvider.UnlockAccount(model.Username, model.UnlockCode))
            {
                return Redirect("/Login");
            }

            ModelState.AddModelError(String.Empty, Languages.LanguageStrings.CodeNotValid);
            model.UnlockCode = String.Empty;
            model.Breadcrumbs = GetBreadcrumbs();
            model.CartSummary = GetCartSummary();

            return View(model);
        }

        [HttpGet]
        [Breadcrumb(nameof(Languages.LanguageStrings.ForgotPassword))]
        public IActionResult ForgotPassword()
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel(GetModelData());

            LoginCacheItem loginCacheItem = GetCachedLoginAttempt(true);
            loginCacheItem.CaptchaText = GetRandomWord(_settings.CaptchaWordLength, CaptchaCharacters);
            //model.CaptchaText = loginCacheItem.CaptchaText;

            return View(model);
        }

        [HttpPost]
        [BadEgg]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            LoginCacheItem loginCacheItem = GetCachedLoginAttempt(true);

            if (!String.IsNullOrEmpty(loginCacheItem.CaptchaText))
            {
                if (!loginCacheItem.CaptchaText.Equals(model.CaptchaText))
                    ModelState.AddModelError(String.Empty, Languages.LanguageStrings.CodeNotValid);
            }

            if (ModelState.IsValid && _loginProvider.ForgottenPassword(model.Username))
            {
                RemoveLoginAttempt();
                return Redirect("/Login/");
            }

            ModelState.AddModelError(String.Empty, Languages.LanguageStrings.InvalidUsernameOrPassword);

            loginCacheItem.CaptchaText = GetRandomWord(_settings.CaptchaWordLength, CaptchaCharacters);
            model.CaptchaText = loginCacheItem.CaptchaText;
            model.Breadcrumbs = GetBreadcrumbs();
            model.CartSummary = GetCartSummary();

            return View(model);
        }

        [HttpGet]
        [Breadcrumb(nameof(Languages.LanguageStrings.Logout))]
        [LoggedIn]
        public ActionResult Logout()
        {
            UserSession session = GetUserSession();

            if (session != null)
            {
                session.UserEmail = String.Empty;
                session.UserName = String.Empty;
                session.UserID = 0;
            }

            CookieDelete(_settings.RememberMeCookieName);

            GetAuthenticationService().SignOutAsync(HttpContext,
                _settings.AuthenticationScheme,
                _claimsProvider.GetAuthenticationProperties());

            return Redirect("/");
        }

        [HttpGet]
        public ActionResult GetCaptchaImage()
        {
            LoginCacheItem loginCacheItem = GetCachedLoginAttempt(false);

            if (loginCacheItem == null)
                return StatusCode(400);

            CaptchaImage ci = new CaptchaImage(loginCacheItem.CaptchaText, 240, 60, "Century Schoolbook");
            try
            {
                // Write the image to the response stream in JPEG format.
                using (MemoryStream ms = new MemoryStream())
                {
                    ci.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    return File(ms.ToArray(), "image/png");
                }
            }
            catch (Exception err)
            {
                if (!err.Message.Contains("Specified method is not supported."))
                    throw;
            }
            finally
            {
                ci.Dispose();
            }

            return null;
        }

        #endregion Public Action Methods

        #region Private Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "it's ok here, nothing to see, move along")]
        private bool ValidateRememberedLogin()
        {
            if (CookieExists(_settings.RememberMeCookieName))
            {
                try
                {
                    string loginId = Decrypt(CookieValue(_settings.RememberMeCookieName, ""), _settings.EncryptionKey);
                    UserLoginDetails loginDetails = new UserLoginDetails(Convert.ToInt64(loginId), true);
                    bool loggedIn = _loginProvider.Login(String.Empty, String.Empty, GetIpAddress(), 0, ref loginDetails) == LoginResult.Remembered;

                    if (loggedIn)
                    {
                        UserSession session = GetUserSession();

                        if (session != null)
                            session.Login(loginDetails.UserId, loginDetails.Username, loginDetails.Email);

                        GetAuthenticationService().SignInAsync(HttpContext,
                            nameof(AspNetCore.PluginManager),
                            new ClaimsPrincipal(_claimsProvider.GetUserClaims(loginDetails.UserId)),
                            _claimsProvider.GetAuthenticationProperties());
                    }

                    return loggedIn;
                }
                catch
                {
                    CookieDelete(_settings.RememberMeCookieName);
                }
            }

            return false;
        }

        private void RemoveLoginAttempt()
        {
            string cacheId = _settings.CacheUseSession ? GetCoreSessionId() : GetIpAddress();

            CacheItem loginCache = _loginCache.Get(cacheId);

            if (loginCache != null)
            {
                _loginCache.Remove(loginCache);
            }
        }

        private LoginCacheItem GetCachedLoginAttempt(bool createIfNotExist)
        {
            LoginCacheItem Result = null;

            string cacheId = _settings.CacheUseSession ? GetCoreSessionId() : GetIpAddress();

            CacheItem loginCache = _loginCache.Get(cacheId);

            if (loginCache != null)
            {
                Result = (LoginCacheItem)loginCache.Value;
            }
            else if (createIfNotExist && loginCache == null)
            {
                Result = new LoginCacheItem();
                loginCache = new CacheItem(cacheId, Result);
                _loginCache.Add(cacheId, loginCache);
            }

            return Result;
        }

        #endregion Private Methods
    }
}

#pragma warning restore CS1591