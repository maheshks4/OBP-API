using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ConsoleApplication1
{
    public class OBPHelper
    {
            string oauth_callback = "http://www.bbc.co.uk/news";
            // Use your own callback URL
            string oauth_consumer_key = "q4vfomz3peqmtoxg0dgs0mg3aafa22a0ku3yei2v";
            // Use your own oauth_consumer_key
            string oauth_nonce = Guid.NewGuid().ToString();
            // Use your own oauth_nonce
            string oauth_signature = "";
            // Leave this blank for now
            string oauth_signature_method = "HMAC-SHA1";
            // "HMAC-SHA1" or "HMAC-SHA256"
   
            string oauth_token_secret = null;

            string oauth_consumer_secret = "xygpxhs01smnphljh32p4n3b3oud0jclzoy3stbq";
            // Use your own oauth_consumer_secret

            string uri = "https://apisandbox.openbankproject.com/oauth/initiate";

            string authorizationstring = null;

        //step1
            public Dictionary<string, string> ObtainRequestToken(string oauth_callback, string oauth_consumer_key, string oauth_nonce,
                   string oauth_consumer_secret, string uri)
            {
                authorizationstring = ProcessRequest(oauth_callback, oauth_consumer_key, oauth_nonce,
                 oauth_consumer_secret, null, uri, null, null, "POST");

                return ParseHtmlResponse(MakeHttpCall(uri, authorizationstring,"POST"));
            }

        //step3
            public Dictionary<string, string> ConvertRequestTokentoAccessToken(string oauth_callback, string oauth_consumer_key, string oauth_nonce,
                      string oauth_consumer_secret, string oauth_token_secret, string uri, string oauth_verifier, string oauth_token)
            {
                 authorizationstring = ProcessRequest(oauth_callback, oauth_consumer_key, oauth_nonce,
                 oauth_consumer_secret, oauth_token_secret, uri, oauth_verifier, oauth_token, "POST");

                 return ParseHtmlResponse(MakeHttpCall(uri, authorizationstring, "POST"));
            }


        //step4 and 5
            public string AccessingProtectedResources(string oauth_callback, string oauth_consumer_key, string oauth_nonce,
                          string oauth_consumer_secret, string oauth_token_secret, string uri, string oauth_verifier, string oauth_token, string method)
            {
                authorizationstring = ProcessRequest(oauth_callback, oauth_consumer_key, oauth_nonce,
                 oauth_consumer_secret, oauth_token_secret, uri, oauth_verifier, oauth_token, method);

                if (method == "GET")
                    return MakeHttpCall(uri, authorizationstring, method);
                else
                {
                    string json = "{\"bank_id\":\"rbs\",\"account_id\":\"reallypoor\",\"amount\":\"1.00\"}";
                    return MakeJsonHttpCall(uri, authorizationstring, method, json);
                }
            }
               



        private string ProcessRequest(string oauth_callback, string oauth_consumer_key, string oauth_nonce, 
                string oauth_consumer_secret, string oauth_token_secret, string uri, string oauth_verifier, string oauth_token, string method)
        {
            this.oauth_callback =  oauth_callback;
            // Use your own callback URL
            this.oauth_consumer_key = oauth_consumer_key;
            // Use your own oauth_consumer_key
            this.oauth_nonce = oauth_nonce;
           
            // Leave this blank for now
            this.oauth_signature_method = "HMAC-SHA1";
            // "HMAC-SHA1" or "HMAC-SHA256"
            string oauth_timestamp = "1496490661";
            // Use your own oauth_timestamp
            string oauth_version = "1";
            // "1.0" or "1"

            this.oauth_token_secret = oauth_token_secret;

            this.oauth_consumer_secret = oauth_consumer_secret;
            // Use your own oauth_consumer_secret

            
            this.uri = uri;

            // Create a list of OAuth parameters
            List<KeyValuePair<string, string>> oauthparameters = new List<KeyValuePair<string, string>>();

            //step1
            if (oauth_callback != null)
                oauthparameters.Add(new KeyValuePair<string, string>("oauth_callback", oauth_callback));
            
            //step3
            if (oauth_verifier != null && oauth_token!= null)
            {
                oauthparameters.Add(new KeyValuePair<string, string>("oauth_verifier", oauth_verifier));
                oauthparameters.Add(new KeyValuePair<string, string>("oauth_token", oauth_token));
            }

            //step4 & 5
            if (oauth_verifier == null && oauth_token != null)
            {                
                oauthparameters.Add(new KeyValuePair<string, string>("oauth_token", oauth_token));
            }
            
           

            oauthparameters.Add(new KeyValuePair<string, string>("oauth_consumer_key", oauth_consumer_key));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_nonce", oauth_nonce));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_signature_method", oauth_signature_method));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_timestamp", oauth_timestamp));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_version", oauth_version));

            // Sort the OAuth parameters on the key
            oauthparameters.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Construct the Base String
            string basestring = method.ToUpper() + "&" + HttpUtility.UrlEncode(uri) + "&";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                basestring += pair.Key + "%3D" + HttpUtility.UrlEncode(pair.Value) + "%26";
            }
            basestring = basestring.Substring(0, basestring.Length - 3);
            //Gets rid of the last "%26" added by the foreach loop

            // Makes sure all the Url encoding gives capital letter hexadecimal 
            // i.e. "=" is encoded to "%3D", not "%3d"
            char[] basestringchararray = basestring.ToCharArray();
            for (int i = 0; i < basestringchararray.Length - 2; i++)
            {
                if (basestringchararray[i] == '%')
                {
                    basestringchararray[i + 1] = char.ToUpper(basestringchararray[i + 1]);
                    basestringchararray[i + 2] = char.ToUpper(basestringchararray[i + 2]);
                }
            }
            basestring = new string(basestringchararray);

            oauth_signature = GetOauth_Signature(basestring, oauth_signature_method, oauth_consumer_secret, oauth_token_secret);

            // Create the Authorization string for the WebRequest header
            string authorizationstring = "";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                authorizationstring += pair.Key;
                authorizationstring += "=";
                authorizationstring += pair.Value;
                authorizationstring += ",";
            }
            authorizationstring += "oauth_signature=" + oauth_signature;

            return authorizationstring;
           
        }


        private string MakeHttpCall(string uri, string authorizationstring, string method)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.Headers.Add("Authorization", "OAuth " + authorizationstring);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
           
        }


        private string MakeJsonHttpCall(string uri, string authorizationstring, string method, string json)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "text/json";
            request.Method = method;
            request.Headers.Add("Authorization", "OAuth " + authorizationstring);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {                
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;

        }

        private Dictionary<string, string> ParseHtmlResponse(string responseFromServer)
        {
            string[] listParams = responseFromServer.Split('&');
            Dictionary<string, string> response = new Dictionary<string, string>();

            foreach (string param in listParams)
            {
                string[] data = param.Split('=');
                response.Add(data[0], data[1]);
            }

            return response;
        }


        private string GetOauth_Signature(string basestring, string oauth_signature_method, string oauth_consumer_secret, string oauth_token_secret)
        {
            string oauth_signature = "";

            // Encrypt with either SHA1 or SHA256, creating the Signature
            var enc = Encoding.ASCII;
            if (oauth_signature_method == "HMAC-SHA1")
            {
                HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret ?? string.Empty));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha1 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
                byte[] resultantArray = new byte[hmacsha1.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha1.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = HttpUtility.UrlEncode(base64);
            }
            else if (oauth_signature_method == "HMAC-SHA256")
            {
                HMACSHA256 hmac = new HMACSHA256(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret ?? string.Empty));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha256 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "")
                    .ToLower();
                byte[] resultantArray = new byte[hmacsha256.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha256.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = HttpUtility.UrlEncode(base64);
            }

            return oauth_signature;
        }
    }
}
