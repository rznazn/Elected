using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Json;
using Android.Net;
using Android.Content;
using Android.Widget;

namespace Elected.Droid
{
    public static class VoterHttpRequest
    {


        static string uriElections = "https://www.googleapis.com/civicinfo/v2/voterinfo";

        static string uriRepresentatives = "https://www.googleapis.com/civicinfo/v2/representatives";

        static string apiKey = "?key=AIzaSyBmSluD9ZHFrX3Xz-b2CL4hz4kTVt_9qPg";

        public static async Task<JsonValue> FetchRepresentativesAsync(string address, Context context)
        {

            ConnectivityManager cm = (ConnectivityManager) context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = cm.ActiveNetworkInfo;
            if (networkInfo == null)
            {
                return null;
            }
            bool isOnline = networkInfo.IsConnected;
            // Create an HTTP web request using the URL:
            if (isOnline)
            {
                string urlReps = buildRepsUrl(address);
                HttpWebRequest requestReps = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(urlReps));
                requestReps.ContentType = "application/json";
                requestReps.Method = "GET";

                // Send the request to the server and wait for the response:
                using (WebResponse responseReps = await requestReps.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream streamReps = responseReps.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(streamReps));

                        // Return the JSON document:
                        return jsonDoc;
                    }
                }
            }
            else
            {
            return null;
            }
        }

        public static string buildRepsUrl(string address)
        {
            string query = WebUtility.UrlEncode(address);
            var requestUrl = uriRepresentatives + apiKey + "&address=" + query;
            Console.Out.WriteLine("REQUEST URL: {0}", requestUrl);
            return requestUrl;
        }

        public static async Task<JsonValue> FetchVoterInfoAsync(string address, Context context)
        {

            ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = cm.ActiveNetworkInfo;
            if (networkInfo == null)
            {
                return null;
            }
            bool isOnline = networkInfo.IsConnected;
            // Create an HTTP web request using the URL:
            if (isOnline)
            {
                string urlVoterInfo = buildVoterInfoUrl(address);
                HttpWebRequest requestVoterInfo = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(urlVoterInfo));
                requestVoterInfo.ContentType = "application/json";
                requestVoterInfo.Method = "GET";
                try
                {
                    // Send the request to the server and wait for the response:
                    using (WebResponse responseVoterInfo = await requestVoterInfo.GetResponseAsync())
                    {
                        // Get a stream representation of the HTTP web response:
                        using (Stream streamVoterInfo = responseVoterInfo.GetResponseStream())
                        {
                            // Use this stream to build a JSON document object:
                            JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(streamVoterInfo));

                            // Return the JSON document:
                            return jsonDoc;
                        }
                    }
                }
                catch (WebException e)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string buildVoterInfoUrl(string address)
        {
            string query = WebUtility.UrlEncode(address);
            var requestUrl = uriElections + apiKey + "&address=" + query;
            Console.Out.WriteLine("REQUEST URL: {0}", requestUrl);
            return requestUrl;
        }
        public static async Task<JsonValue> FetchElectionInfoAsync(string address, string electionId, Context context)
        {

            ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = cm.ActiveNetworkInfo;
            bool isOnline = networkInfo.IsConnected;
            // Create an HTTP web request using the URL:
            if (isOnline)
            {
                string urlElection = buildElectionIdUrl(address, electionId);
                HttpWebRequest requestVoterInfo = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(urlElection));
                requestVoterInfo.ContentType = "application/json";
                requestVoterInfo.Method = "GET";
                try
                {
                    // Send the request to the server and wait for the response:
                    using (WebResponse responseVoterInfo = await requestVoterInfo.GetResponseAsync())
                    {
                        // Get a stream representation of the HTTP web response:
                        using (Stream streamVoterInfo = responseVoterInfo.GetResponseStream())
                        {
                            // Use this stream to build a JSON document object:
                            JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(streamVoterInfo));

                            // Return the JSON document:
                            Console.Out.WriteLine("RESPONSE = {0}", jsonDoc);
                            return jsonDoc;
                        }
                    }
                }
                catch (WebException e)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string buildElectionIdUrl(string address, string electionId)
        {
            string query = WebUtility.UrlEncode(address);
            var requestUrl = uriElections + apiKey + "&address=" + query + "&electionId=" + electionId;
            Console.Out.WriteLine("REQUEST URL: {0}", requestUrl);
            return requestUrl;
        }
    }

}