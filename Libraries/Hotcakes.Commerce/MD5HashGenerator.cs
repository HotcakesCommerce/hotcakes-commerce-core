#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Hotcakes.Commerce
{
    /// <summary>
    ///     This class takes an object, and generates a key to it. There are several possibilities:
    ///     This generator can generate keys of type integer,float,double. The generated key is not necessarly
    ///     unique!
    /// </summary>
    public class MD5HashGenerator
    {
        private static readonly object locker = new object();

        /// <summary>
        ///     Generates a hashed - key for an instance of a class.
        ///     The hash is a classic MD5 hash (e.g. BF20EB8D2C4901112179BF5D242D996B). So you can distinguish different
        ///     instances of a class. Because the object is hashed on the internal state, you can also hash it, then send it to
        ///     someone in a serialized way. Your client can then deserialize it and check if it is in
        ///     the same state.
        ///     The method just just estimates that the object implements the ISerializable interface. What's
        ///     needed to save the state or so, is up to the implementer of the interface.
        ///     <b>The method is thread-safe!</b>
        /// </summary>
        /// <param name="sourceObject">The object you'd like to have a key out of it.</param>
        /// <returns>An string representing a MD5 Hashkey corresponding to the object or null if the object couldn't be serialized.</returns>
        /// <exception cref="ApplicationException">Will be thrown if the key cannot be generated.</exception>
        public static string GenerateKey(object sourceObject)
        {
            var hashString = "";

            //Catch unuseful parameter values
            if (sourceObject == null)
            {
                throw new ArgumentNullException("Null as parameter is not allowed");
            }
            //We determine if the passed object is really serializable.
            try
            {
                //Now we begin to do the real work.
                hashString = ComputeHash(ObjectToByteArray(sourceObject));
                return hashString;
            }
            catch (AmbiguousMatchException ame)
            {
                throw new ApplicationException("Could not definitly decide if object is serializable. Message:" +
                                               ame.Message);
            }
        }

        /// <summary>
        ///     Converts an object to an array of bytes. This array is used to hash the object.
        /// </summary>
        /// <param name="objectToSerialize">Just an object</param>
        /// <returns>A byte - array representation of the object.</returns>
        /// <exception cref="SerializationException">Is thrown if something went wrong during serialization.</exception>
        private static byte[] ObjectToByteArray(object objectToSerialize)
        {
            var memoryStream = new MemoryStream();
            try
            {
                //Here's the core functionality! One Line!
                //To be thread-safe we lock the object
                lock (locker)
                {
                    using (var streamWriter = new StreamWriter(memoryStream))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Serialize(streamWriter, objectToSerialize);
                    }
                }
                return memoryStream.ToArray();
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occurred during serialization. Message: " + se.Message);
                throw;
            }
            finally
            {
                memoryStream.Close();
            }
        }

        /// <summary>
        ///     Generates the hashcode of an given byte-array. The byte-array can be an object. Then the
        ///     method "hashes" this object. The hash can then be used e.g. to identify the object.
        /// </summary>
        /// <param name="objectAsBytes">bytearray representation of an object.</param>
        /// <returns>The MD5 hash of the object as a string or null if it couldn't be generated.</returns>
        private static string ComputeHash(byte[] objectAsBytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            try
            {
                var result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                var sb = new StringBuilder();
                for (var i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException)
            {
                //If something occured during serialization, this method is called with an null argument. 
                Console.WriteLine("Hash has not been generated.");
                throw;
            }
        }
    }
}