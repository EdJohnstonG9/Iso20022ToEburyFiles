using EburyMPIsoFiles.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EburyMPIsoFiles.Services
{
    //public class UserSettingsService
    //{
    //    const string UserSettingsKey = "UserSettings";
    //    public UserSettings GetCurrent()
    //    {
    //        UserSettings settings = new UserSettings();
    //        if (App.Current.Properties.Contains(UserSettingsKey))
    //        {
    //            settings = JsonConvert.DeserializeObject<UserSettings>(App.Current.Properties[UserSettingsKey].ToString());
    //            if (settings == null)
    //                settings = new UserSettings();
    //        }
    //        return settings;
    //    }
    //    public bool SaveCurrent(UserSettings settngs)
    //    {
    //        if (settngs != null)
    //        {
    //            App.Current.Properties[UserSettingsKey] = settngs;
    //            return true;
    //        }
    //        return false;
    //    }
    //}

    public class ObjectToPropertiesService : IObjectToPropertiesService
    {
        public ObjectToPropertiesService()
        {

        }

        public T GetCurrent<T>()
        {
            T prop = default(T);
            if (App.Current.Properties.Contains(typeof(T).Name))
            {
                prop = JsonConvert.DeserializeObject<T>(App.Current.Properties[typeof(T).Name].ToString());
                if (prop == null)
                    prop = default(T);
            }
            return prop;
        }
        public bool SaveCurrent<T>(T settngs)
        {
            if (settngs != null)
            {
                App.Current.Properties[typeof(T).Name] = JsonConvert.SerializeObject(settngs);
                return true;
            }
            return false;
        }

    }

}
