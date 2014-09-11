﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using StringResources = WorkstationController.Core.Properties.Resources;

namespace WorkstationController.Core.Utility
{
    /// <summary>
    /// Utility class for XML serialization
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Serialize an instance of type T to a specified XML file
        /// </summary>
        /// <typeparam name="T">Type of object to be serialized</typeparam>
        /// <param name="xmlFileName">XML file name</param>
        /// <param name="objectGraph">object of type T</param>
        public static void Serialize<T>(string xmlFileName, T objectGraph)
        {
            if(string.IsNullOrEmpty(xmlFileName))
            {
                throw new ArgumentException(StringResources.FileNameArgumentError, "xmlFileName");
            }

            if(objectGraph == null)
            {
                throw new ArgumentNullException("objectGraph", StringResources.ArgumentNullError);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            using (Stream fs = new FileStream(xmlFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UnicodeEncoding(false, false);
                settings.Indent = true;
                settings.OmitXmlDeclaration = false;

                using (XmlWriter writer = XmlWriter.Create(fs, settings))
                {
                    serializer.Serialize(writer, objectGraph);
                }
            }
        }

        /// <summary>
        /// Deserialize an XML file to an instance of type T
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialized</typeparam>
        /// <param name="xmlFileName">XML file name</param>
        /// <returns>object of type T</returns>
        public static T Deserialize<T>(string xmlFileName)
        {
            if (string.IsNullOrEmpty(xmlFileName))
            {
                throw new ArgumentException(StringResources.FileNameArgumentError, "xmlFileName");
            }

            if(!File.Exists(xmlFileName))
            {
                string errorMessage = string.Format(StringResources.FileNotExistsError, xmlFileName);
                throw new ArgumentException(errorMessage, "xmlFileName");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (Stream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;

                using (XmlReader reader = XmlReader.Create(fs, settings))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
        }
    }
}