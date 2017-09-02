using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Management;
using System.Configuration;
using System.Windows.Forms;
using SCTVObjects;
using System.Xml;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace SCTechUtilities
{
    /// <summary>
    /// Encryption/Decryption code
    /// </summary>
    public static class Encryption
    {
        static string passPhrase = "SCTechSCTV";        // can be any string
        static string saltValue = "Dr@egon";        // can be any string
        static string hashAlgorithm = "SHA1";             // can be "MD5"
        static int passwordIterations = 10;                  // can be any number
        static string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
        static int keySize = 256;                // can be 192 or 128


        /// <summary>
        /// Encrypts specified plaintext using Rijndael symmetric key algorithm
        /// and returns a base64-encoded result.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be encrypted.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be 
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Encrypted value formatted as a base64-encoded string.
        /// </returns>

        public static string Encrypt(string plainText)
        {
            return Encrypt(plainText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }

        public static string[] Encrypt(string[] plainTextArray)
        {
            return Encrypt(plainTextArray, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }

        public static string Encrypt(string plainText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        public static string[] Encrypt(string[] plainTextArray,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            ArrayList encryptedArray = new ArrayList();

            for (int x = 0; x < plainTextArray.Length; x++)
            {
                encryptedArray.Add(Encrypt(plainTextArray[x], passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize));
            }

            return (string[])encryptedArray.ToArray(typeof(string));
        }

        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// <remarks>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </remarks>

        public static string Decrypt(string cipherText)
        {
            return Decrypt(cipherText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }

        public static string[] Decrypt(string[] cipherTextArray)
        {
            return Decrypt(cipherTextArray, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }

        public static string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        public static string[] Decrypt(string[] cipherTextArray,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            ArrayList decryptedArray = new ArrayList();

            for (int x = 0; x < cipherTextArray.Length; x++)
            {
                decryptedArray.Add(Decrypt(cipherTextArray[x], passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize));
            }

            return (string[])decryptedArray.ToArray(typeof(string));
        }

    }

    /// <summary>
    /// Get information about a machine (cpu ID, drive serial number, etc)
    /// </summary>
    public static class GetMachineInfo
    {

        /// <summary>
        /// return Volume Serial Number from hard drive
        /// </summary>
        /// <param name="strDriveLetter">[optional] Drive letter</param>
        /// <returns>[string] VolumeSerialNumber</returns>
        public static string GetVolumeSerial(string strDriveLetter)
        {
            if (strDriveLetter == "" || strDriveLetter == null) strDriveLetter = "C";

            ManagementObject disk =
                new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            disk.Get();

            return disk["VolumeSerialNumber"].ToString();
        }

        /// <summary>
        /// Returns MAC Address from first Network Card in Computer
        /// </summary>
        /// <returns>[string] MAC Address</returns>
        public static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)  // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        /// <returns>[string] ProcessorId</returns>
        public static string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {// only return cpuInfo from first CPU
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
            }
            return cpuInfo;
        }
    }

    public static class SCTVActivation
    {
        /// <summary> 
        /// Check activation key or get a new one
        /// </summary>
        /// <returns></returns>
        public static bool isActivated()
        {
//#if DEBUG
//            return true;
//#endif

            bool activated = false;
            string activationKey = ConfigurationManager.AppSettings["ActivationKey"];

            try
            {
                //GetMachineInfo getMachineInfo = new GetMachineInfo();
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (activationKey != null && activationKey.Length > 0)
                {
                    //make sure the activation key matches current cpuID, if not delete it
                    if (Encryption.Decrypt(activationKey) == GetMachineInfo.GetCPUId())
                        activated = true;
                    else
                    {
                        config.AppSettings.Settings["ActivationKey"].Value = "";
                        config.Save(ConfigurationSaveMode.Modified);
                        //config.SaveAs(System.Windows.Forms.Application.ExecutablePath + ".config");
                        System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                        activated = false;
                    }
                }

                if (!activated) //The "ActivationKey" doesn't exist or is no good, they need to activate through webservice
                {
                    //popup form to get user info and activate through webservice
                    ActivationForm activationForm = new ActivationForm();
                    DialogResult activationResult = activationForm.ShowDialog();

                    if (activationResult == DialogResult.OK)
                    {
                        activationKey = activationForm.ActivationKey;

                        //create key if it doesn't exist
                        if (System.Configuration.ConfigurationManager.AppSettings["ActivationKey"] == null)
                        {
                            //MessageBox.Show("creating key");
                            System.Configuration.ConfigurationManager.AppSettings.Add("ActivationKey", activationKey);
                        }
                        else
                        {
                            //MessageBox.Show("updating key");
                            config.AppSettings.Settings["ActivationKey"].Value = activationKey;
                            config.Save(ConfigurationSaveMode.Modified);
                            //config.SaveAs(System.Windows.Forms.Application.ExecutablePath + ".config");
                        }

                        //System.Configuration.ConfigurationManager.RefreshSection("applicationSettings");
                        System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                        activated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message);
            }

            return activated;
        }
    }

    public static class CreateObjects
    {
        // <summary>
        /// Convert an XmlNode to an object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------
        public static object FromXmlNodeWithAttributes(XmlNode node, object targetObj)
        {
            if (node.Attributes.Count == 0)
                return targetObj;

            foreach (XmlAttribute attr in node.Attributes)
            {
                string propertyName = attr.Name;
                string propertyValue = attr.Value;

                PropertyInfo pi = targetObj.GetType().GetProperty(propertyName);

                if (pi != null)
                    pi.SetValue(targetObj, Convert.ChangeType(propertyValue, pi.PropertyType), null);

                else // search if such field exists
                {
                    FieldInfo fi = targetObj.GetType().GetField(propertyName);

                    if (fi != null)
                        fi.SetValue(targetObj, Convert.ChangeType(propertyValue, fi.FieldType));
                }
            }

            return targetObj;
        }

        /// <summary>
        /// Convert an XmlNode to an object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------
        public static object FromXmlNodeWithChildNodes(XmlNode node, object targetObj)
        {
            if (!node.HasChildNodes)
                return targetObj;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                string propertyName = childNode.Name;
                string propertyValue = childNode.InnerText;

                PropertyInfo pi = targetObj.GetType().GetProperty(propertyName);

                if (pi != null)
                    pi.SetValue(targetObj, Convert.ChangeType(propertyValue, pi.PropertyType), null);

                else // search if such field exists
                {
                    FieldInfo fi = targetObj.GetType().GetField(propertyName);

                    if (fi != null)
                        fi.SetValue(targetObj, Convert.ChangeType(propertyValue, fi.FieldType));
                }
            }

            return targetObj;
        }

        public static object FromDataRowView(DataRowView mediaRow, object targetObj)
        {

            foreach (DataColumn column in mediaRow.Row.Table.Columns)
            {
                string columnName = column.ColumnName;
                string columnValue = column.ToString();

                PropertyInfo pi = targetObj.GetType().GetProperty(columnName);

                if (pi != null)
                    pi.SetValue(targetObj, Convert.ChangeType(columnValue, pi.PropertyType), null);
                else // search if such field exists
                {
                    FieldInfo fi = targetObj.GetType().GetField(columnName);

                    if (fi != null)
                        fi.SetValue(targetObj, Convert.ChangeType(columnValue, fi.FieldType));
                }
            }

            return targetObj;

            //Media retMedia = new Media();

            //retMedia.category = mediaRow["category"].ToString();
            //retMedia.coverImage = mediaRow["coverImage"].ToString();
            //retMedia.DateAdded = mediaRow["dateAdded"].ToString();
            //retMedia.Description = mediaRow["description"].ToString();
            //retMedia.Director = mediaRow["director"].ToString();
            ////retMedia.filename = mediaRow["fileName"].ToString();
            //retMedia.filePath = mediaRow["filePath"].ToString();
            //retMedia.grammar = mediaRow["grammar"].ToString();
            //retMedia.IMDBNum = mediaRow["IMDBNum"].ToString();
            //retMedia.LastPlayed = mediaRow["lastPlayed"].ToString();
            //retMedia.MediaType = mediaRow["MediaType"].ToString();
            ////retMedia.Message = mediaRow["message"].ToString();
            //retMedia.Performers = mediaRow["performers"].ToString();
            //retMedia.PreviousTitle = mediaRow["previousTitle"].ToString();
            //retMedia.Rating = mediaRow["rating"].ToString();
            //retMedia.RatingDescription = mediaRow["RatingDescription"].ToString();
            //retMedia.ReleaseYear = mediaRow["ReleaseYear"].ToString();
            //retMedia.SortBy = mediaRow["sortBy"].ToString();
            //retMedia.Stars = mediaRow["stars"].ToString();
            //retMedia.TagLine = mediaRow["tagline"].ToString();
            //retMedia.TimesPlayed = mediaRow["timesPlayed"].ToString();
            //retMedia.Title = mediaRow["title"].ToString();
            //retMedia.LastPlayPosition = mediaRow["LastPlayPosition"].ToString();

            //return retMedia;
        }
    }

    public static class Graphics
    {
        public static System.Drawing.Image SetImgOpacity(System.Drawing.Image imgPic, float imgOpac)
        {
            Bitmap bmpPic = new Bitmap(imgPic.Width, imgPic.Height);

            System.Drawing.Graphics gfxPic = System.Drawing.Graphics.FromImage(bmpPic);

            ColorMatrix cmxPic = new ColorMatrix();

            cmxPic.Matrix33 = imgOpac;

            ImageAttributes iaPic = new ImageAttributes();

            iaPic.SetColorMatrix(cmxPic, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            gfxPic.DrawImage(imgPic, new Rectangle(0, 0, bmpPic.Width, bmpPic.Height), 0, 0, imgPic.Width, imgPic.Height, GraphicsUnit.Pixel, iaPic);

            gfxPic.Dispose();

            return bmpPic;
        }
    }
}
