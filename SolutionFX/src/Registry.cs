using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using DeveloperFX.structs;

namespace DeveloperFX
{
    public class Registry
    {
        private RegistryKey appKey;
        private RegistryKey configKey;
        private Dictionary<string, AppRegistryEntry> configMap;

        public Registry()
        {
            appKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey("Software\\DeveloperFX", true);
            configKey = appKey.OpenSubKey("Configurations", true);
            configMap = getExistingConfigs();
        }

        public void modifyEntry(string name, string pathToExe, string pathToIni)
        {
            AppRegistryEntry entry = configMap[name];
            entry.pathToExe = pathToExe;
            entry.pathToIni = pathToIni;
            configMap[name] = entry;
            modifyKey(configMap[name]);
        }

        public void createEntry(string name, string pathToExe, string pathToIni)
        {
            AppRegistryEntry newEntry;
            newEntry.name = name;
            newEntry.pathToExe = pathToExe;
            newEntry.pathToIni = pathToIni;
            addKey(newEntry);
            configMap.Add(name, newEntry);
            
        }

        private void addKey(AppRegistryEntry newEntry)
        {
            configKey.CreateSubKey(newEntry.name);
            RegistryKey currentConfigKey = configKey.OpenSubKey(newEntry.name, true);
            currentConfigKey.SetValue("path_to_exe", newEntry.pathToExe);
            currentConfigKey.SetValue("path_to_ini", newEntry.pathToIni);
        }
        private void modifyKey(AppRegistryEntry entry)
        {
            RegistryKey currentConfigKey = configKey.OpenSubKey(entry.name, true);
            currentConfigKey.SetValue("path_to_exe", entry.pathToExe);
            currentConfigKey.SetValue("path_to_ini", entry.pathToIni);
        }

        public void deleteEntry(string keyName)
        {
            configKey.DeleteSubKeyTree(keyName);
        }
        public bool containsKey(string key)
        {
            return configMap.ContainsKey(key);
        }

        public AppRegistryEntry getRegistryEntry(string name)
        {
            return configMap[name];
        }

        public string getLastUsedConfig()
        {
            return appKey.GetValue("LastUsedConfig").ToString();
        }

        public void setLastUsedConfig(string name)
        {
            appKey.SetValue("LastUsedConfig", name);
        }

        public List<string> getKeyList()
        {
            return configMap.Keys.ToList();
        }

        private Dictionary<string, AppRegistryEntry> getExistingConfigs()
        {
            Dictionary<string, AppRegistryEntry> map = new Dictionary<string, AppRegistryEntry>();

            string[] names = configKey.GetSubKeyNames();
            if (names.Length > 0)
            {
                foreach (string name in names)
                {
                    RegistryKey currKey = configKey.OpenSubKey(name);

                    AppRegistryEntry newEntry;
                    newEntry.name = name;
                    newEntry.pathToExe = currKey.GetValue("path_to_exe").ToString();
                    newEntry.pathToIni = currKey.GetValue("path_to_ini").ToString();

                    map.Add(name, newEntry);
                }
            }
            return map;
        }
    }
}