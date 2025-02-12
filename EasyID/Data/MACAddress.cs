﻿using Newtonsoft.Json;
using System.Net;
using System;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace EasyID.Data
{
    public class MACAddress : DataTemplate
    {
        // TODO: Check for ffff.ffff.ffff format
        private Driver _driver;
        private List<int> _lengthList = new List<int> { 12, 17 };
        private List<string> _contentList = new List<string> { "alphanumeric", "alphanumersymbolic" };
        private List<string> _symList = new List<string> { ":::::", "-----", ""};
        private Dictionary<string, string> _hex = new Dictionary<string, string> { };
        private string _mac = "";
        private string _company = "";
        // API used: https://api.macvendors.com/<MAC>



        public MACAddress(Driver d) : base(d)
        {
            _driver = d;
        }

        public int? Length
        {
            get
            {
                foreach (int _length in _lengthList)
                {
                    if (_length == _driver.Length)
                    {
                        return _length;
                    }
                }
                return null;
            }
        }
        public string? Content
        {
            get
            {
                foreach (string _content in _contentList)
                {
                    if (_content == _driver.Content)
                    {
                        return _content;
                    }
                }
                return null;
            }
        }
        public string SymList
        {
            get
            {
                foreach (string _s in _symList)
                {
                    if (_s == _driver.SymList)
                    {
                        return _s;
                    }
                }
                return null;
            }
        }

        // API call to return the vendor. If the call can't find the mac, then return null
        public string Mac
        {
            get
            {
                try
                {
                    string url = "https://api.macvendors.com/" + _driver.Input;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _mac = _driver.Input;
                        _company = reader.ReadToEnd();
                        return _mac;
                    }
                }
                catch
                {
                    _mac = null;
                    _company = null;
                    return null;
                }
            }
        }

        // Checks whether or not the input is all hex characters
        public Dictionary<string, string> Hex
        {
            get
            {
                _hex[_driver.Input] = "true";
                if (_driver.Hex[_driver.Input] == "true")
                {
                    return _hex;
                }
                else
                {
                    return null;
                }
            }
        }

        public override string Process()
        {
            string returnString = "";
            returnString += String.Format("This vendor for this hardware address is {0}.", this._company);
            return returnString;
        }

        public override string ToString()
        {
            return "MAC Address";
        }
    }
}