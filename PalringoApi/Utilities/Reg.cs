using Microsoft.Win32;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// Registry class
    /// </summary>
    public static class Reg
    {
        /// <summary>
        /// 
        /// </summary>
        public static string SubKey = "SOFTWARE\\PalLibraryStuff";
        /// <summary>
        /// 
        /// </summary>
        public static RegistryKey BaseRegistryKey = Registry.CurrentUser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string Read(string keyName)
        {
            RegistryKey rk = BaseRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(SubKey);
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(keyName.ToUpper());
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static int ReadInt(string keyName, int defaultvalue = -1)
        {
            RegistryKey rk = BaseRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(SubKey);
            if (sk1 == null)
            {
                return defaultvalue;
            }
            else
            {
                try
                {
                    string key = (string)sk1.GetValue(keyName.ToUpper());
                    int item;
                    if (int.TryParse(key, out item))
                        return item;
                    else
                        return defaultvalue;
                }
                catch
                {
                    return defaultvalue;
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ReadBool(string keyName, bool defaultValue = false)
        {
            RegistryKey rk = BaseRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(SubKey);
            if (sk1 == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(keyName.ToUpper()) == "1";
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static double ReadDouble(string keyName, double defaultvalue = -0.01)
        {
            RegistryKey rk = BaseRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(SubKey);
            if (sk1 == null)
            {
                return defaultvalue;
            }
            else
            {
                try
                {
                    string key = (string)sk1.GetValue(keyName.ToUpper());
                    double item;
                    if (double.TryParse(key, out item))
                        return item;
                    else
                        return defaultvalue;
                }
                catch
                {
                    return defaultvalue;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Write(string keyName, object value)
        {
            try
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(SubKey);
                sk1.SetValue(keyName.ToUpper(), value.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Write(string keyName, bool value)
        {
            try
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(SubKey);
                string val = value ? "1" : "0";
                object objval = val;
                sk1.SetValue(keyName.ToUpper(), objval);

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool DeleteKey(string keyName)
        {
            try
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(SubKey);
                if (sk1 == null)
                    return true;
                else
                    sk1.DeleteValue(keyName);

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool DeleteSubKeyTree()
        {
            try
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(SubKey);
                if (sk1 != null)
                    rk.DeleteSubKeyTree(SubKey);

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int SubKeyCount()
        {
            try
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(SubKey);
                if (sk1 != null)
                    return sk1.SubKeyCount;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int ValueCount()
        {
            try
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(SubKey);
                if (sk1 != null)
                    return sk1.ValueCount;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
