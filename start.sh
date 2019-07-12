#!/bin/sh

# UDP
sudo docker run -d --restart=always --volume /configurations/openvpn-status2mqtt/udp/:/configuration --volume /configurations/haus-openvpn/status-udp/:/openvpn --name openvpn-udp-status2mqtt openvpn-status2mqtt

# TCP
sudo docker run -d --restart=always --volume /configurations/openvpn-status2mqtt/tcp/:/configuration --volume /configurations/haus-openvpn/status-tcp/:/openvpn --name openvpn-tcp-status2mqtt openvpn-status2mqtt