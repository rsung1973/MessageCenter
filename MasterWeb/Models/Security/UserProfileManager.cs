using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonLib.DataAccess;
using WebHome.Models.DataEntity;

namespace WebHome.Models.Security
{
    public class UserProfileManager : GenericManager<MessageCenterDataContext, UserProfile>
    {
        protected HttpContextBase context;

        public UserProfileManager() : base() { }
        public UserProfileManager(GenericManager<MessageCenterDataContext> manager) : base(manager)
        {

        }


        
    }
}