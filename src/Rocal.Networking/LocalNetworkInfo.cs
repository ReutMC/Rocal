using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Rocal.Networking;

public sealed record LocalNetworkInterface(
    string Name,
    string Description,
    IPAddress Address,
    bool IsWireless,
    bool HasGateway);

/// <summary>Enumerates usable IPv4 interfaces of this computer.</summary>
public static class LocalNetworkInfo
{
    public static IReadOnlyList<LocalNetworkInterface> GetUsableInterfaces()
    {
        var result = new List<LocalNetworkInterface>();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus != OperationalStatus.Up) continue;
            if (nic.NetworkInterfaceType is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel) continue;

            var props = nic.GetIPProperties();
            bool hasGateway = props.GatewayAddresses.Any(g =>
                g.Address.AddressFamily == AddressFamily.InterNetwork && !g.Address.Equals(IPAddress.Any));

            foreach (var unicast in props.UnicastAddresses)
            {
                var address = unicast.Address;
                if (address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(address)) continue;
                var bytes = address.GetAddressBytes();
                if (bytes[0] == 169 && bytes[1] == 254) continue; // link-local / APIPA

                result.Add(new LocalNetworkInterface(
                    nic.Name,
                    nic.Description,
                    address,
                    nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211,
                    hasGateway));
            }
        }
        return result;
    }

    /// <summary>Best guess at the LAN-facing interface: prefers interfaces that have a default gateway.</summary>
    public static LocalNetworkInterface? GetPrimary() =>
        GetUsableInterfaces().OrderByDescending(i => i.HasGateway).FirstOrDefault();
}
