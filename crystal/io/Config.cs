using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace crystal.io
{

    /// <summary>
    /// Class Contain methods for work with JSON configuration files
    /// </summary>
    public class Config
    {
        /// <summary>
        /// This method check for a configuration file.
        /// </summary>
        /// <param name="path">path to config file</param>
        /// <returns>Return true if config exist</returns>
        public static bool Exist(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// This method check for a configuration file and matching the specified type.
        /// </summary>
        /// <typeparam name="T">JSON abstraction type</typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exist<T>(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return false;

                JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method reads JSON information from a file and converts it to a specific class.
        /// </summary>
        /// <typeparam name="T">Type of class model</typeparam>
        /// <param name="path">path to config file</param>
        /// <returns>Return an abstract class model</returns>
        /// <exception cref="ConfigException">thrown if file not found</exception>
        public static T ReadConfig<T>(string path)
        {
            if (!Exist<T>(path))
                throw new ConfigException($"Could not find correct config: {path}");

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// This method converts the class to JSON format and saves it to the desired file.
        /// </summary>
        /// <typeparam name="T">type of class</typeparam>
        /// <param name="path">path to file</param>
        /// <param name="config">class of configuration</param>
        public static void SaveConfig<T>(string path, T config)
        {
            string[] pathParts = path.Split(Path.DirectorySeparatorChar);
            if (pathParts.Length != 1)
            {
                for (int i = 0; i < pathParts.Length; i++)
                {
                    if (i > 0)
                        pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);

                    if (!Directory.Exists(pathParts[i]))
                        Directory.CreateDirectory(pathParts[i]);
                }
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(config));
        }

        /// <summary>
        /// This method deletes a specific configuration file
        /// </summary>
        /// <param name="path">path to file</param>
        /// <param name="secure">use deletion with multiple overwrites</param>
        /// <exception cref="ConfigException">thrown if file not found</exception>
        public static void Delete(string path, bool secure = false)
        {
            if (!File.Exists(path))
                throw new ConfigException($"Could not config: {path}");

            if (secure)
            {
                for (int i = 0; i < 64; i++)
                {
                    using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                    {
                        FileInfo info = new FileInfo(path);
                        byte[] rgb = new byte[info.Length];
                        rng.GetBytes(rgb);
                        using (StreamWriter sw = new StreamWriter(path))
                        {
                            sw.Write(rgb);
                        }
                    }
                }
            }

            File.Delete(path);
        }
    }
}