using System;
using System.Collections;
using System.Collections.Generic;

namespace Arleen
{
    /// <summary>
    /// Represents a set of localized texts.
    /// </summary>
    public sealed class LocalizedTexts : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _wrapped;

        /// <summary>
        /// Creates a new instance of LocalizedTexts.
        /// </summary>
        /// <param name="wrapped">A dictionary with the loaded localized texts.</param>
        internal LocalizedTexts(IDictionary<string, string> wrapped)
        {
            _wrapped = wrapped;
        }

        /// <summary>
        /// Returns the number of localized texts that has been loaded.
        /// </summary>
        public int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        ICollection<string> IDictionary<string, string>.Keys
        {
            get
            {
                return _wrapped.Keys;
            }
        }

        ICollection<string> IDictionary<string, string>.Values
        {
            get
            {
                return _wrapped.Values;
            }
        }

        string IDictionary<string, string>.this[string key]
        {
            get
            {
                string result;
                if (_wrapped.TryGetValue(key, out result))
                {
                    return result;
                }
                else
                {
                    return key;
                }
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the localized text for a given key.
        /// </summary>
        /// <param name="key">The key for the localized text.</param>
        /// <returns>The localized text for the given key if exists, the key otherwise.</returns>
        public string this[string key]
        {
            get
            {
                string result;
                if (_wrapped.TryGetValue(key, out result))
                {
                    return result;
                }
                else
                {
                    return key;
                }
            }
        }

        /// <summary>
        /// Check whatever or not the current LocalizedTexts contains a particular key.
        /// </summary>
        /// <param name="key">The key for the localized text.</param>
        /// <returns>true if the localized text has been loaded for the given key, false otherwise.</returns>
        public bool ContainsKey(string key)
        {
            return _wrapped.ContainsKey(key);
        }

        /// <summary>
        /// Copies the loaded texts to an array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="arrayIndex">The starting index for the copy.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates over the loaded texts.
        /// </summary>
        /// <returns>an enumerator that iterates over the loaded texts</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<string, string>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            throw new NotSupportedException();
        }

        void IDictionary<string, string>.Add(string key, string value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<string, string>.Remove(string key)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key for the localized text.</param>
        /// <param name="value">The localized text found under the given key.</param>
        /// <returns>true if the key was found, false otherwise.</returns>
        public bool TryGetValue(string key, out string value)
        {
            return _wrapped.TryGetValue(key, out value);
        }
    }
}