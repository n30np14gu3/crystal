using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace crystal.network.bitnet
{
    public static class BitnetClient
    {
        public static void Setup(string sUri)
        {
            Url = new Uri(sUri);
        }

        public static Uri Url;

        public static ICredentials Credentials;

        public static JObject InvokeMethod(string sMethod, params object[] parameters)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Credentials = Credentials;
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject joe = new JObject();
            joe["jsonrpc"] = "1.0";
            joe["id"] = "1";
            joe["method"] = sMethod;

            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (var p in parameters)
                    {
                        if (p.GetType().IsGenericType && p is IEnumerable)
                        {
                            JArray l = new JArray();
                            foreach (var i in (IEnumerable)p)
                            {
                                l.Add(i);
                            }
                            props.Add(l);
                        }
                        else
                        {
                            props.Add(p);
                        }
                    }
                    joe.Add(new JProperty("params", props));
                }
            }

            string s = JsonConvert.SerializeObject(joe);
            // serialize json for the request
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (Stream dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream str = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str ?? throw new InvalidOperationException()))
                    {
                        return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                    }
                }
            }
        }

        public static void BackupWallet(string destination)
        {
            InvokeMethod("backupwallet", destination);
        }

        public static string CreateRawTransaction(JArray ins, JObject outs)
        {
            return InvokeMethod("createrawtransaction", ins, outs)["result"].ToString();
        }

        public static JObject DecodeRawTransaction(string tx)
        {
            return InvokeMethod("decoderawtransaction", tx)["result"] as JObject;
        }

        public static string GetAccount(string address)
        {
            return InvokeMethod("getaccount", address)["result"].ToString();
        }

        public static string GetAccountAddress(string account)
        {
            return InvokeMethod("getaccountaddress", account)["result"].ToString();
        }

        public static IEnumerable<string> GetAddressesByAccount(string account)
        {
            return from o in InvokeMethod("getaddressesbyaccount", account)["result"]
                   select o.ToString();
        }

        public static double GetBalance(string account = null, int minconf = 1)
        {
            if (account == null)
            {
                return (double)InvokeMethod("getbalance")["result"];
            }
            return (double)InvokeMethod("getbalance", account, minconf)["result"];
        }

        public static string GetBlockByCount(int height)
        {
            return InvokeMethod("getblockbycount", height)["result"].ToString();
        }

        public static int GetBlockCount()
        {
            return (int)InvokeMethod("getblockcount")["result"];
        }

        public static int GetBlockNumber()
        {
            return (int)InvokeMethod("getblocknumber")["result"];
        }

        public static int GetConnectionCount()
        {
            return (int)InvokeMethod("getconnectioncount")["result"];
        }

        public static double GetDifficulty()
        {
            return (double)InvokeMethod("getdifficulty")["result"];
        }

        public static bool GetGenerate()
        {
            return (bool)InvokeMethod("getgenerate")["result"];
        }

        public static double GetHashesPerSec()
        {
            return (double)InvokeMethod("gethashespersec")["result"];
        }

        public static JObject GetInfo()
        {
            return InvokeMethod("getinfo")["result"] as JObject;
        }

        public static string GetNewAddress(string account)
        {
            return InvokeMethod("getnewaddress", account)["result"].ToString();
        }

        public static string GetRawTransaction(string txid)
        {
            return InvokeMethod("getrawtransaction", txid)["result"].ToString();
        }

        public static double GetReceivedByAccount(string account, int minconf = 1)
        {
            return (double)InvokeMethod("getreceivedbyaccount", account, minconf)["result"];
        }

        public static double GetReceivedByAddress(string address, int minconf = 1)
        {
            return (double)InvokeMethod("getreceivedbyaddress", address, minconf)["result"];
        }

        public static JObject GetTransaction(string txid)
        {
            return InvokeMethod("gettransaction", txid)["result"] as JObject;
        }

        public static JObject GetWork()
        {
            return InvokeMethod("getwork")["result"] as JObject;
        }

        public static bool GetWork(string data)
        {
            return (bool)InvokeMethod("getwork", data)["result"];
        }

        public static string Help(string command = "")
        {
            return InvokeMethod("help", command)["result"].ToString();
        }

        public static JObject ListAccounts(int minconf = 1)
        {
            return InvokeMethod("listaccounts", minconf)["result"] as JObject;
        }

        public static JArray ListReceivedByAccount(int minconf = 1, bool includeEmpty = false)
        {
            return InvokeMethod("listreceivedbyaccount", minconf, includeEmpty)["result"] as JArray;
        }

        public static JArray ListReceivedByAddress(int minconf = 1, bool includeEmpty = false)
        {
            return InvokeMethod("listreceivedbyaddress", minconf, includeEmpty)["result"] as JArray;
        }

        public static JArray ListTransactions(string account, int count = 10)
        {
            return InvokeMethod("listtransactions", account, count)["result"] as JArray;
        }
        public static JArray ListUnspent(int minconf = 1, int maxconf = 999999)
        {
            return InvokeMethod("listunspent", minconf, maxconf)["result"] as JArray;
        }
        public static JArray ListUnspent(int minconf, int maxconf, List<string> addresses)
        {
            return InvokeMethod("listunspent", minconf, maxconf, addresses)["result"] as JArray;
        }


        public static bool Move(
            string fromAccount,
            string toAccount,
            double amount,
            int minconf = 1,
            string comment = ""
        )
        {
            return (bool)InvokeMethod(
            "move",
            fromAccount,
            toAccount,
            amount,
            minconf,
            comment
            )["result"];
        }

        public static string SendFrom(
            string fromAccount,
            string toAddress,
            double amount,
            int minconf = 1,
            string comment = "",
            string commentTo = ""
        )
        {
            return InvokeMethod(
            "sendfrom",
            fromAccount,
            toAddress,
            amount,
            minconf,
            comment,
            commentTo
            )["result"].ToString();
        }


        public static JObject SendRawTransaction(string hexstring)
        {
            return InvokeMethod("sendrawtransaction", hexstring)["result"] as JObject;
        }

        public static string SendToAddress(string address, double amount, string comment, string commentTo)
        {
            return InvokeMethod("sendtoaddress", address, amount, comment, commentTo)["result"].ToString();
        }

        public static void SetAccount(string address, string account)
        {
            InvokeMethod("setaccount", address, account);
        }

        public static void SetGenerate(bool generate, int genproclimit = 1)
        {
            InvokeMethod("setgenerate", generate, genproclimit);
        }

        public static JObject SignRawTransaction(string hexstring)
        {
            return InvokeMethod("signrawtransaction", hexstring)["result"] as JObject;
        }

        public static void Stop()
        {
            InvokeMethod("stop");
        }

        public static JObject ValidateAddress(string address)
        {
            return InvokeMethod("validateaddress", address)["result"] as JObject;
        }

        public static JObject WalletPassphrase(string passphrase, int timeout)
        {
            return InvokeMethod("walletpassphrase", passphrase, timeout)["result"] as JObject;
        }

        public static JObject WalletLock()
        {
            return InvokeMethod("walletlock")["result"] as JObject;
        }
    }
}