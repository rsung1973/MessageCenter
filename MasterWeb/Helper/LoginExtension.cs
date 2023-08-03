using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CommonLib.Web;
using Utility;
using WebHome.Models.DataEntity;
using WebHome.Models.Locale;
using WebHome.Models.Security;
using WebHome.Models.ViewModel;
using WebHome.Properties;

namespace WebHome.Helper
{
    public static class LoginExtension
    {
        public static void SignOn(this HttpContextBase context,UserProfile profile,bool remeberMe = true)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(profile.PID, true, Settings.Default.UserTimeoutInMinutes);
            String loginToken = FormsAuthentication.Encrypt(ticket);
            context.Response.SetCookie(new HttpCookie(FormsAuthentication.FormsCookieName, loginToken));
            context.ClearCache();
            context.SetCacheValue("userProfile",profile);

            HttpCookie cookie = new HttpCookie("loginToken", loginToken);
            cookie.Expires = DateTime.Now.AddMinutes(Settings.Default.UserTimeoutInMinutes);
            context.Response.SetCookie(cookie);

            if (remeberMe)
            {
                cookie = new HttpCookie("userID", profile.PID);
                cookie.Expires = DateTime.Now.AddDays(14);
            }
            else
            {
                cookie = new HttpCookie("userID", "");
                cookie.Expires = DateTime.Now.AddHours(24);
            }
            context.Response.SetCookie(cookie);


            /// process sign-on user profile
            /// 
            //var roles = profile.UserRole.Select(r => r.UserRoleDefinition).ToArray();
            //var roleAuth = profile.UserRoleAuthorization.ToArray();
            //var auth = profile.UserRoleAuthorization.Select(r => r.UserRoleDefinition).ToArray();
        }

        public static void ClearCache(this HttpContextBase context)
        {
            DataModelCache caching = new DataModelCache(context);
            caching.Clear();
        }
        public static void ClearCache(this HttpContext context)
        {
            HttpContextDataModelCache caching = new HttpContextDataModelCache(context);
            caching.Clear();
        }


        public static void SetCacheValue(this HttpContextBase context, CachingKey keyName, Object value)
        {
            context.SetCacheValue(keyName.ToString(), value);
        }

        public static void RemoveCache(this HttpContextBase context, CachingKey keyName)
        {
            context.SetCacheValue(keyName.ToString(), null);
        }


        public static Object GetCacheValue(this HttpContextBase context, CachingKey keyName)
        {
            return context.GetCacheValue(keyName.ToString());
        }

        public static void SetCacheValue(this HttpContextBase context,String keyName,Object value)
        {
            DataModelCache caching = new DataModelCache(context);
            caching[keyName] = value;
        }

        public static void SetCacheValue(this HttpContext context, String keyName, Object value)
        {
            HttpContextDataModelCache caching = new HttpContextDataModelCache(context);
            caching[keyName] = value;
        }


        public static Object GetCacheValue(this HttpContextBase context, String keyName)
        {
            DataModelCache caching = new DataModelCache(context);
            return caching[keyName];
        }

        public static bool CreatePassword(this Controller controller,PasswordViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.lockPattern))
            {
                if (String.IsNullOrEmpty(viewModel.Password))
                {
                    controller.ModelState.AddModelError("Password", "請輸入密碼!!");
                    controller.ModelState.AddModelError("lockPattern", "請設定圖形密碼!!");
                    return false;
                }
                else if (viewModel.Password != viewModel.Password2)
                {
                    controller.ModelState.AddModelError("Password2", "密碼確認錯誤!!");
                    return false;
                }
            }
            else
            {
                if (viewModel.lockPattern.Length < 9)
                {
                    controller.ModelState.AddModelError("lockPattern", "設定圖形的密碼過短!!");
                }
                else
                {
                    viewModel.Password = viewModel.lockPattern;
                }
            }
            return true;
        }

        public static String MakePassword(this String password)
        {
            if (String.IsNullOrEmpty(password))
                return null;
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.Default.GetBytes(password)));
        }

        public static void Logout(this HttpContextBase context)
        {
            context.Response.SetCookie(new HttpCookie(FormsAuthentication.FormsCookieName, ""));
            context.Response.SetCookie(new HttpCookie("loginToken", ""));
            context.ClearCache();
        }

        public static UserProfile GetUser(this HttpContextBase context)
        {
            UserProfile profile = (UserProfile)context.GetCacheValue("userProfile");
            //Logger.Debug("profile cache:" + (profile != null));
            if (profile == null)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    //Logger.Debug("Has Identity:" + context.User.Identity.Name);
                    profile = context.User.Identity.Name.getLoginUser();
                }
                else
                {
                    var cookie = context.Request.Cookies["loginToken"];
                    if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
                    {
                        try
                        {
                            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                            //Logger.Debug("Has loginToken:" + ticket.Name);
                            profile = ticket.Name.getLoginUser();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            profile = null;
                        }
                    }
                }
                context.SetCacheValue("userProfile", profile);
            }
            return profile;
        }

        private static UserProfile getLoginUser(this String pid)
        {
            using (ModelSource<UserProfile> Models = new ModelSource<UserProfile>())
            {
                UserProfile profile = Models.EntityList.Where(u => u.PID == pid).FirstOrDefault();
                if (profile != null)
                {
                    //var roles = profile.UserRole.Select(r => r.UserRoleDefinition).ToArray();
                    //var roleAuth = profile.UserRoleAuthorization.ToArray();
                    //var auth = profile.UserRoleAuthorization.Select(r => r.UserRoleDefinition).ToArray();
                }
                return profile;
            }
        }

        public static UserProfile GetUser(this HttpContext context)
        {
            HttpContextDataModelCache caching = new HttpContextDataModelCache(context);
            UserProfile profile =  (UserProfile)caching["userProfile"];
            //Logger.Debug("profile cache:" + (profile != null));
            if (profile == null)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    //Logger.Debug("Has Identity:" + context.User.Identity.Name);
                    profile = context.User.Identity.Name.getLoginUser();
                }
                else
                {
                    var cookie = context.Request.Cookies["loginToken"];
                    if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
                    {
                        try
                        {
                            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                            //Logger.Debug("Has loginToken:" + ticket.Name);
                            profile = ticket.Name.getLoginUser();
                        }
                        catch(Exception ex)
                        {
                            Logger.Error(ex);
                            profile = null;
                        }
                    }
                }
                context.SetCacheValue("userProfile", profile);
            }
            return profile;
        }
    }
}