// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using Android.Content;
using Android.Net;
using Java.Net;
using MvvmCross.Exceptions;
using MvvmCross.Logging;
using MvvmCross.Platforms.Android;
using MvvmCross.Plugin.Network.Reachability;

namespace MvvmCross.Plugin.Network.Platforms.Android
{
    [Preserve(AllMembers = true)]
	public class MvxReachability : IMvxReachability
    {
        private const int ReachableTimeoutInMilliseconds = 5000;

        private ConnectivityManager _connectivityManager;

        protected ConnectivityManager ConnectivityManager
        {
            get
            {
                _connectivityManager = _connectivityManager ?? (ConnectivityManager)Mvx.IoCProvider.Resolve<IMvxAndroidGlobals>().ApplicationContext.GetSystemService(Context.ConnectivityService);
                return _connectivityManager;
            }
        }

        protected bool IsConnected
        {
            get
            {
                try
                {
                    var activeConnection = ConnectivityManager.ActiveNetworkInfo;

                    return activeConnection != null && activeConnection.IsConnected;
                }
                catch (Exception e)
                {
                    MvxPluginLog.Instance.Warn("Unable to get connected state - do you have ACCESS_NETWORK_STATE permission - error: {0}", e.ToLongString());
                    return false;
                }
            }
        }

        public bool IsHostReachable(string host)
        {
            bool reachable = false;

            if (IsConnected)
            {
                if (!string.IsNullOrEmpty(host))
                {
                    // to avoid ICMP/ping issues we don't test ping, but just return true if we have a network here
                    // see discussion in https://github.com/MvvmCross/MvvmCross/pull/408
                    reachable = true;
                }
            }

            return reachable;
        }

        public bool IsHostPingReachable(string host)
        {
            bool reachable = false;

            if (IsConnected)
            {
                if (!string.IsNullOrEmpty(host))
                {
                    try
                    {
                        reachable = InetAddress.GetByName(host).IsReachable(ReachableTimeoutInMilliseconds);
                    }
                    catch (UnknownHostException)
                    {
                        reachable = false;
                    }
                }
            }

            return reachable;
        }
    }
}

/*
protected MvxReachabilityStatus GetReachabilityType()
{
    if (IsConnected)
    {
        var wifiState = ConnectivityManager.GetNetworkInfo(ConnectivityType.Wifi).GetState();
        if (wifiState == NetworkInfo.State.Connected)
        {
            // We are connected via WiFi
            return MvxReachabilityStatus.ViaWiFiNetwork;
        }

        var mobile = ConnectivityManager.GetNetworkInfo(ConnectivityType.Mobile).GetState();
        if (mobile == NetworkInfo.State.Connected)
        {
            // We are connected via carrier data
            return MvxReachabilityStatus.ViaCarrierDataNetwork;
        }
    }

    return MvxReachabilityStatus.Not;
}
*/
